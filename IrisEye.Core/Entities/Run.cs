using System;
using System.Collections.Generic;

namespace IrisEye.Core.Entities
{
    public class Run
    {
        public long Id { get; protected set; }
        public string ReportHash { get; set; }
        
        public DateTime AddedOn { get; set; }
        
        public DateTime ReportTime { get; set; }
        public string Environment { get; set; }
        public string Version { get; set; }
        public string Build { get; set; }
        public string BetaChannel { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public int Blocked { get; set; }
        public int Total { get; set; }
        public int Errors { get; set; }
        public string[] FailedTests { get; set; }
        public IList<Test> Tests { get; set; }
        
        public Folder Folder { get; set; }
    }
}