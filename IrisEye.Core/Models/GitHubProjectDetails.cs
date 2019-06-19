using System;
using System.Collections.Generic;

namespace IrisEye.Core.Models
{
    public class GitHubProjectDetails
    {
        public string Name { get; set; }
        public List<GitHubIssue> GitHubIssues { get; set; }
        public DateTime AddedOn { get; set; }
        
        public string Body { get; set; }
    }
}