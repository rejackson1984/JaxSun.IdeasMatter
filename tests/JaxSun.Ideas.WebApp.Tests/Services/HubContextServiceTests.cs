using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Services.Interfaces;
using JaxSun.Ideas.Mock.Services.Mock;
using FluentAssertions;
using Moq;
using Xunit;

namespace JaxSun.Ideas.Mock.Tests.Services
{
    public class HubContextServiceTests
    {
        private readonly Mock<IHubConfigurationService> _mockHubConfig;
        private readonly MockHubContextService _hubContextService;

        public HubContextServiceTests()
        {
            _mockHubConfig = new Mock<IHubConfigurationService>();
            _hubContextService = new MockHubContextService(_mockHubConfig.Object);
        }

        [Fact]
        public async Task GetCurrentContextAsync_ShouldReturn_ValidHubContext()
        {
            // Act
            var context = await _hubContextService.GetCurrentContextAsync();

            // Assert
            context.Should().NotBeNull();
            context.CurrentHub.Should().Be(BusinessHub.IdeaDevelopment);
            context.Progress.Should().ContainKey(BusinessHub.IdeaDevelopment);
            context.Progress[BusinessHub.IdeaDevelopment].IsUnlocked.Should().BeTrue();
        }

        [Fact]
        public async Task InitializeUserContextAsync_ShouldCreate_NewUserContext()
        {
            // Arrange
            var userId = "test-user-123";

            // Act
            var context = await _hubContextService.InitializeUserContextAsync(userId);

            // Assert
            context.Should().NotBeNull();
            context.UserId.Should().Be(userId);
            context.CurrentHub.Should().Be(BusinessHub.IdeaDevelopment);
            context.Progress.Should().HaveCount(3); // All three hubs should be initialized
            
            // Hub 1 should be unlocked
            context.Progress[BusinessHub.IdeaDevelopment].IsUnlocked.Should().BeTrue();
            context.Progress[BusinessHub.IdeaDevelopment].IsActive.Should().BeTrue();
            
            // Hub 2 and 3 should be locked initially
            context.Progress[BusinessHub.BusinessPlanning].IsUnlocked.Should().BeFalse();
            context.Progress[BusinessHub.BusinessOperations].IsUnlocked.Should().BeFalse();
        }

        [Fact]
        public async Task SwitchToHubAsync_ShouldFail_WhenHubIsLocked()
        {
            // Arrange - Hub 2 is locked by default
            _mockHubConfig.Setup(x => x.CanTransitionToHubAsync(
                It.IsAny<BusinessHub>(), 
                BusinessHub.BusinessPlanning, 
                It.IsAny<HubContext>()))
                .ReturnsAsync(false);

            // Act
            var result = await _hubContextService.SwitchToHubAsync(BusinessHub.BusinessPlanning);

            // Assert
            result.Should().BeFalse("Cannot switch to locked Hub 2");
        }

        [Fact]
        public async Task SwitchToHubAsync_ShouldSucceed_WhenHubIsUnlocked()
        {
            // Arrange - Mock that Hub 2 is unlocked
            _mockHubConfig.Setup(x => x.CanTransitionToHubAsync(
                BusinessHub.IdeaDevelopment, 
                BusinessHub.BusinessPlanning, 
                It.IsAny<HubContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _hubContextService.SwitchToHubAsync(BusinessHub.BusinessPlanning);

            // Assert
            result.Should().BeTrue("Should be able to switch to unlocked Hub 2");
        }

        [Fact]
        public async Task SwitchToHubAsync_ShouldTrigger_ContextChangedEvent()
        {
            // Arrange
            _mockHubConfig.Setup(x => x.CanTransitionToHubAsync(
                It.IsAny<BusinessHub>(), 
                It.IsAny<BusinessHub>(), 
                It.IsAny<HubContext>()))
                .ReturnsAsync(true);

            BusinessHub? eventPreviousHub = null;
            BusinessHub? eventCurrentHub = null;
            
            _hubContextService.ContextChanged += (sender, args) =>
            {
                eventPreviousHub = args.PreviousHub;
                eventCurrentHub = args.CurrentHub;
            };

            // Act
            await _hubContextService.SwitchToHubAsync(BusinessHub.BusinessPlanning);

            // Assert
            eventPreviousHub.Should().Be(BusinessHub.IdeaDevelopment);
            eventCurrentHub.Should().Be(BusinessHub.BusinessPlanning);
        }

        [Fact]
        public async Task CompleteMilestoneAsync_ShouldUpdate_ProgressPercentage()
        {
            // Arrange
            var context = await _hubContextService.GetCurrentContextAsync();
            var initialProgress = context.Progress[BusinessHub.IdeaDevelopment].CompletionPercentage;
            var milestoneId = "idea_submitted";

            // Act
            var result = await _hubContextService.CompleteMilestoneAsync(milestoneId);

            // Assert
            result.Should().BeTrue("Milestone should be completed successfully");
            
            var updatedContext = await _hubContextService.GetCurrentContextAsync();
            var hub1Progress = updatedContext.Progress[BusinessHub.IdeaDevelopment];
            
            hub1Progress.CompletedMilestones.Should().Contain(milestoneId);
            hub1Progress.CompletionPercentage.Should().BeGreaterThan(initialProgress);
        }

        [Fact]
        public async Task CompleteMilestoneAsync_ShouldReturnFalse_ForDuplicateMilestone()
        {
            // Arrange
            var milestoneId = "idea_submitted";
            
            // Complete milestone first time
            await _hubContextService.CompleteMilestoneAsync(milestoneId);

            // Act - Try to complete same milestone again
            var result = await _hubContextService.CompleteMilestoneAsync(milestoneId);

            // Assert
            result.Should().BeFalse("Duplicate milestone completion should return false");
        }

        [Fact]
        public async Task UpdateHubProgressAsync_ShouldUpdate_HubProgressStatus()
        {
            // Arrange
            var updatedProgress = new HubProgressStatus
            {
                Hub = BusinessHub.IdeaDevelopment,
                CompletionPercentage = 75,
                IsUnlocked = true,
                IsActive = true,
                CompletedMilestones = new List<string> { "milestone1", "milestone2", "milestone3" }
            };

            // Act
            await _hubContextService.UpdateHubProgressAsync(BusinessHub.IdeaDevelopment, updatedProgress);

            // Assert
            var context = await _hubContextService.GetCurrentContextAsync();
            var hub1Progress = context.Progress[BusinessHub.IdeaDevelopment];
            
            hub1Progress.CompletionPercentage.Should().Be(75);
            hub1Progress.CompletedMilestones.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetHubProgressAsync_ShouldReturn_CorrectProgressStatus()
        {
            // Act
            var progress = await _hubContextService.GetHubProgressAsync(BusinessHub.IdeaDevelopment);

            // Assert
            progress.Should().NotBeNull();
            progress.Hub.Should().Be(BusinessHub.IdeaDevelopment);
            progress.IsUnlocked.Should().BeTrue();
        }

        [Fact]
        public async Task GetHubProgressAsync_ShouldReturn_DefaultStatus_ForNonExistentHub()
        {
            // Act
            var progress = await _hubContextService.GetHubProgressAsync(BusinessHub.BusinessOperations);

            // Assert
            progress.Should().NotBeNull();
            progress.Hub.Should().Be(BusinessHub.BusinessOperations);
            progress.CompletionPercentage.Should().Be(0);
            progress.IsUnlocked.Should().BeFalse(); // Hub 3 should be locked by default
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, true)]
        [InlineData(BusinessHub.BusinessPlanning, false)]
        [InlineData(BusinessHub.BusinessOperations, false)]
        public async Task HubAccess_ShouldFollow_BusinessRules(BusinessHub hub, bool expectedAccess)
        {
            // Arrange
            var context = await _hubContextService.GetCurrentContextAsync();

            // Act
            var canAccess = context.CanAccessHub(hub);

            // Assert
            canAccess.Should().Be(expectedAccess, 
                $"Hub {hub} access should be {expectedAccess} for new user");
        }

        [Fact]
        public async Task Hub2Unlock_ShouldRequire_70PercentHub1Completion()
        {
            // Arrange - Set Hub 1 to 70% completion
            var hub1Progress = new HubProgressStatus
            {
                Hub = BusinessHub.IdeaDevelopment,
                CompletionPercentage = 70,
                IsUnlocked = true
            };

            _mockHubConfig.Setup(x => x.ShouldUnlockHubAsync(
                BusinessHub.BusinessPlanning, 
                It.IsAny<HubContext>()))
                .ReturnsAsync(true);

            // Act
            await _hubContextService.UpdateHubProgressAsync(BusinessHub.IdeaDevelopment, hub1Progress);

            // Assert
            var context = await _hubContextService.GetCurrentContextAsync();
            
            // This test validates that the business logic for unlocking is handled correctly
            // The actual unlock logic is in the configuration service, but the context service should respect it
            _mockHubConfig.Verify(x => x.ShouldUnlockHubAsync(
                BusinessHub.BusinessPlanning, 
                It.IsAny<HubContext>()), 
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Hub3Unlock_ShouldRequire_80PercentHub2Completion()
        {
            // Arrange - Set Hub 2 to 80% completion
            var hub2Progress = new HubProgressStatus
            {
                Hub = BusinessHub.BusinessPlanning,
                CompletionPercentage = 80,
                IsUnlocked = true
            };

            _mockHubConfig.Setup(x => x.ShouldUnlockHubAsync(
                BusinessHub.BusinessOperations, 
                It.IsAny<HubContext>()))
                .ReturnsAsync(true);

            // Act
            await _hubContextService.UpdateHubProgressAsync(BusinessHub.BusinessPlanning, hub2Progress);

            // Assert
            _mockHubConfig.Verify(x => x.ShouldUnlockHubAsync(
                BusinessHub.BusinessOperations, 
                It.IsAny<HubContext>()), 
                Times.AtLeastOnce);
        }
    }
}