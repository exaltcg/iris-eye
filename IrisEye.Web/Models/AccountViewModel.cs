using System.ComponentModel.DataAnnotations;

namespace IrisEye.Web.Models
{
    public class AccountViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Display(Name = "GitHub User Name")]
        public string GitHubLogin { get; set; }

        [Display(Name = "GitHub Access Token")]
        public string GitHubToken { get; set; }

        [Display(Name = "GitHub Aliases (',' comma-separated)")]
        public string GitHubAliases { get; set; }
        
        
        
        
    }
}