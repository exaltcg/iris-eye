using System;
using System.ComponentModel;
using IrisEye.Core.Models;

namespace IrisEye.Core.Entities
{
    public class AnalysisResult
    {
        public long Id { get; protected set; }
        public virtual Step IdentifiedAt { get; set; }
        public virtual SystemUser By { get; set; }
        public string Message { get; set; }
        public AnalysisStatus AnalysisStatus { get; set; }
        public DateTime StartedOn { get; set; } = DateTime.Now;
        public DateTime FinishedOn { get; set; }
        public int GitHubId { get; set; }
        
    }

    public enum AnalysisStatus
    {
        [Description("Analysis Started")]
        AnalysisStarted = 0,
        [Description("New Issue")]
        NewIssue = 2,
        [Description("Known Issue")]
        KnownIssue = 3,
        [Description("Can't reproduce")]
        CantReproduce = 4
    }
}