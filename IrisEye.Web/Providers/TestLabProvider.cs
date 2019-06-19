using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;
using IrisEye.Data.Extensions;
using IrisEye.Data.Parsers;
using IrisEye.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace IrisEye.Web.Providers
{
    public class TestLabProvider:ITestLabProvider
    {
        private readonly ApplicationDbContext _dbContext;

        public TestLabProvider(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNewTestSuite(int id, SystemUser systemUser)
        {
            var existingSuite = await _dbContext.TestSuites.CountAsync(p => p.GitHubProjectId == id) > 0;
            if (existingSuite) throw new Exception("Test suite with this project id was already added.");
            var gitClient = new GithubApiClient();
            var testRailClient = new TestRailApiClient();
            var issues = gitClient.GetIssuesForProject(id);
            var newTestSuite = new TestSuite
            {
                Name = issues.Name, 
                GitHubProjectId = id,
                TestCases = new List<TestCase>(),
                AddedOn = issues.AddedOn,
                Description = issues.Body
            };
            foreach (var issue in issues.GitHubIssues)
            {
                var newTest = ExtractTestCase(systemUser, issue, testRailClient, newTestSuite);

                newTestSuite.TestCases.Add(newTest);
            }
            var afterExistingSuite = await _dbContext.TestSuites.CountAsync(p => p.GitHubProjectId == id) > 0;
            if (afterExistingSuite) throw new Exception("Test suite with this project id was already added.");

            _dbContext.TestSuites.Add(newTestSuite);
            await _dbContext.SaveChangesAsync();

        }

        private static TestCase ExtractTestCase(SystemUser systemUser, GitHubIssue issue, TestRailApiClient testRailClient,
            TestSuite newTestSuite)
        {
            var testId = issue.body.Split('/').Last();
            TestRailTest testRailTest = null;
            TestCase newTest;
            if (int.TryParse(testId, out var tId))
            {
                testRailTest = testRailClient.GetTest(tId);
                newTestSuite.TestRailId = testRailTest.suite_id;
                newTest = new TestCase
                {
                    Name = testRailTest.title,
                    GitHubId = issue.number,
                    Status = Status.New,
                    TestRailId = testRailTest.id,
                    TestSteps = new List<TestStep>(),
                    Preconditions = testRailTest.custom_preconds
                };
            }
            else
            {
                newTest = new TestCase
                {
                    Name = issue.title,
                    GitHubId = issue.number,
                    Status = Status.ReadyForAutomation,
                    Message = issue.body,
                    IsIssue = true
                };
            }


            if (issue.Column == "Done")
            {
                newTest.Status = Status.Finished;
                newTest.FinishedOn = issue.closed_at ?? DateTime.MinValue;
                newTest.StartedOn = issue.created_at ?? DateTime.MinValue;
                newTest.Assignee = systemUser;

                newTest.HistoryItems.Add(new TestCaseHistory
                {
                    Message = "status updated automatically on import"
                });
            }

            var steps = new List<TestStep>();
            if (testRailTest != null)
                foreach (var customStepsSeparated in testRailTest.custom_steps_separated)
                {
                    steps.Add(new TestStep
                    {
                        Actual = customStepsSeparated.content,
                        Expected = customStepsSeparated.expected,
                        SortIndex = testRailTest.custom_steps_separated.IndexOf(customStepsSeparated)
                    });
                }

            newTest.TestSteps = steps;
            newTest.HistoryItems.Add(new TestCaseHistory
            {
                Author = systemUser,
                Message = "added test from Git Hub"
            });
            return newTest;
        }

        public async Task<TestSuite> GetSuite(long id)
        {
            var query = _dbContext.TestSuites
                .Include(p => p.TestCases)
                .ThenInclude(p => p.Assignee);
            query = query.Include(p => p.TestCases).ThenInclude(p => p.Reviewer);

            var result = await query.FirstOrDefaultAsync(p=>p.Id==id);
            return result;

        }

        public async Task<IEnumerable<TestSuite>> GetAllTestSuits()
        {
            var suits = await _dbContext.TestSuites
                .Include(p=>p.TestCases)
                .ThenInclude(p=>p.Assignee)
                .ToListAsync();
            return suits;
        }

        public async Task DeleteTestSuite(int id)
        {
            var suite = await GetSuite(id);
            if (suite != null)
            {
                _dbContext.TestSuites.Remove(suite);
                await _dbContext.SaveChangesAsync();
            }
            
        }

        public async Task DeleteTestCase(int id)
        {
            var test = await GetTest(id);
            _dbContext.TestCases.Remove(test);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<TestCase> GetTest(long id)
        {
            var query = _dbContext.TestCases
                .Include(p => p.Assignee)
                .Include(p => p.Reviewer)
                .Include(p => p.TestSuite)
                .Include(p => p.TestSteps)
                .Include(p => p.HistoryItems)
                .ThenInclude(p => p.Author);

            query = query.Include(p => p.TestCaseComments)
                .ThenInclude(p => p.AddedBy);
            return await query 
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateTest(TestCase test)
        {
            _dbContext.TestCases.Update(test);
            await _dbContext.SaveChangesAsync();
        }
        

        public async Task<long> AddNewIssue(int issueId, long testSuiteId, SystemUser user)
        {
            var existingIssue = await _dbContext.TestCases
                .Include(p=>p.TestSuite)
                .FirstOrDefaultAsync(p => p.GitHubId == issueId);
            if (existingIssue!=null) throw new Exception($@"The issue {issueId} was already added to {existingIssue.TestSuite?.Name} test suite.");
            var testSuite = await GetSuite(testSuiteId);
            var client = new GithubApiClient();
            var issue = client.GetIssue(issueId);
            var newTestCase = new TestCase
            {
                GitHubId = issue.number,
                IsIssue = !issue.body.Contains("testrail"),
                Name = issue.title,
                Status = Status.ReadyForAutomation,
                Message =  issue.body
            };
            newTestCase.HistoryItems.Add( new TestCaseHistory
            {
                Message = "added issue from the GitHub",
                Author = user,
            });
            if (!newTestCase.IsIssue && int.TryParse(issue.body.Split('/').Last(), out var trId))
            {
                var trClient = new TestRailApiClient();
                var testRailIssue = trClient.GetTest(trId);
                newTestCase.TestSteps = new List<TestStep>();
                var steps = new List<TestStep>();
                foreach (var customStepsSeparated in testRailIssue.custom_steps_separated)
                {
                    steps.Add(new TestStep
                    {
                        Actual = customStepsSeparated.content,
                        Expected = customStepsSeparated.expected,
                        SortIndex = testRailIssue.custom_steps_separated.IndexOf(customStepsSeparated)
                    });
                }

                newTestCase.TestSteps = steps;
                
            }
            testSuite.TestCases.Add(newTestCase);
            _dbContext.TestSuites.Update(testSuite);
            await _dbContext.SaveChangesAsync();
            return newTestCase.Id;
        }

        public async Task AddHistoryItem(long testId, TestCaseHistory item)
        {
            var test = await GetTest(testId);
            test.HistoryItems.Add(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SubmitForReview(long testId, int pullRequestId, SystemUser user)
        {
            var test = await GetTest(testId);
            test.PullRequestId = pullRequestId;
            test.HistoryItems.Add(new TestCaseHistory
            {
                Author = user,
                Message = $@"changed status from {test.Status.GetDescription()} to {Status.ReadyForReview.GetDescription()}"
            });
            test.Status = Status.ReadyForReview;
            await UpdateTest(test);
        }

        public async Task UpdateTestsUser(int userId, int testId, bool isAssignee, SystemUser who)
        {

            var testToUpdate = await GetTest(testId);
            var user = await _dbContext.SystemUsers.FirstOrDefaultAsync(p => p.Id == userId);

            if (isAssignee)
            {
                testToUpdate.Assignee = user;
                testToUpdate.HistoryItems.Add(new TestCaseHistory
                {
                    Author = who,
                    Message = $@"changed assignee to <strong>{user.Name}</strong>"
                });
                if (!string.IsNullOrEmpty(user.GitHubToken))
                {
                    var client = new GithubApiClient(user.GitHubAccount, user.GitHubToken);
                    client.AddAssignees(testToUpdate.GitHubId, new []{user.GitHubAccount});
                }
            }
            else
            {
                testToUpdate.Reviewer = user;
                testToUpdate.HistoryItems.Add(new TestCaseHistory
                {
                    Author = who,
                    Message = $@"changed reviewer to <strong>{user.Name}</strong>"
                });
                if (!string.IsNullOrEmpty(user.GitHubToken) && testToUpdate.PullRequestId!=0)
                {
                    var client = new GithubApiClient(user.GitHubAccount, user.GitHubToken);
                    client.AddReviewer(testToUpdate.PullRequestId, new []{user.GitHubAccount});
                }


            }

            await UpdateTest(testToUpdate);
        }

        public async Task<GitHubPullRequest> CreateNewPullRequest(long id, string login, string token, string title, string body)
        {
            var test = await GetTest(id);
            var client = new GithubApiClient(login, token);
            var pr = client.CreateNewPullRequest(title, body,
                test.GitHubId.ToString());
            client.AddAssignees(pr.number, new[]{login});
            return pr;
        }

        public async Task AddTestCaseComment(long testId, TestCaseComment comment)
        {
            var test = await GetTest(testId);
            comment.TestCase = test;
            await _dbContext.TestCaseComments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TestCaseComment>> GetMessagesForUser(long userProfileId)
        {
            var user = await _dbContext.SystemUsers.FirstOrDefaultAsync(p => p.Id == userProfileId);
            var allMessages = await _dbContext.TestCaseComments
                .Include(p=>p.TestCase)
                .Include(p=>p.AddressedTo)
                .Include(p=>p.AddedBy)
                .Where(p => (p.AddressedTo == user) && !p.IsRead)
                .ToListAsync();
            return allMessages;
        }

        public async Task<TestCaseComment> GetComment(long id)
        {
            return await _dbContext.TestCaseComments
                .Include(p=>p.TestCase)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateComment(TestCaseComment comment)
        {
            _dbContext.TestCaseComments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ClearAllNotificationsForUser(SystemUser user)
        {
            var unreadComments = await _dbContext.TestCaseComments.Where(p => p.AddressedTo==user && !p.IsRead)
                .ToListAsync();
            unreadComments.ForEach(p=>p.IsRead=true);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CloseTest(long id, SystemUser user)
        {
            var test = await GetTest(id);
            test.Status = Status.Closed;
            await AddHistoryItem((int) id, new TestCaseHistory
            {
                Message = "closed the test",
                Author = user
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task RefreshTestSuits(long id, SystemUser user)
        {
            var gitClient = new GithubApiClient();
            var firstSuits = await GetSuite(id);
            var newGitHubIssues = gitClient.GetIssuesForProject(firstSuits.GitHubProjectId);
            var trClient = new TestRailApiClient();

            var testSuit = await GetSuite(id);
            foreach (var issue in newGitHubIssues.GitHubIssues)
            {
                var seekingElement = testSuit.TestCases.FirstOrDefault(p => p.GitHubId == issue.number);
                if (seekingElement != null) continue;
                //add new issue
                var newTest = ExtractTestCase(user, issue, trClient, testSuit);
                testSuit.TestCases.Add(newTest);
            }

            _dbContext.TestSuites.Update(testSuit);
            await _dbContext.SaveChangesAsync();


        }

        public async Task<List<TestCase>> GetTestsForPeriod(DateTime begin, DateTime end)
        {
            var tests = await _dbContext.TestCases
                .Include(p=>p.TestSuite)
                .Include(p=>p.Assignee)
                .Where(
                    p => p.MergedDate.HasValue 
                         && (p.MergedDate.Value.Date >= begin && p.MergedDate.Value.Date <= end)
                         && p.TestSuite!=null)
                .ToListAsync();
            return tests;
        }

        public async Task RefreshTestSteps(int id)
        {
            var test = await GetTest(id);
            var client = new TestRailApiClient();
            var testRailTest =  client.GetTest(test.TestRailId);
            test.Preconditions = testRailTest.custom_preconds;
            var newSteps = new List<TestStep>();
            testRailTest.custom_steps_separated.ForEach(p=>newSteps.Add(new TestStep
            {
                Actual = p.content,
                Expected = p.expected,
                SortIndex = testRailTest.custom_steps_separated.IndexOf(p)
            }));
            foreach (var testTestStep in test.TestSteps.ToList())
            {
                test.TestSteps.Remove(testTestStep);
            }

            test.TestSteps = newSteps;
            await UpdateTest(test);
        }

        public async Task ReopenTestCase(long id, SystemUser user)
        {
            var test = await GetTest(id);
            test.MergedDate = null;
            test.Reviewer = null;
            test.Status = Status.ReadyForAutomation;
            await AddHistoryItem(test.Id, new TestCaseHistory
            {
                Author = user,
                Message = "re-opened test case"

            });
            await UpdateTest(test);
        }
    }
}