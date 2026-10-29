using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IDocumentTextExtractor
    {
        Task<string> ExtractTextAsync(
            Stream stream,
            string contentType);
    }
}
