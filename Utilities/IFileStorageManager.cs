using FileManagerApi.Models;
using Microsoft.AspNetCore.Http;

namespace FileManagerApi.Utilities.Interfaces;

/// <summary>
/// Управление физическим хранением файлов и директорий.
/// </summary>
public interface IFileStorageManager
{
    /// <summary>
    /// Сохраняет файл в хранилище и возвращает путь к нему.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя-владельца.</param>
    /// <param name="folderId">Идентификатор папки, в которую сохраняется файл.</param>
    /// <param name="file">Файл из HTTP-запроса.</param>
    /// <returns>Путь к сохраненному файлу в хранилище.</returns>
    Task<string> SaveFileAsync(int userId, Guid folderId, IFormFile file);

    /// <summary>
    /// Возвращает поток для чтения файла из хранилища.
    /// </summary>
    /// <param name="storagePath">Физический путь к файлу в хранилище.</param>
    Task<Stream> GetFileStreamAsync(string storagePath);

    /// <summary>
    /// Удаляет файл из хранилища.
    /// </summary>
    /// <param name="storagePath">Физический путь к файлу в хранилище.</param>
    Task DeleteFileAsync(string storagePath);

    /// <summary>
    /// Перемещает файл в новое место.
    /// </summary>
    /// <param name="sourcePath">Исходный путь к файлу.</param>
    /// <param name="destinationPath">Новый путь к файлу.</param>
    Task MoveFileAsync(string sourcePath, string destinationPath);

    /// <summary>
    /// Создает директорию в хранилище.
    /// </summary>
    /// <param name="path">Полный путь к директории.</param>
    Task CreateDirectoryAsync(string path);

    /// <summary>
    /// Рекурсивно удаляет директорию и всё её содержимое.
    /// </summary>
    /// <param name="path">Полный путь к директории.</param>
    Task DeleteDirectoryAsync(string path);

    /// <summary>
    /// Генерирует путь для хранения файла на основе структуры пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="folderId">Идентификатор папки.</param>
    /// <param name="fileName">Исходное имя файла (для извлечения расширения).</param>
    /// <returns>Сгенерированный уникальный путь к файлу.</returns>
    string GenerateFilePath(int userId, Guid folderId, string fileName);
}