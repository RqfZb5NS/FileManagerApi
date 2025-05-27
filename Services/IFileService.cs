using FileManagerApi.Models;
using Microsoft.AspNetCore.Http;

namespace FileManagerApi.Services;

public interface IFileService
{
    // ------------------- Файлы -------------------
    
    /// <summary>
    /// Загрузить файл в указанную папку
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="folderId">ID целевой папки</param>
    /// <param name="file">Файл из запроса</param>
    /// <returns>Метаданные файла</returns>
    Task<FileEntity> UploadFileAsync(int userId, Guid folderId, IFormFile file);

    /// <summary>
    /// Скачать файл по ID
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="fileId">ID файла</param>
    /// <returns>Tuple (FileStream, MIME-тип, имя файла)</returns>
    Task<(Stream, string, string)> DownloadFileAsync(int userId, Guid fileId);

    /// <summary>
    /// Удалить файл
    /// </summary>
    Task DeleteFileAsync(int userId, Guid fileId);

    /// <summary>
    /// Переименовать файл
    /// </summary>
    Task<FileEntity> RenameFileAsync(int userId, Guid fileId, string newName);

    // ------------------- Папки -------------------
    
    /// <summary>
    /// Создать новую папку
    /// </summary>
    Task<Folder> CreateFolderAsync(int userId, Guid? parentFolderId, string name);

    /// <summary>
    /// Удалить папку (рекурсивно)
    /// </summary>
    Task DeleteFolderAsync(int userId, Guid folderId);

    /// <summary>
    /// Переименовать папку
    /// </summary>
    Task<Folder> RenameFolderAsync(int userId, Guid folderId, string newName);

    /// <summary>
    /// Получить содержимое папки
    /// </summary>
    Task<(List<FileEntity>, List<Folder>)> GetFolderContentAsync(int userId, Guid folderId);

    // ------------------- Права доступа -------------------
    
    /// <summary>
    /// Добавить разрешение для пользователя
    /// </summary>
    Task<Permission> AddPermissionAsync(
        int ownerUserId, 
        Guid targetId, 
        int targetUserId, 
        AccessLevel accessLevel,
        bool isFolder = false);

    /// <summary>
    /// Удалить разрешение
    /// </summary>
    Task RemovePermissionAsync(int ownerUserId, Guid permissionId);

    // ------------------- Вспомогательные методы -------------------
    
    /// <summary>
    /// Проверить доступ к файлу/папке
    /// </summary>
    Task<bool> HasAccessAsync(int userId, Guid targetId, AccessLevel requiredLevel, bool isFolder = false);

    /// <summary>
    /// Получить корневую папку пользователя
    /// </summary>
    Task<Folder> GetRootFolderAsync(int userId);

    /// <summary>
    /// Сменить владельца файла/папки (только для администраторов)
    /// </summary>
    Task ChangeOwnerAsync(
        int adminUserId,
        Guid targetId,
        int newOwnerId,
        bool isFolder = false);

    /// <summary>
    /// Сгенерировать публичную ссылку для доступа
    /// </summary>
    Task<string> GenerateShareLinkAsync(
        int userId,
        Guid targetId,
        TimeSpan? expiration = null,
        bool isFolder = false);

    /// <summary>
    /// Отозвать публичную ссылку
    /// </summary>
    Task RevokeShareLinkAsync(int userId, Guid linkId);

    /// <summary>
    /// Переместить файл/папку в другую папку
    /// </summary>
    Task MoveAsync(
        int userId,
        Guid targetId,
        Guid newParentFolderId,
        bool isFolder = false);

}