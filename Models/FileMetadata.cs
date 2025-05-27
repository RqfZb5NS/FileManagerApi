namespace FileManagerApi.Models;

public class FileMetadata
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string PhysicalPath { get; set; } = null!;
    public long Size { get; set; }
    public string? MimeType { get; set; }
    public bool IsDirectory { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Modified { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int? ParentId { get; set; }
    public FileMetadata? Parent { get; set; }
    public List<FileMetadata> Children { get; set; } = new();
}