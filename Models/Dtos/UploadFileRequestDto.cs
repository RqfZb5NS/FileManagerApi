using System.ComponentModel.DataAnnotations;

namespace FileManagerApi.Models.Dtos;

public record UploadFileRequestDto(
    [Required] IFormFile File,
    [StringLength(500)] string? TargetPath);