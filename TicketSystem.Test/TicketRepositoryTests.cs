using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Repository;

namespace TicketSystem.Test
{
    [TestFixture]
    public class TicketRepositoryTests
    {
        private TicketRepository _repository;
        private string _testFilePath;

        [SetUp]
        public void SetUp()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), "test_tickets.json");
            _repository = new TicketRepository(_testFilePath);

            var tickets = new List<Ticket>
            {
                new Ticket { TicketID = "1", Description = "Test Ticket 1", Status = "Open", Priority = "High" },
                new Ticket { TicketID = "2", Description = "Test Ticket 2", Status = "Closed", Priority = "Low" }
            };

            File.WriteAllText(_testFilePath, System.Text.Json.JsonSerializer.Serialize(tickets));
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
        public async Task GetAllTicketsAsync_ShouldReturnAllTickets()
        {
            var result = await _repository.GetAllTicketsAsync();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetTicketByIdAsync_ShouldReturnCorrectTicket()
        {
            var result = await _repository.GetTicketByIdAsync("1");

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Ticket 1", result.Description);
        }

        [Test]
        public async Task GetTicketByIdAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            var result = await _repository.GetTicketByIdAsync("3");

            Assert.IsNull(result);
        }

        [Test]
        public async Task AddTicketAsync_ShouldAddTicketAndSaveToFile()
        {
            var newTicket = new Ticket { TicketID = "3", Description = "New Ticket", Status = "New", Priority = "Medium" };

            await _repository.AddTicketAsync(newTicket);

            var tickets = await _repository.GetAllTicketsAsync();
            Assert.AreEqual(3, tickets.Count);
            Assert.IsTrue(tickets.Exists(t => t.TicketID == "3"));
        }

        [Test]
        public async Task UpdateTicketAsync_ShouldUpdateTicketAndSaveToFile()
        {
            var updatedTicket = new Ticket { TicketID = "1", Description = "Updated Ticket", Status = "Closed", Priority = "High" };

            await _repository.UpdateTicketAsync(updatedTicket);

            var tickets = await _repository.GetAllTicketsAsync();
            var ticket = tickets.Find(t => t.TicketID == "1");
            Assert.IsNotNull(ticket);
            Assert.AreEqual("Updated Ticket", ticket.Description);
            Assert.AreEqual("Closed", ticket.Status);
        }

        [Test]
        public async Task DeleteTicketAsync_ShouldRemoveTicket()
        {
            var result = await _repository.DeleteTicketAsync("1");

            Assert.IsTrue(result);

            var tickets = await _repository.GetAllTicketsAsync();
            Assert.AreEqual(1, tickets.Count);
            Assert.IsFalse(tickets.Exists(t => t.TicketID == "1"));
        }

        [Test]
        public async Task DeleteTicketAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            var result = await _repository.DeleteTicketAsync("999");

            Assert.IsFalse(result);
        }
        
        [Test]
        public void UpdateTicketAsync_ShouldThrowException_WhenTicketDoesNotExist()
        {
            var updatedTicket = new Ticket { TicketID = "999", Description = "Non-existent Ticket", Status = "Open", Priority = "High" };

            var exception = Assert.ThrowsAsync<System.Exception>(async () => await _repository.UpdateTicketAsync(updatedTicket));

            Assert.AreEqual("Ticket with ID: 999 not found.", exception.Message);
        }

    }
}

