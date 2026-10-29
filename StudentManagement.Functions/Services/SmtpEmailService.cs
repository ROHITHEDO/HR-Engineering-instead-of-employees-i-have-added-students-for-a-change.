using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using StudentManagement.Functions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Functions.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendStudentCreatedEmailAsync(
            StudentCreatedEvent student)
        {
            var host = _configuration["Smtp:Host"]
                ?? throw new InvalidOperationException(
                    "SMTP host is missing.");

            var port = int.Parse(
                _configuration["Smtp:Port"]
                ?? throw new InvalidOperationException(
                    "SMTP port is missing."));

            var username = _configuration["Smtp:Username"]
                ?? throw new InvalidOperationException(
                    "SMTP username is missing.");

            var password = _configuration["Smtp:Password"]
                ?? throw new InvalidOperationException(
                    "SMTP password is missing.");

            var senderEmail = _configuration["Smtp:SenderEmail"]
                ?? throw new InvalidOperationException(
                    "SMTP sender email is missing.");

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "Student Management System",
                    senderEmail));

            message.To.Add(
                new MailboxAddress(
                    $"{student.FirstName} {student.LastName}",
                    student.Email));

            message.Subject =
                "Student Registration Successful";

            message.Body = new BodyBuilder
            {
                HtmlBody = $"""
                <html>
                    <body>
                        <h2>Student Registration Successful</h2>

                        <p>
                            Hello {student.FirstName} {student.LastName},
                        </p>

                        <p>
                            Your student profile has been
                            successfully created.
                        </p>

                        <p>
                            <strong>Student ID:</strong>
                            {student.StudentNumber}
                            <br/>

                            <strong>Course:</strong>
                            {student.Course}
                        </p>

                        <p>
                            Regards,<br/>
                            Student Management System
                        </p>
                    </body>
                </html>
                """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                username,
                password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}
