using System.ComponentModel.DataAnnotations;

namespace FileManagerApi.Models.Dtos;

public record CreateDirectoryDto(
    [Required] string Path // System.ComponentModel.DataAnnotations
);