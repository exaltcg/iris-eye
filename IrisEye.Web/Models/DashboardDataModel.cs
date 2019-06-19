using System;
using System.Collections;
using System.Collections.Generic;
using IrisEye.Core.Entities;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace IrisEye.Web.Models
{
    public class DashboardDataModel
    {
        public IList<Run> LatestResults { get; set; } = new List<Run>();
        public IList<FailedTestPerPlatform> FailedTests { get; set; }
        
        public IList<HistoricData> HistoricData { get; set; }
        
        public IList<SystemUser> Users { get; set; }
        public IList<LatestStatsModel> LatestStats { get; set; } = new List<LatestStatsModel>();
    }

    public class FailedTestPerPlatform : IEquatable<FailedTestPerPlatform>
    {
        public SystemUser CurrentUser { get; set; }
       public string Test { get; set; }
       public long? TestId { get; set; }
       public string Suit { get; set; }
       public string Author { get; set; }
       public bool? Linux { get; set; }
       public bool? Windows10 { get; set; }
       public bool? Windows7 { get; set; }
       public bool? Osx { get; set; }
       public long? LinuxRunId { get; set; }
       public long? OsxRunId { get; set; }
       public long? Win10RunId { get; set; }
       public long? Win7RunId { get; set; }

       public bool Equals(FailedTestPerPlatform other)
       {
           if (ReferenceEquals(null, other)) return false;
           if (ReferenceEquals(this, other)) return true;
           return string.Equals(Test, other.Test) && string.Equals(Suit, other.Suit) && Linux == other.Linux && Windows10 == other.Windows10 && Windows7 == other.Windows7 && Osx == other.Osx;
       }

       public override bool Equals(object obj)
       {
           if (ReferenceEquals(null, obj)) return false;
           if (ReferenceEquals(this, obj)) return true;
           if (obj.GetType() != this.GetType()) return false;
           return Equals((FailedTestPerPlatform) obj);
       }

       public override int GetHashCode()
       {
           unchecked
           {
               var hashCode = (Test != null ? Test.GetHashCode() : 0);
               hashCode = (hashCode * 397) ^ (Suit != null ? Suit.GetHashCode() : 0);
               hashCode = (hashCode * 397) ^ Linux.GetHashCode();
               hashCode = (hashCode * 397) ^ Windows10.GetHashCode();
               hashCode = (hashCode * 397) ^ Windows7.GetHashCode();
               hashCode = (hashCode * 397) ^ Osx.GetHashCode();
               return hashCode;
           }
       }
    }

    public class HistoricData
    {
        public string Platform { get; set; }
        public IList<HistoricDataItems> HistoricDataItems { get; set; }
    }
    public class HistoricDataItems
    {
        public string DateLabel { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public int Blocked { get; set; }
        public int Total { get; set; }
    }
}