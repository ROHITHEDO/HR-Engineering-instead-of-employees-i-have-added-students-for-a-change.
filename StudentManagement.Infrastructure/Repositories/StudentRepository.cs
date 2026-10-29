using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Student?> GetByStudentIdAsync(
        string studentId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(x => x.StudentId == studentId);
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<(List<Student> Students, int TotalCount)> SearchAsync(string? studentId,string? name,
    string? email,string? course,int page,int pageSize)
    {
        IQueryable<Student> query =
            _context.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(studentId))
        {
            query = query.Where(x =>
                x.StudentId.Contains(studentId));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x =>
                (x.FirstName + " " + x.LastName)
                    .Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(x =>
                x.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(course))
        {
            query = query.Where(x =>
                x.Course.Contains(course));
        }

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
    }

    public void Update(Student student)
    {
        _context.Students.Update(student);
    }

    public void Delete(Student student)
    {
        _context.Students.Remove(student);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students
            .AnyAsync(x => x.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}