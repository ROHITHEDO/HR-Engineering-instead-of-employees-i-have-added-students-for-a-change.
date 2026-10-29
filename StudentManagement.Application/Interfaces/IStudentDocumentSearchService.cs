using StudentManagement.Application.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IStudentDocumentSearchService
    {
        Task<List<StudentDocumentSearchResponse>> SearchAsync(
            int studentId,
            string query,
            int topK = 5);
    }
}
