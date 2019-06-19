using System;

namespace IrisEye.Web.Models
{
    public class StatsViewModel
    {
        public string Date { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public int Errors { get; set; }
    }
}