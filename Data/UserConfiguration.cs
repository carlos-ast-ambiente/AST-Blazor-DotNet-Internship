using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlazorApp.Models;


namespace BlazorApp.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x=> x.Id);

            builder.HasMany(x=> x.Plants)
            .WithMany(u=> u.Users)
            .UsingEntity(i=> i.ToTable("UserPlant"));
        }
    }
}