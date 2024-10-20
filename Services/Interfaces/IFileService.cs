using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderPath);
        Task<FileResult> DownloadFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
    }
}
