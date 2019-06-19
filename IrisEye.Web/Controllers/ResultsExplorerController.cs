using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Data.Extensions;
using IrisEye.Web.ActionsFilters;
using IrisEye.Web.Enums;
using IrisEye.Web.Models;
using IrisEye.Web.Providers;
using IrisEye.Web.Utils;
using JW;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Utilities;

namespace IrisEye.Web.Controllers
{
    [ServiceFilter(typeof(AvailableRunsServiceFilter))]
    public class ResultsExplorerController : Controller
    {
        private readonly ITestLabProvider _testLabProvider;
        private readonly IRunsProvider _runsProvider;
        private readonly IUsersProvider _usersProvider;
        private readonly IFoldersProvider _foldersProvider;

        public ResultsExplorerController(ITestLabProvider testLabProvider, IRunsProvider runsProvider, IUsersProvider usersProvider, IFoldersProvider foldersProvider)
        {
            _testLabProvider = testLabProvider;
            _runsProvider = runsProvider;
            _usersProvider = usersProvider;
            _foldersProvider = foldersProvider;
        }

        // GET
        public async Task<IActionResult> Index(int id, string suit, int filterOption, string query, string userFilter, int page=1)
        {
            var filter = (TestExplorerFilter) filterOption;
            if (string.IsNullOrEmpty(suit))
            {
                suit = "ALL";
            }
            var run = await _runsProvider.GetRun(id);
            var testsInfo = await _runsProvider.GetAllTestsInfo();
            var testInfos = testsInfo as TestInfo[] ?? testsInfo.ToArray();
            var testInfoTests = testInfos.Select(p => p.TestName).ToList();
            testsInfo = testInfos.ToList();
           
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
                        var newTest = new ViewTest(test, testInfos);
                        exists.Tests.Add(newTest);
                    }
                    else
                    {
                       
                        var newTestSuite = new TestSet
                        {
                            Name = testSuite.SuiteName,
                            Id = testSuite.Id
                        };
                        var newTest = new ViewTest(test, testInfos);
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
                Name = "ALL", 
                FailedTests = run.Tests.Count(p => p.Steps.ToList().Exists(z => !z.IsPassed)),
                Id = -1,
            };
            allTestSet.Tests = new List<ViewTest>();
            run.Tests.ToList().ForEach(p=>allTestSet.Tests.Add(new ViewTest(p, testInfos)
            ));
            model.TestSets.Add(allTestSet);

            var notAssignedTests = run.Tests.Where(p => !testInfoTests.Contains(p.Name + ".py")).ToList();
            //Display not assigned
            var notAssignedTestSet = new TestSet
            {
                Name = "NOT ASSIGNED",
                FailedTests = 0,
                Id = 0,
                Tests = new List<ViewTest>()
            };
            notAssignedTests.ToList().ForEach(p=>allTestSet.Tests.Add(new ViewTest(p, testInfos)
            ));
            model.TestSets.Add(notAssignedTestSet);



            model.FilteredTestSets = model.TestSets.Clone();

           
            if (!string.IsNullOrEmpty(suit))
            {
                model.SelectedSuite = suit;
                model.FilteredTestSets = model.FilteredTestSets.Where(p => p.Name == suit).ToList();
            }

            switch (filter)
            {
                case TestExplorerFilter.Total:
                    break;
                case TestExplorerFilter.Passed:
                    foreach (var testSets in model.FilteredTestSets)
                    {
                        testSets.Tests = testSets.Tests.Where(p => p.Status == TestStatus.Passed).ToList();
                    }

                    break;
                case TestExplorerFilter.Failed:
                    foreach (var testSets in model.FilteredTestSets)
                    {
                        testSets.Tests = testSets.Tests.Where(p => p.Status == TestStatus.Failed).ToList();
                    }
                    break;
                case TestExplorerFilter.Errors:
                    foreach (var testSets in model.FilteredTestSets)
                    {
                        testSets.Tests = testSets.Tests.Where(p => p.Status == TestStatus.Error).ToList();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

          
            model.TestExplorerFilter = filter;
            model.Query = query;

            if (!string.IsNullOrEmpty(query))
            {
                foreach (var testSet in model.FilteredTestSets)
                {
                    if (query.Contains("!"))
                    {
                        testSet.Tests = testSet.Tests.Where(p => p.Name.Equals(query.Replace("!",""))).ToList();
                    }
                    else
                    {
                        testSet.Tests = testSet.Tests.Where(p => p.Name.Contains(query)).ToList();
                    }
                    
                }                
            }
            
            model.Users = testInfos.Select(p => p.AuthorLogin).Distinct();
            if (userFilter != null)
            {
                model.UserFilter = userFilter;
                foreach (var testSet in model.FilteredTestSets)
                {
                    testSet.Tests = testSet.Tests.Where(p => p.Author == userFilter).ToList();
                }
            }
            
            model.Pager = new Pager(model.FilteredTestSets.SelectMany(p=>p.Tests).Count(), page);


            return View(model);
        }

        public async Task<IActionResult> CompareEnvironments(int id)
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
            return View(model);

        }

        [Authorize]
        public async Task<IActionResult> AnalyzeTest(long id)
        {
            var user = await _usersProvider.GetUser(User.GetUserId());
            var testInfos = await _runsProvider.GetAllTestsInfo();
            var model = new AnalyzeTestViewModel();
            var test = await _runsProvider.GetTest(id);
            var viewTest = new ViewTest(test, testInfos);
            model.TestId = id;
            model.CurrentUser = user;
            model.Test = viewTest;
            model.Run = test.Run;
            var failedStep = test.Steps.FirstOrDefault(p => !p.IsPassed);
            model.SelectedStepId = failedStep?.Id;
            model.Title = $@"Fix {viewTest.Name} test";
            model.Message =
                $@"Can not reproduce failure described at step: ""{failedStep?.Message.Trim()}"" for {test.Run.Environment.ConvertEnvironment()}";
            if (failedStep?.Stacktrace != null)
            {
                model.Message +=
                    $@" with the following stacktrace:{Environment.NewLine + failedStep?.Stacktrace.Replace("<<", "")}";
            }
            
            model.NewIssueMessage = $@"The test fails at step: ""{failedStep?.Message.Trim()}"" for {test.Run.Environment.ConvertEnvironment()}";
            if (failedStep?.Stacktrace != null)
            {
                model.NewIssueMessage +=
                    $@" with the following stacktrace:{Environment.NewLine + failedStep?.Stacktrace.Replace("<<", "")}";
            }

            model.KnownIssueMessage =
                $@"The failure is still reproducible for {test.Run.Environment.ConvertEnvironment()}";

            var testSuits = await _testLabProvider.GetAllTestSuits();
            model.TestSuitsModel = testSuits.OrderByDescending(p=>p.Id).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            });
            model.Resolutions = ((AnalysisResolution[]) Enum.GetValues(typeof(AnalysisResolution))).Select(p =>
                new SelectListItem
                {
                    Text = p.GetDescription(),
                    Value = ((int)p).ToString()
                });
            model.TestSuitsModel = model.TestSuitsModel.OrderByDescending(p => int.Parse(p.Value));
            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AnalyzeTest(AnalyzeTestViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var testInfos = await _runsProvider.GetAllTestsInfo();
                var test = await _runsProvider.GetTest(model.TestId);
                var viewTest = new ViewTest(test, testInfos);
                model.Test = viewTest;
                model.Run = test.Run;
                var user = await _usersProvider.GetUser(User.GetUserId());
                var analyzeResolution = (AnalysisResolution) model.SelectedResolutionId;
                if (!model.SelectedStepId.HasValue) throw new Exception("Step was not specified");
                await _runsProvider.SetAnalysisState(model.SelectedStepId.Value, analyzeResolution, model.IsExistingIssue,
                    model.ExistingIssueId, model.Title, model.Message, model.SelectedSuiteId, user);
                return RedirectToAction("AnalyzeTest", "ResultsExplorer", new {@id = model.TestId});
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> FailedTests(long runId)
        {
            var testInfos = await _runsProvider.GetAllTestsInfo();
            var testInfosList = testInfos.ToList();
            var model = new FailedTestsViewModel();
            var folder = await _foldersProvider.GetActiveFolder();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _usersProvider.GetUser(User.GetUserId());
                folder = user.SelectedFolder;
            }

            var runs = await _runsProvider.GetRunsForFolder(folder);
            var grouped = runs.GroupBy(p => p.ReportTime.Date);
            foreach (var item in grouped)
            {
                model.Runs.Add(item.OrderByDescending(p => p.Tests.Count).FirstOrDefault());
            }

            if (runId == 0)
            {
                model.SelectedRunDate = model.Runs.OrderByDescending(p => p.ReportTime).FirstOrDefault().ReportTime;
            }
            else
            {
                var run = await _runsProvider.GetRun(runId);
                model.SelectedRunDate = run.ReportTime;
            }

            var allRunsForDate = await _runsProvider.GetAllRunsForDate(model.SelectedRunDate, folder);
            var allTestsForDate = allRunsForDate.SelectMany(p => p.Tests).ToList();
            foreach (var failedTest in allTestsForDate.Where(p=>p.Steps.Any(step=>!step.IsPassed)))
            {
                var existingTest = model.FailedTests.FirstOrDefault(p => p.Name == failedTest.Name);
                if (existingTest == null)
                {
                    
                    model.FailedTests.Add(new FailedTests(failedTest, testInfosList));
                }
                else
                {
                    switch (failedTest.Run.Environment)
                    {
                        case "osx":
                            existingTest.Osx = failedTest.Status;
                            break;
                        case "linux":
                            existingTest.Linux = failedTest.Status;
                            break;
                        case "win":
                            existingTest.Win10 = failedTest.Status;
                            break;
                        case "win7":
                            existingTest.Win7 = failedTest.Status;
                            break;           
                    }
                }
            }

            var notFailed = allTestsForDate.Where(p => p.Steps.All(step => step.IsPassed)).ToList();
            foreach (var failedTest in model.FailedTests)
            {
                var notFailedOsx = notFailed.FirstOrDefault(p => p.Name == failedTest.Name && p.Run.Environment=="osx");
                if (notFailedOsx != null)
                {
                    failedTest.Osx = notFailedOsx.Status;
                }
                var notFailedLinux = notFailed.FirstOrDefault(p => p.Name == failedTest.Name && p.Run.Environment=="linux");
                if (notFailedLinux != null)
                {
                    failedTest.Linux = notFailedLinux.Status;
                }
                var notFailedWin10 = notFailed.FirstOrDefault(p => p.Name == failedTest.Name && p.Run.Environment=="win");
                if (notFailedWin10 != null)
                {
                    failedTest.Win10 = notFailedWin10.Status;
                }
                var notFailedWin7 = notFailed.FirstOrDefault(p => p.Name == failedTest.Name && p.Run.Environment=="win7");
                if (notFailedWin7 != null)
                {
                    failedTest.Win7 = notFailedWin7.Status;
                }
                
                    
                
            }

            model.TestSuites = await _testLabProvider.GetAllTestSuits();

            return View(model);
        }
    }
}