using StudentManagement.Application.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IStudentRagService
    {
        Task<StudentRagResponse> AskAsync(
            int studentId,
            string question);
    }
}
