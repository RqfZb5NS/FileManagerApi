using System.ComponentModel.DataAnnotations;

namespace FileManagerApi.Models;

public class FileEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    public string MimeType { get; set; }
    
    [Required]
    public string StoragePath { get; set; }
    
    // Владелец файла
    public int OwnerId { get; set; }
    public User Owner { get; set; }
    
    // Папка, в которой находится файл
    public Guid FolderId { get; set; }
    public Folder Folder { get; set; }
    
    public List<Permission> Permissions { get; set; } = new();
}