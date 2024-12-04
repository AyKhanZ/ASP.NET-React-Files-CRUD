using Microsoft.AspNetCore.Mvc;

namespace UploadAndReadImages.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadFileController : ControllerBase
{

    [HttpPost("uploadFile")]
    public async Task<IActionResult> Post(List<IFormFile> formFiles)
    {
        string message;
        try
        {
            if (formFiles == null || formFiles.Count == 0)
            {
                message = "No files were provided.";
                return BadRequest(new { Message = message });
            }

            foreach (var file in formFiles)
            {
                if (file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);

                    // Генерируем уникальное имя файла с расширением
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";

                    var filePath = Path.Combine(
                        "C:\\Users\\Aykhan Zeynalov\\Desktop\\Images",
                        fileName
                    );

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            message = "All files were uploaded successfully.";
        }
        catch (Exception ex)
        {
            message = $"An error occurred while uploading files: {ex.Message}";
            return StatusCode(500, new { Message = message });
        }

        return Ok(new { Message = message });
    }

    [HttpGet("getFile/{fileName}")]
    public IActionResult GetFile(string fileName)
    {
        // Путь к папке, где хранятся изображения
        var filePath = Path.Combine("C:\\Users\\Aykhan Zeynalov\\Desktop\\Images", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { Message = "File not found" });
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var contentType = GetContentType(filePath);

        return File(fileBytes, contentType, fileName);
    }

    [HttpGet("getAllFiles")]
    public IActionResult GetAllFiles()
    {
        var directoryPath = "C:\\Users\\Aykhan Zeynalov\\Desktop\\Images";

        if (!Directory.Exists(directoryPath))
            return NotFound(new { Message = "Directory not found" });

        try
        {
            var files = Directory.GetFiles(directoryPath)
                .Where(file => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" }
                .Contains(Path.GetExtension(file).ToLower()))
                .Select(Path.GetFileName)
                .ToList();

            return Ok(files);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }

    // Метод для определения типа контента
    private string GetContentType(string path)
    {
        var types = new Dictionary<string, string>
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".svg", "image/svg+xml" }
    };

        var ext = Path.GetExtension(path).ToLowerInvariant();
        return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
    }



    [HttpPut("updateFile/{fileName}")]
    public async Task<IActionResult> UpdateFile(string fileName, IFormFile newFile)
    {
        var directoryPath = "C:\\Users\\Aykhan Zeynalov\\Desktop\\Images";
        var filePath = Path.Combine(directoryPath, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { Message = "File not found" });
        }

        try
        {
            if (newFile.Length > 0)
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await newFile.CopyToAsync(stream);
                }

                return Ok(new { Message = "File updated successfully" });
            }
            else
            {
                return BadRequest(new { Message = "New file is empty" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while updating the file: {ex.Message}" });
        }
    }



    [HttpDelete("deleteFile/{fileName}")]
    public IActionResult DeleteFile(string fileName)
    {
        var directoryPath = "C:\\Users\\Aykhan Zeynalov\\Desktop\\Images";
        var filePath = Path.Combine(directoryPath, fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { Message = "File not found" });

        try
        {
            System.IO.File.Delete(filePath);
            return Ok(new { Message = "File deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while deleting the file: {ex.Message}" });
        }
    }
}