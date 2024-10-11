using NUnit.Framework;
using TicketSystem.Models;

namespace TicketSystem.Tests
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void User_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var user = new User();

            // Act
            user.UserID = "U123";
            user.Name = "Test User";
            user.Email = "test@example.com";
            user.Role = "Admin";

            // Assert
            Assert.AreEqual("U123", user.UserID);
            Assert.AreEqual("Test User", user.Name);
            Assert.AreEqual("test@example.com", user.Email);
            Assert.AreEqual("Admin", user.Role);
        }

        [Test]
        public void User_ShouldHandleNullNameAndEmail()
        {
            // Arrange & Act
            var user = new User { Name = null, Email = null };

            // Assert
            Assert.IsNull(user.Name);
            Assert.IsNull(user.Email);
        }
    }
}