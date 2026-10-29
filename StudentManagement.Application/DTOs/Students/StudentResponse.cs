using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Students
{
    public class StudentResponse
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Course { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
