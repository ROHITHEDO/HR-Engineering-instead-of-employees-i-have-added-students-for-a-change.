using StudentManagement.Application.DTOs.Search;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SemanticKernel;

namespace StudentManagement.Infrastructure.AI
{
    public class StudentRagService : IStudentRagService
    {
        private readonly IVectorSearchService _vectorSearch;
        private readonly ILanguageModelService _languageModel;
        private readonly Kernel _kernel;

        public StudentRagService(
            IVectorSearchService vectorSearch,
            ILanguageModelService languageModel, Kernel kernel)
        {
            _vectorSearch = vectorSearch;
            _languageModel = languageModel;
            _kernel = kernel;
        }

        public async Task<StudentRagResponse> AskAsync(
            int studentId,
            string question)
        {
            var chunks =
                await _vectorSearch.SearchAsync(
                    question,
                    studentId,
                    5);

            if (chunks.Count == 0)
            {
                return new StudentRagResponse
                {
                    Answer =
                        "I could not find enough information in the student's documents.",
                    Sources = []
                };
            }

            var context = new StringBuilder();

            foreach (var chunk in chunks)
            {
                context.AppendLine(
                    $"[Document: {chunk.DocumentId}, " +
                    $"Chunk: {chunk.ChunkId}]");

                context.AppendLine(chunk.Text);
                context.AppendLine();
            }

            var prompt = $"""
            You are a student information assistant.

            Answer the user's question using ONLY the
            information provided in the context.

            If the answer cannot be determined from the
            context, say:
            "I don't have enough information in the
            student's records to answer that."

            Do not invent or assume information.

            Student ID:
            {studentId}

            User Question:
            {question}

            Context:
            {context}

            Answer:
            """;


           // var answer =await _languageModel.GenerateAnswerAsync(prompt);

            //instead manully invoking the model, let kernal call the model
            var result = await _kernel.InvokePromptAsync(prompt);

            var answer = result.ToString();

            return new StudentRagResponse
            {
                Answer = answer,

                Sources = chunks
                    .Select(x => new StudentRagSource
                    {
                        DocumentId = x.DocumentId,
                        ChunkId = x.ChunkId,
                        Score = x.Score,
                        Text = x.Text
                    })
                    .ToList()
            };
        }
    }
}
