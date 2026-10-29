using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Interfaces;

namespace StudentManagement.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/students/{studentId:int}/ask")]
    public class StudentRagController : ControllerBase
    {
        private readonly IStudentRagService _ragService;

        public StudentRagController(
            IStudentRagService ragService)
        {
            _ragService = ragService;
        }

        [HttpGet]
        public async Task<IActionResult> Ask(
            int studentId,
            [FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest(
                    "Question cannot be empty.");

            var response =
                await _ragService.AskAsync(
                    studentId,
                    question);

            return Ok(response);
        }
    }
}
