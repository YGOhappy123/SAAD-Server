using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Response;

namespace milktea_server.Interfaces.Services
{
    public interface IFileService
    {
        Task<ServiceResponse> UploadImageToCloudinary(IFormFile imageFile, string? folderName);
        Task<ServiceResponse> DeleteImageFromCloudinary(string imageUrl);
    }
}
