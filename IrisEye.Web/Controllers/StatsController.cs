using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;
using IrisEye.Data.Extensions;
using IrisEye.Web.Migrations;
using IrisEye.Web.Models;
using IrisEye.Web.Providers;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrisEye.Web.Controllers
{
    public class StatsController : Controller
    {
        private readonly IRunsProvider _runsProvider;
        private readonly IUsersProvider _usersProvider;
        private readonly IFoldersProvider _foldersProvider;

        public StatsController(IRunsProvider runsProvider, IUsersProvider usersProvider, IFoldersProvider foldersProvider)
        {
            _runsProvider = runsProvider;
            _usersProvider = usersProvider;
            _foldersProvider = foldersProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestStatsForEnvironment(string env, long folderId)
        {
            var stats = await _runsProvider.GetLatestRunForEnvironment(env, folderId);
            if (stats == null) return NotFound("Requested environment data were not found.");
            return Json(stats);
        }

        [HttpPost]
        public async Task<IActionResult> GetStatsForRun(string environment)
        {
            var model = new List<StatsViewModel>();
            var folder = await _foldersProvider.GetActiveFolder();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _usersProvider.GetUser(User.GetUserId());
                folder = user.SelectedFolder;
            }
            var items = await _runsProvider.GetLatestRunsForEnvironment(environment, folder);
            foreach (var item in items.OrderBy(p=>p.ReportTime))
            {
                model.Add(new StatsViewModel
                {
                    Date = item.ReportTime.ToString("d"),
                    Passed = item.Passed,
                    Failed=item.Failed,
                    Errors=item.Errors,
                    Skipped = item.Skipped
                });
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetComparedTests(long id)
        {
            var folder = await _foldersProvider.GetActiveFolder();
            if (User.Identity.IsAuthenticated)
            {
                var user =await _usersProvider.GetUser(User.GetUserId());
                folder = user.SelectedFolder;
            }
            var runs = await _runsProvider.GetTestsForComparison(id, folder);
            var testInfo = await _runsProvider.GetAllTestsInfo();
            var model = new List<ViewTest>();
            foreach (var item in runs)
            {
                var info = testInfo.FirstOrDefault(p => p.TestName == item?.Name + ".py");
                if (info == null) continue;
                var newTest = new ViewTest(item)
                {
                    SuitName = info.SuiteName, 
                    Author = info.AuthorLogin
                };
                model.Add(newTest);
            }

            return PartialView("ResultsExplorer/_ResultsComparisonPartial", model);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> StartAnalysis(long testId, long stepId)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            var analysisState = await _runsProvider.StartAnalysis(testId, stepId, user);
            if (user != analysisState.By && analysisState.AnalysisStatus == AnalysisStatus.AnalysisStarted)
            {
                return NotFound(
                    $@"The analysis was already started by {analysisState.By.Name} {analysisState.StartedOn.Humanize(true, DateTime.Now)}");
            }
            return Json(new AnalysisViewModel(analysisState));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetAnalysis(
            int status, 
            long analysisId, 
            int testSuiteId, 
            int issueId, 
            string title, 
            string message, 
            bool isAlreadyExists)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            var analysisResult = await _runsProvider.SetTestAnalysisState(user, testSuiteId, status, analysisId, issueId, title, message, isAlreadyExists);
            return Json(new AnalysisViewModel(analysisResult));
        }
        
        
    }
}