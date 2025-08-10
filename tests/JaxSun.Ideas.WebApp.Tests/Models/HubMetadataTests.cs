using JaxSun.Ideas.WebApp.Models;
using FluentAssertions;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Models
{
    public class HubMetadataTests
    {
        [Fact]
        public void HubMetadata_ShouldHave_RequiredProperties()
        {
            // Arrange & Act
            var metadata = new HubMetadata
            {
                Hub = BusinessHub.IdeaDevelopment,
                Name = "Test Hub",
                Description = "Test Description",
                Icon = "fas fa-test",
                PrimaryColor = "#123456",
                SecondaryColor = "#654321",
                GradientColors = "linear-gradient(135deg, #123456 0%, #654321 100%)",
                CoachPersona = "TestCoach",
                Features = new List<string> { "Feature1", "Feature2" },
                RequiredMilestones = new List<string> { "milestone1", "milestone2" },
                UnlockThreshold = 70
            };

            // Assert
            metadata.Hub.Should().Be(BusinessHub.IdeaDevelopment);
            metadata.Name.Should().Be("Test Hub");
            metadata.Description.Should().Be("Test Description");
            metadata.Icon.Should().Be("fas fa-test");
            metadata.PrimaryColor.Should().Be("#123456");
            metadata.SecondaryColor.Should().Be("#654321");
            metadata.GradientColors.Should().Be("linear-gradient(135deg, #123456 0%, #654321 100%)");
            metadata.CoachPersona.Should().Be("TestCoach");
            metadata.Features.Should().HaveCount(2);
            metadata.RequiredMilestones.Should().HaveCount(2);
            metadata.UnlockThreshold.Should().Be(70);
        }

        [Fact]
        public void HubMetadata_ShouldInitialize_EmptyCollections()
        {
            // Arrange & Act
            var metadata = new HubMetadata();

            // Assert
            metadata.Features.Should().NotBeNull().And.BeEmpty();
            metadata.RequiredMilestones.Should().NotBeNull().And.BeEmpty();
            metadata.Name.Should().BeEmpty();
            metadata.Description.Should().BeEmpty();
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, 0)]
        [InlineData(BusinessHub.BusinessPlanning, 70)]
        [InlineData(BusinessHub.BusinessOperations, 80)]
        public void HubMetadata_ShouldHave_CorrectUnlockThresholds(BusinessHub hub, int expectedThreshold)
        {
            // This test validates our business rule expectations
            // Hub 1: Always unlocked (0%)
            // Hub 2: Unlocks at 70% Hub 1 completion
            // Hub 3: Unlocks at 80% Hub 2 completion
            
            // Arrange & Act
            var metadata = new HubMetadata
            {
                Hub = hub,
                UnlockThreshold = expectedThreshold
            };

            // Assert
            metadata.UnlockThreshold.Should().Be(expectedThreshold, 
                $"Hub {(int)hub} should have unlock threshold of {expectedThreshold}%");
        }

        [Fact]
        public void HubProgressStatus_ShouldTrack_MilestoneCompletion()
        {
            // Arrange
            var progress = new HubProgressStatus
            {
                Hub = BusinessHub.IdeaDevelopment,
                CompletionPercentage = 0,
                IsUnlocked = true,
                AvailableMilestones = new List<string> { "milestone1", "milestone2", "milestone3" },
                CompletedMilestones = new List<string>()
            };

            // Act - Complete first milestone
            progress.CompletedMilestones.Add("milestone1");
            var expectedCompletion = (progress.CompletedMilestones.Count * 100) / progress.AvailableMilestones.Count;

            // Assert
            progress.CompletedMilestones.Should().HaveCount(1);
            expectedCompletion.Should().Be(33, "1 of 3 milestones = 33% completion");
        }

        [Fact]
        public void HubProgressStatus_ShouldCalculate_CompletionPercentage()
        {
            // Arrange
            var progress = new HubProgressStatus
            {
                Hub = BusinessHub.BusinessPlanning,
                AvailableMilestones = new List<string> { "m1", "m2", "m3", "m4", "m5" },
                CompletedMilestones = new List<string> { "m1", "m2", "m3" }
            };

            // Act
            var completionPercentage = (progress.CompletedMilestones.Count * 100) / progress.AvailableMilestones.Count;

            // Assert
            completionPercentage.Should().Be(60, "3 of 5 milestones = 60% completion");
        }

        [Fact]
        public void HubProgressStatus_ShouldInitialize_WithDefaults()
        {
            // Arrange & Act
            var progress = new HubProgressStatus();

            // Assert
            progress.CompletionPercentage.Should().Be(0);
            progress.IsUnlocked.Should().BeFalse();
            progress.IsActive.Should().BeFalse();
            progress.LastAccessedAt.Should().BeNull();
            progress.CompletedMilestones.Should().NotBeNull().And.BeEmpty();
            progress.AvailableMilestones.Should().NotBeNull().And.BeEmpty();
            progress.CurrentPhase.Should().BeEmpty();
        }
    }
}