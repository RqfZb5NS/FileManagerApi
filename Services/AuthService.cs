using System.Security.Claims;
using System.Security.Cryptography;
using FileManagerApi.Models;
using Microsoft.EntityFrameworkCore;
using FileManagerApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FileManagerApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;


    public AuthService(
        AppDbContext context, 
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public async Task<User> Register(string username, string email, string password)
    {
        using var activity = _logger.BeginScope("Registration for {Username}", username);
        try
        {
            _logger.LogInformation("Starting user registration");

            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new InvalidOperationException($"User with username '{username}' already exists");

            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException($"User with email '{email}' already exists");

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User registered successfully. UserID: {UserId}, Email: {Email}",
                user.Id,
                user.Email);


            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Registration failed for {Username}. Error: {ErrorMessage}",
                username,
                ex.Message);
            throw;
        }
    }

    public async Task<string> Login(string usernameOrEmail, string password)
    {
        using var activity = _logger.BeginScope("Login attempt for {UsernameOrEmail}", usernameOrEmail);
        try
        {
            _logger.LogDebug("Starting authentication process");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user is null)
            {
                _logger.LogWarning("User not found: {UsernameOrEmail}", usernameOrEmail);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            _logger.LogDebug("User found. Checking password...");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for {UsernameOrEmail}", usernameOrEmail);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            _logger.LogInformation("Successful login for {UsernameOrEmail}", usernameOrEmail);
            return GenerateJwtToken(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Login failed for {UsernameOrEmail}. Error: {ErrorMessage}",
                usernameOrEmail,
                ex.Message);
            throw;
        }
    }

    private string GenerateJwtToken(User user)
    {
        try
        {
            _logger.LogDebug("Generating JWT token for {UserId}", user.Id);

            var claims = new List<Claim>
            {

                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(
                _config.GetValue<int>("Jwt:ExpirationHours"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogDebug("JWT for {User}: {Jwt}", user.Username, jwt);

            return jwt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT for {User}", user.Id);
            throw;
        }
    }
}
