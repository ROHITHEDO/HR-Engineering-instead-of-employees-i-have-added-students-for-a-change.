using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using StudentManagement.Application.Events;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace StudentManagement.Infrastructure.Messaging
{
    public class ServiceBusStudentEventPublisher
     : IStudentEventPublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusStudentEventPublisher(
            IConfiguration configuration)
        {
            var connectionString =
                configuration["ServiceBus:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Service Bus connection string is not configured.");

            var queueName =
                configuration["ServiceBus:QueueName"]
                ?? throw new InvalidOperationException(
                    "Service Bus queue name is not configured.");

            var client =
                new ServiceBusClient(connectionString);

            _sender = client.CreateSender(queueName);
        }

        public async Task PublishStudentCreatedAsync(
            StudentCreatedEvent studentCreatedEvent)
        {
            var json = JsonSerializer.Serialize(
                studentCreatedEvent);

            var message = new ServiceBusMessage(json)
            {
                Subject = "StudentCreated",
                ContentType = "application/json"
            };

            await _sender.SendMessageAsync(message);
        }
    }
}
