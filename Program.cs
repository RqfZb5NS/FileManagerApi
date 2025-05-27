using FileManagerApi.Data;
using Microsoft.EntityFrameworkCore;
   

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

// Конфигурация Entity Framework
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
/*
// Регистрация кастомных сервисов

builder.Services.AddScoped<FileService>();
builder.Services.AddSingleton<PathResolver>(provider => 
    new PathResolver(builder.Configuration["Storage:RootPath"]!));
builder.Services.AddSingleton<FileValidationService>();
*/
var app = builder.Build();

// Создаем физический каталог для хранения файлов
var storagePath = Path.Combine(Directory.GetCurrentDirectory(), 
    builder.Configuration["Storage:RootPath"]!);
Directory.CreateDirectory(storagePath);

// Конфигурация middleware
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
//app.MapHub<FileHub>("/filehub");

app.Run();