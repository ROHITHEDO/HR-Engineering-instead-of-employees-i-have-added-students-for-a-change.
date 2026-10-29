using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Functions.Models
{
    public class StudentCreatedEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string StudentNumber { get; set; }
        public string Course { get; set; }
    }
}
