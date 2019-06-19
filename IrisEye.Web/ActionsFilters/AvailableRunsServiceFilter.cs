using System.Linq;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Web.Data;
using IrisEye.Web.Providers;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal.Account.Manage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IrisEye.Web.ActionsFilters
{
    public class AvailableRunsServiceFilter:ActionFilterAttribute
    {
        private readonly IFoldersProvider _foldersProvider;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITestLabProvider _testLabProvider;
        private readonly IUsersProvider _usersProvider;

        public AvailableRunsServiceFilter(IFoldersProvider foldersProvider, 
            UserManager<IdentityUser> userManager, 
            IUsersProvider usersProvider, 
            ITestLabProvider testLabProvider)
        {
            _foldersProvider = foldersProvider;
            _userManager = userManager;
            _usersProvider = usersProvider;
            _testLabProvider = testLabProvider;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;

            if (controller != null)
            {
                var folder = await _foldersProvider.GetActiveFolder();
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    controller.ViewBag.isCollapsed = context.HttpContext.Session.GetString("isCollapsed");
                    var contextUser = context.HttpContext.User;
                    var userId = contextUser.GetUserId();
                    var userProfile = await _usersProvider.GetUser(userId);
                    if(userProfile!=null)
                    {
                        controller.ViewBag.user = userProfile;
                        folder = userProfile.SelectedFolder;
                        var messagesForUser = await _testLabProvider.GetMessagesForUser(userProfile.Id);
                        var unreadMessages = messagesForUser.Where(p => !p.IsRead);
                        controller.ViewBag.messages = unreadMessages;

                    }
                }
                
                
                controller.ViewBag.runs = folder.Runs;
                controller.ViewBag.folders = await _foldersProvider.GetAllFolders();
                
            }

            await next();
        }
    }
}