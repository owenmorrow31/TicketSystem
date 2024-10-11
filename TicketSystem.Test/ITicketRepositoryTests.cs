using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Interfaces;

namespace TicketSystem.Test
{
    [TestFixture]
    public class ITicketRepositoryTests
    {
        private Mock<ITicketRepository> _mockRepository;
        private List<Ticket> _testTickets;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of the ITicketRepository.
            _mockRepository = new Mock<ITicketRepository>();

            // Initialize a list of test tickets.
            _testTickets = new List<Ticket>
            {
                new Ticket { TicketID = "1", Description = "Test Ticket 1", Status = "Open", Priority = "High" },
                new Ticket { TicketID = "2", Description = "Test Ticket 2", Status = "Closed", Priority = "Low" },
            };
        }

        [Test]
        public async Task GetAllTicketsAsync_ShouldReturnAllTickets()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllTicketsAsync()).ReturnsAsync(_testTickets);

            // Act
            var result = await _mockRepository.Object.GetAllTicketsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1", result[0].TicketID);
            Assert.AreEqual("Test Ticket 1", result[0].Description);
        }

        [Test]
        public async Task GetTicketByIdAsync_ShouldReturnCorrectTicket_WhenIdExists()
        {
            // Arrange
            var ticketId = "1";
            _mockRepository.Setup(repo => repo.GetTicketByIdAsync(ticketId)).ReturnsAsync(_testTickets[0]);

            // Act
            var result = await _mockRepository.Object.GetTicketByIdAsync(ticketId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ticketId, result.TicketID);
            Assert.AreEqual("Test Ticket 1", result.Description);
        }

        [Test]
        public async Task GetTicketByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var ticketId = "999"; // Non-existent ID.
            _mockRepository.Setup(repo => repo.GetTicketByIdAsync(ticketId)).ReturnsAsync((Ticket)null);

            // Act
            var result = await _mockRepository.Object.GetTicketByIdAsync(ticketId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddTicketAsync_ShouldInvokeRepositoryMethod()
        {
            // Arrange
            var newTicket = new Ticket { TicketID = "3", Description = "New Ticket", Status = "New", Priority = "Medium" };

            // Act
            await _mockRepository.Object.AddTicketAsync(newTicket);

            // Assert
            _mockRepository.Verify(repo => repo.AddTicketAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Test]
        public async Task UpdateTicketAsync_ShouldInvokeRepositoryMethod()
        {
            // Arrange
            var updatedTicket = new Ticket { TicketID = "1", Description = "Updated Ticket 1", Status = "In Progress", Priority = "High" };

            // Act
            await _mockRepository.Object.UpdateTicketAsync(updatedTicket);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateTicketAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Test]
        public async Task DeleteTicketAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            var ticketId = "1";
            _mockRepository.Setup(repo => repo.DeleteTicketAsync(ticketId)).ReturnsAsync(true);

            // Act
            var result = await _mockRepository.Object.DeleteTicketAsync(ticketId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteTicketAsync_ShouldReturnFalse_WhenDeletionFails()
        {
            // Arrange
            var ticketId = "999"; // Non-existent ID.
            _mockRepository.Setup(repo => repo.DeleteTicketAsync(ticketId)).ReturnsAsync(false);

            // Act
            var result = await _mockRepository.Object.DeleteTicketAsync(ticketId);

            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public async Task GetAllTicketsAsync_ShouldReturnEmptyList_WhenNoTicketsExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllTicketsAsync()).ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _mockRepository.Object.GetAllTicketsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public async Task DeleteTicketAsync_ShouldReturnFalse_WhenTicketIdIsNullOrEmpty()
        {
            // Arrange
            string nullId = null;
            string emptyId = "";
            _mockRepository.Setup(repo => repo.DeleteTicketAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var resultWithNull = await _mockRepository.Object.DeleteTicketAsync(nullId);
            var resultWithEmpty = await _mockRepository.Object.DeleteTicketAsync(emptyId);

            // Assert
            Assert.IsFalse(resultWithNull);
            Assert.IsFalse(resultWithEmpty);
        }
        
       


        

      

        

      
        


    }
}
