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

            _mockRepository.Setup(repo => repo.GetAllTicketsAsync())
                .ReturnsAsync(new List<Ticket>
                {
                    new Ticket { TicketID = "1", Description = "Test Ticket", Status = "Open", Priority = "High" }
                });

            _ticketService = new TicketService(_mockRepository.Object);
        }

        [Test]
        public async Task GetAllTickets_ShouldReturnTickets()
        {
            var result = await _ticketService.GetAllTicketsAsync();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual("Test Ticket", result[0].Description);
        }

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
        public async Task GetTicketById_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            _mockRepository.Setup(repo => repo.GetTicketByIdAsync("2"))
                .ReturnsAsync((Ticket)null);

            var result = await _ticketService.GetTicketByIdAsync("2");

            Assert.IsNull(result);
        }
        
       
        
        [Test]
        public void ValidateAndPrepareTicket_ShouldSetStatusToNew_WhenValidTicketIsProvided()
        {
            // Arrange
            var validTicket = new Ticket { TicketID = "5", Description = "A valid description" };

            // Act
            var result = _ticketService.ValidateAndPrepareTicket(validTicket);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New", result.Status);
        }
    
        [Test]
        public async Task GetAllTicketsAsync_ShouldReturnEmptyList_WhenNoTicketsAreAvailable()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllTicketsAsync()).ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _ticketService.GetAllTicketsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }





        

        
        
       


        
        
    }
}

