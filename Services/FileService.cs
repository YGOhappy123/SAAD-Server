using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using milktea_server.Dtos.Response;
using milktea_server.Interfaces.Services;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;

            var account = new Account(
                _configuration["Cloudinary:CloudName"],
                _configuration["Cloudinary:ApiKey"],
                _configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        private string GetPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var publicIdWithExtension = segments.Last();
            var publicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));

            return string.Join("/", segments.Skip(segments.Length - 2).Take(1)) + "/" + publicId;
        }

        public async Task<ServiceResponse> UploadImageToCloudinary(IFormFile imageFile, string? folderName)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                    Folder = string.IsNullOrWhiteSpace(folderName) ? "ptit-milk-tea" : folderName,
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                bool isSuccess = uploadResult.StatusCode == System.Net.HttpStatusCode.OK;

                return new ServiceResponse
                {
                    Status = isSuccess ? ResStatusCode.OK : ResStatusCode.INTERNAL_SERVER_ERROR,
                    Success = isSuccess,
                    Message = isSuccess ? SuccessMessage.UPLOAD_IMAGE_SUCCESSFULLY : ErrorMessage.UPLOAD_IMAGE_FAILED,
                    ImageUrl = isSuccess ? uploadResult.SecureUrl.ToString() : "",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.INTERNAL_SERVER_ERROR,
                    Success = false,
                    Message = ErrorMessage.UPLOAD_IMAGE_FAILED,
                };
            }
        }

        public async Task<ServiceResponse> DeleteImageFromCloudinary(string imageUrl)
        {
            try
            {
                var publicId = GetPublicIdFromUrl(imageUrl);
                var deletionParams = new DeletionParams(publicId);

                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                bool isSuccess = deletionResult.StatusCode == System.Net.HttpStatusCode.OK;

                return new ServiceResponse
                {
                    Status = isSuccess ? ResStatusCode.OK : ResStatusCode.INTERNAL_SERVER_ERROR,
                    Success = isSuccess,
                    Message = isSuccess ? SuccessMessage.DELETE_IMAGE_SUCCESSFULLY : ErrorMessage.DELETE_IMAGE_FAILED,
                };
            }
            catch (Exception)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.INTERNAL_SERVER_ERROR,
                    Success = false,
                    Message = ErrorMessage.DELETE_IMAGE_FAILED,
                };
            }
        }
    }
}
