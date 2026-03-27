using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Document>(entity =>
        {
            entity.HasOne(d => d.Category)
                  .WithMany(c => c.Documents)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.UploadedBy)
                  .WithMany()
                  .HasForeignKey(d => d.UploadedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(d => d.Title);
        });

        builder.Entity<DocumentVersion>(entity =>
        {
            entity.HasOne(v => v.Document)
                  .WithMany(d => d.Versions)
                  .HasForeignKey(v => v.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.UploadedBy)
                  .WithMany()
                  .HasForeignKey(v => v.UploadedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.HasOne(a => a.User)
                  .WithMany()
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(a => a.Timestamp);
        });

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fatura", Description = "Fatura belgeleri" },
            new Category { Id = 2, Name = "Sözleşme", Description = "Sözleşme belgeleri" },
            new Category { Id = 3, Name = "Dilekçe", Description = "Dilekçe belgeleri" },
            new Category { Id = 4, Name = "Rapor", Description = "Rapor belgeleri" },
            new Category { Id = 5, Name = "Diğer", Description = "Diğer belgeler" }
        );
    }
}
