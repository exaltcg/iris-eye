using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IrisEye.Core.Entities;

namespace IrisEye.Web.Models
{
    public class FailedTestsViewModel
    {
        public IList<Run> Runs { get; set; } = new List<Run>();
        public DateTime SelectedRunDate { get; set; }
        public IList<FailedTests> FailedTests { get; set; } = new List<FailedTests>();
        public IEnumerable<TestSuite> TestSuites { get; set; }
    }

    public class FailedTests
    {
        public FailedTests(Test test, IEnumerable<TestInfo> testInfos)
        {
            Name = test.Name;
            TestId = test.Id;
            AnalysisResult = test.AnalysisResult;
            var testInfo = testInfos.FirstOrDefault(p => p.TestName == test.Name + ".py");
            if (testInfo != null)
            {
                Suite = testInfo.SuiteName;
                Author = testInfo.AuthorLogin;
            }
            switch (test.Run.Environment)
            {
                case "osx":
                    Osx = test.Status;
                    break;
                case "linux":
                    Linux = test.Status;
                    break;
                case "win":
                    Win10 = test.Status;
                    break;
                case "win7":
                    Win7 = test.Status;
                    break;           
            }
        }
        
        public long TestId { get; set; }

        public string Author { get; set; }

        public string Suite { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public TestStatus Osx { get; set; }
        public TestStatus Linux { get; set; }
        public TestStatus Win10 { get; set; }
        public TestStatus Win7 { get; set; }
        
        public AnalysisResult AnalysisResult { get; set; }
    }
}