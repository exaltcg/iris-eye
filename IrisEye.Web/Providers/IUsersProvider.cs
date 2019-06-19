using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IrisEye.Core.Entities;

namespace IrisEye.Web.Providers
{
    public interface IUsersProvider
    {
        Task AddNewUser(SystemUser systemUser);
        Task UpdateUser(SystemUser systemUser);
        Task<SystemUser> GetUser(string id);
        Task SwitchFolder(int id, ClaimsPrincipal httpContextUser);
        Task<List<SystemUser>> GetAllUsers();
        Task<SystemUser> GetUserById(long id);
    }
}