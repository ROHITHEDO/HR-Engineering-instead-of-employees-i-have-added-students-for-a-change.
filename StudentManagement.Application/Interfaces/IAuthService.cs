using StudentManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);

        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
