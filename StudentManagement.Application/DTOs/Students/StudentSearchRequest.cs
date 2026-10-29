using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Students
{
    public class StudentSearchRequest
    {
        public string? StudentId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Course { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
