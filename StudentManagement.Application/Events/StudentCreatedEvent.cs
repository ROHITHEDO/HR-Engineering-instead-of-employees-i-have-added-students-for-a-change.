using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Events
{
    public class StudentCreatedEvent
    {
        public int StudentId { get; set; }

        public string StudentNumber { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Course { get; set; } = null!;
    }
}
