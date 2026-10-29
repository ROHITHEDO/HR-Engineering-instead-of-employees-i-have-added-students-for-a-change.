using StudentManagement.Application.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IVectorSearchService
    {
        Task IndexAsync(
            string text,
            int studentId,
            int documentId,
            int chunkId);

        Task<List<VectorSearchResult>> SearchAsync(
            string query,
            int studentId,
            int topK = 5);
    }
}
