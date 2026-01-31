using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RAGDocument.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAGDocument.Infrastructure.Persistence
{
    public class AppDbContext: DbContext
    {
        public DbSet<Document> Documents { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Document>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(x => x.Embedding)
                .IsRequired()
                .HasColumnType("varbinary(max)");

            entity.Property(x => x.ContentHash)
                .IsRequired();

            entity.HasIndex(x => x.ContentHash)
                .IsUnique();

            entity.Property(x => x.Metadata)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        }

        
    }
}
