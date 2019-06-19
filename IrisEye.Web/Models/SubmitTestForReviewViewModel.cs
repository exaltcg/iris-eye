using System.ComponentModel.DataAnnotations;

namespace IrisEye.Web.Models
{
    public class SubmitTestForReviewViewModel
    {
        public long TestId { get; set; }
        
        public string TestName { get; set; }
        
        [Required]
        [Display(Name = "Pull Request Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int PullRequestId { get; set; }
    }
}