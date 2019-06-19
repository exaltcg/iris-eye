using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;
using IrisEye.Data.Extensions;
using IrisEye.Data.Parsers;
using IrisEye.Web.ActionsFilters;
using IrisEye.Web.Enums;
using IrisEye.Web.Models;
using IrisEye.Web.Providers;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace IrisEye.Web.Controllers
{
    [ServiceFilter(typeof(AvailableRunsServiceFilter))]

    public class TestLabController : Controller
    {
        private readonly ITestLabProvider _testLabProvider;
        private readonly IUsersProvider _usersProvider;

        public TestLabController(ITestLabProvider testLabProvider, IUsersProvider usersProvider)
        {
            _testLabProvider = testLabProvider;
            _usersProvider = usersProvider;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var model = await _testLabProvider.GetAllTestSuits();
            return
                View(model);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteTestSuite(int id)
        {
            await _testLabProvider.DeleteTestSuite(id);
            return RedirectToAction("Index", "TestLab");
        }

        public async Task<IActionResult> TestSuite(int id)
        {
            var testSuite = await _testLabProvider.GetSuite(id);
            return View(testSuite);
        }

        [Authorize]
        public async Task<IActionResult> SwitchState(int id, int statusId, string returnurl)
        {
            var test = await _testLabProvider.GetTest(id);
            var who = await _usersProvider.GetUser(User.GetUserId());
            var statusType = (Status) statusId;
            var historyItem = new TestCaseHistory
            {
                Author = who, 
                Message = $@"changed status from <strong>{test.Status.GetDescription()}</strong> to <strong>{statusType.GetDescription()}</strong>"
            };
            
            switch (statusType)
            {
                case Status.New:
                    break;
                case Status.ReadyForAutomation:
                    break;
                case Status.InProgress:
                    test.Assignee = who;
                    test.StartedOn = DateTime.Now;
                    if (!string.IsNullOrEmpty(who.GitHubToken))
                    {
                        var gitClient = new GithubApiClient(who.GitHubAccount, who.GitHubToken);
                        gitClient.AddAssignees(test.GitHubId, new []{who.GitHubAccount});
                    }
                    break;
                case Status.ToCrossPlatform:
                    break;
                case Status.ReadyForReview:
                    return RedirectToAction("SubmitForReview", "TestLab", new {@id=id});
                case Status.ReviewStarted:
                    test.Reviewer = who;
                    if (!string.IsNullOrEmpty(who.GitHubToken))
                    {
                        var gitClient = new GithubApiClient(who.GitHubAccount, who.GitHubToken);
                        gitClient.AddReviewer(test.PullRequestId, new []{who.GitHubAccount});
                    }
                    break;
                case Status.CannotAutomate:
                    break;
                case Status.Finished:
                    test.FinishedOn = DateTime.Now;
                    break;
                case Status.ChangesRequested:
                    return RedirectToAction("RequestChange", "TestLab", new {@id=id});
                case Status.Fixed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            test.Status = statusType;
            test.HistoryItems.Add(historyItem);
            await _testLabProvider.UpdateTest(test);
            if (string.IsNullOrEmpty(returnurl))
            {
                return Redirect(Url.RouteUrl(new {controller = "TestLab", action = "TestSuite"}) + "/" +
                                test.TestSuite.Id + "#test-" + test.Id);
            }
            else
            {
                return LocalRedirect(returnurl);
            }
        }

        public async Task<IActionResult> TestCase(int id)
        {
            var gitClient = new GithubApiClient();
            var model = new TestCaseViewModel
            {
                TestCase = await _testLabProvider.GetTest(id), 
                Users = await _usersProvider.GetAllUsers(),
            };
            model.GitHubComments = gitClient.GetCommentsForIssue(model.TestCase.GitHubId);
            if (model.TestCase.PullRequestId == 0) return View(model);
            try
            {
                model.PullRequestComment = gitClient.GetPullRequestComments(model.TestCase.PullRequestId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View(model);
        }

       
        [Authorize]
        public IActionResult ImportIssues(int testSuiteId = 0)
        {
            var model = new ImportIssuesViewModel {TestSuiteId = testSuiteId};
            return View(model);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ImportIssues(ImportIssuesViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var user = await _usersProvider.GetUser(User.GetUserId());
                if (model.TestSuiteId==0)
                {
                
                    await _testLabProvider.AddNewTestSuite(model.ProjectId, user);
                    return RedirectToAction("Index", "TestLab");
                }
                var issueId = await _testLabProvider.AddNewIssue(model.IssueId, model.TestSuiteId, user);
                return RedirectToAction("TestCase", "TestLab", new {@id = issueId});

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> SubmitForReview(int id)
        {
            var test = await _testLabProvider.GetTest(id);
            var model = new SubmitTestForReviewViewModel
            {
                TestName = test.Name,
                TestId = test.Id
            };
            return View(model);

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitForReview(SubmitTestForReviewViewModel model)
        {
            try
            {
                var client = new GithubApiClient();
                var pullRequest = client.GetPullRequest(model.PullRequestId);
                if (pullRequest.title==null) throw new Exception("Entered pull request does not exist.");
                var user = await _usersProvider.GetUser(User.GetUserId());
                await _testLabProvider.SubmitForReview(model.TestId, model.PullRequestId, user);
                var test = await _testLabProvider.GetTest(model.TestId);
                return RedirectToAction("TestSuite", "TestLab", new {@id = test.TestSuite.Id});

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError(string.Empty, e.Message);
            }

            return View(model);
        }

        public async Task<IActionResult> Merge(int id)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            var test = await _testLabProvider.GetTest(id);
            test.MergedDate = DateTime.Now;
            test.HistoryItems.Add(new TestCaseHistory
            {
                Author = user,
                Message = "merged into dev"
            });
            await _testLabProvider.UpdateTest(test);
            return RedirectToAction("TestSuite", "TestLab", new {@id = test.TestSuite.Id});
        }

        [Authorize]
        public async Task<IActionResult> UpdateAssignee(int userId, int testId)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            await _testLabProvider.UpdateTestsUser(userId, testId, true, user);
            return RedirectToAction("TestCase", "TestLab", new {@id = testId});
        }
        
        [Authorize]
        public async Task<IActionResult> UpdateReviewer(int userId, int testId)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            await _testLabProvider.UpdateTestsUser(userId, testId, false, user);
            return RedirectToAction("TestCase", "TestLab", new {@id = testId});
        }

        public async Task<IActionResult> Statistics(string filter, int period = 5)
        {
            var selectedPeriod = (StatisticsPeriod) period;
            var testSuits = await _testLabProvider.GetAllTestSuits();
            var testSuites = testSuits.ToList();
            var model = new TestLabStatisticsViewModel
            {
                Tests = testSuites.SelectMany(p=>p.TestCases).ToList()
            };
            
            switch (selectedPeriod)
            {
                case StatisticsPeriod.Today:
                    model.Tests = model.Tests.Where(p =>
                        p.StartedOn.Date == DateTime.Now.Date 
                        || p.FinishedOn.Date == DateTime.Now.Date
                        || p.MergedDate?.Date == DateTime.Now.Date).ToList();
                    break;
                case StatisticsPeriod.ThisWeek:
                    var begin = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                    var end = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Friday);
                    model.Tests = model.Tests = model.Tests.Where(p =>
                        (p.StartedOn.Date >= begin && p.StartedOn.Date <= end)
                        || (p.FinishedOn.Date >= begin && p.FinishedOn.Date <= end)
                        || (p.MergedDate?.Date >= begin && p.MergedDate?.Date <= end)).ToList(); 
                    break;
                case StatisticsPeriod.ThisMonth:
                    var now = DateTime.Now;
                    var beginMonth = new DateTime(now.Year, now.Month, 1);
                    var endMonth = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Friday);
                    model.Tests = model.Tests = model.Tests.Where(p =>
                        (p.StartedOn.Date >= beginMonth.Date && p.StartedOn.Date <= endMonth.Date)
                        || (p.FinishedOn.Date >= beginMonth.Date && p.FinishedOn.Date <= endMonth.Date)
                        || (p.MergedDate?.Date >= beginMonth.Date && p.MergedDate?.Date <= endMonth.Date)).ToList(); 
                    break;
                case StatisticsPeriod.ThisYear:
                    var beginYear = new DateTime(DateTime.Now.Year, 1, 1);
                    var endYear = beginYear.AddYears(1);
                    model.Tests = model.Tests = model.Tests.Where(p =>
                        (p.StartedOn.Date >= beginYear.Date && p.StartedOn.Date <= endYear.Date)
                        || (p.FinishedOn.Date >= beginYear.Date && p.FinishedOn.Date <= endYear.Date)
                        || (p.MergedDate?.Date >= beginYear.Date && p.MergedDate?.Date <= endYear.Date)).ToList(); 

                    break;
                case StatisticsPeriod.AllTheTime:
                    break;
                case StatisticsPeriod.PreviousWeek:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            model.TotalTests = model.Tests.Count();
            model.TestsInProgress = model.Tests.Count(p => p.Status == Status.InProgress
                                                           || p.Status == Status.ReviewStarted
                                                           || p.Status == Status.ReadyForReview
                                                           || p.Status == Status.ToCrossPlatform);
            model.AwaitingMerge = model.Tests.Count(p => p.Status == Status.Finished && !p.MergedDate.HasValue);
            model.AwaitingReview = model.Tests.Count(p => p.Status == Status.ReadyForReview);

            switch (filter)
            {
                case "ip":
                    model.Tests = model.Tests.Where(p => p.Status == Status.InProgress
                                                         || p.Status == Status.ReviewStarted
                                                         || p.Status == Status.ReadyForReview
                                                         || p.Status == Status.ToCrossPlatform);
                    break;
                case "am":
                    model.Tests = model.Tests.Where(p => p.Status == Status.Finished && !p.MergedDate.HasValue);
                    break;
                case "ar":
                    model.Tests = model.Tests.Where(p => p.Status == Status.ReadyForReview);
                    break;
                    
            }

            model.Filter = filter;
            model.Period = selectedPeriod;
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> CreateNewPullRequest(int id)
        {
            var test = await _testLabProvider.GetTest(id);
            var model = new CreateNewPullRequestViewModel
            {
                TestId = test.Id,
                Title = $@"Issue {test.GitHubId}",
                Body = $@"Added coverage for #{test.GitHubId}, please review.",
                TestName = test.Name
            };

            return View(model);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewPullRequest(CreateNewPullRequestViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var user = await _usersProvider.GetUser(User.GetUserId());
                var pr = await _testLabProvider.CreateNewPullRequest(
                    model.TestId, 
                    user.GitHubAccount, 
                    user.GitHubToken,
                    model.Title,
                    model.Body);
                await _testLabProvider.SubmitForReview(model.TestId, pr.number, user);
                return RedirectToAction("TestCase", "TestLab", new {@id = model.TestId});


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError(string.Empty, e.Message + e.StackTrace);
            }
            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(long id, string message)
        {
            var currentUser = await _usersProvider.GetUser(User.GetUserId());
            var test = await _testLabProvider.GetTest(id);
            var addressed = test.Assignee == currentUser ? test.Reviewer : test.Assignee;
            await _testLabProvider.AddTestCaseComment(id, new TestCaseComment
            {
                Message = message,
                AddedBy = currentUser,
                AddressedTo = addressed
            });
            test = await _testLabProvider.GetTest(id);

            return PartialView("_TestCaseCommentsPartial", test);
        }

        [Authorize]
        public async Task<IActionResult> ReadComment(long id)
        {
            var comment = await _testLabProvider.GetComment(id);
            comment.IsRead = true;
            await _testLabProvider.UpdateComment(comment);
            return Redirect(Url.RouteUrl(new {controller = "TestLab", action = "TestCase"}) + "/" +
                            comment.TestCase.Id + "#issue-comments");
        }

        public async Task<IActionResult> ClearAllNotifications()
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            await _testLabProvider.ClearAllNotificationsForUser(user);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> RequestChange(int id)
        {
            var test = await _testLabProvider.GetTest(id);
            var model = new RequestChangeViewModel
            {
                TestId = id,
                TestName = test.Name
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestChange(RequestChangeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var test = await _testLabProvider.GetTest(model.TestId);
                var user = await _usersProvider.GetUser(User.GetUserId());
                await _testLabProvider.AddTestCaseComment(test.Id, new TestCaseComment
                {
                    Message = model.Message,
                    AddressedTo = test.Assignee,
                    AddedBy = user
                });
                test.Status = Status.ChangesRequested;
                await _testLabProvider.UpdateTest(test);
                return RedirectToAction("TestCase", "TestLab", new {@id = test.Id});

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> CloseTestCase(long id)
        {
            var test = await _testLabProvider.GetTest(id);
            var user = await _usersProvider.GetUser(User.GetUserId());
            await _testLabProvider.CloseTest(id, user);
            return RedirectToAction("TestSuite", "TestLab", new {@id = test.TestSuite.Id});
        }

        public async Task<IActionResult> RefreshIssues(long id)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            await _testLabProvider.RefreshTestSuits(id, user);
            return RedirectToAction("TestSuite", "TestLab", new {@id = id});
        }

        public async Task<IActionResult> CoverageProgress(int periodId = 0)
        {
            var period = periodId == 0 ? StatisticsPeriod.ThisWeek : (StatisticsPeriod) periodId;

            DateTime begin, end;
            switch (period)
            {
                case StatisticsPeriod.Today:
                    begin = DateTime.Now.Date;
                    end = DateTime.Now.Date;
                    break;
                case StatisticsPeriod.ThisWeek:
                    begin = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                    end = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Friday);
                    break;
                case StatisticsPeriod.ThisMonth:
                    begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    end = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Friday);
                    break;
                case StatisticsPeriod.ThisYear:
                    begin = new DateTime(DateTime.Now.Year, 1, 1);
                    end = begin.AddYears(1);
                    break;
                case StatisticsPeriod.AllTheTime:
                    begin = DateTime.MinValue;
                    end = DateTime.MaxValue;
                    break;
                case StatisticsPeriod.PreviousWeek:
                    begin = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                    end = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Friday);
                    begin = begin.AddDays(-7);
                    end = end.AddDays(-7);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var allSuits = await _testLabProvider.GetAllTestSuits();
            var allTests = allSuits.SelectMany(p => p.TestCases).ToList();
            var groupedTests = allTests.Where(p=>p.MergedDate.HasValue).GroupBy(p => p.MergedDate.Value.GetWeekOfYear()).OrderBy(p=>p.Key);
            
            var tests = await _testLabProvider.GetTestsForPeriod(begin, end);
            var model = new CoverageProgressViewModel
            {
                StatisticsPeriod = period, 
                IssuesCovered = tests
            };
            var totalDays = (int)(end - begin).TotalDays;
            if (totalDays == 0)
            {
                totalDays = 1;
            }
            model.TestsVelocity = model.IssuesCovered.Count(p => !p.IsIssue) / 6.0 / (totalDays + 1);
            model.IssuesVelocity = model.IssuesCovered.Count(p => p.IsIssue) / 6.0 / (totalDays + 1);

            model.AllTestSuits = await _testLabProvider.GetAllTestSuits();
            var modelAllTestSuits = model.AllTestSuits as TestSuite[] ?? model.AllTestSuits.ToArray();
            model.TotalCoverage = (double)modelAllTestSuits.SelectMany(p=>p.TestCases).Count(p=>p.MergedDate.HasValue) / modelAllTestSuits.SelectMany(p=>p.TestCases).Count() * 100;
            return View(model);
        }

        public async Task<IActionResult> RefreshSteps(int id)
        {
            await _testLabProvider.RefreshTestSteps(id);

            return RedirectToAction("TestCase", "TestLab", new {@id = id});
        }

        public async Task<IActionResult> GetProgress()
        {
            
            var model = new List<TestsProgressModel>();
            var allSuits = await _testLabProvider.GetAllTestSuits();
            var finishedDate = DateTime.Parse("03/29/2019");
            var testSuites = allSuits.ToList();
            var tests = testSuites.SelectMany(p => p.TestCases).Where(p => p.MergedDate.HasValue && !p.IsIssue)
                .GroupBy(p => p.MergedDate.Value.GetWeekOfYear()).OrderBy(p => p.Key);
            var issues = testSuites.SelectMany(p => p.TestCases).Where(p => p.MergedDate.HasValue && p.IsIssue)
                .GroupBy(p => p.MergedDate.Value.GetWeekOfYear()).OrderBy(p => p.Key);

            var textJson = "[{\"label\": \"Tests\", \"data\": [";
            foreach (var test in tests.Where(p=>p.Key>=13))
            {
                textJson += $@"[""{SystemExtension.FirstDateOfWeekISO8601(DateTime.Now.Year, test.Key):M}"", {test.Count()}], ";
            }
            

            textJson += "]";
            textJson = textJson.Replace(", ]", "]},{\"label\":\"Issues\", \"data\":[");
            foreach (var issue in issues.Where(p=>p.Key>=13))
            {
                textJson += $@"[""{SystemExtension.FirstDateOfWeekISO8601(DateTime.Now.Year, issue.Key):M}"", {issue.Count()}], ";
            }

            textJson = textJson.Substring(0, textJson.Length - 2) + "]}]";
           

            return Content(textJson, "application/json");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteTestCase(int id)
        {
            var test = await _testLabProvider.GetTest(id);
            await _testLabProvider.DeleteTestCase(id);
            return RedirectToAction("TestSuite", "TestLab", new {@id = test.TestSuite.Id});
        }

        [Authorize]
        public async Task<IActionResult> ReopenTestCase(long id)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            await _testLabProvider.ReopenTestCase(id, user);
            return RedirectToAction("TestCase", "TestLab", new {@id = id});
        }
    }
}