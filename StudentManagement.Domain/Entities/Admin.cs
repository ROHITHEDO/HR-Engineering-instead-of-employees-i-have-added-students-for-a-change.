using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Domain.Entities
{
    public class Admin
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
