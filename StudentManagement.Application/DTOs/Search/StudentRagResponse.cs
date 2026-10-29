using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Search
{
    public class StudentRagResponse
    {
        public string Answer { get; set; } = null!;

        public List<StudentRagSource> Sources { get; set; } = [];
    }

    public class StudentRagSource
    {
        public int DocumentId { get; set; }

        public int ChunkId { get; set; }

        public float Score { get; set; }

        public string Text { get; set; } = null!;
    }
}
