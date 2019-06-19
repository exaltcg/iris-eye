using System.Collections.Generic;

namespace IrisEye.Core.Models
{    
    public class Error
    {
        public string resource { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class GitHubError
    {
        public string message { get; set; }
        public List<Error> errors { get; set; }
        public string documentation_url { get; set; }
    }
}