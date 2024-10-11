using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Interfaces;
using TicketSystem.Services;

namespace TicketSystem.Test
{
    [TestFixture]
    public class ITicketServiceTests
    {
        private TicketService _service;
        private Mock<ITicketRepository> _mockTicketRepository;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of ITicketRepository.
            _mockTicketRepository = new Mock<ITicketRepository>();

            // Initialize the service with the mock repository.
            _service = new TicketService(_mockTicketRepository.Object);
        }

        [Test]
        public void ValidateAndPrepareTicket_ShouldReturnValidTicket_WhenInputIsCorrect()
        {
            // Arrange
            var validTicket = new Ticket 
            { 
                TicketID = "123",
                Description = "Valid description"
            };

            // Act
            var result = _service.ValidateAndPrepareTicket(validTicket);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.TicketID);
            Assert.AreEqual("Valid description", result.Description);
            Assert.AreEqual("New", result.Status); // Check if the status is set to "New".
        }

        [Test]
        public void ValidateAndPrepareTicket_ShouldThrowArgumentException_WhenDescriptionIsMissing()
        {
            // Arrange
            var invalidTicket = new Ticket 
            { 
                TicketID = "123",
                Description = "" // Missing description.
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _service.ValidateAndPrepareTicket(invalidTicket));
            Assert.AreEqual("Ticket description cannot be empty", ex.Message);
        }

        [Test]
        public void ValidateAndPrepareTicket_ShouldSetStatusToNew_WhenTicketIsValid()
        {
            // Arrange
            var ticket = new Ticket 
            { 
                TicketID = "124",
                Description = "Valid description"
            };

            // Act
            var result = _service.ValidateAndPrepareTicket(ticket);

            // Assert
            Assert.AreEqual("New", result.Status);
        }

        [Test]
        public void ValidateAndPrepareTicket_ShouldReturnTicket_WhenDescriptionHasSpecialCharacters()
        {
            // Arrange
            var specialCharTicket = new Ticket 
            { 
                TicketID = "125",
                Description = "This is a description with special characters: !@#$%^&*()"
            };

            // Act
            var result = _service.ValidateAndPrepareTicket(specialCharTicket);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("125", result.TicketID);
            Assert.AreEqual("This is a description with special characters: !@#$%^&*()", result.Description);
            Assert.AreEqual("New", result.Status);
        }
    }
}
