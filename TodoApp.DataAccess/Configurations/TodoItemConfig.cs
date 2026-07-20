using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.DataAccess.Entities;
using TodoApp.Domain.Entities;

namespace TodoApp.DataAccess.Configurations
{
    public class TodoItemConfig:IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder) {
            builder.HasKey(todoItem => todoItem.Id);

            builder.Property(todoItem => todoItem.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(todoItem => todoItem.Description)
                .HasMaxLength(250);

            builder.Property(todoItem => todoItem.UserId)
            .IsRequired();

            builder.HasIndex(todoItem => new
            {
                todoItem.UserId,
                todoItem.CreatedAt
            });

            builder.HasOne(todoItem => todoItem.Category)
            .WithMany(category => category.TodoItems)
            .HasForeignKey(todoItem => todoItem.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(todoItem => todoItem.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
