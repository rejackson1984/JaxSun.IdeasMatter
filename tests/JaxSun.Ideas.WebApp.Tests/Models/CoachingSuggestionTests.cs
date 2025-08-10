using FluentAssertions;
using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Services.Interfaces;
using JaxSun.Ideas.Mock.Services.Mock;
using Xunit;

namespace JaxSun.Ideas.Mock.Tests.Models
{
    /// <summary>
    /// Comprehensive tests for CoachingSuggestion functionality
    /// Tests AI-driven recommendations, suggestion types, and personalized guidance
    /// </summary>
    public class CoachingSuggestionTests
    {
        private readonly ICoachPersonaService _coachPersonaService;

        public CoachingSuggestionTests()
        {
            _coachPersonaService = new MockCoachPersonaService();
        }

        #region CoachingSuggestion Construction Tests

        [Fact]
        public void CoachingSuggestion_ShouldInitialize_WithDefaultValues()
        {
            // Act
            var suggestion = new CoachingSuggestion();

            // Assert
            suggestion.Id.Should().BeEmpty("Id should start empty");
            suggestion.Title.Should().BeEmpty("Title should start empty");
            suggestion.Message.Should().BeEmpty("Message should start empty");
            suggestion.Icon.Should().BeEmpty("Icon should start empty");
            suggestion.Type.Should().Be(default(CoachingSuggestionType));
            suggestion.Priority.Should().Be(0, "Priority should default to 0");
            suggestion.ActionUrl.Should().BeNull("ActionUrl should be null by default");
            suggestion.ActionText.Should().BeNull("ActionText should be null by default");
            suggestion.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CoachingSuggestion_ShouldAllow_PropertyInitialization()
        {
            // Arrange & Act
            var timestamp = DateTime.UtcNow;
            var suggestion = new CoachingSuggestion
            {
                Id = "test-suggestion",
                Title = "Test Suggestion",
                Message = "This is a test suggestion for your business development.",
                Icon = "fas fa-lightbulb",
                Type = CoachingSuggestionType.NextStep,
                Priority = 8,
                ActionUrl = "/test-action",
                ActionText = "Take Action",
                CreatedAt = timestamp
            };

            // Assert
            suggestion.Id.Should().Be("test-suggestion");
            suggestion.Title.Should().Be("Test Suggestion");
            suggestion.Message.Should().Be("This is a test suggestion for your business development.");
            suggestion.Icon.Should().Be("fas fa-lightbulb");
            suggestion.Type.Should().Be(CoachingSuggestionType.NextStep);
            suggestion.Priority.Should().Be(8);
            suggestion.ActionUrl.Should().Be("/test-action");
            suggestion.ActionText.Should().Be("Take Action");
            suggestion.CreatedAt.Should().Be(timestamp);
        }

        #endregion

        #region Suggestion Type Tests

        [Theory]
        [InlineData(CoachingSuggestionType.NextStep, "Complete market research analysis")]
        [InlineData(CoachingSuggestionType.Encouragement, "You're making great progress!")]
        [InlineData(CoachingSuggestionType.Tips, "Pro tip: Focus on customer validation")]
        [InlineData(CoachingSuggestionType.Warning, "Important: Review legal requirements")]
        [InlineData(CoachingSuggestionType.Achievement, "Congratulations on reaching 50% completion!")]
        [InlineData(CoachingSuggestionType.Educational, "Learn about market sizing techniques")]
        public void CoachingSuggestion_ShouldSupport_AllSuggestionTypes(CoachingSuggestionType type, string sampleMessage)
        {
            // Act
            var suggestion = new CoachingSuggestion
            {
                Type = type,
                Message = sampleMessage
            };

            // Assert
            suggestion.Type.Should().Be(type);
            suggestion.Message.Should().Be(sampleMessage);
        }

        [Fact]
        public void CoachingSuggestion_ShouldCategorize_SuggestionsByPriority()
        {
            // Arrange & Act
            var highPrioritySuggestion = new CoachingSuggestion { Priority = 10, Type = CoachingSuggestionType.Warning };
            var mediumPrioritySuggestion = new CoachingSuggestion { Priority = 5, Type = CoachingSuggestionType.Tips };
            var lowPrioritySuggestion = new CoachingSuggestion { Priority = 1, Type = CoachingSuggestionType.Educational };

            // Assert
            highPrioritySuggestion.Priority.Should().BeGreaterThan(mediumPrioritySuggestion.Priority);
            mediumPrioritySuggestion.Priority.Should().BeGreaterThan(lowPrioritySuggestion.Priority);
        }

        #endregion

        #region AI-Driven Suggestion Generation Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldReturn_ContextualSuggestions_ForIdeaDevelopmentHub()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 30,
                CompletedMilestones = new List<string> { "idea_submitted" },
                IsFirstTimeInHub = false
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            suggestions.Should().NotBeEmpty("Should provide suggestions for idea development");
            suggestions.Should().AllSatisfy(suggestion =>
            {
                suggestion.Id.Should().NotBeEmpty("Suggestion should have an ID");
                suggestion.Title.Should().NotBeEmpty("Suggestion should have a title");
                suggestion.Message.Should().NotBeEmpty("Suggestion should have a message");
                suggestion.Priority.Should().BeGreaterThan(0, "Suggestion should have a priority");
            });
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldReturn_ContextualSuggestions_ForBusinessPlanningHub()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 60,
                CompletedMilestones = new List<string> { "idea_validated", "market_research_completed" },
                IsFirstTimeInHub = false
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            suggestions.Should().NotBeEmpty("Should provide suggestions for business planning");
            suggestions.Should().Contain(s => s.Type == CoachingSuggestionType.NextStep, "Should include next step suggestions");
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldReturn_ContextualSuggestions_ForBusinessOperationsHub()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessOperations,
                UserProgress = 80,
                CompletedMilestones = new List<string> { "business_plan_finalized", "funding_secured" },
                IsFirstTimeInHub = false
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessOperations, context);

            // Assert
            suggestions.Should().NotBeEmpty("Should provide suggestions for business operations");
            suggestions.Should().Contain(s => s.Message.Contains("KPI") || s.Message.Contains("dashboard"), 
                "Should include operations-specific suggestions");
        }

        #endregion

        #region Priority-Based Suggestion Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldReturn_LimitedHighPrioritySuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 25,
                CompletedMilestones = new List<string>()
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            suggestions.Should().HaveCountLessOrEqualTo(3, "Should limit number of suggestions returned");
            suggestions.Should().BeInDescendingOrder(s => s.Priority, "Should be ordered by priority descending");
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldPrioritize_NextStepSuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 40,
                CompletedMilestones = new List<string> { "idea_validated" }
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            var nextStepSuggestions = suggestions.Where(s => s.Type == CoachingSuggestionType.NextStep).ToList();
            if (nextStepSuggestions.Any())
            {
                nextStepSuggestions.Should().AllSatisfy(suggestion =>
                {
                    suggestion.Priority.Should().BeGreaterThan(5, "NextStep suggestions should have high priority");
                });
            }
        }

        #endregion

        #region Progress-Based Suggestion Adaptation Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_WithHighProgress_ShouldSuggestAdvancedActions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 85,
                CompletedMilestones = new List<string> 
                { 
                    "idea_submitted", "market_research_completed", "validation_completed" 
                }
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            suggestions.Should().Contain(s => s.Type == CoachingSuggestionType.Achievement || 
                                             s.Message.Contains("planning") ||
                                             (s.ActionUrl != null && s.ActionUrl.Contains("planning")),
                "High progress should suggest moving to next hub or celebrate achievements");
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_WithLowProgress_ShouldSuggestFoundationalActions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 15,
                CompletedMilestones = new List<string>(),
                IsFirstTimeInHub = true
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            suggestions.Should().Contain(s => s.Type == CoachingSuggestionType.NextStep,
                "Low progress should include foundational next steps");
        }

        #endregion

        #region Hub-Specific Suggestion Content Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ForIdeaDevelopment_ShouldInclude_MarketResearchSuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 30
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            suggestions.Should().Contain(s => 
                s.Message.Contains("market", StringComparison.OrdinalIgnoreCase) ||
                s.Title.Contains("research", StringComparison.OrdinalIgnoreCase) ||
                (s.ActionUrl != null && s.ActionUrl.Contains("market")),
                "Idea development should include market research suggestions");
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ForBusinessPlanning_ShouldInclude_FinancialSuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 50
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            suggestions.Should().Contain(s => 
                s.Message.Contains("financial", StringComparison.OrdinalIgnoreCase) ||
                s.Title.Contains("financial", StringComparison.OrdinalIgnoreCase) ||
                (s.ActionUrl != null && s.ActionUrl.Contains("financial")),
                "Business planning should include financial suggestions");
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ForBusinessOperations_ShouldInclude_KPISuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessOperations,
                UserProgress = 40
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessOperations, context);

            // Assert
            suggestions.Should().Contain(s => 
                s.Message.Contains("KPI", StringComparison.OrdinalIgnoreCase) ||
                s.Message.Contains("dashboard", StringComparison.OrdinalIgnoreCase) ||
                s.Title.Contains("dashboard", StringComparison.OrdinalIgnoreCase),
                "Business operations should include KPI/dashboard suggestions");
        }

        #endregion

        #region Actionable Suggestion Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldInclude_ActionableSuggestions()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 35
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            suggestions.Should().AllSatisfy(suggestion =>
            {
                if (suggestion.Type == CoachingSuggestionType.NextStep)
                {
                    suggestion.ActionUrl.Should().NotBeNull("NextStep suggestions should have action URLs");
                    suggestion.ActionText.Should().NotBeNull("NextStep suggestions should have action text");
                }
            });
        }

        [Fact]
        public async Task GetNextStepGuidanceAsync_ShouldReturn_HubSpecificGuidance()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 45,
                CompletedMilestones = new List<string> { "idea_validated" }
            };

            // Act
            var guidance = await _coachPersonaService.GetNextStepGuidanceAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            guidance.Should().NotBeNull("Should provide next step guidance");
            guidance.Type.Should().Be(CoachingSuggestionType.NextStep);
            guidance.Priority.Should().Be(10, "Next step guidance should have highest priority");
            guidance.ActionUrl.Should().NotBeNull("Should provide actionable URL");
            guidance.ActionText.Should().NotBeNull("Should provide action text");
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment)]
        [InlineData(BusinessHub.BusinessPlanning)]
        [InlineData(BusinessHub.BusinessOperations)]
        public async Task GetNextStepGuidanceAsync_ShouldReturn_UniqueGuidance_ForEachHub(BusinessHub hub)
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = hub,
                UserProgress = 50
            };

            // Act
            var guidance = await _coachPersonaService.GetNextStepGuidanceAsync(hub, context);

            // Assert
            guidance.Should().NotBeNull($"Should provide guidance for {hub}");
            guidance.Id.Should().NotBeEmpty($"Should have unique ID for {hub}");
            guidance.Title.Should().NotBeEmpty($"Should have title for {hub}");
            guidance.Message.Should().NotBeEmpty($"Should have message for {hub}");
        }

        #endregion

        #region Suggestion Validation Tests

        [Fact]
        public void CoachingSuggestion_ShouldSupport_LongMessages()
        {
            // Arrange
            var longMessage = new string('A', 500);

            // Act
            var suggestion = new CoachingSuggestion
            {
                Message = longMessage,
                Title = "Long Message Test"
            };

            // Assert
            suggestion.Message.Should().HaveLength(500);
            suggestion.Message.Should().Be(longMessage);
        }

        [Fact]
        public void CoachingSuggestion_ShouldHandle_NullValues_Gracefully()
        {
            // Act
            var suggestion = new CoachingSuggestion
            {
                ActionUrl = null,
                ActionText = null
            };

            // Assert
            suggestion.ActionUrl.Should().BeNull();
            suggestion.ActionText.Should().BeNull();
        }

        [Fact]
        public void CoachingSuggestion_ShouldSupport_VariedIconFormats()
        {
            // Arrange & Act
            var suggestions = new[]
            {
                new CoachingSuggestion { Icon = "fas fa-lightbulb" },
                new CoachingSuggestion { Icon = "far fa-chart-bar" },
                new CoachingSuggestion { Icon = "fal fa-rocket" },
                new CoachingSuggestion { Icon = "fab fa-github" }
            };

            // Assert
            suggestions.Should().AllSatisfy(suggestion =>
            {
                suggestion.Icon.Should().NotBeNull();
                suggestion.Icon.Should().MatchRegex(@"^fa[srlab] fa-\w+", "Should follow FontAwesome pattern");
            });
        }

        #endregion

        #region Suggestion Collection Management Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldSupport_SuggestionFiltering()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 60
            };

            // Act
            var allSuggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);
            var nextStepSuggestions = allSuggestions.Where(s => s.Type == CoachingSuggestionType.NextStep).ToList();
            var tipSuggestions = allSuggestions.Where(s => s.Type == CoachingSuggestionType.Tips).ToList();

            // Assert
            allSuggestions.Should().NotBeEmpty("Should have suggestions");
            nextStepSuggestions.Should().NotBeEmpty("Should have next step suggestions");
            if (tipSuggestions.Any())
            {
                tipSuggestions.Should().AllSatisfy(s => s.Type.Should().Be(CoachingSuggestionType.Tips));
            }
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldSupport_PriorityOrdering()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 45
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            suggestions.Should().NotBeEmpty("Should have suggestions");
            for (int i = 0; i < suggestions.Count - 1; i++)
            {
                suggestions[i].Priority.Should().BeGreaterOrEqualTo(suggestions[i + 1].Priority,
                    "Suggestions should be ordered by priority descending");
            }
        }

        #endregion

        #region Suggestion Timing Tests

        [Fact]
        public void CoachingSuggestion_ShouldTrack_CreationTime()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var suggestion = new CoachingSuggestion
            {
                Title = "Timed Suggestion",
                Message = "This suggestion tracks creation time"
            };

            var afterCreation = DateTime.UtcNow;

            // Assert
            suggestion.CreatedAt.Should().BeOnOrAfter(beforeCreation);
            suggestion.CreatedAt.Should().BeOnOrBefore(afterCreation);
        }

        [Fact]
        public void CoachingSuggestion_ShouldSupport_TimestampSorting()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var suggestions = new[]
            {
                new CoachingSuggestion { CreatedAt = baseTime.AddMinutes(-10), Title = "First" },
                new CoachingSuggestion { CreatedAt = baseTime.AddMinutes(-5), Title = "Second" },
                new CoachingSuggestion { CreatedAt = baseTime, Title = "Third" }
            };

            // Act
            var orderedSuggestions = suggestions.OrderBy(s => s.CreatedAt).ToList();

            // Assert
            orderedSuggestions[0].Title.Should().Be("First");
            orderedSuggestions[1].Title.Should().Be("Second");
            orderedSuggestions[2].Title.Should().Be("Third");
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_WithInvalidHub_ShouldHandleGracefully()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = (BusinessHub)999,
                UserProgress = 50
            };

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () =>
                await _coachPersonaService.GetCoachingSuggestionsAsync((BusinessHub)999, context));

            exception.Should().BeNull("Should handle invalid hub gracefully");
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_WithNullContext_ShouldHandleGracefully()
        {
            // Act & Assert
            var exception = await Record.ExceptionAsync(async () =>
                await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, null!));

            exception.Should().BeNull("Should handle null context gracefully");
        }

        #endregion

        #region Suggestion Content Quality Tests

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldProvide_MeaningfulContent()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 40,
                CompletedMilestones = new List<string> { "idea_submitted" }
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            suggestions.Should().AllSatisfy(suggestion =>
            {
                suggestion.Title.Should().NotBeEmpty("Title should not be empty");
                suggestion.Title.Length.Should().BeGreaterThan(5, "Title should be meaningful");
                suggestion.Message.Should().NotBeEmpty("Message should not be empty");
                suggestion.Message.Length.Should().BeGreaterThan(10, "Message should be descriptive");
            });
        }

        [Fact]
        public async Task GetCoachingSuggestionsAsync_ShouldProvide_CoherentActionFlow()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 30
            };

            // Act
            var suggestions = await _coachPersonaService.GetCoachingSuggestionsAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            var actionableSuggestions = suggestions.Where(s => s.ActionUrl != null).ToList();
            actionableSuggestions.Should().AllSatisfy(suggestion =>
            {
                suggestion.ActionUrl.Should().StartWith("/", "Action URLs should be valid paths");
                suggestion.ActionText.Should().NotBeEmpty("Action text should describe the action");
            });
        }

        #endregion
    }
}