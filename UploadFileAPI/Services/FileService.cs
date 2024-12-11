using System.Data.SqlClient;
using System.Data;
using UploadFileAPI.Models;

namespace UploadFileAPI.Services
{
    public interface IFileService
    {
        Task<ApiResponse> UploadFileAsync(IFormFile file);
        ApiResponse SaveFileDetails(SaveFileRequest request);
    }

    public class FileService : IFileService
    {
        private readonly string _uploadFolderPath;
        private readonly long _maxFileSize;
        private readonly string _connectionString;
        private readonly string _storeProcedureName;

        public FileService(IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings");
            _uploadFolderPath = appSettings["UploadFolderPath"];
            _maxFileSize = long.Parse(appSettings["MaxFileSize"]);
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _storeProcedureName = appSettings["StoreProcedureName"];
        }

        public ApiResponse SaveFileDetails(SaveFileRequest request)
        {
            if (string.IsNullOrEmpty(request.FileName))
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "FileName is required."
                };
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(_storeProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@FileName", request.FileName);
                        command.Parameters.AddWithValue("@Duration", request.Duration);
                        command.Parameters.AddWithValue("@Status", request.Status);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return new ApiResponse
                {
                    IsSuccess = true,
                    Message = "File details saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "No file was uploaded or the file is empty."
                };
            }

            if (file.Length > _maxFileSize)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = $"File size exceeds the limit of {_maxFileSize / 1024 / 1024} MB."
                };
            }

            try
            {
                // Ensure upload folder exists
                if (!Directory.Exists(_uploadFolderPath))
                {
                    Directory.CreateDirectory(_uploadFolderPath);
                }

                // Save file to server
                var filePath = Path.Combine(_uploadFolderPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new ApiResponse
                {
                    IsSuccess = true,
                    Message = "File uploaded successfully."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
    }
}
