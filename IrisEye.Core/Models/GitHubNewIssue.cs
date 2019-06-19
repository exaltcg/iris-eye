using System.Collections.Generic;

namespace IrisEye.Core.Models
{
    public class GitHubNewIssue
    {
        public string title { get; set; }
        public string body { get; set; }
        public List<string> assignees { get; set; }
    }
}