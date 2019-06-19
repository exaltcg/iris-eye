using System.Collections.Generic;
using FluentScheduler;

namespace IrisEye.Web.Providers
{
    public interface ISchedulerProvider
    {
        void Init();

        IEnumerable<Schedule> GetActiveSchedules();

        void RunScheduleNow(string name);
    }
}