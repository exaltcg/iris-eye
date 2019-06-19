using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    
        public class StepAnalysisItemMapping:IEntityTypeConfiguration<StepAnalysisItem>
        {
            public void Configure(EntityTypeBuilder<StepAnalysisItem> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Message).IsRequired();
                builder.Property(x => x.GitHubId);
                builder.Property(x => x.BugzillaId);
            }
        }
    
}