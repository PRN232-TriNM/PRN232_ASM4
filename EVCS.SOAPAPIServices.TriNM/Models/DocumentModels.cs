using System;

namespace EVCS.SOAPAPIServices.TriNM.Models
{
    public class DocumentUploadResponse
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string PublicUrl { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UserId { get; set; }
        public string FileType { get; set; }
    }

    public class DocumentUrlResponse
    {
        public string FilePath { get; set; }
        public string PublicUrl { get; set; }
    }

    public class DocumentListResponse
    {
        public List<DocumentFileInfo> Files { get; set; }
        public int TotalCount { get; set; }
    }

    public class DocumentFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string PublicUrl { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string FileType { get; set; }
    }
}