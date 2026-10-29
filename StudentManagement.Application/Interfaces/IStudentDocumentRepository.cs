using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IStudentDocumentRepository
    {
        Task AddAsync(StudentDocument document);

        Task<List<StudentDocument>> GetByStudentIdAsync(int studentId);

        Task<StudentDocument?> GetByIdAsync(
            int studentId,
            int documentId);

        void Delete(StudentDocument document);

        Task SaveChangesAsync();
    }
}
