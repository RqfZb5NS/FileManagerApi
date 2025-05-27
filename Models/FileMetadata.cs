namespace FileManagerApi.Models;

public class FileMetadata
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public long Size { get; set; }
    public string MimeType { get; set; } = null!;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}