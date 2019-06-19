using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    public class TestCaseHistoryMapping:IEntityTypeConfiguration<TestCaseHistory>
    {
        public void Configure(EntityTypeBuilder<TestCaseHistory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AddedOn).IsRequired();
            builder.Property(x => x.Message).IsRequired();
        }
    }

}