using FileManagerApi.Models.Dtos;
using FileManagerApi.Services;
using Microsoft.AspNetCore.Mvc;
using FileManagerApi.Models;

namespace FileManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(UserRegisterDto dto)
    {
        var user = await _authService.Register(dto.Username, dto.Email, dto.Password);
        return new UserDto(user.Username, user.Email, user.CreatedAt);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserLoginDto dto)
    {
        var token = await _authService.Login(dto.Username, dto.Password);
        return Ok(new { Token = token });
    }
}