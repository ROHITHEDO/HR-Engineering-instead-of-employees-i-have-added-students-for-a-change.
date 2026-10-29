using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs.Students;
using StudentManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagement.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(
            IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateStudentRequest request)
        {
            var student =
                await _studentService.CreateAsync(request);
             
            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id },
                student);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student =
                await _studentService.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students =
                await _studentService.GetAllAsync();

            return Ok(students);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] StudentSearchRequest request)
        {
            var result =
                await _studentService.SearchAsync(request);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateStudentRequest request)
        {
            var student =
                await _studentService.UpdateAsync(
                    id,
                    request);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted =
                await _studentService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
