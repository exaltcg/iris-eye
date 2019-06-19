using IrisEye.Core.Models;
using IrisEye.Data.Parsers;

namespace IrisEye.Web.Utils
{
    public static class StaticHelper
    {
        public static GitHubBranch GetBranch(string name)
        {
            var client = new GithubApiClient();
            return client.GetBranch(name);
        }

        public static GitHubPullRequest GetPullRequest(int id)
        {
            var client = new GithubApiClient();
            return client.GetPullRequest(id);
            
        }
    }
}