using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Data.Parsers;
using IrisEye.Web.ActionsFilters;
using IrisEye.Web.Models;
using IrisEye.Web.Providers;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IrisEye.Web.Controllers
{
    [ServiceFilter(typeof(AvailableRunsServiceFilter))]
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFoldersProvider _foldersProvider;
        private readonly IUsersProvider _usersProvider;
        private readonly ITestLabProvider _testLabProvider;

        public AuthController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            IFoldersProvider foldersProvider, 
            IUsersProvider usersProvider, 
            ITestLabProvider testLabProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _foldersProvider = foldersProvider;
            _usersProvider = usersProvider;
            _testLabProvider = testLabProvider;
        }

        public IActionResult Index(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            
            return
                View(new LoginViewModel
                {
                    ReturnUrl = returnUrl
                });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) throw new Exception("The email address or password is incorrect.");
                var result =
                    await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (!result.Succeeded) throw new Exception("The email address or password is incorrect.");
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return LocalRedirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");

            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);

            }
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ModelState.AddModelError(string.Empty, "User with this email was already registered.");
                return View(model);
            }

            var newUser = new IdentityUser
            {
                Email = model.Email,
                UserName = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, SystemRoles.User.ToString());
                var newUserProfile = new SystemUser
                {
                    EntityId = newUser.Id,
                    Name = model.Name,
                    SelectedFolder = await _foldersProvider.GetActiveFolder()
                };
                try
                {
                    await _usersProvider.AddNewUser(newUserProfile);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(model);
                }
                   
            }

            foreach (var identityError in result.Errors)
            {
                ModelState.AddModelError(string.Empty, identityError.Description);    
            }
                    
            return View(model);
        }
        
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile(long id, string filter)
        {
            var user = await _usersProvider.GetUserById(id);
            var allTestSuits = await _testLabProvider.GetAllTestSuits();
            var allTests = allTestSuits.SelectMany(p => p.TestCases).ToList();
            var model = new UserProfileViewModel
            {
                User = user,
                TestCases = allTests,
                TotalAutomated = allTests.Count(p => p.Assignee == user && p.Status==Status.Finished),
                TotalReviewed = allTests.Count(p => p.Reviewer == user && p.Status == Status.Finished),
                InProgress = allTests.Count(p => p.Assignee == user && (p.Status == Status.InProgress
                                                 || p.Status == Status.ReviewStarted
                                                 || p.Status == Status.ReadyForReview
                                                 || p.Status == Status.ToCrossPlatform)),
                InReview = allTests.Count(p => p.Reviewer == user && (p.Status == Status.ReviewStarted||p.Status == Status.ChangesRequested || p.Status == Status.Fixed))
            };
            switch (filter)
            {
                case "ta":
                    model.TestCases = model.TestCases.Where(p => p.Assignee == user && p.Status==Status.Finished).ToList();
                    break;
                case "tr":
                    model.TestCases = model.TestCases.Where(p => p.Reviewer == user && p.Status == Status.Finished).ToList();
                    break;
                case "ip":
                    model.TestCases = model.TestCases.Where(p => p.Assignee == user && (p.Status == Status.InProgress
                                                                                        || p.Status == Status.ReviewStarted
                                                                                        || p.Status == Status.ReadyForReview
                                                                                        || p.Status == Status.ToCrossPlatform)).ToList();
                    break;
                case "ir":
                    model.TestCases = model.TestCases.Where(p => p.Reviewer == user && (p.Status == Status.ReviewStarted||p.Status == Status.ChangesRequested || p.Status == Status.Fixed)).ToList();
                    break;
                default:
                    model.TestCases = model.TestCases.Where(p => p.Assignee == user).ToList();
                    break;
            }
            return View(model);


        }

        public async Task<IActionResult> Account()
        {
            var model = new AccountViewModel();
            var user = await _usersProvider.GetUser(User.GetUserId());
            var userManager = await _userManager.FindByIdAsync(User.GetUserId());
            model.Name = user.Name;
            model.Email = userManager.Email;
            model.GitHubLogin = user.GitHubAccount;
            model.GitHubToken = user.GitHubToken;
            model.GitHubAliases = user.GithubAliases!=null?string.Join(',',user.GithubAliases):null;
            
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Account(AccountViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var user = await _usersProvider.GetUser(User.GetUserId());
                user.GithubAliases = model.GitHubAliases?.Split(',');
                if (model.GitHubLogin != user.GitHubAccount)
                {
                    var apiClient = new GithubApiClient();
                    if (!apiClient.UserExists(model.GitHubLogin)) throw new Exception("GitHub User with provided login does not exist.");
                        
                }
                user.GitHubAccount = model.GitHubLogin;
                if (model.GitHubToken != user.GitHubToken)
                {
                    var client = new GithubApiClient(user.GitHubAccount, model.GitHubToken);
                    if (!client.CredentialsAreValid()) throw new Exception("Specified token is not valid");
                }
                user.GitHubToken = model.GitHubToken;
                user.Name = model.Name;
                await _usersProvider.UpdateUser(user);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(model);
        }

        public IActionResult SidebarHide(bool collapsed)
        {
            HttpContext.Session.SetString("isCollapsed", collapsed ? "layout-collapsed" : "");
            return Json("OK");
        }
    }
}