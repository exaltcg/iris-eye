using System.ComponentModel.DataAnnotations;

namespace IrisEye.Web.Models
{
    public class CreateNewPullRequestViewModel
    {
        public long TestId { get; set; }
       
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Message")]

        public string Body { get; set; }
        
        public string TestName { get; set; }
    }
}