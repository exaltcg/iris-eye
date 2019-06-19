using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    
    public class TestSuiteMapping:IEntityTypeConfiguration<TestSuite>
    {
        public void Configure(EntityTypeBuilder<TestSuite> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description);
            builder.Property(x => x.TestRailId);
            builder.Property(x => x.AddedOn);
            builder.Property(x => x.GitHubProjectId).IsRequired();
        }
    }

}