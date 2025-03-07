using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class FirebaseService : IFirebaseService
{
    private readonly string _bucketName = "tickethub-af919.appspot.com";
    private readonly StorageClient _storageClient;

    public FirebaseService(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    public async Task<ResponseDto> DeleteImage(string filePath)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(_bucketName, filePath);

            return new ResponseDto
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = $"Delete image {filePath} successfully!"
            };
        }
        catch (Exception)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "Delete image failed!"
            };
        }
    }

    /*public Task<MemoryStream> GetImage(string filePath)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDto> UploadImageTicket(IFormFile file, string folder)
    {
        if (file is null || file.Length == 0)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "File is empty!"
            };
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}"; // Tạo tên file duy nhất
        var filePath = $"{folder}/{fileName}"; // Đường dẫn đầy đủ cho tệp trong Firebase

        // Khởi tạo luồng đọc từ file
        await using (var stream = file.OpenReadStream())
        {
            // Upload file lên Firebase
            var result = await _storageClient.UploadObjectAsync(
                _bucketName,
                filePath,
                file.ContentType, // Loại MIME của tệp
                stream,
                new UploadObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead // Để tệp có thể truy cập công khai
                }
            );
        }

        // Tạo URL công khai cho hình ảnh vừa tải lên
        string publicUrl = $"https://storage.googleapis.com/{_bucketName}/{filePath}";

        return new ResponseDto()
        {
            IsSuccess = true,
            StatusCode = 200,
            Result = publicUrl, // Trả về URL công khai
            Message = "Upload image successfully!"
        };
    }


    public async Task<ResponseDto> UploadImageUser(IFormFile file, string folder)
    {
        if (file is null || file.Length == 0)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "File is empty!"
            };
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}"; // Tạo tên file duy nhất
        var filePath = $"{folder}/{fileName}"; // Đường dẫn đầy đủ cho tệp trong Firebase

        // Khởi tạo luồng đọc từ file
        await using (var stream = file.OpenReadStream())
        {
            // Upload file lên Firebase
            var result = await _storageClient.UploadObjectAsync(
                _bucketName,
                filePath,
                file.ContentType, // Loại MIME của tệp
                stream,
                new UploadObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead // Để tệp có thể truy cập công khai
                }
            );
        }

        // Tạo URL công khai cho hình ảnh vừa tải lên
        string publicUrl = $"https://storage.googleapis.com/{_bucketName}/{filePath}";

        return new ResponseDto()
        {
            IsSuccess = true,
            StatusCode = 200,
            Result = publicUrl, // Trả về URL công khai
            Message = "Upload image successfully!"
        };
    }

    public async Task<ResponseDto> UploadImageChPlay(IFormFile file, string folder)
    {
        if (file is null || file.Length == 0)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "File is empty!"
            };
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = $"{folder}/{fileName}";

        await using (var stream = file.OpenReadStream())
        {
            var result = await _storageClient.UploadObjectAsync(
                _bucketName,
                filePath,
                file.ContentType,
                stream,
                new UploadObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead
                }
            );
        }

        string publicUrl = $"https://storage.googleapis.com/{_bucketName}/{filePath}";

        return new ResponseDto()
        {
            IsSuccess = true,
            StatusCode = 200,
            Result = publicUrl,
            Message = "Upload image successfully!"
        };
    }

    /// <summary>
    /// This method for delete an image from firebase storage bucket
    ///  </summary>
    ///  <param name="filePath">The path of the file</param>
    ///  <returns></returns>
    public async Task<ResponseDto> DeleteImage(string filePath)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(_bucketName, filePath);

            return new ResponseDto()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = $"Delete image {filePath} successfully!"
            };
        }
        catch (Exception e)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "Delete image failed!"
            };
        }
    }*/

    public async Task<ResponseDto> UploadImage(IFormFile file, string folder, string? oldFileUrl = null)
    {
        if (file is null || file.Length == 0)
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "File is empty!"
            };

        // 🔹 Nếu có ảnh cũ, xóa ảnh cũ trước khi upload ảnh mới
        if (!string.IsNullOrEmpty(oldFileUrl))
        {
            // Trích xuất đường dẫn từ URL công khai
            var filePath = oldFileUrl.Replace($"https://storage.googleapis.com/{_bucketName}/", "");
            await DeleteImage(filePath);
        }

        // 🔹 Tạo tên file mới
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePathNew = $"{folder}/{fileName}";

        // 🔹 Upload file lên Firebase
        await using (var stream = file.OpenReadStream())
        {
            await _storageClient.UploadObjectAsync(
                _bucketName,
                filePathNew,
                file.ContentType,
                stream,
                new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
            );
        }

        var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{filePathNew}";

        return new ResponseDto
        {
            IsSuccess = true,
            StatusCode = 200,
            Result = publicUrl,
            Message = "Upload image successfully!"
        };
    }
}