using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
        public class TestStepMapping:IEntityTypeConfiguration<TestStep>
        {
            public void Configure(EntityTypeBuilder<TestStep> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Actual).IsRequired();
                builder.Property(x => x.Expected).IsRequired();
                builder.Property(x => x.SortIndex).IsRequired();
            }
        }
    
}