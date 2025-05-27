namespace FileManagerApi.Models.Dtos;

public record FileMetadataDto(
    int Id,
    string FileName,
    long Size,
    string MimeType,
    DateTime UploadDate);