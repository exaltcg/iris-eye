using System.Collections.Generic;
using IrisEye.Core.Entities;

namespace IrisEye.Web.Models
{
    public class UserProfileViewModel
    {
        public SystemUser User { get; set; }
        public IEnumerable<TestCase> TestCases { get; set; }
        public int TotalAutomated { get; set; }
        public int TotalReviewed { get; set; }
        public int InProgress { get; set; }
        public int InReview { get; set; }
    }
}