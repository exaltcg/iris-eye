using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
        public class TestCaseMapping:IEntityTypeConfiguration<TestCase>
        {
            public void Configure(EntityTypeBuilder<TestCase> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Name).IsRequired();
                builder.Property(x => x.TestRailId).IsRequired();
                builder.Property(x => x.Status).IsRequired();
                builder.Property(x => x.MergedDate);
                builder.Property(x => x.StartedOn);
                builder.Property(x => x.FinishedOn);
                builder.Property(x => x.AddedOn);
                builder.Property(x => x.AttentionRequired);
                builder.Property(x => x.GitHubId);
                builder.Property(x => x.Message);
                builder.Property(x => x.Preconditions);
                builder.Property(x => x.PullRequestId);
                builder.Property(x => x.IsIssue).IsRequired();
            }
        }

}