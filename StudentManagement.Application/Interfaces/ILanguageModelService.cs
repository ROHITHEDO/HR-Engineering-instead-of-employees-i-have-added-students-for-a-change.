using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Application.Interfaces
{
    public interface ILanguageModelService
    {
        Task<string> GenerateAnswerAsync(
            string prompt);
    }
}
