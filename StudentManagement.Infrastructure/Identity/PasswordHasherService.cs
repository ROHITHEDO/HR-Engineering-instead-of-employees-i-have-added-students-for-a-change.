using Microsoft.AspNetCore.Identity;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Infrastructure.Identity
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<Admin> _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(
                new Admin(),
                password);
        }

        public bool Verify(
            string password,
            string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(
                new Admin(),
                passwordHash,
                password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
