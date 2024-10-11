using NUnit.Framework;
using System;
using System.Collections.Generic;
using TicketSystem.Models;

namespace TicketSystem.Tests
{
    [TestFixture]
    public class TicketTests
    {
        [Test]
        public void Ticket_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var ticket = new Ticket();

            // Act
            ticket.TicketID = "T123";
            ticket.Description = "Test ticket description";
            ticket.Status = "Open";
            ticket.Priority = "High";
            var createdDate = DateTime.UtcNow;
            var lastUpdatedDate = DateTime.UtcNow;
            ticket.DateCreated = createdDate;
            ticket.LastUpdated = lastUpdatedDate;
            ticket.CreatedByUserId = "U123";
            ticket.AssignedToUserId = "U456";
            ticket.Comments = new List<Comment>();

            // Assert
            Assert.AreEqual("T123", ticket.TicketID);
            Assert.AreEqual("Test ticket description", ticket.Description);
            Assert.AreEqual("Open", ticket.Status);
            Assert.AreEqual("High", ticket.Priority);
            Assert.AreEqual(createdDate, ticket.DateCreated);
            Assert.AreEqual(lastUpdatedDate, ticket.LastUpdated);
            Assert.AreEqual("U123", ticket.CreatedByUserId);
            Assert.AreEqual("U456", ticket.AssignedToUserId);
            Assert.IsNotNull(ticket.Comments);
            Assert.IsEmpty(ticket.Comments);
        }

        [Test]
        public void Ticket_ShouldHandleNullComments()
        {
            // Arrange & Act
            var ticket = new Ticket { Comments = null };

            // Assert
            Assert.IsNull(ticket.Comments);
        }

        [Test]
        public void Ticket_ShouldAddComment()
        {
            // Arrange
            var ticket = new Ticket { Comments = new List<Comment>() };
            var comment = new Comment { CommentID = "C123", CommentText = "Test comment" };

            // Act
            ticket.Comments.Add(comment);

            // Assert
            Assert.AreEqual(1, ticket.Comments.Count);
            Assert.AreEqual("C123", ticket.Comments[0].CommentID);
            Assert.AreEqual("Test comment", ticket.Comments[0].CommentText);
        }
    }
}
