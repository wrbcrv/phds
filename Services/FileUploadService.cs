using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public class FileUploadService(IWebHostEnvironment env) : IFileUploadService
    {
        private readonly IWebHostEnvironment _env = env;

        public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido.");

            var uploadPath = Path.Combine(_env.WebRootPath, folderPath);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderPath, fileName);
        }

        public async Task<FileResult> DownloadFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Arquivo não encontrado.", filePath);
            }

            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var fileName = Path.GetFileName(fullPath);
            return new FileStreamResult(fileStream, "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }        
    }
}
