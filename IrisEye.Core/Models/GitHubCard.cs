using System;

namespace IrisEye.Core.Models
{
    public class GitHubCard
    {
        public string url { get; set; }
        public string project_url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public object note { get; set; }
        public bool archived { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string column_url { get; set; }
        public string content_url { get; set; }
    }

    public class CardForIssue
    {
        public string Issue { get; set; }
        public string Column { get; set; }
    }
}