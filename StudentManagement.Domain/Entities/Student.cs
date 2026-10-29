using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Domain.Entities
{
    public class Student
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
        public ICollection<StudentDocument> Documents { get; set; }
       = new List<StudentDocument>();
    }
}
