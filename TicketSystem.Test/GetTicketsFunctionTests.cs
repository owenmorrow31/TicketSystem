using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicketSystem.Models;
using TicketSystem.Repository;
using Newtonsoft.Json;

namespace TicketSystem.Test
{
    [TestFixture]
    public class GetTicketsFunctionTests
    {
        private string _testFilePath;
        private Mock<ILogger> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), "test_tickets.json");
            _mockLogger = new Mock<ILogger>();

            // Prepare test data
            var tickets = new List<Ticket>
            {
                new Ticket { TicketID = "1", Description = "Test Ticket 1", Status = "Open", Priority = "High" },
                new Ticket { TicketID = "2", Description = "Test Ticket 2", Status = "Closed", Priority = "Low" }
            };

            // Write the test data to the temporary file
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(tickets));
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Test]
        public async Task RunAsync_ShouldReturnOkObjectResult_WithListOfTickets()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;

            Environment.SetEnvironmentVariable("FilePath", _testFilePath);

            // Act
            var result = await GetTicketsFunction.RunAsync(request, _mockLogger.Object, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result, "Expected OkObjectResult, but got a different result.");
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult?.Value, "OkObjectResult's Value should not be null.");
            var tickets = okResult.Value as List<Ticket>;
            Assert.IsNotNull(tickets, "Expected a list of tickets, but got null.");
            Assert.AreEqual(2, tickets.Count, $"Expected 2 tickets, but found {tickets.Count}.");
        }

        [Test]
        public async Task RunAsync_ShouldReturnNotFound_WhenTicketFileDoesNotExist()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }

            var context = new DefaultHttpContext();
            var request = context.Request;

            Environment.SetEnvironmentVariable("FilePath", _testFilePath);

            var result = await GetTicketsFunction.RunAsync(request, _mockLogger.Object, null);

            Assert.IsInstanceOf<NotFoundObjectResult>(result, "Expected NotFoundObjectResult, but got a different result.");
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "NotFoundObjectResult should not be null.");
            Assert.AreEqual("Tickets file not found.", notFoundResult.Value, "NotFound message did not match the expected value.");
        }

        [Test]
        public async Task RunAsync_ShouldReturnTicket_WhenValidTicketIdIsProvided()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;

            
            Environment.SetEnvironmentVariable("FilePath", _testFilePath);

            var result = await GetTicketsFunction.RunAsync(request, _mockLogger.Object, "1");

            Assert.IsInstanceOf<OkObjectResult>(result, "Expected OkObjectResult when a valid ticket ID is provided.");
            var okResult = result as OkObjectResult;
            var ticket = okResult?.Value as Ticket;
            Assert.IsNotNull(ticket, "Expected a Ticket object, but got null.");
            Assert.AreEqual("1", ticket?.TicketID, $"Expected ticket ID '1', but got '{ticket?.TicketID}'.");
        }
        [Test]
        public async Task RunAsync_ShouldReturnInternalServerError_WhenJsonIsInvalid()
        {
            // Write invalid JSON to simulate a corrupted file.
            File.WriteAllText(_testFilePath, "Invalid JSON Content");

            var context = new DefaultHttpContext();
            var request = context.Request;

            Environment.SetEnvironmentVariable("FilePath", _testFilePath);

            // Act
            var result = await GetTicketsFunction.RunAsync(request, _mockLogger.Object, null);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result, "Expected an ObjectResult for internal server error.");
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult?.StatusCode);
            Assert.AreEqual("Error reading ticket data.", objectResult?.Value);
        }
        [Test]
        public async Task RunAsync_ShouldReturnEmptyList_WhenJsonIsEmpty()
        {
            // Write an empty JSON array to simulate no tickets.
            File.WriteAllText(_testFilePath, "[]");

            var context = new DefaultHttpContext();
            var request = context.Request;

            Environment.SetEnvironmentVariable("FilePath", _testFilePath);

            // Act
            var result = await GetTicketsFunction.RunAsync(request, _mockLogger.Object, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result, "Expected OkObjectResult for an empty ticket list.");
            var okResult = result as OkObjectResult;
            var tickets = okResult?.Value as List<Ticket>;
            Assert.IsNotNull(tickets);
            Assert.AreEqual(0, tickets.Count, "Expected 0 tickets for an empty JSON file.");
        }
       
        [Test]
        public async Task RunAsync_ShouldReturnInternalServerError_OnUnexpectedException()
        {
            // Mock a scenario where reading the file throws an IOException.
            var mockLogger = new Mock<ILogger>();
            var context = new DefaultHttpContext();
            var request = context.Request;

            // Create a temporary file and then lock it by opening a FileStream.
            using (var fileStream = new FileStream(_testFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                Environment.SetEnvironmentVariable("FilePath", _testFilePath);

                // Act
                var result = await GetTicketsFunction.RunAsync(request, mockLogger.Object, null);

                // Assert
                Assert.IsInstanceOf<StatusCodeResult>(result, "Expected StatusCodeResult for internal server error.");
                var statusCodeResult = result as StatusCodeResult;
                Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult?.StatusCode);
            }
        }
        
        
      





        
    }
}




