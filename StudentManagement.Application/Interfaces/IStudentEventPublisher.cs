using StudentManagement.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IStudentEventPublisher
    {
        Task PublishStudentCreatedAsync(
            StudentCreatedEvent studentCreatedEvent);
    }
}
