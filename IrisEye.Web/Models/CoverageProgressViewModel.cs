using System.Collections.Generic;
using IrisEye.Core.Entities;
using IrisEye.Web.Enums;

namespace IrisEye.Web.Models
{
    public class CoverageProgressViewModel
    {
        public StatisticsPeriod StatisticsPeriod { get; set; }
        public IList<TestCase> IssuesCovered { get; set; }
        public IEnumerable<TestSuite> AllTestSuits { get; set; }
        public double TestsVelocity { get; set; }
        public double IssuesVelocity { get; set; }
        public double TotalCoverage { get; set; }
    }
}