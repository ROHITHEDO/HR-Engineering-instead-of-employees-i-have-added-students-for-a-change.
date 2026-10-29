using StudentManagement.Functions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Functions.Services
{
    public interface IEmailService
    {
        Task SendStudentCreatedEmailAsync(StudentCreatedEvent student);
    }
}
