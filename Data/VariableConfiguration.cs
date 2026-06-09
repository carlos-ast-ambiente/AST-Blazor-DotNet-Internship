using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlazorApp.Models;

namespace BlazorApp.Data
{
    public class VariableConfiguration : IEntityTypeConfiguration<Variable>
    {
        public void Configure(EntityTypeBuilder<Variable> builder) {
            builder.HasKey(x=> x.Id);

            builder.HasOne(x=> x.Group)
            .WithMany(y=> y.Variables)
            .HasForeignKey(x=> x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}