using System.ComponentModel.DataAnnotations;

namespace UploadFileAPI.Models
{
    public class SaveFileRequest
    {
        public string FileName { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
    }
}
