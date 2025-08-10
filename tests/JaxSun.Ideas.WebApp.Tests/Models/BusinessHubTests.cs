using JaxSun.Ideas.WebApp.Models;
using FluentAssertions;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Models
{
    public class BusinessHubTests
    {
        [Fact]
        public void BusinessHub_ShouldHave_ThreeDistinctHubs()
        {
            // Arrange & Act
            var hubs = Enum.GetValues<BusinessHub>();
            
            // Assert
            hubs.Should().HaveCount(3);
            hubs.Should().Contain(BusinessHub.IdeaDevelopment);
            hubs.Should().Contain(BusinessHub.BusinessPlanning);
            hubs.Should().Contain(BusinessHub.BusinessOperations);
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, 1)]
        [InlineData(BusinessHub.BusinessPlanning, 2)]
        [InlineData(BusinessHub.BusinessOperations, 3)]
        public void BusinessHub_ShouldHave_CorrectOrderingValues(BusinessHub hub, int expectedValue)
        {
            // Act & Assert
            ((int)hub).Should().Be(expectedValue);
        }

        [Fact]
        public void BusinessHub_IdeaDevelopment_ShouldBe_FirstHub()
        {
            // Arrange
            var allHubs = Enum.GetValues<BusinessHub>();
            
            // Act
            var firstHub = allHubs.Min();
            
            // Assert
            firstHub.Should().Be(BusinessHub.IdeaDevelopment);
        }

        [Fact]
        public void BusinessHub_BusinessOperations_ShouldBe_LastHub()
        {
            // Arrange
            var allHubs = Enum.GetValues<BusinessHub>();
            
            // Act
            var lastHub = allHubs.Max();
            
            // Assert
            lastHub.Should().Be(BusinessHub.BusinessOperations);
        }
    }
}