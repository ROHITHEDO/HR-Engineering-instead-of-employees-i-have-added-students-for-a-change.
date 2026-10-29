using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }
    }
}
