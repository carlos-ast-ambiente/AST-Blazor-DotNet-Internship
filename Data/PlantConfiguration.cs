using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlazorApp.Models;


namespace BlazorApp.Data
{
    public class PlantConfiguration : IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.HasKey(x=> x.Id);

            builder.HasMany(x=> x.Variables)
            .WithMany(y=> y.Plants)
            .UsingEntity(z=> z.ToTable("PlantVariable"));
        }
    }
}