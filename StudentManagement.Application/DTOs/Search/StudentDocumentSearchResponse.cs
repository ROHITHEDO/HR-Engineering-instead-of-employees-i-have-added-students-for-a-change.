using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Search
{
    public class StudentDocumentSearchResponse
    {
        public string Text { get; set; } = null!;
        public int DocumentId { get; set; }
        public int ChunkId { get; set; }
        public float Score { get; set; }
    }
}
