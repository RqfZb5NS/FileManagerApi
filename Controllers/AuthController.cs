using FileManagerApi.Models.Dtos;
using FileManagerApi.Services;
using Microsoft.AspNetCore.Mvc;
using FileManagerApi.Models;
using Microsoft.Extensions.Logging;

namespace FileManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Register(UserRegisterDto dto)
    {
        using var scope = _logger.BeginScope("Registration for {Username}", dto.Username);
        try
        {
            _logger.LogInformation(
                "Starting registration. Username: {Username}, Email: {Email}",
                dto.Username,
                dto.Email);

            var user = await _authService.Register(dto.Username, dto.Email, dto.Password);
            
            _logger.LogInformation(
                "User registered successfully. UserID: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            return new UserDto(user.Id, user.Username, user.Email, user.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Registration failed. Username: {Username}, Error: {ErrorMessage}",
                dto.Username,
                ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> Login(UserLoginDto dto)
    {
        using var scope = _logger.BeginScope("Login attempt for {Username}", dto.Username);
        try
        {
            _logger.LogInformation("Login attempt for username: {Username}", dto.Username);
            
            var token = await _authService.Login(dto.Username, dto.Password);
            
            _logger.LogInformation("Successful login for {Username}", dto.Username);
            return Ok(new { Token = token });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                "Invalid login attempt. Username: {Username}, Error: {ErrorMessage}",
                dto.Username,
                ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Login error. Username: {Username}, Error: {ErrorMessage}",
                dto.Username,
                ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}