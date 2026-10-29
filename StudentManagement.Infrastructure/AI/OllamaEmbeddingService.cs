using Microsoft.Extensions.Configuration;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace StudentManagement.Infrastructure.AI
{
    public class OllamaEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OllamaEmbeddingService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var model =
                _configuration["Ollama:EmbeddingModel"]
                ?? throw new InvalidOperationException(
                    "Ollama embedding model is not configured.");

            var request = new
            {
                model,
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/api/embed",
                request);

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content
                    .ReadFromJsonAsync<OllamaEmbeddingResponse>();

            if (result?.Embeddings == null ||
                result.Embeddings.Count == 0)
            {
                throw new InvalidOperationException(
                    "Ollama did not return an embedding.");
            }

            return result.Embeddings[0];
        }

        private class OllamaEmbeddingResponse
        {
            public List<float[]> Embeddings { get; set; } = [];
        }
    }
    //public class OllamaEmbeddingService : IEmbeddingService
    //{
    //    private readonly HttpClient _httpClient;

    //    public OllamaEmbeddingService(HttpClient httpClient)
    //    {
    //        _httpClient = httpClient;
    //    }

    //    public async Task<float[]> GenerateEmbeddingAsync(string text)
    //    {
    //        var request = new
    //        {
    //            model = "nomic-embed-text",
    //            input = text
    //        };

    //        var response = await _httpClient.PostAsJsonAsync(
    //            "/api/embed",
    //            request);

    //        response.EnsureSuccessStatusCode();

    //        var result =
    //            await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();

    //        if (result?.Embeddings == null ||
    //            result.Embeddings.Count == 0)
    //        {
    //            throw new InvalidOperationException(
    //                "Ollama did not return an embedding.");
    //        }

    //        return result.Embeddings[0];
    //    }

    //    private class OllamaEmbeddingResponse
    //    {
    //        public List<float[]> Embeddings { get; set; } = [];
    //    }
    //}
}
