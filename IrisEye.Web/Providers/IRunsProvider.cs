using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Web.Models;

namespace IrisEye.Web.Providers
{
    public interface IRunsProvider
    {
        Task<Test> GetTest(long id);
        Task AddNewRun(Run run, long folderId);
        Task AddMultipleRuns(List<Run> run);
        Task<IEnumerable<Run>> GetLatestRuns();

        Task<Run> GetRun(long id);
        Task<Step> GetStep(long id);
        Task DeleteRun(int id);
        Task RefreshTestInfo();

        Task<IEnumerable<TestInfo>> GetAllTestsInfo();


        Task<IEnumerable<Test>> GetTestsForComparison(long id, Folder folder);
        Task<LatestStatsModel> GetLatestRunForEnvironment(string env, long folderId);
        Task SetAnalysisState(long stepId, AnalysisResolution resolution, bool isAlreadyExists, int issueId, string title, string message, long suiteId, SystemUser user);
        Task<TestInfo> GetTestInfo(string name);
        Task UpdateTestInfo(TestInfo testInfo);
        Task<IEnumerable<Run>> GetLatestRunsForEnvironment(string environment, Folder folder);
        Task<IEnumerable<Run>> GetAllRunsForDate(DateTime modelSelectedRunDate, Folder folder);
        Task<IEnumerable<Run>> GetRunsForFolder(Folder folder);
        Task<AnalysisResult> StartAnalysis(long testId,long stepId, SystemUser user);
        Task<AnalysisResult> SetTestAnalysisState(
            SystemUser user, 
            int testSuiteId, 
            int status, 
            long analysisId, 
            int issueId, 
            string title, 
            string message, 
            bool isAlreadyExists
            );

        Task MigrateTestStatuses();
    }
}