using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IrisEye.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IrisEye.Web.Models
{
    public class AnalyzeTestViewModel
    {
        public long TestId { get; set; }
        public SystemUser CurrentUser { get; set; }
        public ViewTest Test { get; set; }
        public Run Run { get; set; }
        public AnalysisResolution? AnalysisResolution { get; set; }
        
        public long SelectedSuiteId { get; set; }
        public int SelectedResolutionId { get; set; }
        public bool IsExistingIssue { get; set; }
        public IEnumerable<SelectListItem> TestSuitsModel { get; set; }
        public IEnumerable<SelectListItem> Resolutions { get; set; }
        
        [Display(Name = "Title")]
        public string Title { get; set; }
        
        [Display(Name = "Message")]
        public string Message { get; set; }

        public int ExistingIssueId { get; set; }
        
        public long? SelectedStepId { get; set; }
        
        public string KnownIssueMessage { get; set; }
        public string NewIssueMessage { get; set; }
    }

  
}