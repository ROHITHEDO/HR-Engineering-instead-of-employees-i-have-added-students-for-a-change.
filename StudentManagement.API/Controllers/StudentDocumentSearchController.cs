using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Interfaces;

namespace StudentManagement.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/students/{studentId:int}/search")]
    public class StudentDocumentSearchController : ControllerBase
    {
        private readonly IStudentDocumentSearchService _searchService;

        public StudentDocumentSearchController(
            IStudentDocumentSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            int studentId,
            [FromQuery] string query,
            [FromQuery] int topK = 5)
        {
            var results =
                await _searchService.SearchAsync(
                    studentId,
                    query,
                    topK);

            return Ok(results);
        }
    }
}
