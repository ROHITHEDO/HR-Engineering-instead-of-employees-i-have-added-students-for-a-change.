using StudentManagement.Application.DTOs.Search;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Infrastructure.Search
{
    public class StudentDocumentSearchService
    : IStudentDocumentSearchService
    {
        private readonly IVectorSearchService _vectorSearchService;

        public StudentDocumentSearchService(
            IVectorSearchService vectorSearchService)
        {
            _vectorSearchService = vectorSearchService;
        }

        public async Task<List<StudentDocumentSearchResponse>> SearchAsync(
            int studentId,
            string query,
            int topK = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException(
                    "Search query cannot be empty.");

            var results =
                await _vectorSearchService.SearchAsync(
                    query,
                    studentId,
                    topK);

            return results
                .Select(x => new StudentDocumentSearchResponse
                {
                    Text = x.Text,
                    DocumentId = x.DocumentId,
                    ChunkId = x.ChunkId,
                    Score = x.Score
                })
                .ToList();
        }
    }
}
