using System.ComponentModel.DataAnnotations;

namespace FileManagerApi.Models.Dtos;

public record UserLoginDto(
    [Required] string Username,
    [Required] string Password);