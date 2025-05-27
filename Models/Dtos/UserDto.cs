namespace FileManagerApi.Models.Dtos;

public record UserDto(
    int Id,
    string Username,
    string Email,
    DateTime CreatedAt);