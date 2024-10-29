using NUnit.Framework;
using Moq;
using TicketSystem.Interfaces;
using TicketSystem.Services;
using TicketSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketSystem.Tests
{
    [TestFixture]
    public class TicketServiceTests
    {
        private Mock<ITicketRepository> _mockRepository;
        private ITicketService _ticketService;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ITicketRepository>();

            // Example setup for initial GetAllTicketsAsync
            _mockRepository.Setup(repo => repo.GetAllTicketsAsync())
                .ReturnsAsync(new List<Ticket>
                {
                    new Ticket { TicketID = "1", Description = "Test Ticket", Status = "Open", Priority = "High" }
                });

            _ticketService = new TicketService(_mockRepository.Object);
        }

        // Test for AddTicketAsync
        [Test]
        public async Task AddTicketAsync_ShouldAddTicket_WhenTicketIsValid()
        {
            // Arrange
            var newTicket = new Ticket { TicketID = "3", Description = "New Ticket", Priority = "High" };

            // Act
            await _ticketService.AddTicketAsync(newTicket);

            // Assert
            _mockRepository.Verify(repo => repo.AddTicketAsync(It.Is<Ticket>(t => t.TicketID == "3" && t.Status == "New")), Times.Once);
        }

        // Test for UpdateTicketAsync
        [Test]
        public async Task UpdateTicketAsync_ShouldUpdateTicket_WhenTicketIsValid()
        {
            // Arrange
            var existingTicket = new Ticket { TicketID = "4", Description = "Existing Ticket", Priority = "Medium", Status = "Open" };

            // Act
            await _ticketService.UpdateTicketAsync(existingTicket);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateTicketAsync(It.Is<Ticket>(t => t.TicketID == "4" && t.Status == "New")), Times.Once);
        }

        // Test for DeleteTicketAsync returning true
        [Test]
        public async Task DeleteTicketAsync_ShouldReturnTrue_WhenTicketExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteTicketAsync("1")).ReturnsAsync(true);

            // Act
            var result = await _ticketService.DeleteTicketAsync("1");

            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(repo => repo.DeleteTicketAsync("1"), Times.Once);
        }

        // Test for DeleteTicketAsync returning false
        [Test]
        public async Task DeleteTicketAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteTicketAsync("2")).ReturnsAsync(false);

            // Act
            var result = await _ticketService.DeleteTicketAsync("2");

            // Assert
            Assert.IsFalse(result);
            _mockRepository.Verify(repo => repo.DeleteTicketAsync("2"), Times.Once);
        }

        // Additional example tests for completeness
        [Test]
        public async Task GetTicketById_ShouldReturnTicket_WhenTicketExists()
        {
            _mockRepository.Setup(repo => repo.GetTicketByIdAsync("1"))
                .ReturnsAsync(new Ticket { TicketID = "1", Description = "Test Ticket" });

            var result = await _ticketService.GetTicketByIdAsync("1");

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Ticket", result.Description);
        }

        [Test]
        public void ValidateAndPrepareTicket_ShouldSetStatusToNew_WhenValidTicketIsProvided()
        {
            var validTicket = new Ticket { TicketID = "5", Description = "A valid description" };

            var result = _ticketService.ValidateAndPrepareTicket(validTicket);

            Assert.IsNotNull(result);
            Assert.AreEqual("New", result.Status);
        }
    }
}

        





        

        
        
       


        
        
    
