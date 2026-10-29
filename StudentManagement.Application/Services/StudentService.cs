using StudentManagement.Application.DTOs.Students;
using StudentManagement.Application.Events;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IStudentEventPublisher _eventPublisher;

    public StudentService(
     IStudentRepository studentRepository,
     IStudentEventPublisher eventPublisher)
    {
        _repository = studentRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<StudentResponse> CreateAsync(
        CreateStudentRequest request)
    {
        var existingStudent =
            await _repository.GetByStudentIdAsync(
                request.StudentId);

        if (existingStudent != null)
        {
            throw new InvalidOperationException(
                "Student ID already exists.");
        }

        var student = new Student
        {
            StudentId = request.StudentId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Course = request.Course,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(student);
        await _repository.SaveChangesAsync();

        var studentCreatedEvent = new StudentCreatedEvent
        {
            StudentId = student.Id,
            StudentNumber = student.StudentId,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Course = student.Course
        };

        await _eventPublisher.PublishStudentCreatedAsync(
            studentCreatedEvent);

        return MapToResponse(student);
    }

    public async Task<StudentResponse?> GetByIdAsync(int id)
    {
        var student =
            await _repository.GetByIdAsync(id);

        if (student == null)
        {
            return null;
        }

        return MapToResponse(student);
    }

    public async Task<List<StudentResponse>> GetAllAsync()
    {
        var students =
            await _repository.GetAllAsync();

        return students
            .Select(MapToResponse)
            .ToList();
    }
    public async Task<PagedResult<StudentResponse>> SearchAsync(
    StudentSearchRequest request)
    {
        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize < 1
            ? 10
            : request.PageSize;

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var result = await _repository.SearchAsync(
            request.StudentId,
            request.Name,
            request.Email,
            request.Course,
            page,
            pageSize);

        return new PagedResult<StudentResponse>
        {
            Items = result.Students
                .Select(MapToResponse)
                .ToList(),

            Page = page,

            PageSize = pageSize,

            TotalCount = result.TotalCount
        };
    }

    public async Task<StudentResponse?> UpdateAsync(
        int id,
        UpdateStudentRequest request)
    {
        var student =
            await _repository.GetByIdAsync(id);

        if (student == null)
        {
            return null;
        }

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.Email = request.Email;
        student.Course = request.Course;
        student.DateOfBirth = request.DateOfBirth;
        student.UpdatedAt = DateTime.UtcNow;

        _repository.Update(student);

        await _repository.SaveChangesAsync();

        return MapToResponse(student);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student =
            await _repository.GetByIdAsync(id);

        if (student == null)
        {
            return false;
        }

        _repository.Delete(student);

        await _repository.SaveChangesAsync();

        return true;
    }

    private static StudentResponse MapToResponse(
        Student student)
    {
        return new StudentResponse
        {
            Id = student.Id,
            StudentId = student.StudentId,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Course = student.Course,
            DateOfBirth = student.DateOfBirth,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }
}