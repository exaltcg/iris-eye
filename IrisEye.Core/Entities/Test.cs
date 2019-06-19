using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IrisEye.Core.Entities
{
    public class Test
    {
        public long Id { get; protected set; }
        
        public Run Run { get; set; }
        public string Name { get; set; }
        
        public string SuiteName { get; set; }
        public string Description { get; set; }

        public TestStatus Status { get; set; }
               
        public IList<Step> Steps { get; set; }
        public AnalysisResult AnalysisResult { get; set; }
    }

    public enum TestStatus
    {
        Unidentified = 0,
        Passed = 1,
        Failed = 2,
        Error = 3
    }

  
}