using DocumentFormat.OpenXml.Packaging;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Wordprocessing;

namespace StudentManagement.Infrastructure.AI
{
    public class DocumentTextExtractor : IDocumentTextExtractor
    {
        public async Task<string> ExtractTextAsync(
            Stream stream,
            string contentType)
        {
            return contentType switch
            {
                "application/pdf" =>
                    ExtractPdf(stream),

                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" =>
                    ExtractDocx(stream),

                _ => throw new NotSupportedException(
                    $"Unsupported document type: {contentType}")
            };
        }

        private static string ExtractPdf(Stream stream)
        {
            using var document =
                PdfDocument.Open(stream);

            var text = string.Join(
                Environment.NewLine,
                document.GetPages()
                    .Select(page => page.Text));

            return text;
        }

        private static string ExtractDocx(Stream stream)
        {
            using var document =
                WordprocessingDocument.Open(
                    stream,
                    false);

            var body =
                document.MainDocumentPart?
                    .Document
                    .Body;

            if (body == null)
                return string.Empty;

            return string.Join(
                Environment.NewLine,
                body
                    .Descendants<Text>()
                    .Select(x => x.Text));
        }
    }
}
