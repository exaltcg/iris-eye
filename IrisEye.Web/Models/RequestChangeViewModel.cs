using System.ComponentModel.DataAnnotations;

namespace IrisEye.Web.Models
{
    public class RequestChangeViewModel
    {
        public long TestId { get; set; }
        
        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
        public string TestName { get; set; }
    }
}