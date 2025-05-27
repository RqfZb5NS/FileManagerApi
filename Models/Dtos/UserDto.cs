namespace FileManagerApi.Models.Dtos;

public record UserDto(
    string Username,
    string Email,
    DateTime CreatedAt);