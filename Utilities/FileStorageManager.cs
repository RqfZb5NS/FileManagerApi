using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using FileManagerApi.Utilities.Interfaces;

namespace FileManagerApi.Utilities
{
    /// <summary>
    /// Реализация IFileStorageManager на локальной файловой системе.
    /// </summary>
    public class FileStorageManager : IFileStorageManager
    {
        private readonly string _rootPath;
        private readonly long _maxFileSize;
        private readonly HashSet<string> _allowedMimeTypes;

        /// <summary>
        /// Конструктор с внедрением конфигурации.
        /// </summary>
        /// <param name="configuration">IConfiguration для получения настроек.</param>
        public FileStorageManager(IConfiguration configuration)
        {
            _rootPath = configuration["FileStorage:RootPath"] ?? "./FileStorage";
            _maxFileSize = long.TryParse(configuration["FileStorage:MaxFileSize"], out var size) ? size : 104857600;
            _allowedMimeTypes = configuration.GetSection("FileStorage:AllowedMimeTypes").Get<string[]>()?.ToHashSet() ?? new HashSet<string> { "image/jpeg", "application/pdf" };
            Directory.CreateDirectory(_rootPath);
        }

        /// <inheritdoc/>
        public async Task<string> SaveFileAsync(int userId, Guid folderId, IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (file.Length == 0)
                throw new ArgumentException("Файл пустой.", nameof(file));
            if (file.Length > _maxFileSize)
                throw new InvalidOperationException($"Размер файла превышает максимальный: {_maxFileSize} байт.");

            if (!_allowedMimeTypes.Contains(file.ContentType))
                throw new InvalidOperationException($"Тип файла {file.ContentType} не разрешен.");

            var filePath = GenerateFilePath(userId, folderId, file.FileName);
            var fullPath = Path.Combine(_rootPath, filePath);

            var directory = Path.GetDirectoryName(fullPath)!;
            try
            {
                Directory.CreateDirectory(directory);

                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка сохранения файла.", ex);
            }

            // Возвращаем относительный от root путь (filePath)
            return filePath;
        }

        /// <inheritdoc/>
        public async Task<Stream> GetFileStreamAsync(string storagePath)
        {
            var fullPath = Path.Combine(_rootPath, storagePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Файл не найден.", fullPath);

            try
            {
                // Файл открывается асинхронно на чтение
                return await Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка открытия файла для чтения.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteFileAsync(string storagePath)
        {
            var fullPath = Path.Combine(_rootPath, storagePath);
            try
            {
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка удаления файла.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            var fullSource = Path.Combine(_rootPath, sourcePath);

            var fullDestination = Path.Combine(_rootPath, destinationPath);
            var destinationDirectory = Path.GetDirectoryName(fullDestination)!;

            try
            {
                Directory.CreateDirectory(destinationDirectory);
                await Task.Run(() => File.Move(fullSource, fullDestination, true));
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка перемещения файла.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task CreateDirectoryAsync(string path)
        {
            var fullPath = Path.Combine(_rootPath, path);
            try
            {
                await Task.Run(() => Directory.CreateDirectory(fullPath));
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка создания директории.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteDirectoryAsync(string path)
        {
            var fullPath = Path.Combine(_rootPath, path);
            try
            {
                if (Directory.Exists(fullPath))
                {
                    await Task.Run(() => Directory.Delete(fullPath, recursive: true));
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка удаления директории.", ex);
            }
        }

        /// <inheritdoc/>
        public string GenerateFilePath(int userId, Guid folderId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Имя файла не может быть пустым.", nameof(fileName));
                
            var extension = Path.GetExtension(fileName);
            var newFileName = $"{Guid.NewGuid()}{extension}";
            // Структура: userId/folderId/новыйфайл.расширение
            var path = Path.Combine(userId.ToString(), folderId.ToString(), newFileName);

            // Возвращаем относительный путь (от root)
            return path;
        }
    }
}
