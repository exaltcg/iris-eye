using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    public class StepMapping:IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Issued).IsRequired();
            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.Stacktrace);
            builder.Property(x => x.IsPassed).IsRequired();
        }
    }
}