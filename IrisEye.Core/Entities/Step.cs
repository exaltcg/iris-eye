using System;
using System.Collections.Generic;

namespace IrisEye.Core.Entities
{
    public class Step
    {
        public long Id { get; protected set; }
        public Test Test { get; set; }
        public IList<StepAnalysisItem> StepAnalysisItem { get; set; } = new List<StepAnalysisItem>();
        public DateTime Issued { get; set; }
        public bool IsPassed { get; set; }
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        
    }

}