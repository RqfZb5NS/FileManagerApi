using FileManagerApi.Data;
using FileManagerApi.Services;
using FileManagerApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

// База данных
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Кастомные сервисы
builder.Services.AddScoped<FileService>();
builder.Services.AddSingleton<PathResolver>(provider => 
    new PathResolver(builder.Configuration["Storage:RootPath"]!));
builder.Services.AddSingleton<FileValidationService>();
builder.Services.AddScoped<AuthService>();

// JWT аутентификация
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            )
        };
    });



var app = builder.Build();

// Создание хранилища файлов
var storagePath = Path.Combine(Directory.GetCurrentDirectory(), 
    builder.Configuration["Storage:RootPath"]!);
Directory.CreateDirectory(storagePath);

// Middleware pipeline
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapHub<FileHub>("/filehub");

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
