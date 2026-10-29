using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Students
{
    public class UpdateStudentRequest
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Course { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }
    }
}
