namespace FileManagerApi.Models.Dtos;

public record FileTreeDto(
    int Id,
    string Name,
    bool IsDirectory,
    List<FileTreeDto> Children);