using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using IrisEye.Data.Parsers;
using IrisEye.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IrisEye.Web.Providers
{
    public class SchedulerProvider:ISchedulerProvider
    {
        private IConfiguration Configuration { get; }
        private readonly Registry _registry;
        private const int EmailInterval = 1;
        private const int TestInfoInterval = 6;

        public SchedulerProvider(IRunsProvider runs, IConfiguration configuration)
        {
            Configuration = configuration;
            _registry = new Registry();
        }

        public void Init()
        {
            JobManager.Initialize(_registry);
            
            JobManager.AddJob(GetResults, (s) => s.WithName("Email Crawler")
                .ToRunEvery(5)
                .Minutes());
            
            JobManager.AddJob(RefreshTestsInfo, (s) => s.WithName("Tests Refresher")
                .ToRunEvery(TestInfoInterval)
                .Hours());
        }

        public IEnumerable<Schedule> GetActiveSchedules()
        {
            return JobManager.AllSchedules;
        }

        public void RunScheduleNow(string name)
        {
            
            JobManager.RemoveJob(name);
            switch (name)
            {
                case "Email Crawler":
                    JobManager.AddJob(GetResults, (s) => s.WithName("Email Crawler")
                        .ToRunNow()
                        .AndEvery(5)
                        .Minutes());
                    break;
                case "Tests Refresher":
                    JobManager.AddJob(RefreshTestsInfo, (s) => s.WithName("Tests Refresher")
                        .ToRunNow()
                        .AndEvery(EmailInterval)
                        .Hours());
                    break;
            }
            
        }

        private void GetResults()
        {
            try
            {
                var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
                using (var db = new ApplicationDbContext(contextOptions.Options))
                {
                    var runsProvider = new RunsProvider(db);
                    var results = runsProvider.GetAllTestsInfo().Result;
                    using (var emailParser = new EmailParser(results.ToList()))
                    {
                        var reports = emailParser.GetReportAsync();
                        var runs = new RunsProvider(db);
                        runs.AddMultipleRuns(reports).Wait();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void RefreshTestsInfo()
        {
            try
            {
                var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
                using (var db = new ApplicationDbContext(contextOptions.Options))
                {
                    var runs = new RunsProvider(db);
                    runs.RefreshTestInfo().Wait();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

       
    }
}