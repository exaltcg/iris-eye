using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;

namespace IrisEye.Web.Providers
{
    public interface ITestLabProvider
    {
        Task AddNewTestSuite(int id, SystemUser systemUser);

        Task<TestSuite> GetSuite(long id);

        Task<IEnumerable<TestSuite>> GetAllTestSuits();
        Task DeleteTestSuite(int id);
        Task DeleteTestCase(int id);
        Task<TestCase> GetTest(long id);
        Task UpdateTest(TestCase test);
        Task<long> AddNewIssue(int issueId, long testSuiteId, SystemUser user);

        Task AddHistoryItem(long testId, TestCaseHistory item);
        Task SubmitForReview(long testId, int pullRequestId, SystemUser user);
        Task UpdateTestsUser(int userId, int testId, bool isAssignee, SystemUser who);
        Task<GitHubPullRequest> CreateNewPullRequest(long id, string login, string password, string title, string body);
        Task AddTestCaseComment(long testId, TestCaseComment comment);
        Task<List<TestCaseComment>> GetMessagesForUser(long userProfileId);
        Task<TestCaseComment> GetComment(long id);
        Task UpdateComment(TestCaseComment comment);
        Task ClearAllNotificationsForUser(SystemUser user);
        Task CloseTest(long id, SystemUser user);
        Task RefreshTestSuits(long id, SystemUser user);
        Task<List<TestCase>> GetTestsForPeriod(DateTime begin, DateTime end);
        Task RefreshTestSteps(int id);
        Task ReopenTestCase(long id, SystemUser user);
    }
    
}