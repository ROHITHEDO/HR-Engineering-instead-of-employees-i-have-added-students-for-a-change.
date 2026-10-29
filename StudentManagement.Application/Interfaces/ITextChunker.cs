using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface ITextChunker
    {
        List<string> Chunk(
            string text,
            int chunkSize = 1000,
            int overlap = 200);
    }
}
