using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    public class RunMapping :IEntityTypeConfiguration<Run>
    {
        public void Configure(EntityTypeBuilder<Run> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ReportHash).IsRequired();
            builder.Property(x => x.Build).IsRequired();
            builder.Property(x => x.BetaChannel);
            builder.Property(x => x.Errors).IsRequired();
            builder.Property(x => x.Failed).IsRequired();
            builder.Property(x => x.Passed).IsRequired();
            builder.Property(x => x.Blocked).IsRequired();
            builder.Property(x => x.Total).IsRequired();
            builder.Property(x => x.Skipped).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.Failed).IsRequired();
            builder.Property(x => x.Environment).IsRequired();
            builder.Property(x => x.AddedOn).IsRequired();
            builder.Property(x => x.ReportTime).IsRequired();
            builder.Property(x => x.FailedTests).IsRequired();
        }
    }
}