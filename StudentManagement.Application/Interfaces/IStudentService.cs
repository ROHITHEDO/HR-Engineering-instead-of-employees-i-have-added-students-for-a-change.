using StudentManagement.Application.DTOs.Students;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponse> CreateAsync(
            CreateStudentRequest request);

        Task<StudentResponse?> GetByIdAsync(int id);

        Task<List<StudentResponse>> GetAllAsync();
        Task<PagedResult<StudentResponse>> SearchAsync(StudentSearchRequest request);

        Task<StudentResponse?> UpdateAsync(
            int id,
            UpdateStudentRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
