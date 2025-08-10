using JaxSun.Ideas.WebApp.Services.Interfaces;
using JaxSun.Ideas.WebApp.Services.Mock;
using FluentAssertions;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Features.Hub1
{
    public class IdeaValidationServiceTests
    {
        private readonly MockIdeaValidationService _validationService;

        public IdeaValidationServiceTests()
        {
            _validationService = new MockIdeaValidationService();
        }

        [Fact]
        public async Task ValidateIdeaAsync_ShouldReturn_ComprehensiveValidationResult()
        {
            // Arrange
            var request = CreateValidIdeaRequest();

            // Act
            var result = await _validationService.ValidateIdeaAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Request.Should().BeEquivalentTo(request);
            result.OverallScore.Should().BeInRange(1, 100);
            result.CategoryScores.Should().NotBeEmpty();
            result.Strengths.Should().NotBeEmpty();
            result.NextSteps.Should().NotBeEmpty();
            result.MarketOpportunity.Should().NotBeNull();
            result.Competition.Should().NotBeNull();
        }

        [Theory]
        [InlineData("AI-powered productivity tool", 70, 90)]
        [InlineData("Basic web service", 40, 70)]
        [InlineData("Revolutionary blockchain solution", 60, 85)]
        public async Task ValidateIdeaAsync_ShouldScore_BasedOnIdeaQuality(string description, int minScore, int maxScore)
        {
            // Arrange
            var request = CreateIdeaRequest(description, "Increases productivity", "Business professionals");

            // Act
            var result = await _validationService.ValidateIdeaAsync(request);

            // Assert
            result.OverallScore.Should().BeInRange(minScore, maxScore);
        }

        [Fact]
        public async Task ValidateIdeaAsync_ShouldInclude_AllSWOTCategories()
        {
            // Arrange
            var request = CreateValidIdeaRequest();

            // Act
            var result = await _validationService.ValidateIdeaAsync(request);

            // Assert
            result.Strengths.Should().NotBeEmpty("Should identify idea strengths");
            result.Weaknesses.Should().NotBeEmpty("Should identify areas for improvement");
            result.Opportunities.Should().NotBeEmpty("Should identify market opportunities");
            result.Threats.Should().NotBeEmpty("Should identify potential threats");
        }

        [Fact]
        public async Task CalculateValidationScoreAsync_ShouldReturn_ValidScore()
        {
            // Arrange
            var request = CreateValidIdeaRequest();

            // Act
            var score = await _validationService.CalculateValidationScoreAsync(request);

            // Assert
            score.Should().BeInRange(1, 100, "Validation score should be between 1-100");
        }

        [Theory]
        [InlineData("Quick", 3, 5)]
        [InlineData("Deep-Dive", 5, 8)]
        [InlineData("Launch", 8, 12)]
        public async Task GetValidationCriteriaAsync_ShouldReturn_CorrectCriteriaCount(string strategy, int minCriteria, int maxCriteria)
        {
            // Act
            var criteria = await _validationService.GetValidationCriteriaAsync(strategy);

            // Assert
            criteria.Should().HaveCountGreaterOrEqualTo(minCriteria);
            criteria.Should().HaveCountLessOrEqualTo(maxCriteria);
            criteria.Should().OnlyContain(c => !string.IsNullOrEmpty(c.Name));
            criteria.Should().OnlyContain(c => !string.IsNullOrEmpty(c.Description));
            criteria.Should().OnlyContain(c => c.Weight > 0);
        }

        [Fact]
        public async Task GetImprovementRecommendationsAsync_ShouldProvide_ActionableRecommendations()
        {
            // Arrange
            var request = CreateValidIdeaRequest();
            var validationResult = await _validationService.ValidateIdeaAsync(request);

            // Act
            var recommendations = await _validationService.GetImprovementRecommendationsAsync(validationResult);

            // Assert
            recommendations.Should().NotBeEmpty("Should provide improvement recommendations");
            recommendations.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r));
            recommendations.Should().Contain(r => r.Length > 10, "Recommendations should be descriptive");
        }

        [Theory]
        [InlineData(85, true)]
        [InlineData(70, true)]
        [InlineData(65, false)]
        [InlineData(30, false)]
        public async Task IsReadyForOperationsHubAsync_ShouldDetermineReadiness_BasedOnScore(int overallScore, bool expectedReadiness)
        {
            // Arrange
            var validationResult = CreateMockValidationResult(overallScore);

            // Act
            var isReady = await _validationService.IsReadyForNextHubAsync(validationResult);

            // Assert
            isReady.Should().Be(expectedReadiness, $"Score of {overallScore} should {(expectedReadiness ? "" : "not ")}be ready for next hub");
        }

        [Fact]
        public async Task GetMarketResearchSuggestionsAsync_ShouldProvide_RelevantSuggestions()
        {
            // Arrange
            var request = CreateIdeaRequest("Mobile app for food delivery", "Solves hunger", "Busy professionals");

            // Act
            var suggestions = await _validationService.GetMarketResearchSuggestionsAsync(request);

            // Assert
            suggestions.Should().NotBeEmpty("Should provide market research suggestions");
            suggestions.Should().Contain(s => s.ToLower().Contains("market") || s.ToLower().Contains("research"));
            suggestions.Should().HaveCount(count => count >= 3, "Should provide multiple research avenues");
        }

        [Fact]
        public async Task AnalyzeCompetitionAsync_ShouldProvide_CompetitiveAnalysis()
        {
            // Arrange
            var request = CreateValidIdeaRequest();

            // Act
            var competition = await _validationService.AnalyzeCompetitionAsync(request);

            // Assert
            competition.Should().NotBeNull();
            competition.DirectCompetitors.Should().NotBeEmpty("Should identify direct competitors");
            competition.IndirectCompetitors.Should().NotBeEmpty("Should identify indirect competitors");
            competition.CompetitiveLandscape.Should().NotBeEmpty("Should describe competitive landscape");
            competition.CompetitiveAdvantages.Should().NotBeEmpty("Should identify competitive advantages");
            competition.MarketGaps.Should().NotBeEmpty("Should identify market gaps");
            competition.CompetitionIntensity.Should().BeInRange(1, 10, "Competition intensity should be 1-10 scale");
        }

        [Fact]
        public async Task AnalyzeCompetitionAsync_ShouldIdentify_BothDirectAndIndirectCompetitors()
        {
            // Arrange
            var request = CreateIdeaRequest("Project management software", "Organizes tasks", "Small businesses");

            // Act
            var competition = await _validationService.AnalyzeCompetitionAsync(request);

            // Assert
            competition.DirectCompetitors.Should().NotBeEmpty().And.OnlyContain(c => c.IsDirect == true);
            competition.IndirectCompetitors.Should().NotBeEmpty().And.OnlyContain(c => c.IsDirect == false);
            
            // Validate competitor data structure
            var allCompetitors = competition.DirectCompetitors.Concat(competition.IndirectCompetitors);
            allCompetitors.Should().OnlyContain(c => !string.IsNullOrEmpty(c.Name));
            allCompetitors.Should().OnlyContain(c => !string.IsNullOrEmpty(c.Description));
        }

        [Fact]
        public async Task EstimateMarketOpportunityAsync_ShouldProvide_MarketAnalysis()
        {
            // Arrange
            var request = CreateValidIdeaRequest();

            // Act
            var opportunity = await _validationService.EstimateMarketOpportunityAsync(request);

            // Assert
            opportunity.Should().NotBeNull();
            opportunity.MarketSize.Should().NotBeEmpty("Should estimate market size");
            opportunity.GrowthRate.Should().NotBeEmpty("Should provide growth rate");
            opportunity.Trends.Should().NotBeEmpty("Should identify market trends");
            opportunity.KeyDrivers.Should().NotBeEmpty("Should identify key market drivers");
            opportunity.Barriers.Should().NotBeEmpty("Should identify barriers to entry");
            opportunity.OpportunityScore.Should().BeInRange(1, 100, "Opportunity score should be 1-100");
            opportunity.SizeCategory.Should().BeOneOf("Niche", "Medium", "Large", "Should categorize market size");
        }

        [Theory]
        [InlineData("Enterprise software solution", "Large")]
        [InlineData("Local service app", "Niche")]
        [InlineData("E-commerce platform", "Medium")]
        public async Task EstimateMarketOpportunityAsync_ShouldCategorize_MarketSize(string ideaDescription, string expectedCategory)
        {
            // Arrange
            var request = CreateIdeaRequest(ideaDescription, "Solves business problems", "Various audiences");

            // Act
            var opportunity = await _validationService.EstimateMarketOpportunityAsync(request);

            // Assert
            opportunity.SizeCategory.Should().Be(expectedCategory, $"Idea '{ideaDescription}' should be categorized as {expectedCategory}");
        }

        [Fact]
        public async Task ValidationResult_ShouldInclude_TimestampAndId()
        {
            // Arrange
            var request = CreateValidIdeaRequest();
            var beforeValidation = DateTime.UtcNow;

            // Act
            var result = await _validationService.ValidateIdeaAsync(request);

            // Assert
            result.Id.Should().NotBeEmpty("Should have unique ID");
            result.ValidatedAt.Should().BeOnOrAfter(beforeValidation, "Should timestamp validation");
            result.ValidatedAt.Should().BeOnOrBefore(DateTime.UtcNow.AddSeconds(1), "Timestamp should be recent");
        }

        [Fact]
        public async Task ValidationResult_ShouldHave_ConsistentCategoryScores()
        {
            // Arrange
            var request = CreateValidIdeaRequest();

            // Act
            var result = await _validationService.ValidateIdeaAsync(request);

            // Assert
            result.CategoryScores.Should().NotBeEmpty("Should have category breakdowns");
            result.CategoryScores.Values.Should().OnlyContain(score => score >= 0 && score <= 100);
            
            // Common validation categories
            var expectedCategories = new[] { "Market", "Competition", "Feasibility", "Revenue", "Execution" };
            result.CategoryScores.Keys.Should().Contain(key => expectedCategories.Any(cat => key.Contains(cat)));
        }

        [Fact]
        public async Task ValidationService_ShouldHandle_EmptyInputs_Gracefully()
        {
            // Arrange
            var emptyRequest = new IdeaValidationRequest
            {
                Description = "",
                ProblemSolved = "",
                TargetAudience = "",
                RevenueModel = ""
            };

            // Act
            var result = await _validationService.ValidateIdeaAsync(emptyRequest);

            // Assert
            result.Should().NotBeNull("Should handle empty inputs gracefully");
            result.OverallScore.Should().BeGreaterThan(0, "Should provide minimum score even for empty inputs");
            result.NextSteps.Should().NotBeEmpty("Should provide guidance for incomplete ideas");
        }

        private IdeaValidationRequest CreateValidIdeaRequest()
        {
            return CreateIdeaRequest(
                "AI-powered project management tool that automatically prioritizes tasks",
                "Helps teams focus on high-impact work and reduce decision fatigue",
                "Software development teams and project managers"
            );
        }

        private IdeaValidationRequest CreateIdeaRequest(string description, string problemSolved, string targetAudience)
        {
            return new IdeaValidationRequest
            {
                Description = description,
                ProblemSolved = problemSolved,
                TargetAudience = targetAudience,
                RevenueModel = "SaaS subscription model",
                Strategy = "Deep-Dive",
                SubmittedAt = DateTime.UtcNow
            };
        }

        private IdeaValidationResult CreateMockValidationResult(int overallScore)
        {
            return new IdeaValidationResult
            {
                Id = Guid.NewGuid().ToString(),
                OverallScore = overallScore,
                CategoryScores = new Dictionary<string, int>
                {
                    ["Market"] = overallScore + Random.Shared.Next(-10, 10),
                    ["Competition"] = overallScore + Random.Shared.Next(-10, 10),
                    ["Feasibility"] = overallScore + Random.Shared.Next(-10, 10)
                },
                ValidatedAt = DateTime.UtcNow
            };
        }
    }
}