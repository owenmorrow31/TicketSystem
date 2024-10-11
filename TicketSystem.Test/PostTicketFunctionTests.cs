using System;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.IO;
using TicketSystem.Models;
using System.Collections.Generic;

namespace TicketSystem.Test
{
    [TestFixture]
    public class PostTicketFunctionTests
    {
        private string _testFilePath;
        private Mock<ILogger> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), "test_tickets.json");
            _mockLogger = new Mock<ILogger>();
            Environment.SetEnvironmentVariable("FilePath", _testFilePath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }

            Environment.SetEnvironmentVariable("FilePath", null);
        }

        [Test]
        public async Task RunAsync_ShouldReturnOkResult_WhenTicketIsValid()
        {
            var newTicket = new Ticket { TicketID = "2", Description = "New ticket", Status = "Open", Priority = "High" };
            var json = JsonConvert.SerializeObject(newTicket);
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            request.Body.Seek(0, SeekOrigin.Begin);

            var result = await PostTicketFunction.RunAsync(request, _mockLogger.Object);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Ticket added successfully: {newTicket.TicketID}", okResult.Value);
        }

        [Test]
        public async Task RunAsync_ShouldReturnBadRequestResult_WhenTicketIsInvalid()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Invalid JSON"));
            request.Body.Seek(0, SeekOrigin.Begin);

            var result = await PostTicketFunction.RunAsync(request, _mockLogger.Object);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid ticket data provided.", badRequestResult.Value);
        }

        [Test]
        public async Task RunAsync_ShouldAddTicketToFile_WhenTicketIsValid()
        {
            var newTicket = new Ticket { TicketID = "3", Description = "Another ticket", Status = "In Progress", Priority = "Medium" };
            var json = JsonConvert.SerializeObject(newTicket);
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            request.Body.Seek(0, SeekOrigin.Begin);

            await PostTicketFunction.RunAsync(request, _mockLogger.Object);

            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            var tickets = JsonConvert.DeserializeObject<List<Ticket>>(fileContent);
            Assert.IsNotNull(tickets);
            Assert.IsTrue(tickets.Exists(t => t.TicketID == "3"));
        }

        [Test]
        public async Task RunAsync_ShouldAppendToFile_WhenFileAlreadyContainsTickets()
        {
            var existingTicket = new Ticket { TicketID = "1", Description = "Existing ticket", Status = "Closed", Priority = "Low" };
            var initialTickets = new List<Ticket> { existingTicket };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(initialTickets));

            var newTicket = new Ticket { TicketID = "2", Description = "New ticket", Status = "Open", Priority = "High" };
            var json = JsonConvert.SerializeObject(newTicket);
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            request.Body.Seek(0, SeekOrigin.Begin);

            await PostTicketFunction.RunAsync(request, _mockLogger.Object);

            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            var tickets = JsonConvert.DeserializeObject<List<Ticket>>(fileContent);
            Assert.AreEqual(2, tickets.Count);
            Assert.IsTrue(tickets.Exists(t => t.TicketID == "1"));
            Assert.IsTrue(tickets.Exists(t => t.TicketID == "2"));
        }

        [Test]
        public async Task RunAsync_ShouldUseDefaultFilePath_WhenFilePathIsNotSet()
        {
            Environment.SetEnvironmentVariable("FilePath", null);

            var newTicket = new Ticket { TicketID = "4", Description = "Default path ticket", Status = "Pending", Priority = "Low" };
            var json = JsonConvert.SerializeObject(newTicket);
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            request.Body.Seek(0, SeekOrigin.Begin);

            string defaultPath = Path.Combine(Environment.CurrentDirectory, "tickets.json");
            if (File.Exists(defaultPath))
            {
                File.Delete(defaultPath);
            }

            var result = await PostTicketFunction.RunAsync(request, _mockLogger.Object);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Ticket added successfully: {newTicket.TicketID}", okResult.Value);

            Assert.IsTrue(File.Exists(defaultPath));
            var fileContent = await File.ReadAllTextAsync(defaultPath);
            var tickets = JsonConvert.DeserializeObject<List<Ticket>>(fileContent);
            Assert.IsTrue(tickets.Exists(t => t.TicketID == "4"));

            if (File.Exists(defaultPath))
            {
                File.Delete(defaultPath);
            }
        }

        [Test]
        public async Task RunAsync_ShouldReturnBadRequest_WhenTicketFieldsAreMissing()
        {
            var incompleteTicketJson = "{ \"TicketID\": \"5\" }";
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(incompleteTicketJson));
            request.Body.Seek(0, SeekOrigin.Begin);

            var result = await PostTicketFunction.RunAsync(request, _mockLogger.Object);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid ticket data provided.", badRequestResult.Value);
        }
    }
}

