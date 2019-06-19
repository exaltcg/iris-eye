using System.ComponentModel.DataAnnotations;

namespace IrisEye.Web.Models
{
    public class ImportIssuesViewModel
    {
        [Required]
        [Display(Name = "GitHub project ID")]
        public int ProjectId { get; set; }

        public int TestSuiteId { get; set; } = 0;

        [Required]
        [Display(Name = "GitHub Issue ID")]
        public int IssueId { get; set; }
    }
}