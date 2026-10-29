using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Interfaces;

namespace StudentManagement.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/students/{studentId:int}/documents")]
    public class StudentDocumentsController : ControllerBase
    {
        private readonly IStudentDocumentService _documentService;

        public StudentDocumentsController(
            IStudentDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            int studentId,
            IFormFile file)
        {
            try
            {
                var document =
                    await _documentService.UploadAsync(
                        studentId,
                        file);

                return Ok(document);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDocuments(
            int studentId)
        {
            var documents =
                await _documentService
                    .GetByStudentIdAsync(studentId);

            return Ok(documents);
        }

        [HttpDelete("{documentId:int}")]
        public async Task<IActionResult> Delete(
            int studentId,
            int documentId)
        {
            var deleted =
                await _documentService.DeleteAsync(
                    studentId,
                    documentId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
