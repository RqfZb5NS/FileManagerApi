namespace FileManagerApi.Models;
public enum AccessLevel
{
    Read,
    Write,
    Manage
}

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Кому предоставлен доступ
    public int UserId { get; set; }
    public User User { get; set; }
    
    // На что предоставлен доступ (File или Folder)
    public Guid? FileId { get; set; }
    public FileEntity? File { get; set; }
    
    public Guid? FolderId { get; set; }
    public Folder? Folder { get; set; }
    
    public AccessLevel AccessLevel { get; set; }
}