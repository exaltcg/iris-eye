using System;

namespace IrisEye.Web.Models
{
    public class LatestStatsModel
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Errors { get; set; }
        public int NewIssues { get; set; }
        public int NotAnalyzed { get; set; }
        public int KnownIssues { get; set; }
        public int CannotReproduce { get; set; }
        public int Total { get; set; }
        public string Environment { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }
        public decimal Skipped { get; set; }
        public long RunId { get; set; }
    }
}