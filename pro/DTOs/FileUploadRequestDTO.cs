namespace pro.DTOs
{
    public class FileUploadRequestDTO
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public bool Status { get; set; } 
        public int PositionId { get; set; }
    }
}
