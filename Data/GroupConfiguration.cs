using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlazorApp.Models;


namespace BlazorApp.Data
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>    
    {
        public void Configure(EntityTypeBuilder<Group> builder) {
            builder.HasKey(x=> x.Id);

            builder.HasMany(x=> x.Variables)
            .WithOne(y=> y.Group)
            .HasForeignKey(x=> x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}