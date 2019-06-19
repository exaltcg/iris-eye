using System;

namespace IrisEye.Core.Models
{
    public class GitHubColumns
    {
        public string url { get; set; }
        public string project_url { get; set; }
        public string cards_url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}