using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;
using IrisEye.Data.Extensions;
using IrisEye.Data.Parsers;
using IrisEye.Web.Data;
using IrisEye.Web.Migrations;
using IrisEye.Web.Models;
using LibGit2Sharp;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TestInfo = IrisEye.Core.Entities.TestInfo;
using TestStatus = IrisEye.Core.Entities.TestStatus;

namespace IrisEye.Web.Providers
{
    
    public class RunsProvider:IRunsProvider
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITestLabProvider _testLabProvider;

        public RunsProvider(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _testLabProvider = new TestLabProvider(_dbContext);
        }

        public async Task<Test> GetTest(long id)
        {
            var query = _dbContext.Tests
                .Include(p=>p.AnalysisResult)
                .Include(p => p.Run)
                .Include(p => p.Steps).ThenInclude(p => p.StepAnalysisItem);
            return await query.FirstOrDefaultAsync(p => p.Id == id);

        }

        public async Task AddNewRun(Run run, long folderId)
        {
            var runWithSameHash = await _dbContext.Runs.FirstOrDefaultAsync(p => p.ReportHash == run.ReportHash);
            
            if (runWithSameHash!=null) throw new Exception($@"File with the same content was already submitted to {runWithSameHash.Folder.Name} folder.");
            var folder = await _dbContext.Folders.FirstOrDefaultAsync(p => p.Id == folderId);
            run.Folder = folder;
            
            _dbContext.Runs.Add(run);
            await _dbContext.SaveChangesAsync();
            
            var justSavedRun = await GetRun(run.Id);
            foreach (var failedTest in justSavedRun.Tests.Where(p=>p.Steps.Any(step=>!step.IsPassed)))
            {
                var failedStep = failedTest.Steps.FirstOrDefault(p => !p.IsPassed);
                var analysisResult = await _dbContext.AnalysisResults
                    .Include(p => p.IdentifiedAt)
                    .ThenInclude(p=>p.Test)
                    .FirstOrDefaultAsync(p =>
                        p.Message == failedStep.Message 
                        && p.IdentifiedAt.Test.Name == failedStep.Test.Name
                        && (p.AnalysisStatus == AnalysisStatus.NewIssue || p.AnalysisStatus == AnalysisStatus.KnownIssue));
                if (analysisResult == null) continue;
                {
                    failedTest.AnalysisResult = new AnalysisResult
                    {
                        AnalysisStatus = AnalysisStatus.KnownIssue,
                        By = analysisResult.By,
                        FinishedOn = DateTime.Now,
                        GitHubId = analysisResult.GitHubId,
                        IdentifiedAt = failedTest.Steps.FirstOrDefault(p => !p.IsPassed),
                        Message = analysisResult.Message
                    };

                    _dbContext.Tests.Update(failedTest);
                }

            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddMultipleRuns(List<Run> runs)
        {
            if (runs.Count==0) return;
            var activeFolder = await _dbContext.Folders.FirstOrDefaultAsync(p => p.IsActive);
            foreach (var run in runs)
            {
                try
                {
                    await AddNewRun(run, activeFolder.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }
        }

        public async Task<IEnumerable<Run>> GetLatestRuns()
        {
            var runs = await _dbContext.Runs.GroupBy(p => p.Environment).ToListAsync();
            var retList = new List<Run>();
            foreach (var run in runs)
            {
                retList.Add(run.OrderByDescending(p=>p.ReportTime).FirstOrDefault());
            }

            return retList;
        }

        public async Task<Run> GetRun(long id)
        {
            return await _dbContext.Runs
                .Include(p=>p.Tests)
                .ThenInclude(p=>p.Steps)
                .ThenInclude(p=>p.StepAnalysisItem)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Step> GetStep(long id)
        {
            return await _dbContext.Steps
                .Include(p=>p.Test)
                .ThenInclude(p=>p.Run)
                .FirstOrDefaultAsync(p=>p.Id==id);
        }

        public async Task DeleteRun(int id)
        {
            var runToRemove = await GetRun(id);
            _dbContext.Runs.Remove(runToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RefreshTestInfo()
        {
            var parser = new GitRepository();
            var existingTestInfos = await _dbContext.TestInfos.ToListAsync();
            var updatedItems = parser.GetTestInfos(existingTestInfos.Select(p=>p.TestName));
            foreach (var item in updatedItems)
            {
                var existingItem = existingTestInfos.FirstOrDefault(p => p.TestName == item.TestName);
                if (existingItem==null)
                {
                    _dbContext.TestInfos.Add(item);
                }
            }

            await _dbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<TestInfo>> GetAllTestsInfo()
        {
            return await _dbContext.TestInfos.ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetTestsForComparison(long id, Folder folder)
        {
            var ret = new List<Test>();
            var targetTest = await GetTest(id);
            var listOfEnvironments = new List<string>();
            listOfEnvironments.AddRange(new []{"osx","linux","win","win7"});
            listOfEnvironments.Remove(targetTest.Run.Environment);
            ret.Add(targetTest);
            foreach (var env in listOfEnvironments)
            {
                var testToAdd = await _dbContext.Tests
                    .Include(p=>p.Run)
                    .Include(p=>p.Steps)
                    .Where(p=>p.Run.Folder==folder)
                    .OrderByDescending(p => p.Run.ReportTime).FirstOrDefaultAsync(p => p.Name == targetTest.Name && p.Run.Environment == env);
                ret.Add(testToAdd);
            }
            return ret;
        }

        public async Task<LatestStatsModel> GetLatestRunForEnvironment(string env, long folderId)
        {
            var folder = await _dbContext.Folders.FindAsync(folderId);
            if (folder == null) return null;
            var item = await _dbContext.Runs
                .Include(p=>p.Tests)
                .ThenInclude(p=>p.AnalysisResult)
                .Include(p=>p.Tests)
                .ThenInclude(p=>p.Steps)
                .OrderByDescending(p => p.ReportTime)
                .FirstOrDefaultAsync(p => p.Environment == env && p.Folder==folder);
            if (item == null)
                return null;
            var model = new LatestStatsModel
            {
                RunId = item.Id,
                Skipped = item.Skipped,
                Environment = item.Environment,
                Date = item.ReportTime,
                Version = string.IsNullOrEmpty(item.BetaChannel)?item.Version:item.BetaChannel,
                Total = item.Total,
                Passed = item.Passed,
                Failed = item.Failed,
                Errors = item.Errors,
                NotAnalyzed = item.Tests.Count(test => test.AnalysisResult==null && test.Steps.Any(p=>!p.IsPassed)), 
                NewIssues = item.Tests.Count(test=>test.AnalysisResult!=null && test.AnalysisResult.AnalysisStatus==AnalysisStatus.NewIssue),
                KnownIssues = item.Tests.Count(test=>test.AnalysisResult!=null && test.AnalysisResult.AnalysisStatus==AnalysisStatus.KnownIssue),
                CannotReproduce = item.Tests.Count(test=>test.AnalysisResult!=null && test.AnalysisResult.AnalysisStatus==AnalysisStatus.CantReproduce)  
            };
            return model;

        }

        public async Task SetAnalysisState(long stepId, AnalysisResolution resolution, bool isAlreadyExists, int issueId, string title, string message, long suiteId, SystemUser user)
        {
            var suite = await _dbContext.TestSuites.FirstOrDefaultAsync(p => p.Id == suiteId);
            var gitClient = new GithubApiClient(user.GitHubAccount, user.GitHubToken);
            var step = await GetStep(stepId);
            if (step==null) throw new Exception("Can't find requested step!");
            var newAnalysis = new StepAnalysisItem
            {
                AddedBy = user,
                AnalysisResolution = resolution,
                Message = message
            };

            switch (resolution)
            {
                case AnalysisResolution.CannotReproduce:
                    if (isAlreadyExists)
                    {
                        var existingIssue = gitClient.GetIssue(issueId);
                        if (existingIssue == null) throw new Exception("Can't find requested issue id");
                        //gitClient.AddComment(issueId, message);
                        newAnalysis.GitHubId = issueId;
                    }
                    else
                    {
                        var issue = gitClient.CreateNewIssue(title, message, suite.GitHubProjectId);
                        newAnalysis.GitHubId = issue.number;
                        await _testLabProvider.AddNewIssue(issue.number, suiteId, user);
                    }
                    break;
                case AnalysisResolution.KnownIssue:
                    //gitClient.AddComment(issueId, message);
                    newAnalysis.GitHubId = issueId;
                    break;
                case AnalysisResolution.NewIssue:
                    var newIssue = gitClient.CreateNewIssue(title, message, suite.GitHubProjectId);
                    newAnalysis.GitHubId = newIssue.number;
                    await _testLabProvider.AddNewIssue(newIssue.number, suiteId, user);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
            step.StepAnalysisItem.Add(newAnalysis);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TestInfo> GetTestInfo(string name)
        {
            return await _dbContext.TestInfos.FirstOrDefaultAsync(p => p.TestName == name);
        }

        public async Task UpdateTestInfo(TestInfo testInfo)
        {
            _dbContext.TestInfos.Update(testInfo);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Run>> GetLatestRunsForEnvironment(string environment, Folder folder)
        {
            var ret = new List<Run>();
            var items = await _dbContext.Runs
                .Include(p=>p.Tests)
                .Where(p=>p.Folder==folder)
                .Where(p => p.Environment == environment)               
                .ToListAsync();
            var grouped = items
                .GroupBy(p => p.ReportTime.Date)
                .OrderByDescending(p => p.Key);

            foreach (var run in grouped.Take(10))
            {
                ret.Add(run.OrderByDescending(p=>p.Tests.Count).FirstOrDefault());
            }

            return ret;
        }

        public async Task<IEnumerable<Run>> GetAllRunsForDate(DateTime selectedRunDate, Folder folder)
        {
            var query = _dbContext.Runs
                .Include(p => p.Tests)
                .ThenInclude(p => p.Steps)
                .Include(p => p.Tests)
                .ThenInclude(p => p.AnalysisResult)
                .ThenInclude(p=>p.By);
            var allRuns = await query
                .Where(p => p.Folder == folder && p.ReportTime.Date == selectedRunDate.Date).ToListAsync();

            return allRuns.GroupBy(p => p.Environment).Select(item => item.OrderByDescending(p => p.Tests.Count).FirstOrDefault()).ToList();
        }

        public async Task<IEnumerable<Run>> GetRunsForFolder(Folder folder)
        {
            return await _dbContext.Runs
                .Include(p => p.Tests)
                .Where(p => p.Folder == folder).ToListAsync();
        }

        public async Task<AnalysisResult> StartAnalysis(long testId,long stepId, SystemUser user)
        {
            var step = await _dbContext.Steps.FirstOrDefaultAsync(p => p.Id == stepId);
            var analysisExists = await _dbContext.AnalysisResults
                .Include(p=>p.By)
                .FirstOrDefaultAsync(p => p.IdentifiedAt == step);
            if (analysisExists!=null) return analysisExists;
            var test = await GetTest(testId);
            var newAnalysis = new AnalysisResult
            {
                AnalysisStatus = AnalysisStatus.AnalysisStarted,
                By = user,
                IdentifiedAt = step,
                Message = string.Empty
            };
            test.AnalysisResult = newAnalysis;
            _dbContext.Tests.Update(test);
            await _dbContext.SaveChangesAsync();
            return newAnalysis;
        }

        public async Task<AnalysisResult> SetTestAnalysisState(
            SystemUser user, 
            int testSuiteId, 
            int status, 
            long analysisId, 
            int issueId, 
            string title, 
            string message, 
            bool isAlreadyExists)
        {
            var analysis = await _dbContext.AnalysisResults
                .Include(p=>p.IdentifiedAt)
                .ThenInclude(p=>p.Test)
                .FirstOrDefaultAsync(p => p.Id == analysisId);
            var analysisState = (AnalysisStatus) status;
            var gitClient = new GithubApiClient(user.GitHubAccount, user.GitHubToken);
            var testSuite = await _dbContext.TestSuites.FirstOrDefaultAsync(p => p.Id == testSuiteId);
            switch (analysisState)
            {
                case AnalysisStatus.NewIssue:
                    var newIssue = gitClient.CreateNewIssue(title, message, testSuite.GitHubProjectId);
                    analysis.GitHubId = newIssue.number;
                    await _testLabProvider.AddNewIssue(newIssue.number, testSuite.Id, user);
                    break;
                case AnalysisStatus.KnownIssue:
                    var existingKnownIssue = gitClient.GetIssue(issueId);
                    if (existingKnownIssue == null) throw new Exception("Can't find requested issue id");
                    analysis.GitHubId = issueId;
                    break;
                case AnalysisStatus.CantReproduce:
                    if (isAlreadyExists)
                    {
                        var existingIssue = gitClient.GetIssue(issueId);
                        if (existingIssue == null) throw new Exception("Can't find requested issue id");
                        analysis.GitHubId = issueId;
                    }
                    else
                    {
                        var issue = gitClient.CreateNewIssue(title, message, testSuite.GitHubProjectId);
                        analysis.GitHubId = issue.number;
                        await _testLabProvider.AddNewIssue(issue.number, testSuite.Id, user);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            analysis.FinishedOn = DateTime.Now;
            analysis.Message = message;
            analysis.AnalysisStatus = analysisState;
            _dbContext.AnalysisResults.Update(analysis);

            //find the same steps within tests and apply the same analysis result
            if (analysisState == AnalysisStatus.NewIssue || analysisState==AnalysisStatus.KnownIssue)
            {
                var testName = analysis.IdentifiedAt.Test.Name;
                var stepMessage = analysis.IdentifiedAt.Message;
                var similarTests = await _dbContext.Tests
                    .Include(p=>p.Steps)
                    .Where(p => p.Name == testName 
                                && p.AnalysisResult==null
                                && p.Steps.Any(step=>!step.IsPassed) 
                                && p.Steps.Any(z => z.Message == stepMessage))
                    .ToListAsync();
                foreach (var similarTest in similarTests)
                {
                    var newAnalysis = new AnalysisResult
                    {
                        AnalysisStatus = AnalysisStatus.KnownIssue,
                        By = analysis.By,
                        FinishedOn = DateTime.Now,
                        GitHubId = analysis.GitHubId,
                        Message = analysis.Message,
                        IdentifiedAt = similarTest.Steps.FirstOrDefault(p => !p.IsPassed)
                    };
                    similarTest.AnalysisResult = newAnalysis;
                    _dbContext.Tests.Update(similarTest);
                }

            }

            await _dbContext.SaveChangesAsync();
            
            return analysis;
        }

        public async Task MigrateTestStatuses()
        {
            var allTests = await _dbContext.Tests
                .Include(p => p.Steps)
                .Where(p => p.Status == TestStatus.Unidentified).ToListAsync();
            foreach (var test in allTests)
            {
                if (test.Steps.Any(p => p.Message.Contains("[ERROR]")))
                {
                    test.Status = TestStatus.Error;
                } else if (test.Steps.Any(p => !p.IsPassed))
                {
                    test.Status = TestStatus.Failed;
                }
                else
                {
                    test.Status = TestStatus.Passed;
                }

                _dbContext.Tests.Update(test);
                

            }
            
            await _dbContext.SaveChangesAsync();
        }
    }
}