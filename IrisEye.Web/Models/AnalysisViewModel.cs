using System;
using IrisEye.Core.Entities;
using IrisEye.Data.Extensions;

namespace IrisEye.Web.Models
{
    public class AnalysisViewModel
    {
        public AnalysisViewModel()
        {
            
        }

        public AnalysisViewModel(AnalysisResult analysisResult)
        {
            AnalysisStatus = analysisResult.AnalysisStatus;
            AnalysisStatusDescription = analysisResult.AnalysisStatus.GetDescription();
            StepId = analysisResult.IdentifiedAt.Id;
            Message = analysisResult.Message;
            User = analysisResult.By.Name;
            StartedOn = analysisResult.StartedOn;
            FinishedOn = analysisResult.FinishedOn;
            Id = analysisResult.Id;
            GitHubId = analysisResult.GitHubId;
        }

        public int GitHubId { get; set; }

        public string AnalysisStatusDescription { get; set; }

        public long Id { get; set; }
        public DateTime FinishedOn { get; set; }

        public DateTime StartedOn { get; set; }

        public AnalysisStatus AnalysisStatus { get; set; }
        public long StepId { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
    }
}