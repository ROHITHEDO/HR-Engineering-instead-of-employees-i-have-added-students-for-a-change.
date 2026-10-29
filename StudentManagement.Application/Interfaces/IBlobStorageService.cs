using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream,string blobName,string contentType);

        Task DeleteAsync(string blobName);
    }
}
