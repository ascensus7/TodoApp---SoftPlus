using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.DataAccess.Entities;
using TodoApp.Domain.Entities;

namespace TodoApp.DataAccess.Configurations
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(category => category.Id);

            builder.Property(category => category.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(category => category.NormalizedName)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(category => category.UserId)
            .IsRequired();

            builder.HasIndex(category => new
            {
                category.UserId,
                category.NormalizedName
            })
            .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(category => category.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
