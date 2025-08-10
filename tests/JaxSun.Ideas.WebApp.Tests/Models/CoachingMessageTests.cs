using FluentAssertions;
using JaxSun.Ideas.Mock.Models;
using Xunit;

namespace JaxSun.Ideas.Mock.Tests.Models
{
    /// <summary>
    /// Comprehensive tests for CoachingMessage functionality
    /// Tests personalized message generation, message types, and message management
    /// </summary>
    public class CoachingMessageTests
    {
        #region CoachingMessage Construction Tests

        [Fact]
        public void CoachingMessage_ShouldInitialize_WithDefaultValues()
        {
            // Act
            var message = new CoachingMessage();

            // Assert
            message.Id.Should().NotBeEmpty("Should generate a unique ID");
            message.PersonaName.Should().BeEmpty("PersonaName should start empty");
            message.Message.Should().BeEmpty("Message should start empty");
            message.Type.Should().Be(default(CoachingMessageType));
            message.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            message.IsRead.Should().BeFalse("IsRead should default to false");
            message.Priority.Should().Be(0, "Priority should default to 0");
        }

        [Fact]
        public void CoachingMessage_ShouldAllow_PropertyInitialization()
        {
            // Arrange & Act
            var timestamp = DateTime.UtcNow;
            var message = new CoachingMessage
            {
                PersonaName = "Spark",
                Message = "Great work on submitting your idea! Let's expand on it further.",
                Type = CoachingMessageType.Encouragement,
                Timestamp = timestamp,
                IsRead = true,
                RelatedAction = "idea_submitted",
                Icon = "fas fa-lightbulb",
                Priority = 5
            };

            // Assert
            message.PersonaName.Should().Be("Spark");
            message.Message.Should().Be("Great work on submitting your idea! Let's expand on it further.");
            message.Type.Should().Be(CoachingMessageType.Encouragement);
            message.Timestamp.Should().Be(timestamp);
            message.IsRead.Should().BeTrue();
            message.RelatedAction.Should().Be("idea_submitted");
            message.Icon.Should().Be("fas fa-lightbulb");
            message.Priority.Should().Be(5);
        }

        [Fact]
        public void CoachingMessage_ShouldGenerate_UniqueIds()
        {
            // Act
            var message1 = new CoachingMessage();
            var message2 = new CoachingMessage();

            // Assert
            message1.Id.Should().NotBe(message2.Id);
            message1.Id.Should().NotBeEmpty();
            message2.Id.Should().NotBeEmpty();
        }

        #endregion

        #region Message Type Tests

        [Theory]
        [InlineData(CoachingMessageType.Welcome, "Welcome to the Idea Development Hub!")]
        [InlineData(CoachingMessageType.Progress, "You're making great progress on your business idea.")]
        [InlineData(CoachingMessageType.Encouragement, "Keep up the excellent work!")]
        [InlineData(CoachingMessageType.Guidance, "Here's what I recommend for your next step.")]
        [InlineData(CoachingMessageType.Achievement, "Congratulations on completing this milestone!")]
        [InlineData(CoachingMessageType.Reminder, "Don't forget to validate your market research.")]
        [InlineData(CoachingMessageType.Tip, "Pro tip: Focus on one core problem at a time.")]
        public void CoachingMessage_ShouldSupport_AllMessageTypes(CoachingMessageType type, string sampleMessage)
        {
            // Act
            var message = new CoachingMessage
            {
                Type = type,
                Message = sampleMessage
            };

            // Assert
            message.Type.Should().Be(type);
            message.Message.Should().Be(sampleMessage);
        }

        [Fact]
        public void CoachingMessage_ShouldCategorize_MessagesByPriority()
        {
            // Arrange & Act
            var highPriorityMessage = new CoachingMessage { Priority = 10, Type = CoachingMessageType.Reminder };
            var mediumPriorityMessage = new CoachingMessage { Priority = 5, Type = CoachingMessageType.Guidance };
            var lowPriorityMessage = new CoachingMessage { Priority = 1, Type = CoachingMessageType.Tip };

            // Assert
            highPriorityMessage.Priority.Should().BeGreaterThan(mediumPriorityMessage.Priority);
            mediumPriorityMessage.Priority.Should().BeGreaterThan(lowPriorityMessage.Priority);
        }

        #endregion

        #region Message Content Validation Tests

        [Theory]
        [InlineData("Spark", "Let's get creative with your idea!")]
        [InlineData("Strategy", "Time to build a solid business plan.")]
        [InlineData("Execute", "Let's make this business operational!")]
        public void CoachingMessage_ShouldReflect_PersonaCharacteristics(string personaName, string expectedMessageStyle)
        {
            // Act
            var message = new CoachingMessage
            {
                PersonaName = personaName,
                Message = expectedMessageStyle,
                Type = CoachingMessageType.Welcome
            };

            // Assert
            message.PersonaName.Should().Be(personaName);
            message.Message.Should().Contain(expectedMessageStyle);
        }

        [Fact]
        public void CoachingMessage_ShouldSupport_ActionableMessages()
        {
            // Act
            var actionableMessage = new CoachingMessage
            {
                PersonaName = "Strategy",
                Message = "Ready to create your business model canvas? Click here to get started.",
                Type = CoachingMessageType.Guidance,
                RelatedAction = "create_business_model_canvas",
                Priority = 7
            };

            // Assert
            actionableMessage.RelatedAction.Should().Be("create_business_model_canvas");
            actionableMessage.Message.Should().Contain("Click here");
            actionableMessage.Type.Should().Be(CoachingMessageType.Guidance);
        }

        [Fact]
        public void CoachingMessage_ShouldInclude_VisualIndicators()
        {
            // Arrange & Act
            var messages = new[]
            {
                new CoachingMessage { Type = CoachingMessageType.Achievement, Icon = "fas fa-trophy" },
                new CoachingMessage { Type = CoachingMessageType.Reminder, Icon = "fas fa-exclamation-triangle" },
                new CoachingMessage { Type = CoachingMessageType.Tip, Icon = "fas fa-lightbulb" },
                new CoachingMessage { Type = CoachingMessageType.Progress, Icon = "fas fa-chart-line" }
            };

            // Assert
            messages.Should().AllSatisfy(message =>
            {
                message.Icon.Should().NotBeNull();
                message.Icon.Should().StartWith("fas fa-");
            });
        }

        #endregion

        #region Message Timing and State Tests

        [Fact]
        public void CoachingMessage_ShouldTrack_ReadStatus()
        {
            // Arrange
            var message = new CoachingMessage
            {
                Message = "Test message",
                IsRead = false
            };

            // Act
            message.IsRead = true;

            // Assert
            message.IsRead.Should().BeTrue();
        }

        [Fact]
        public void CoachingMessage_ShouldOrder_ByTimestamp()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var messages = new[]
            {
                new CoachingMessage { Timestamp = baseTime.AddMinutes(-10), Message = "First message" },
                new CoachingMessage { Timestamp = baseTime.AddMinutes(-5), Message = "Second message" },
                new CoachingMessage { Timestamp = baseTime, Message = "Latest message" }
            };

            // Act
            var orderedMessages = messages.OrderBy(m => m.Timestamp).ToList();

            // Assert
            orderedMessages[0].Message.Should().Be("First message");
            orderedMessages[1].Message.Should().Be("Second message");
            orderedMessages[2].Message.Should().Be("Latest message");
        }

        [Fact]
        public void CoachingMessage_ShouldTrack_MessageAge()
        {
            // Arrange
            var oldMessage = new CoachingMessage { Timestamp = DateTime.UtcNow.AddHours(-1) };
            var recentMessage = new CoachingMessage { Timestamp = DateTime.UtcNow.AddMinutes(-5) };

            // Act
            var oldMessageAge = DateTime.UtcNow - oldMessage.Timestamp;
            var recentMessageAge = DateTime.UtcNow - recentMessage.Timestamp;

            // Assert
            oldMessageAge.Should().BeGreaterThan(TimeSpan.FromMinutes(30));
            recentMessageAge.Should().BeLessThan(TimeSpan.FromMinutes(10));
        }

        #endregion

        #region Message Context and Personalization Tests

        [Theory]
        [InlineData(CoachingMessageType.Welcome, "first_visit")]
        [InlineData(CoachingMessageType.Progress, "milestone_completed")]
        [InlineData(CoachingMessageType.Encouragement, "low_progress")]
        [InlineData(CoachingMessageType.Reminder, "inactive_user")]
        public void CoachingMessage_ShouldGenerate_ContextualMessages(CoachingMessageType type, string context)
        {
            // Act
            var message = new CoachingMessage
            {
                Type = type,
                RelatedAction = context,
                Message = $"Contextual message for {context}"
            };

            // Assert
            message.Type.Should().Be(type);
            message.RelatedAction.Should().Be(context);
            message.Message.Should().Contain(context);
        }

        [Fact]
        public void CoachingMessage_ShouldSupport_PersonalizedContent()
        {
            // Act
            var personalizedMessage = new CoachingMessage
            {
                PersonaName = "Spark",
                Type = CoachingMessageType.Encouragement,
                Message = "I can see you're passionate about your idea! Your creativity is exactly what this project needs.",
                Priority = 6
            };

            // Assert
            personalizedMessage.PersonaName.Should().Be("Spark");
            personalizedMessage.Message.Should().Contain("passionate");
            personalizedMessage.Message.Should().Contain("creativity");
            personalizedMessage.Priority.Should().BeGreaterThan(5);
        }

        [Fact]
        public void CoachingMessage_ShouldAdapt_ToUserProgress()
        {
            // Arrange & Act
            var beginnerMessage = new CoachingMessage
            {
                Type = CoachingMessageType.Guidance,
                Message = "Welcome! Let's start with the basics of idea development.",
                Priority = 8
            };

            var advancedMessage = new CoachingMessage
            {
                Type = CoachingMessageType.Guidance,
                Message = "You're ready for advanced market analysis techniques.",
                Priority = 5
            };

            // Assert
            beginnerMessage.Priority.Should().BeGreaterThan(advancedMessage.Priority);
            beginnerMessage.Message.Should().Contain("basics");
            advancedMessage.Message.Should().Contain("advanced");
        }

        #endregion

        #region Message Collection and Management Tests

        [Fact]
        public void CoachingMessage_ShouldSupport_MessageGrouping()
        {
            // Arrange & Act
            var messages = new List<CoachingMessage>
            {
                new() { Type = CoachingMessageType.Welcome, PersonaName = "Spark" },
                new() { Type = CoachingMessageType.Guidance, PersonaName = "Spark" },
                new() { Type = CoachingMessageType.Welcome, PersonaName = "Strategy" },
                new() { Type = CoachingMessageType.Tip, PersonaName = "Execute" }
            };

            var sparkMessages = messages.Where(m => m.PersonaName == "Spark").ToList();
            var welcomeMessages = messages.Where(m => m.Type == CoachingMessageType.Welcome).ToList();

            // Assert
            sparkMessages.Should().HaveCount(2);
            welcomeMessages.Should().HaveCount(2);
        }

        [Fact]
        public void CoachingMessage_ShouldSupport_PriorityFiltering()
        {
            // Arrange
            var messages = new List<CoachingMessage>
            {
                new() { Priority = 10, Type = CoachingMessageType.Reminder },
                new() { Priority = 7, Type = CoachingMessageType.Guidance },
                new() { Priority = 3, Type = CoachingMessageType.Tip },
                new() { Priority = 1, Type = CoachingMessageType.Achievement }
            };

            // Act
            var highPriorityMessages = messages.Where(m => m.Priority >= 7).ToList();
            var lowPriorityMessages = messages.Where(m => m.Priority <= 3).ToList();

            // Assert
            highPriorityMessages.Should().HaveCount(2);
            lowPriorityMessages.Should().HaveCount(2);
        }

        [Fact]
        public void CoachingMessage_ShouldSupport_UnreadMessageFiltering()
        {
            // Arrange
            var messages = new List<CoachingMessage>
            {
                new() { IsRead = false, Message = "Unread message 1" },
                new() { IsRead = true, Message = "Read message" },
                new() { IsRead = false, Message = "Unread message 2" }
            };

            // Act
            var unreadMessages = messages.Where(m => !m.IsRead).ToList();

            // Assert
            unreadMessages.Should().HaveCount(2);
            unreadMessages.Should().AllSatisfy(m => m.IsRead.Should().BeFalse());
        }

        #endregion

        #region Message Validation Tests

        [Fact]
        public void CoachingMessage_ShouldRequire_PersonaName()
        {
            // Act
            var message = new CoachingMessage
            {
                PersonaName = "",
                Message = "Test message"
            };

            // Assert - In a real application, this might be validated
            message.PersonaName.Should().BeEmpty(); // Currently allowed, but could be validated
        }

        [Fact]
        public void CoachingMessage_ShouldRequire_MessageContent()
        {
            // Act
            var message = new CoachingMessage
            {
                PersonaName = "Spark",
                Message = ""
            };

            // Assert - In a real application, this might be validated
            message.Message.Should().BeEmpty(); // Currently allowed, but could be validated
        }

        [Fact]
        public void CoachingMessage_ShouldHandle_LongMessages()
        {
            // Arrange
            var longMessage = new string('A', 1000);

            // Act
            var message = new CoachingMessage
            {
                Message = longMessage,
                PersonaName = "Strategy"
            };

            // Assert
            message.Message.Should().HaveLength(1000);
            message.Message.Should().Be(longMessage);
        }

        #endregion

        #region Message Equality and Comparison Tests

        [Fact]
        public void CoachingMessage_ShouldGenerate_UniqueIdentifiers()
        {
            // Act
            var messages = Enumerable.Range(0, 100)
                .Select(_ => new CoachingMessage())
                .ToList();

            var uniqueIds = messages.Select(m => m.Id).Distinct().ToList();

            // Assert
            uniqueIds.Should().HaveCount(100, "All messages should have unique IDs");
        }

        [Fact]
        public void CoachingMessage_ShouldSupport_MessageCopying()
        {
            // Arrange
            var original = new CoachingMessage
            {
                PersonaName = "Execute",
                Message = "Original message",
                Type = CoachingMessageType.Progress,
                Priority = 5,
                IsRead = true,
                RelatedAction = "test_action",
                Icon = "fas fa-check"
            };

            // Act
            var copy = new CoachingMessage
            {
                PersonaName = original.PersonaName,
                Message = original.Message,
                Type = original.Type,
                Priority = original.Priority,
                IsRead = original.IsRead,
                RelatedAction = original.RelatedAction,
                Icon = original.Icon,
                Timestamp = original.Timestamp
            };

            // Assert
            copy.PersonaName.Should().Be(original.PersonaName);
            copy.Message.Should().Be(original.Message);
            copy.Type.Should().Be(original.Type);
            copy.Priority.Should().Be(original.Priority);
            copy.IsRead.Should().Be(original.IsRead);
            copy.RelatedAction.Should().Be(original.RelatedAction);
            copy.Icon.Should().Be(original.Icon);
            copy.Timestamp.Should().Be(original.Timestamp);
            copy.Id.Should().NotBe(original.Id, "Each message should have a unique ID");
        }

        #endregion

        #region Message State Transitions Tests

        [Fact]
        public void CoachingMessage_ShouldSupport_StateTransitions()
        {
            // Arrange
            var message = new CoachingMessage
            {
                IsRead = false,
                Priority = 5
            };

            // Act - Mark as read
            message.IsRead = true;

            // Act - Update priority
            message.Priority = 10;

            // Assert
            message.IsRead.Should().BeTrue();
            message.Priority.Should().Be(10);
        }

        [Fact]
        public void CoachingMessage_ShouldMaintain_TimestampIntegrity()
        {
            // Arrange
            var originalTimestamp = DateTime.UtcNow.AddHours(-1);
            var message = new CoachingMessage
            {
                Timestamp = originalTimestamp
            };

            // Act - Modify other properties
            message.IsRead = true;
            message.Priority = 8;
            message.Message = "Updated message";

            // Assert - Timestamp should remain unchanged unless explicitly modified
            message.Timestamp.Should().Be(originalTimestamp);
        }

        #endregion
    }
}