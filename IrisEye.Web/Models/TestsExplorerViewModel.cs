using System;
using System.Collections.Generic;
using System.Linq;
using IrisEye.Core.Entities;
using IrisEye.Web.Enums;
using JW;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace IrisEye.Web.Models
{
    public class TestsExplorerViewModel
    {
        public string UserFilter { get; set; }
        public Pager Pager { get; set; }
        public long RunId { get; set; }
        public List<TestSet> TestSets { get; set; } = new List<TestSet>();
        public List<TestSet> FilteredTestSets { get; set; } = new List<TestSet>();
        public DateTime ReportTime { get; set; }
        public string Environment { get; set; }
        public string SelectedSuite { get; set; }
        public TestExplorerFilter TestExplorerFilter { get; set; }
        public string Query { get; set; }
        
        public IEnumerable<string> Users { get; set; }
    }

    public class TestSet
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<ViewTest> Tests { get; set; } = new List<ViewTest>();
        public int TotalTests { get; set; }
        public int FailedTests { get; set; }
        public int AnalysedFails { get; set; }
    }

    public class ViewTest
    {
        public ViewTest()
        {
            
        }

        public ViewTest(Test test)
        {
            Init(test);
        }

        private void Init(Test test)
        {
            Name = test.Name;
            Description = test.Description;
            Failed = test.Steps.Any(p => !p.IsPassed);
            Environment = test.Run.Environment;
            Steps = test.Steps.Select(p => new ViewStep(p)).ToList();
            TestId = test.Id;
            Date = test.Steps.OrderBy(p => p.Issued).FirstOrDefault()?.Issued;
            Status = test.Status;
        }

        public ViewTest(Test test, IEnumerable<TestInfo> testInfos)
        {
            Init(test);
            var existingInfo = testInfos.FirstOrDefault(p => p.TestName == test.Name + ".py");
            if (existingInfo == null) return;
            Author = existingInfo.AuthorLogin;
            SuitName = existingInfo.SuiteName;
        }
        public string Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Failed { get; set; }
        public List<ViewStep> Steps { get; set; } = new List<ViewStep>();
        public string SuitName { get; set; }
        public string Environment { get; set; }
        public long TestId { get; set; }
        public DateTime? Date { get; set; }

        public TestStatus Status { get; set; }

    }

    public class ViewStep
    {
        public ViewStep()
        {
            
        }

        public ViewStep(Step step)
        {
            Message = step.Message;
            StackTrace = step.Stacktrace;
            DateTime = step.Issued;
            IsPassed = step.IsPassed;
            StepAnalysisItems = step.StepAnalysisItem;
            StepId = step.Id;
        }
        
        public long StepId { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        
        public bool IsPassed { get; set; }

        public IList<StepAnalysisItem> StepAnalysisItems;
    }
    
}