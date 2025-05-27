using FileManagerApi.Data;
using FileManagerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using FileManagerApi.Utilities;

namespace FileManagerApi.Services;

public class FileService
{
    private readonly AppDbContext _context;
    private readonly PathResolver _pathResolver;
    private readonly FileValidationService _validationService;
    private readonly IConfiguration _config;

    public FileService(
        AppDbContext context,
        PathResolver pathResolver,
        FileValidationService validationService,
        IConfiguration config)
    {
        _context = context;
        _pathResolver = pathResolver;
        _validationService = validationService;
        _config = config;
    }

    public async Task<FileMetadata> UploadFileAsync(
        IFormFile file, 
        string targetPath, 
        int userId)
    {
        await Task.Run(() => _validationService.ValidateFile(file));
        
        // Создание структуры каталогов
        var (parent, fullPath) = await ProcessPathHierarchy(targetPath, userId);
        
        // Сохранение файла
        var physicalPath = Path.Combine(fullPath, file.FileName);
        await SaveToDiskAsync(file, physicalPath);

        // Создание метаданных
        var metadata = new FileMetadata
        {
            Name = file.FileName,
            PhysicalPath = physicalPath,
            MimeType = file.ContentType,
            Size = file.Length,
            UserId = userId,
            IsDirectory = false,
            Parent = parent,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        await _context.Files.AddAsync(metadata);
        await UpdateParentModifiedDate(parent);
        await _context.SaveChangesAsync();

        return metadata;
    }

    public async Task<FileMetadata> CreateDirectoryAsync(
        string path, 
        int userId)
    {
        _validationService.ValidateDirectoryName(path);
        
        var (parent, fullPath) = await ProcessPathHierarchy(path, userId);
        
        Directory.CreateDirectory(fullPath);

        var directory = new FileMetadata
        {
            Name = Path.GetFileName(fullPath),
            PhysicalPath = fullPath,
            IsDirectory = true,
            UserId = userId,
            Parent = parent,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        await _context.Files.AddAsync(directory);
        await UpdateParentModifiedDate(parent);
        await _context.SaveChangesAsync();

        return directory;
    }

    public FileStream DownloadFile(int fileId)
    {
        var file = _context.Files
            .FirstOrDefault(f => f.Id == fileId && !f.IsDirectory)
            ?? throw new FileNotFoundException("File not found");

        return new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read);
    }

    private async Task SaveToDiskAsync(IFormFile file, string path)
    {
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream); // Добавили await
    }

    private async Task UpdateParentModifiedDate(FileMetadata? parent)
    {
        // В методе UpdateParentModifiedDate
        while (parent != null)
        {
            parent.Modified = DateTime.UtcNow;
            _context.Entry(parent).Property(x => x.Modified).IsModified = true;
            parent = parent.Parent; // Безопасно, так как parent может стать null
        }
    }

    private async Task<(FileMetadata? parent, string fullPath)> ProcessPathHierarchy(
        string path, 
        int userId)
    {
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        FileMetadata? currentParent = null;
        var currentPath = _pathResolver.ResolvePath(string.Empty);

        foreach (var segment in pathSegments)
        {
            currentPath = Path.Combine(currentPath, segment);
            
            var directory = await _context.Files.FirstOrDefaultAsync(f => 
                f.PhysicalPath == currentPath && 
                f.IsDirectory && 
                f.UserId == userId).ConfigureAwait(false);

            if (directory == null)
            {
                await Task.Run(() => Directory.CreateDirectory(currentPath));
                // ... остальная логика ...
            }
        }
        return (currentParent, currentPath);
    }

    public async Task DeleteAsync(int fileId, int userId)
    {
        var file = await _context.Files
            .Include(f => f.Children)
            .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId)
            ?? throw new FileNotFoundException();

        if (file.IsDirectory)
        {
            Directory.Delete(file.PhysicalPath, recursive: true);
        }
        else
        {
            System.IO.File.Delete(file.PhysicalPath);
        }

        _context.Files.Remove(file);
        await UpdateParentModifiedDate(file.Parent);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<FileMetadata>> ListFilesAsync(string? path, int userId)
    {
        var basePath = _pathResolver.ResolvePath(path ?? string.Empty);
        
        return await _context.Files
            .Where(f => f.Parent != null && f.Parent.PhysicalPath == basePath && f.UserId == userId)
            .OrderBy(f => f.IsDirectory)
            .ThenBy(f => f.Name)
            .ToListAsync();
    }

    public FileMetadata GetFileMetadata(int fileId)
    {
        return _context.Files
            .Include(f => f.Parent)
            .FirstOrDefault(f => f.Id == fileId)
            ?? throw new FileNotFoundException();
    }
}