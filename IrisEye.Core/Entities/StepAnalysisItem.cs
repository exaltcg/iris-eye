using System;
using System.ComponentModel;

namespace IrisEye.Core.Entities
{
    public class StepAnalysisItem
    {
        public long Id { get; protected set; }
        public long ParentStepId { get; set; }
        public Step ParentStep { get; set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
        public AnalysisResolution AnalysisResolution { get; set; }
        public SystemUser AddedBy { get; set; }
        public string Message { get; set; }
        public int GitHubId { get; set; }
        public int BugzillaId { get; set; }
        
    }

    public enum AnalysisResolution
    {
        [Description("Can't reproduce")]
        CannotReproduce = 0,
        [Description("Known issue")]
        KnownIssue = 1,
        [Description("New issue")]
        NewIssue = 2
        
        
        
    }
}