using Microsoft.Extensions.Configuration;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace StudentManagement.Infrastructure.AI
{
    public class OllamaLanguageModelService
      : ILanguageModelService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OllamaLanguageModelService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateAnswerAsync(
            string prompt)
        {
            var model =
                _configuration["Ollama:ChatModel"]
                ?? throw new InvalidOperationException(
                    "Ollama chat model is not configured.");

            var request = new
            {
                model,
                prompt,
                stream = false
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "/api/generate",
                    request);

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content
                    .ReadFromJsonAsync<OllamaResponse>();

            if (result == null ||
                string.IsNullOrWhiteSpace(result.Response))
            {
                throw new InvalidOperationException(
                    "Ollama did not return an answer.");
            }

            return result.Response;
        }

        private class OllamaResponse
        {
            public string Response { get; set; } = string.Empty;
        }
    }
}
