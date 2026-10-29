using Moq;
using StudentManagement.Application.DTOs.Students;
using StudentManagement.Application.Interfaces;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Tests.Application
{
    public class StudentServiceTests
    {
        [Fact]
        //When the StudentService runs, it talks to this
        //fake in-memory object instead of your real infrastructure.
        public async Task CreateAsync_ShouldCreateStudent_WhenStudentIdDoesNotExist()
        {
            // Arrange

            var repository = new Mock<IStudentRepository>();
            var eventPublisher = new Mock<IStudentEventPublisher>();

            repository
                .Setup(x => x.GetByStudentIdAsync("STU001"))
                .ReturnsAsync((Student?)null);

            var service = new StudentService(
                repository.Object,
                eventPublisher.Object);

            var request = new CreateStudentRequest
            {
                StudentId = "STU001",
                FirstName = "Rohith",
                LastName = "Kumar",
                Email = "rohith@test.com",
                Course = ".NET",
                DateOfBirth = new DateTime(2001, 6, 30)
            };

            // Act

            var result = await service.CreateAsync(request);

            // Assert

            Assert.NotNull(result);

            Assert.Equal("STU001", result.StudentId);
            Assert.Equal("Rohith", result.FirstName);
            Assert.Equal("Kumar", result.LastName);
            Assert.Equal("rohith@test.com", result.Email);

            repository.Verify(
                x => x.AddAsync(It.IsAny<Student>()),
                Times.Once);

            repository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            eventPublisher.Verify(
                x => x.PublishStudentCreatedAsync(
                    It.IsAny<StudentManagement.Application.Events.StudentCreatedEvent>()),
                Times.Once);
        }
    }
}
