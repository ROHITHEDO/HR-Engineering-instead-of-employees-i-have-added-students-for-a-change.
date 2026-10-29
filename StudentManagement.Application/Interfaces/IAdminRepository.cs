using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByEmailAsync(string email);

        Task AddAsync(Admin admin);

        Task SaveChangesAsync();
    }
}
