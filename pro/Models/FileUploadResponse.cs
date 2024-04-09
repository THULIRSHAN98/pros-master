using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pro.Models
{
    public class FileUploadResponse
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public string ContentType { get; set; }

        public int PositionId { get; set; } 

        public bool Status { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
