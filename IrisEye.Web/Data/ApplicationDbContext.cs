using IrisEye.Core.Entities;
using IrisEye.Data.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IrisEye.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<StepAnalysisItem> StepAnalysisItems { get; set; }
        public DbSet<TestInfo> TestInfos { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        
        public DbSet<TestSuite> TestSuites { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestCaseComment> TestCaseComments { get; set; }
        public DbSet<TestStep> TestSteps { get; set; }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
        public DbSet<TestCaseHistory> TestCaseHistoryItems { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SystemUser>(entity => {
                entity.ToTable("Users");
            });
            builder.ApplyConfiguration(new RunMapping());
            builder.ApplyConfiguration(new TestMapping());
            builder.ApplyConfiguration(new StepMapping());
            builder.ApplyConfiguration(new StepAnalysisItemMapping());
            builder.ApplyConfiguration(new FolderMapping());
            builder.ApplyConfiguration(new TestInfoMapping());
            builder.ApplyConfiguration(new UserMapping());
            builder.ApplyConfiguration(new TestSuiteMapping());
            builder.ApplyConfiguration(new TestCaseMapping());
            builder.ApplyConfiguration(new TestStepMapping());
            builder.ApplyConfiguration(new TestCaseHistoryMapping());
            builder.ApplyConfiguration(new TestCaseCommentMapping());
            builder.ApplyConfiguration(new AnalysisResultMapping());
        }
    }
}