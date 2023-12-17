using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TicketAPI.CloudStorage
{
    public interface ICloudStorage
    {
        Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage);
        Task DeleteFileAsync(string fileNameForStorage);
    }
}
