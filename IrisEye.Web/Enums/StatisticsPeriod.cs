using System.ComponentModel;

namespace IrisEye.Web.Enums
{
    public enum StatisticsPeriod
    {
        [Description("Today")]
        Today = 1,
        [Description("This week")]
        ThisWeek = 2,
        [Description("Previous week")]
        PreviousWeek = 3,
        [Description("This month")]
        ThisMonth = 4,
        [Description("This year")]
        ThisYear = 5,
        [Description("All the time")]
        AllTheTime = 6
       
    }
}