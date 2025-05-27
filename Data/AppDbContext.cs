using FileManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManagerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<FileMetadata> Files => Set<FileMetadata>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<FileMetadata>()
        .HasOne(f => f.Parent)
        .WithMany(f => f.Children)
        .HasForeignKey(f => f.ParentId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<FileMetadata>()
        .HasOne(f => f.User)
        .WithMany(u => u.Files)
        .HasForeignKey(f => f.UserId)
        .OnDelete(DeleteBehavior.Cascade);
}
}