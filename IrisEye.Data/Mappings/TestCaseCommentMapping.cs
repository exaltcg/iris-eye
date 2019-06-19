using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
        public class TestCaseCommentMapping:IEntityTypeConfiguration<TestCaseComment>
        {
            public void Configure(EntityTypeBuilder<TestCaseComment> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Message).IsRequired();
                builder.Property(x => x.AddedOn).IsRequired();
                builder.Property(x => x.IsRead).IsRequired();
            }
        }
}