using IrisEye.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrisEye.Data.Mappings
{
    public class FolderMapping:IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CreatedOn).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
        }
    }
}