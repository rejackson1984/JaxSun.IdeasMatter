using JaxSun.Ideas.WebApp.Models;
using FluentAssertions;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Models
{
    public class HubContextTests
    {
        [Fact]
        public void HubContext_CanAccessHub_IdeaDevelopment_ShouldAlways_ReturnTrue()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>()
            };

            // Act
            var canAccess = context.CanAccessHub(BusinessHub.IdeaDevelopment);

            // Assert
            canAccess.Should().BeTrue("Hub 1 (Idea Development) should always be accessible");
        }

        [Fact]
        public void HubContext_CanAccessHub_BusinessPlanning_ShouldReturnFalse_WhenNotUnlocked()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = false
                    }
                }
            };

            // Act
            var canAccess = context.CanAccessHub(BusinessHub.BusinessPlanning);

            // Assert
            canAccess.Should().BeFalse("Hub 2 should not be accessible when not unlocked");
        }

        [Fact]
        public void HubContext_CanAccessHub_BusinessPlanning_ShouldReturnTrue_WhenUnlocked()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = true
                    }
                }
            };

            // Act
            var canAccess = context.CanAccessHub(BusinessHub.BusinessPlanning);

            // Assert
            canAccess.Should().BeTrue("Hub 2 should be accessible when unlocked");
        }

        [Fact]
        public void HubContext_GetOverallCompletion_ShouldReturn_AverageOfUnlockedHubs()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        IsUnlocked = true,
                        CompletionPercentage = 80
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = true,
                        CompletionPercentage = 60
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        IsUnlocked = false,
                        CompletionPercentage = 0
                    }
                }
            };

            // Act
            var overallCompletion = context.GetOverallCompletion();

            // Assert
            overallCompletion.Should().Be(70, "Should average unlocked hubs only: (80 + 60) / 2 = 70");
        }

        [Fact]
        public void HubContext_GetOverallCompletion_ShouldReturnZero_WhenNoUnlockedHubs()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = false,
                        CompletionPercentage = 50
                    }
                }
            };

            // Act
            var overallCompletion = context.GetOverallCompletion();

            // Assert
            overallCompletion.Should().Be(0, "Should return 0 when no hubs are unlocked");
        }

        [Theory]
        [InlineData(70, true)]
        [InlineData(69, false)]
        [InlineData(100, true)]
        public void HubContext_BusinessPlanningUnlock_ShouldDependOn_Hub1Completion(int hub1Completion, bool expectedUnlock)
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = hub1Completion,
                        IsUnlocked = true
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = expectedUnlock
                    }
                }
            };

            // Act & Assert
            if (expectedUnlock)
            {
                context.CanAccessHub(BusinessHub.BusinessPlanning).Should().BeTrue(
                    $"Hub 2 should be unlocked when Hub 1 is {hub1Completion}% complete");
            }
            else
            {
                context.CanAccessHub(BusinessHub.BusinessPlanning).Should().BeFalse(
                    $"Hub 2 should not be unlocked when Hub 1 is only {hub1Completion}% complete");
            }
        }

        [Theory]
        [InlineData(80, true)]
        [InlineData(79, false)]
        [InlineData(100, true)]
        public void HubContext_BusinessOperationsUnlock_ShouldDependOn_Hub2Completion(int hub2Completion, bool expectedUnlock)
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = hub2Completion,
                        IsUnlocked = true
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        IsUnlocked = expectedUnlock
                    }
                }
            };

            // Act & Assert
            if (expectedUnlock)
            {
                context.CanAccessHub(BusinessHub.BusinessOperations).Should().BeTrue(
                    $"Hub 3 should be unlocked when Hub 2 is {hub2Completion}% complete");
            }
            else
            {
                context.CanAccessHub(BusinessHub.BusinessOperations).Should().BeFalse(
                    $"Hub 3 should not be unlocked when Hub 2 is only {hub2Completion}% complete");
            }
        }
    }
}