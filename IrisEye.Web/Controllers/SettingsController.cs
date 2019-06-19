using System;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Web.ActionsFilters;
using IrisEye.Web.Models;
using IrisEye.Web.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IrisEye.Web.Controllers
{
    [ServiceFilter(typeof(AvailableRunsServiceFilter))]
    [Authorize]
    public class SettingsController : Controller
    {

        private readonly IFoldersProvider _folders;
        private readonly IRunsProvider _runs;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IUsersProvider _usersProvider;

        public SettingsController(
            IFoldersProvider folders, 
            IRunsProvider runs, 
            ISchedulerProvider schedulerProvider, IUsersProvider usersProvider)
        {
            _folders = folders;
            _runs = runs;
            _schedulerProvider = schedulerProvider;
            _usersProvider = usersProvider;
        }

        public IActionResult Index()
        {
            return
                View();
        }

        public async Task<IActionResult> DeleteRun(int id)
        {
            await _runs.DeleteRun(id);
            return RedirectToAction("Index");
        }

        public IActionResult NewFolder()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> NewFolder(NewFolderModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var newFolder = new Folder
            {
                Name = model.Name, 
                IsActive = model.IsActive
            };
            try
            {
                await _folders.AddNewFolder(newFolder);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(model);
        }

        public async Task<IActionResult> SwitchFolder(int id)
        {
            await _usersProvider.SwitchFolder(id, HttpContext.User);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> RefreshTestsInfo()
        {
            await _runs.RefreshTestInfo();
            return RedirectToAction("Index");
        }

        public IActionResult GetActiveTasks()
        {
            var schedules = _schedulerProvider.GetActiveSchedules();
            return View(schedules);
        }

        public IActionResult RunScheduleNow(string name)
        {
            _schedulerProvider.RunScheduleNow(name);
            return RedirectToAction("GetActiveTasks");
        }
    }
}