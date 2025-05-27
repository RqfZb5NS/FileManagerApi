using System.ComponentModel.DataAnnotations;

namespace FileManagerApi.Models.Dtos;

public record UserRegisterDto(
    [Required] string Username,
    [EmailAddress] string Email,
    [MinLength(6)] string Password);