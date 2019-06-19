using System.Collections.Generic;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;

namespace IrisEye.Web.Models
{
    public class TestCaseViewModel
    {
        public List<SystemUser> Users { get; set; }
        public TestCase TestCase { get; set; }
        public List<GitHubComment> GitHubComments { get; set; } = new List<GitHubComment>();
        public List<GitHubReviewComment> PullRequestComment { get; set; } = new List<GitHubReviewComment>();
    }
}