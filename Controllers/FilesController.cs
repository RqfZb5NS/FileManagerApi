using FileManagerApi.Models.Dtos;
using FileManagerApi.Services;
using FileManagerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace FileManagerApi.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(
        FileService fileService,
        ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileResponseDto>> UploadFile(
        [FromForm] UploadFileRequestDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var metadata = await _fileService.UploadFileAsync(
                request.File, 
                request.TargetPath ?? string.Empty, 
                userId);

            return Ok(MapToDto(metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed");
            return Problem(ex.Message, statusCode: 400);
        }
    }

    [HttpPost("directory")]
    public async Task<ActionResult<FileResponseDto>> CreateDirectory(
        [FromBody] CreateDirectoryDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var directory = await _fileService.CreateDirectoryAsync(
                request.Path, 
                userId);

            return CreatedAtAction(nameof(GetFileInfo), MapToDto(directory));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Directory creation failed");
            return Problem(ex.Message, statusCode: 400);
        }
    }

    [HttpGet("{id}/download")]
    public IActionResult DownloadFile(int id)
    {
        try
        {
            var stream = _fileService.DownloadFile(id);
            var metadata = _fileService.GetFileMetadata(id);
            
            return File(stream, metadata.MimeType!, metadata.Name);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File download failed");
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<FileResponseDto> GetFileInfo(int id)
    {
        try
        {
            var metadata = _fileService.GetFileMetadata(id);
            return Ok(MapToDto(metadata));
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<FileResponseDto>>> ListFiles(
        [FromQuery] string? path = null)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var files = await _fileService.ListFilesAsync(path, userId);
            
            return Ok(files.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File listing failed");
            return Problem(ex.Message, statusCode: 400);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            await _fileService.DeleteAsync(id, userId);
            
            return NoContent();
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    private static FileResponseDto MapToDto(FileMetadata metadata) => new(
        metadata.Id,
        metadata.Name,
        metadata.IsDirectory,
        metadata.MimeType,
        metadata.Size,
        metadata.Created,
        metadata.Modified,
        metadata.ParentId);
}