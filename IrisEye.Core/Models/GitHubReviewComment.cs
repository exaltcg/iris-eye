using System;

namespace IrisEye.Core.Models
{
    public class Html
    {
        public string href { get; set; }
    }

    public class PullRequest
    {
        public string href { get; set; }
    }

    public class GitHubReviewComment
    {
        public int id { get; set; }
        public string node_id { get; set; }
        public User user { get; set; }
        public string body { get; set; }
        public string state { get; set; }
        public string html_url { get; set; }
        public string pull_request_url { get; set; }
        public string author_association { get; set; }
        public DateTime submitted_at { get; set; }
        public string commit_id { get; set; }
    }
  
}