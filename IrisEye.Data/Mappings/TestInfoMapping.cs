using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    public class TestInfoMapping:IEntityTypeConfiguration<TestInfo>
    {
        public void Configure(EntityTypeBuilder<TestInfo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TestName).IsRequired();
            builder.Property(x => x.SuiteName).IsRequired();
            builder.Property(x => x.AuthorLogin).IsRequired();
            builder.Property(x => x.AuthorGitHubId);
        }
    }
}