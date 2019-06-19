using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    public class UserMapping:IEntityTypeConfiguration<SystemUser>
    {
        public void Configure(EntityTypeBuilder<SystemUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EntityId).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.GithubAliases);
            builder.Property(x => x.GitHubAccount);
            builder.Property(x => x.GitHubToken);
        }
    }
}