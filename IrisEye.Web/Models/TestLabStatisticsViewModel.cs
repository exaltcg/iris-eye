using System.Collections.Generic;
using IrisEye.Core.Entities;
using IrisEye.Web.Enums;

namespace IrisEye.Web.Models
{
    public class TestLabStatisticsViewModel
    {
        public IEnumerable<TestCase> Tests { get; set; }
        public int TotalTests { get; set; }
        public int TestsInProgress { get; set; }
        public int AwaitingReview { get; set; }
        public int AwaitingMerge { get; set; }
        
        public string Filter { get; set; }
        public StatisticsPeriod Period { get; set; }
    }
}