namespace FileManagerApi.Models.Dtos;

public record FileResponseDto(
    int Id,
    string Name,
    bool IsDirectory,
    string? MimeType,
    long Size,
    DateTime Created,
    DateTime Modified,
    int? ParentId);