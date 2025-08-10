using FluentAssertions;
using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using JaxSun.Ideas.WebApp.Services.Mock;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Services
{
    /// <summary>
    /// Comprehensive tests for Coach Persona Service functionality
    /// Tests all three coach personas (Spark, Strategy, Execute) and their behavior
    /// </summary>
    public class CoachPersonaServiceTests
    {
        private readonly ICoachPersonaService _coachPersonaService;

        public CoachPersonaServiceTests()
        {
            _coachPersonaService = new MockCoachPersonaService();
        }

        #region Basic Coach Persona Tests

        [Fact]
        public async Task GetCoachForHubAsync_ShouldReturn_SparkCoach_ForIdeaDevelopmentHub()
        {
            // Act
            var result = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.IdeaDevelopment);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Spark");
            result.Hub.Should().Be(BusinessHub.IdeaDevelopment);
            result.Style.Should().Be(CoachingStyle.Enthusiastic);
            result.Description.Should().ContainEquivalentOf("idea");
        }

        [Fact]
        public async Task GetCoachForHubAsync_ShouldReturn_StrategyCoach_ForBusinessPlanningHub()
        {
            // Act
            var result = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.BusinessPlanning);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Strategy");
            result.Hub.Should().Be(BusinessHub.BusinessPlanning);
            result.Style.Should().Be(CoachingStyle.Analytical);
            result.Description.Should().ContainEquivalentOf("business");
        }

        [Fact]
        public async Task GetCoachForHubAsync_ShouldReturn_ExecuteCoach_ForBusinessOperationsHub()
        {
            // Act
            var result = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.BusinessOperations);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Execute");
            result.Hub.Should().Be(BusinessHub.BusinessOperations);
            result.Style.Should().Be(CoachingStyle.ResultsOriented);
            result.Description.Should().ContainEquivalentOf("operations");
        }

        #endregion

        #region Coach Persona Properties Tests

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, "Spark")]
        [InlineData(BusinessHub.BusinessPlanning, "Strategy")]
        [InlineData(BusinessHub.BusinessOperations, "Execute")]
        public async Task GetCoachForHubAsync_ShouldHave_CorrectName_ForEachHub(BusinessHub hub, string expectedName)
        {
            // Act
            var result = await _coachPersonaService.GetCoachForHubAsync(hub);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(expectedName);
        }

        [Fact]
        public async Task AllCoaches_ShouldHave_RequiredProperties()
        {
            // Arrange
            var hubs = new[] { BusinessHub.IdeaDevelopment, BusinessHub.BusinessPlanning, BusinessHub.BusinessOperations };

            foreach (var hub in hubs)
            {
                // Act
                var coach = await _coachPersonaService.GetCoachForHubAsync(hub);

                // Assert
                coach.Should().NotBeNull($"Coach should exist for {hub}");
                coach!.Name.Should().NotBeEmpty($"Coach name should not be empty for {hub}");
                coach.Avatar.Should().NotBeEmpty($"Coach avatar should not be empty for {hub}");
                coach.Personality.Should().NotBeEmpty($"Coach personality should not be empty for {hub}");
                coach.VoicePattern.Should().NotBeEmpty($"Coach voice pattern should not be empty for {hub}");
                coach.Description.Should().NotBeEmpty($"Coach description should not be empty for {hub}");
                coach.Hub.Should().Be(hub, $"Coach hub should match requested hub {hub}");
            }
        }

        #endregion

        #region Coach Specialties and Phrases Tests

        [Fact]
        public async Task SparkCoach_ShouldHave_CreativeSpecialties()
        {
            // Act
            var spark = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.IdeaDevelopment);

            // Assert
            spark.Should().NotBeNull();
            spark!.Specialties.Should().NotBeEmpty("Spark should have specialties");
            spark.Specialties.Should().Contain(s => s.Contains("idea", StringComparison.OrdinalIgnoreCase) ||
                                                   s.Contains("creative", StringComparison.OrdinalIgnoreCase) ||
                                                   s.Contains("innovation", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task AllCoaches_ShouldHave_UniquePersonalities()
        {
            // Act
            var spark = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.IdeaDevelopment);
            var strategy = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.BusinessPlanning);
            var execute = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.BusinessOperations);

            // Assert
            var personalities = new[] { spark!.Personality, strategy!.Personality, execute!.Personality };
            personalities.Should().OnlyHaveUniqueItems("Each coach should have a unique personality");
            
            var voicePatterns = new[] { spark.VoicePattern, strategy.VoicePattern, execute.VoicePattern };
            voicePatterns.Should().OnlyHaveUniqueItems("Each coach should have a unique voice pattern");
        }

        #endregion

        #region Coach Messaging Tests

        [Fact]
        public async Task GetCoachMessageAsync_ShouldReturn_PersonalizedMessage_ForEachHub()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 25,
                IsFirstTimeInHub = true
            };
            var hubs = new[] { BusinessHub.IdeaDevelopment, BusinessHub.BusinessPlanning, BusinessHub.BusinessOperations };

            foreach (var hub in hubs)
            {
                // Act
                var coach = await _coachPersonaService.GetCoachForHubAsync(hub);
                var message = await _coachPersonaService.GetCoachMessageAsync(hub, context);

                // Assert
                message.Should().NotBeNull($"Coach message should exist for {hub}");
                message.Should().NotBeEmpty($"Coach message should not be empty for {hub}");
            }
        }

        [Fact]
        public async Task GetGreetingMessageAsync_ShouldReturn_PersonalizedGreeting()
        {
            // Arrange & Act
            var newUserGreeting = await _coachPersonaService.GetGreetingMessageAsync(BusinessHub.IdeaDevelopment, false);
            var returningUserGreeting = await _coachPersonaService.GetGreetingMessageAsync(BusinessHub.IdeaDevelopment, true);

            // Assert
            newUserGreeting.Should().NotBeEmpty("Should provide greeting for new users");
            returningUserGreeting.Should().NotBeEmpty("Should provide greeting for returning users");
            newUserGreeting.Should().NotBe(returningUserGreeting, "Greetings should be different for new vs returning users");
        }

        #endregion

        #region Coach Suggestions Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldReturn_RelevantSuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 50,
                CompletedMilestones = new List<string> { "idea_validated", "market_research_completed" }
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            suggestions.Should().NotBeEmpty("Should provide suggestions");
            suggestions.Should().AllSatisfy(suggestion =>
            {
                suggestion.Id.Should().NotBeEmpty("Suggestion should have an ID");
                suggestion.Title.Should().NotBeEmpty("Suggestion should have a title");
                suggestion.Message.Should().NotBeEmpty("Suggestion should have a message");
                suggestion.Priority.Should().BeGreaterThan(0, "Suggestion should have a priority");
            });
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, CoachingSuggestionType.NextStep)]
        [InlineData(BusinessHub.BusinessPlanning, CoachingSuggestionType.Tips)]
        [InlineData(BusinessHub.BusinessOperations, CoachingSuggestionType.Warning)]
        public async Task GetCoachingSuggestionsAsync_ShouldInclude_SpecificSuggestionTypes(BusinessHub hub, CoachingSuggestionType expectedType)
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = hub,
                UserProgress = 30
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(hub, context);

            // Assert
            suggestions.Should().Contain(s => s.Type == expectedType, 
                $"Should include {expectedType} suggestions for {hub}");
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task GetCoachForHubAsync_ShouldHandle_InvalidHub_Gracefully()
        {
            // Act
            var result = await _coachPersonaService.GetCoachForHubAsync((BusinessHub)999);

            // Assert - Should either return null or handle gracefully without throwing
            var exception = await Record.ExceptionAsync(async () => 
                await _coachPersonaService.GetCoachForHubAsync((BusinessHub)999));
            
            exception.Should().BeNull("Should handle invalid hub gracefully");
        }

        [Fact]
        public async Task GetCoachMessageAsync_ShouldHandle_NullContext_Gracefully()
        {
            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await _coachPersonaService.GetCoachMessageAsync(BusinessHub.IdeaDevelopment, null!));
            
            exception.Should().BeNull("Should handle null context gracefully");
        }

        #endregion

        #region Coach Consistency Tests

        [Fact]
        public async Task GetCoachForHubAsync_ShouldReturn_ConsistentCoach_ForSameHub()
        {
            // Act - Call multiple times
            var coach1 = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.IdeaDevelopment);
            var coach2 = await _coachPersonaService.GetCoachForHubAsync(BusinessHub.IdeaDevelopment);

            // Assert - Should return the same coach data
            coach1.Should().NotBeNull();
            coach2.Should().NotBeNull();
            coach1!.Name.Should().Be(coach2!.Name);
            coach1.Hub.Should().Be(coach2.Hub);
            coach1.Style.Should().Be(coach2.Style);
        }

        [Fact]
        public async Task AllCoaches_ShouldHave_DistinctStyles()
        {
            // Act
            var coaches = new List<CoachPersona>();
            foreach (var hub in Enum.GetValues<BusinessHub>())
            {
                var coach = await _coachPersonaService.GetCoachForHubAsync(hub);
                if (coach != null)
                {
                    coaches.Add(coach);
                }
            }

            // Assert
            coaches.Should().HaveCount(3, "Should have exactly 3 coaches");
            coaches.Select(c => c.Style).Should().OnlyHaveUniqueItems("Each coach should have a unique coaching style");
            coaches.Select(c => c.Name).Should().OnlyHaveUniqueItems("Each coach should have a unique name");
        }

        #endregion
    }
}