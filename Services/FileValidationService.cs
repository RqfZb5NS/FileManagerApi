using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileManagerApi.Services;

public class FileValidationService
{
    private readonly string[] _allowedMimeTypes;
    private readonly long _maxFileSize;

    public FileValidationService(IConfiguration config)
    {
        _allowedMimeTypes = config.GetSection("Storage:AllowedMimeTypes").Get<string[]>()!;
        _maxFileSize = config.GetValue<long>("Storage:MaxFileSize", 100 * 1024 * 1024);
    }

    public void ValidateDirectoryName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Directory name cannot be empty");

        var invalidChars = Path.GetInvalidFileNameChars();
        if (name.Any(c => invalidChars.Contains(c)))
            throw new ValidationException("Invalid directory name");
    }

    public void ValidateFile(IFormFile file) // Убрали async и переименовали
{
    if (file.Length == 0)
        throw new ValidationException("File is empty");

    if (file.Length > _maxFileSize)
        throw new ValidationException($"File size exceeds limit of {_maxFileSize} bytes");

    if (!_allowedMimeTypes.Contains(file.ContentType))
        throw new ValidationException($"File type {file.ContentType} is not allowed");
}
}