using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using StudentManagement.Functions.Services;
using StudentManagement.Functions.Models;

namespace StudentManagement.Functions;

public class StudentCreatedFunction
{
    private readonly ILogger<StudentCreatedFunction> _logger;
    private readonly IEmailService _emailService;

    public StudentCreatedFunction(
        ILogger<StudentCreatedFunction> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function("StudentCreatedFunction")]
    public async Task Run(
        [ServiceBusTrigger(
            "queue.1",
            Connection = "ServiceBusConnection")]
        string message)
    {
        _logger.LogInformation(
            "Received StudentCreated message: {Message}",
            message);

        var student =
            JsonSerializer.Deserialize<StudentCreatedEvent>(
                message);

        if (student == null)
        {
            _logger.LogError(
                "Failed to deserialize StudentCreated message.");

            return;
        }

        await _emailService.SendStudentCreatedEmailAsync(
            student);

        _logger.LogInformation(
            "Student creation email sent to {Email}.",
            student.Email);
    }
}