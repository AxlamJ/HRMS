using Dapper;
using DocumentFormat.OpenXml.InkML;
using HrManagement.Data;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Service
{
    public interface IFileUploadService
    {
        Task<FileMediaUpload> UploadAsync(IFormFile file, string folderName = "uploads", string[]? allowedExtensions = null);
        Task<int> UpsertFileMediaAsync(FileMediaUpload file);
    }
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;

        public FileUploadService(IWebHostEnvironment env, DataContext context)
        {
            _env = env;
            _context = context;
        }

        public async Task<FileMediaUpload> UploadAsync(IFormFile file, string folderName = "uploads", string[]? allowedExtensions = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (allowedExtensions != null && !allowedExtensions.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not allowed.");
            string type = GetFileTypeFromExtension(extension);
            var uploadPath = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, uniqueFileName);
            var relativePath = Path.Combine(folderName, uniqueFileName).Replace("\\", "/");

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileMediaUpload = new FileMediaUpload
            {
                FileName = uniqueFileName,
                FilePath = "/" + relativePath,
                ContentType = file.ContentType,
                Size = file.Length,
                Extension = extension,
                Type = type
            };
            return fileMediaUpload;
        }

        private string GetFileTypeFromExtension(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv" };
            string[] documentExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".rtf" };

            if (imageExtensions.Contains(extension))
                return "Image";
            if (videoExtensions.Contains(extension))
                return "Video";
            if (documentExtensions.Contains(extension))
                return "Document";

            return "File"; // fallback/default type
        }



        //
        public async Task<int> UpsertFileMediaAsync(FileMediaUpload file)
        {
            using var connection = _context.CreateConnection();

            if (file.Id == 0)
            {
                string sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", "FileMediaUpload", "Create.sql");
                string createQuery = await File.ReadAllTextAsync(sqlFilePath);

                int insertedId = await connection.QuerySingleAsync<int>(createQuery, file);
                if (insertedId > 0) { return insertedId; }
            }
            else
            {
                string sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", "FileMediaUpload", "Update.sql");
                string updateQuery = await File.ReadAllTextAsync(sqlFilePath);
                int isUpdated = await connection.ExecuteAsync(updateQuery, file);
                if (file.Id > 0) { return file.Id; }
            }
            return 0;
        }
    }
}
