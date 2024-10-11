using NUnit.Framework;
using System;
using TicketSystem.Models;

namespace TicketSystem.Tests
{
    [TestFixture]
    public class CommentTests
    {
        [Test]
        public void Comment_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var comment = new Comment();

            // Act
            comment.CommentID = "C123";
            comment.TicketID = "T123";
            comment.UserID = "U123";
            comment.CommentText = "This is a comment.";
            var now = DateTime.UtcNow;
            comment.DateCreated = now;

            // Assert
            Assert.AreEqual("C123", comment.CommentID);
            Assert.AreEqual("T123", comment.TicketID);
            Assert.AreEqual("U123", comment.UserID);
            Assert.AreEqual("This is a comment.", comment.CommentText);
            Assert.AreEqual(now, comment.DateCreated);
        }

        [Test]
        public void Comment_ShouldHandleNullCommentText()
        {
            // Arrange & Act
            var comment = new Comment { CommentText = null };

            // Assert
            Assert.IsNull(comment.CommentText);
        }
    }
}