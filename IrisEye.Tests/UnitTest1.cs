using System;
using System.Data;
using System.IO;
using System.Linq;
using IrisEye.Data.Extensions;
using IrisEye.Data.Parsers;
using IrisEye.Web.Data;
using IrisEye.Web.Providers;
using LibGit2Sharp;
using RestSharp;
using RestSharp.Authenticators;
using Xunit;
using Xunit.Abstractions;

namespace IrisEye.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TestGit()
        {
            var gitClient = new GithubApiClient("", "");
            gitClient.AddComment(2747, "Added by mistake");
        }

        [Fact]
        public void TestGitRepository()
        {
            var repo = new GitRepository();
            //var ts = repo.GetTestInfos();
        }

        [Fact]
        public void TestGit2sharp()
        {
            var tempDir = Path.GetTempPath() + "test";
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
                var clonedRepoPath = Repository.Clone("https://github.com/mozilla/iris.git", tempDir);
            }

            var repo = new Repository(tempDir);
            /*var pullOptions = new PullOptions()
            {
                MergeOptions = new MergeOptions()
                {
                    FastForwardStrategy = FastForwardStrategy.NoFastForward,
                    
                }
            };*/

            var fileHistory = repo.Commits.QueryBy("iris/tests/antivirus_interoperability/no_crash_after_closing_container_tab.py", new CommitFilter { SortBy = CommitSortStrategies.Time }).ToList();
            
            //var signature = new Signature("User", "Name", DateTimeOffset.Now);
            //var dev = repo.Branches["origin/dev"];
            //var branch = repo.CreateBranch("dev", dev.Tip);
            //Commands.Checkout(repo, branch);
            Commands.Fetch(repo, "origin", new string[0], new FetchOptions(), "" );
        }


        [Fact]
        public void Test1()
        {
            var restClient = new RestClient("https://testrail.stage.mozaws.net");
            restClient.AddDefaultHeader("Content-Type", "application/json");
            restClient.Authenticator =
                new HttpBasicAuthenticator("", "");

            var request = new RestRequest("/index.php?/api/v2/get_test/163197", Method.GET);

            var response = restClient.Execute(request).Content;
        }
    }
}