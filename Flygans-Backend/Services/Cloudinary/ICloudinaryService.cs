using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Flygans_Backend.Services.Cloudinary
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string publicId);
    }
}
