using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{

    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);

        Task<Student?> GetByStudentIdAsync(string studentId);

        Task<List<Student>> GetAllAsync();
        Task<(List<Student> Students, int TotalCount)> SearchAsync(string? studentId,string? name,string? email,
            string? course,int page,int pageSize);

        Task AddAsync(Student student);

        void Update(Student student);

        void Delete(Student student);

        Task<bool> ExistsAsync(int id);

        Task SaveChangesAsync();
    }
}
