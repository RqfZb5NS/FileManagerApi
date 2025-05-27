using FileManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManagerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<FileEntity> Files => Set<FileEntity>();
    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Folder
        modelBuilder.Entity<Folder>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.Folders)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Folder>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.Subfolders)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Folder>()
            .HasIndex(f => new { f.OwnerId, f.ParentFolderId });

        // File
        modelBuilder.Entity<FileEntity>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileEntity>()
            .HasOne(f => f.Folder)
            .WithMany(f => f.Files)
            .HasForeignKey(f => f.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileEntity>()
            .HasIndex(f => new { f.OwnerId, f.FolderId });

        // Permission
        modelBuilder.Entity<Permission>()
            .HasOne(p => p.User)
            .WithMany(u => u.Permissions)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Permission>()
            .HasIndex(p => new { p.UserId, p.FileId, p.FolderId });
    }
}