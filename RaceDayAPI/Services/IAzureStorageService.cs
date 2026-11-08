using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RaceDayAPI.Services
{
    public interface IAzureStorageService
    {
        // This method takes a file and a folder name, and returns the public URL of the uploaded file
        Task<string> UploadFileAsync(IFormFile file, string folderName);
    }
}
