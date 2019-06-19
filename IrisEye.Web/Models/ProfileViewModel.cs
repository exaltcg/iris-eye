using System.Collections.Generic;
using IrisEye.Core.Entities;

namespace IrisEye.Web.Models
{
    public class ProfileViewModel
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Team { get; set; }
        public string Bio { get; set; }
        public IEnumerable<SystemUser> Followers { get; set; }
    }
}