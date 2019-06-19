namespace IrisEye.Core.Models
{
    public class GitHubNewPullRequest
    {
        public string title { get; set; }
        public string head { get; set; }
        public string @base { get; set; }
        public string body { get; set; }
    }
}