using Microsoft.AspNetCore.Http;
using StudentManagement.Application.DTOs.Documents;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace StudentManagement.Application.Services
{
    public class StudentDocumentService : IStudentDocumentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IStudentDocumentRepository _documentRepository;
        private readonly IDocumentTextExtractor _textExtractor;
        private readonly ITextChunker _chunker;
        private readonly IVectorSearchService _vectorSearch;

        public StudentDocumentService(
             IStudentRepository studentRepository,
             IBlobStorageService blobStorageService,
             IStudentDocumentRepository documentRepository,
             IDocumentTextExtractor textExtractor,
             ITextChunker chunker,
             IVectorSearchService vectorSearch)
        {
            _studentRepository = studentRepository;
            _blobStorageService = blobStorageService;
            _documentRepository = documentRepository;
            _textExtractor = textExtractor;
            _chunker = chunker;
            _vectorSearch = vectorSearch;
        }

        public async Task<StudentDocumentResponse> UploadAsync(
            int studentId,
            IFormFile file)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File is required.");

            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
                throw new InvalidOperationException(
                    "File size cannot exceed 10 MB.");

            var extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions = new[]
            {
            ".pdf",
            ".docx"
        };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "Only PDF and DOCX files are allowed.");

            var allowedContentTypes = new[]
            {
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

            if (!allowedContentTypes.Contains(file.ContentType))
                throw new InvalidOperationException(
                    "Invalid file content type.");

            var safeFileName = Path.GetFileName(file.FileName);

            var blobName =
                $"students/{student.StudentId}/{Guid.NewGuid()}_{safeFileName}";

            await using var stream = file.OpenReadStream();

            await _blobStorageService.UploadAsync(
                stream,
                blobName,
                file.ContentType);

            try
            {
                var document = new StudentDocument
                {
                    StudentId = studentId,
                    FileName = safeFileName,
                    BlobName = blobName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow
                };

                await _documentRepository.AddAsync(document);
                await _documentRepository.SaveChangesAsync();

                // AI INDEXING

                using var fileStream = file.OpenReadStream();

                var text =
                    await _textExtractor.ExtractTextAsync(
                        fileStream,
                        file.ContentType);

                var chunks =
                    _chunker.Chunk(text);

                for (int i = 0; i < chunks.Count; i++)
                {
                    await _vectorSearch.IndexAsync(
                        chunks[i],
                        studentId,
                        document.Id,
                        i);
                }

                return Map(document);
            }
            catch (Exception)
            {
                await _blobStorageService.DeleteAsync(blobName);
                throw;
            }
            
        }

        public async Task<List<StudentDocumentResponse>> GetByStudentIdAsync(
            int studentId)
        {
            var documents = await _documentRepository.GetByStudentIdAsync(studentId);

            return documents
                .Select(Map)
                .ToList();
        }

        public async Task<bool> DeleteAsync(
            int studentId,
            int documentId)
        {
            var document = await _documentRepository.GetByIdAsync(
        studentId,
        documentId);

            if (document == null)
                return false;

            await _blobStorageService.DeleteAsync(document.BlobName);

            _documentRepository.Delete(document);

            await _documentRepository.SaveChangesAsync();

            return true;
        }

        private static StudentDocumentResponse Map(
            StudentDocument document)
        {
            return new StudentDocumentResponse
            {
                Id = document.Id,
                StudentId = document.StudentId,
                FileName = document.FileName,
                ContentType = document.ContentType,
                FileSize = document.FileSize,
                UploadedAt = document.UploadedAt
            };
        }
    }
}
