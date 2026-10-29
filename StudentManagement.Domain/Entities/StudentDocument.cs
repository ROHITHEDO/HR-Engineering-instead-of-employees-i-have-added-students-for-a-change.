using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Domain.Entities
{
    public class StudentDocument
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string FileName { get; set; } = null!;

        public string BlobName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }

        public Student Student { get; set; } = null!;
    }
}
