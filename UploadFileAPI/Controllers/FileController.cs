using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using UploadFileAPI.Models;
using UploadFileAPI.Services;

namespace UploadFileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var response = await _fileService.UploadFileAsync(file);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        [Route("save-file-details")]
        public IActionResult SaveFileDetails([FromBody] SaveFileRequest request)
        {
            var response = _fileService.SaveFileDetails(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }

}
