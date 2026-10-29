using Microsoft.AspNetCore.Http;
using StudentManagement.Application.DTOs.Documents;
using System;
using System.Collections.Generic;
using System.Text;


namespace StudentManagement.Application.Interfaces
{
    public interface IStudentDocumentService
    {
        Task<StudentDocumentResponse> UploadAsync(int studentId,IFormFile file);

        Task<List<StudentDocumentResponse>> GetByStudentIdAsync(int studentId);

        Task<bool> DeleteAsync(int studentId,int documentId);
    }
}
