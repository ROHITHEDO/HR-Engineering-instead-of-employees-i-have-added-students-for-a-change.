using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Infrastructure.AI
{
    public class TextChunker : ITextChunker
    {
        public List<string> Chunk(
            string text,
            int chunkSize = 1000,
            int overlap = 200)
        {
            var chunks = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
                return chunks;

            text = text.Trim();

            var start = 0;

            while (start < text.Length)
            {
                var length =
                    Math.Min(
                        chunkSize,
                        text.Length - start);

                var chunk =
                    text.Substring(start, length);

                chunks.Add(chunk);

                if (start + length >= text.Length)
                    break;

                start += chunkSize - overlap;
            }

            return chunks;
        }
    }
}
