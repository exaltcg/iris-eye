using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IrisEye.Core.Entities
{
    public class TestCase
    {
        public long Id { get; protected set; }
        public int TestRailId { get; set; }
        public int GitHubId { get; set; }
        
        public bool IsIssue { get; set; }
        public TestSuite TestSuite { get; set; }
        public string Name { get; set; }
        public SystemUser Assignee { get; set; }
        public SystemUser Reviewer { get; set; }
        public Status Status { get; set; }
        public DateTime StartedOn { get; set; }
        
        public DateTime AddedOn { get; set; } = DateTime.Now;
        public DateTime FinishedOn { get; set; }
        public DateTime? MergedDate { get; set; }
        public string Preconditions { get; set; }
        public IList<TestStep> TestSteps { get; set; }
        public IList<TestCaseHistory> HistoryItems { get; set; } = new List<TestCaseHistory>();
        
        public IList<TestCaseComment> TestCaseComments { get; set; } = new List<TestCaseComment>();
        public string Message { get; set; }
        
        public int PullRequestId { get; set; }
        public bool AttentionRequired { get; set; }
    }

    public enum Status
    {
        [Description("New")]
        New = 0,
        
        [Description("Ready for automation")]
        ReadyForAutomation = 1,
        
        [Description("In progress")]
        InProgress = 2,
        
        [Description("To cross-platform")]
        ToCrossPlatform = 3,
        
        [Description("Ready for review")]
        ReadyForReview = 4,
        
        [Description("Review started")]
        ReviewStarted = 5,
        
        [Description("Cannot automate")]
        CannotAutomate = 6,
        
        [Description("Finished")]
        Finished = 7,
        
        [Description("Changes Requested")]
        ChangesRequested = 8,
        
        [Description("Fixed")]
        Fixed = 9,
        
        [Description("Closed")]
        Closed = 10
        
    }
}