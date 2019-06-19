using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Data.Extensions;
using IrisEye.Data.Parsers;
using IrisEye.Web.ActionsFilters;
using IrisEye.Web.Data;
using IrisEye.Web.Migrations;
using Microsoft.AspNetCore.Mvc;
using IrisEye.Web.Models;
using IrisEye.Web.Providers;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;

namespace IrisEye.Web.Controllers
{
    [ServiceFilter(typeof(AvailableRunsServiceFilter))]
    public class HomeController : Controller
    {
        private readonly IRunsProvider _runsProvider;
        private readonly IFoldersProvider _foldersProvider;
        private readonly IUsersProvider _usersProvider;

        public HomeController(IRunsProvider runsProvider, IFoldersProvider foldersProvider, IUsersProvider usersProvider)
        {
            _runsProvider = runsProvider;
            _foldersProvider = foldersProvider;
            _usersProvider = usersProvider;
        }
        
        public async Task<IActionResult> Index()
        {
            var folder = await _foldersProvider.GetActiveFolder();
            SystemUser user=null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _usersProvider.GetUser(HttpContext.User.GetUserId());

                folder = user.SelectedFolder;
            }

            var platforms = new string[]{"osx", "win7", "win", "linux"};

            var runs = folder.Runs;
           
            var model = new DashboardDataModel
            {
                FailedTests = new List<FailedTestPerPlatform>(),
                Users = await _usersProvider.GetAllUsers()
            };
            foreach (var platform in platforms)
            { 
                model.LatestStats.Add(await _runsProvider.GetLatestRunForEnvironment(platform, folder.Id));
            }
            
            foreach (var run in runs.GroupBy(p=>p.Environment).OrderBy(p=>p.Key))
            {
                var runner = run.OrderByDescending(p => p.ReportTime).FirstOrDefault();
                model.LatestResults.Add(runner);
                var allTestsInfo = await _runsProvider.GetAllTestsInfo();
                foreach (var test in runner.FailedTests)
                {
                    var testInfo = allTestsInfo.FirstOrDefault(p => p.TestName == test + ".py");
                    var failedTest = new FailedTestPerPlatform
                    {
                        LinuxRunId = runs.Where(p=>p.Environment=="linux").OrderByDescending(p=>p.ReportTime).FirstOrDefault()?.Id,
                        OsxRunId = runs.Where(p=>p.Environment=="osx").OrderByDescending(p=>p.ReportTime).FirstOrDefault()?.Id,
                        Win10RunId = runs.Where(p=>p.Environment=="win").OrderByDescending(p=>p.ReportTime).FirstOrDefault()?.Id,
                        Win7RunId = runs.Where(p=>p.Environment=="win7").OrderByDescending(p=>p.ReportTime).FirstOrDefault()?.Id,
                        TestId = runs.OrderByDescending(p=>p.ReportTime).Where(p=>p.Tests!=null).SelectMany(z=>z.Tests).FirstOrDefault(q=>q.Name==test)?.Id,
                        Test = test,
                        Author = testInfo?.AuthorLogin,
                        Suit = testInfo?.SuiteName,
                        CurrentUser = user,
                        Linux = runs.OrderByDescending(p=>p.ReportTime).FirstOrDefault(p=>p.Environment=="linux")?.FailedTests.ToList().Exists(p=>p==test),
                        Windows7 = runs.OrderByDescending(p=>p.ReportTime).FirstOrDefault(p=>p.Environment=="win7")?.FailedTests.ToList().Exists(p=>p==test),
                        Osx = runs.OrderByDescending(p=>p.ReportTime).FirstOrDefault(p=>p.Environment=="osx")?.FailedTests.ToList().Exists(p=>p==test),
                        Windows10 = runs.OrderByDescending(p=>p.ReportTime).FirstOrDefault(p=>p.Environment=="win")?.FailedTests.ToList().Exists(p=>p==test)
                        
                    };
                    
                    model.FailedTests.Add(failedTest);

                }
                
            }

            model.FailedTests = model.FailedTests.Distinct().ToList();
            
            //Historic data
            var groupedEnvironment = runs.GroupBy(p => p.Environment);
            model.HistoricData = new List<HistoricData>();
            foreach (var grouping in groupedEnvironment)
            {
                var newHistoricData = new HistoricData {Platform = grouping.Key};
                var groupedDate = grouping.GroupBy(p => p.ReportTime.Date).OrderBy(p=>p.Key);
                newHistoricData.HistoricDataItems = new List<HistoricDataItems>();
                foreach (var dt in groupedDate)
                {
                    var item = dt.OrderByDescending(p => p.Total).FirstOrDefault();
                    if (item != null)
                        newHistoricData.HistoricDataItems.Add(new HistoricDataItems
                        {
                            DateLabel = dt.Key.ToShortDateString(),
                            Blocked = item.Blocked,
                            Failed = item.Failed,
                            Passed = item.Passed,
                            Skipped = item.Skipped,
                            Total = item.Total
                        });
                }
                model.HistoricData.Add(newHistoricData);
            }
            
            return View(model);
        }

        [Authorize]
        public IActionResult Uploader()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> TestsExplorer(int id, string suit, bool failedOnly)
        {
           var run = await _runsProvider.GetRun(id);
           var testsInfo = await _runsProvider.GetAllTestsInfo();
           testsInfo = testsInfo.ToList();
           
           var model = new TestsExplorerViewModel
           {
               Environment = run.Environment, 
               ReportTime = run.ReportTime,
               RunId = run.Id
           };
           foreach (var test in run.Tests)
           {
               var testSuite = testsInfo.FirstOrDefault(p => p.TestName == test.Name+".py");
               if (testSuite == null) continue;
               {
                   var exists = model.TestSets.FirstOrDefault(p => p.Name == testSuite.SuiteName);
                   if (exists != null)
                   {
                       //add new tests to existing suite
                       var newTest = new ViewTest
                       {
                           Name = test.Name, 
                           Description = test.Description, 
                           Author = testSuite.AuthorLogin,
                           Failed = test.Steps.Count(p=>!p.IsPassed)!=0
                       };
                       foreach (var testStep in test.Steps)
                       {
                           newTest.Steps.Add(new ViewStep
                           {
                               DateTime = testStep.Issued,
                               Message = testStep.Message,
                               StackTrace = testStep.Stacktrace,
                               IsPassed = testStep.IsPassed
                           });
                       }
                       exists.Tests.Add(newTest);
                   }
                   else
                   {
                       
                       var newTestSuite = new TestSet
                       {
                           Name = testSuite.SuiteName,
                           Id = testSuite.Id
                       };
                       var newTest = new ViewTest
                       {
                           Name = test.Name, 
                           Description = test.Description, 
                           Author = testSuite.AuthorLogin,
                           Failed = test.Steps.Count(p=>!p.IsPassed)!=0
                       };
                       foreach (var testStep in test.Steps)
                       {
                           newTest.Steps.Add(new ViewStep
                           {
                               DateTime = testStep.Issued,
                               Message = testStep.Message,
                               StackTrace = testStep.Stacktrace,
                               IsPassed = testStep.IsPassed
                           });
                       }
                       newTestSuite.Tests.Add(newTest);
                       model.TestSets.Add(newTestSuite);
                   }
               }
           }
            
           foreach (var testSet in model.TestSets)
           {
               testSet.FailedTests = testSet.Tests.Count(p => p.Failed);
           }
           
           //display all tests
           var allTestSet = new TestSet
           {
               Name = "All", 
               FailedTests = run.Tests.Count(p => p.Steps.ToList().Exists(z => !z.IsPassed)),
               Id = 0
           };
           allTestSet.Tests = new List<ViewTest>();
           run.Tests.ToList().ForEach(p=>allTestSet.Tests.Add(new ViewTest
           {
               Name = p.Name,
               Description = p.Description,
               Failed = p.Steps.ToList().Exists(z=>!z.IsPassed),
               Steps = p.Steps.Select(q=>new ViewStep(q)).ToList(),
               Author = testsInfo.FirstOrDefault(z=>z.TestName==(p.Name+".py"))?.AuthorLogin
               
           }));
           model.TestSets.Add(allTestSet);


           model.FilteredTestSets = model.TestSets.Clone();
           
           if (!string.IsNullOrEmpty(suit))
           {
               model.SelectedSuite = suit;
               model.FilteredTestSets = model.FilteredTestSets.Where(p => p.Name == suit).ToList();
           }

           if (!failedOnly) return View(model);
           {
               foreach (var testSets in model.FilteredTestSets)
               {
                   testSets.Tests = testSets.Tests.Where(p => p.Failed).ToList();
               }
           }
           
           return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Uploader(List<IFormFile> files)
        {
            try
            {
                var testInfos = await _runsProvider.GetAllTestsInfo();
                var parser = new LogParser(testInfos.ToList());
                var size = files.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = Path.GetTempFileName();

                foreach (var formFile in files)
                {
                    if (formFile.Length <= 0) continue;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    parser.LoadFromFile(filePath, formFile.FileName);
                    var newRun = parser.Parse();
                    var user = await _usersProvider.GetUser(User.GetUserId());
                    if (user!=null)
                    {
                    await _runsProvider.AddNewRun(newRun, user.SelectedFolder.Id);
                    }

                }

                return RedirectToAction("Index", "Home");

            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Search(string query)
        {
            var model = new SearchResultsModel {Query = query};
            return View(model);
        }

        [Authorize]
        [HttpGet]
            public async Task<IActionResult> MigrateTestStatuses()
        {
            await _runsProvider.MigrateTestStatuses();
            return RedirectToAction("Index");
        }

      
        
        

    }
}