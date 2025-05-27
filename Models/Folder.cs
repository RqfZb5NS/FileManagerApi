namespace FileManagerApi.Models;

public class Folder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    
    // Владелец папки
    public int OwnerId { get; set; }
    public User Owner { get; set; }
    
    // Родительская папка (для вложенности)
    public Guid? ParentFolderId { get; set; }
    public Folder? ParentFolder { get; set; }
    
    // Навигационные свойства
    public List<FileEntity> Files { get; set; } = new();
    public List<Folder> Subfolders { get; set; } = new();
    public List<Permission> Permissions { get; set; } = new();
}