using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Infrastructure.Repositories
{
    public class StudentDocumentRepository : IStudentDocumentRepository
    {
        private readonly AppDbContext _context;

        public StudentDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StudentDocument document)
        {
            await _context.StudentDocuments.AddAsync(document);
        }

        public async Task<List<StudentDocument>> GetByStudentIdAsync(
            int studentId)
        {
            return await _context.StudentDocuments
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync();
        }

        public async Task<StudentDocument?> GetByIdAsync(
            int studentId,
            int documentId)
        {
            return await _context.StudentDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.StudentId == studentId);
        }

        public void Delete(StudentDocument document)
        {
            _context.StudentDocuments.Remove(document);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
