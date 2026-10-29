using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Documents
{
    public class StudentDocumentResponse
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string FileName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
