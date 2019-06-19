using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Web.Data;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IrisEye.Web.Providers
{
    public class UsersProvider:IUsersProvider
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFoldersProvider _foldersProvider;

        public UsersProvider(
            ApplicationDbContext dbContext, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            IFoldersProvider foldersProvider)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _foldersProvider = foldersProvider;
        }

        public async Task AddNewUser(SystemUser systemUser)
        {
            var userExists = await _dbContext.SystemUsers.FirstOrDefaultAsync(p => p.EntityId == systemUser.EntityId);
            if (userExists!=null) throw new Exception("User with similar Id was already added.");
            _dbContext.SystemUsers.Add(systemUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUser(SystemUser systemUser)
        {
            _dbContext.SystemUsers.Update(systemUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SystemUser> GetUser(string id)
        {
            return await _dbContext.SystemUsers
                .Include(p=>p.SelectedFolder)
                .ThenInclude(p=>p.Runs)
                .FirstOrDefaultAsync(p => p.EntityId == id);
        }

        public async Task SwitchFolder(int id, ClaimsPrincipal httpContextUser)
        {
            var userId = httpContextUser.GetUserId();
            var user = await GetUser(userId);
            var folder = await _foldersProvider.GetFolder(id);
            user.SelectedFolder = folder;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SystemUser>> GetAllUsers()
        {
            return await _dbContext.SystemUsers.ToListAsync();
        }

        public async Task<SystemUser> GetUserById(long id)
        {
            return await _dbContext.SystemUsers.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}