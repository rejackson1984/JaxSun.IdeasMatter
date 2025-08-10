using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using JaxSun.Ideas.WebApp.Services.Mock;
using FluentAssertions;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Features.Hub2
{
    public class BusinessPlanServiceTests
    {
        private readonly MockBusinessPlanService _businessPlanService;

        public BusinessPlanServiceTests()
        {
            _businessPlanService = new MockBusinessPlanService();
        }

        [Fact]
        public async Task GenerateBusinessPlanAsync_ShouldReturn_ComprehensiveBusinessPlan()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var result = await _businessPlanService.GenerateBusinessPlanAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Request.Should().BeEquivalentTo(request);
            result.ExecutiveSummary.Should().NotBeEmpty();
            result.MarketAnalysis.Should().NotBeNull();
            result.CompetitiveAnalysis.Should().NotBeNull();
            result.MarketingStrategy.Should().NotBeNull();
            result.OperationalPlan.Should().NotBeNull();
            result.FinancialProjections.Should().NotBeNull();
            result.RiskAssessment.Should().NotBeNull();
            result.FundingRequirements.Should().NotBeNull();
        }

        [Theory]
        [InlineData("AI-powered fitness app", "Technology")]
        [InlineData("Local restaurant delivery", "Food Service")]
        [InlineData("Educational platform", "Education")]
        public async Task GenerateBusinessPlanAsync_ShouldCategorize_BusinessCorrectly(string businessIdea, string expectedCategory)
        {
            // Arrange
            var request = CreateBusinessPlanRequest(businessIdea, "Solves problems", "Target users", "SaaS model");

            // Act
            var result = await _businessPlanService.GenerateBusinessPlanAsync(request);

            // Assert
            result.BusinessCategory.Should().Be(expectedCategory);
        }

        [Fact]
        public async Task GenerateFinancialProjectionsAsync_ShouldProvide_ThreeYearProjections()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var projections = await _businessPlanService.GenerateFinancialProjectionsAsync(request);

            // Assert
            projections.Should().NotBeNull();
            projections.YearlyBreakdown.Should().HaveCount(3);
            projections.Revenue.Should().NotBeNull();
            projections.Expenses.Should().NotBeNull();
            projections.Metrics.Should().NotBeNull();
            projections.CashFlow.Should().NotBeNull();
            projections.CashFlow.Monthly.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("SaaS subscription", true)]
        [InlineData("One-time purchase", false)]
        [InlineData("Commission-based", false)]
        public async Task GenerateFinancialProjectionsAsync_ShouldIdentify_RecurringRevenue(string revenueModel, bool expectRecurring)
        {
            // Arrange
            var request = CreateBusinessPlanRequest("Test business", "Solves problems", "Target users", revenueModel);

            // Act
            var projections = await _businessPlanService.GenerateFinancialProjectionsAsync(request);

            // Assert
            var hasRecurringRevenue = projections.Revenue.RevenueModel.Contains("subscription", StringComparison.OrdinalIgnoreCase) ||
                                     projections.Revenue.RevenueModel.Contains("recurring", StringComparison.OrdinalIgnoreCase);
            hasRecurringRevenue.Should().Be(expectRecurring);
        }

        [Fact]
        public async Task GenerateMarketingStrategyAsync_ShouldProvide_ComprehensiveMarketingPlan()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var strategy = await _businessPlanService.GenerateMarketingStrategyAsync(request);

            // Assert
            strategy.Should().NotBeNull();
            strategy.TargetMarketSegments.Should().NotBeEmpty();
            strategy.ValueProposition.Should().NotBeEmpty();
            strategy.MarketingChannels.Should().NotBeEmpty();
            strategy.CustomerAcquisitionStrategy.Should().NotBeEmpty();
            strategy.BrandingStrategy.Should().NotBeNull();
            strategy.PricingStrategy.Should().NotBeNull();
            strategy.ValueProposition.Should().NotBeEmpty();
            strategy.MarketingBudget.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GenerateOperationalPlanAsync_ShouldDefine_BusinessOperations()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var plan = await _businessPlanService.GenerateOperationalPlanAsync(request);

            // Assert
            plan.Should().NotBeNull();
            plan.BusinessModel.Should().NotBeEmpty();
            plan.OperationalStructure.Should().NotBeEmpty();
            plan.TechnologyRequirements.Should().NotBeEmpty();
            plan.StaffingPlan.Should().NotBeNull();
            plan.SupplyChainManagement.Should().NotBeEmpty();
            plan.QualityControl.Should().NotBeEmpty();
            plan.ScalingStrategy.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GenerateRiskAssessmentAsync_ShouldIdentify_BusinessRisks()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var assessment = await _businessPlanService.GenerateRiskAssessmentAsync(request);

            // Assert
            assessment.Should().NotBeNull();
            assessment.MarketRisks.Should().NotBeEmpty();
            assessment.CompetitiveRisks.Should().NotBeEmpty();
            assessment.FinancialRisks.Should().NotBeEmpty();
            assessment.OperationalRisks.Should().NotBeEmpty();
            assessment.TechnicalRisks.Should().NotBeEmpty();
            assessment.RegulatoryRisks.Should().NotBeEmpty();
            assessment.MitigationStrategies.Should().NotBeEmpty();
            assessment.OverallRiskLevel.Should().BeOneOf("Low", "Medium", "High");
        }

        [Fact]
        public async Task CalculateBusinessViabilityScoreAsync_ShouldReturn_ValidScore()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var score = await _businessPlanService.CalculateBusinessViabilityScoreAsync(request);

            // Assert
            score.Should().BeInRange(1, 100, "Business viability score should be between 1-100");
        }

        [Theory]
        [InlineData(85, true)]
        [InlineData(80, true)]
        [InlineData(75, false)]
        [InlineData(60, false)]
        public async Task IsReadyForOperationsHubAsync_ShouldDetermineReadiness_BasedOnScore(int viabilityScore, bool expectedReadiness)
        {
            // Arrange
            var businessPlan = CreateMockBusinessPlan(viabilityScore);

            // Act
            var isReady = await _businessPlanService.IsReadyForOperationsHubAsync(businessPlan);

            // Assert
            isReady.Should().Be(expectedReadiness, $"Score of {viabilityScore} should {(expectedReadiness ? "" : "not ")}be ready for operations hub");
        }

        [Fact]
        public async Task GetBusinessPlanTemplatesAsync_ShouldReturn_IndustrySpecificTemplates()
        {
            // Act
            var templates = await _businessPlanService.GetBusinessPlanTemplatesAsync();

            // Assert
            templates.Should().NotBeEmpty("Should provide business plan templates");
            templates.Should().HaveCountGreaterOrEqualTo(5, "Should have multiple template options");
            templates.Should().OnlyContain(t => !string.IsNullOrEmpty(t.Name));
            templates.Should().OnlyContain(t => !string.IsNullOrEmpty(t.Industry));
            templates.Should().OnlyContain(t => !string.IsNullOrEmpty(t.Description));
            templates.Should().OnlyContain(t => t.Sections.Count > 0);
        }

        [Fact]
        public async Task GetFundingOptionsAsync_ShouldRecommend_AppropiateFundingSources()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var options = await _businessPlanService.GetFundingOptionsAsync(request);

            // Assert
            options.Should().NotBeEmpty("Should provide funding options");
            options.Should().HaveCountGreaterOrEqualTo(3, "Should provide multiple funding sources");
            options.Should().OnlyContain(o => !string.IsNullOrEmpty(o.FundingType));
            options.Should().OnlyContain(o => !string.IsNullOrEmpty(o.Description));
            options.Should().OnlyContain(o => o.TypicalAmountRange.Contains("-") || o.TypicalAmountRange.Contains("+"));
            options.Should().OnlyContain(o => o.SuitabilityScore >= 0 && o.SuitabilityScore <= 100);
        }

        [Fact]
        public async Task ValidateBusinessModelAsync_ShouldAssess_ModelViability()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var validation = await _businessPlanService.ValidateBusinessModelAsync(request);

            // Assert
            validation.Should().NotBeNull();
            validation.IsViable.Should().BeTrue();
            validation.ViabilityScore.Should().BeInRange(0, 100);
            validation.Strengths.Should().NotBeEmpty();
            validation.Weaknesses.Should().NotBeEmpty();
            validation.Recommendations.Should().NotBeEmpty();
            validation.KeyMetrics.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GenerateCompetitiveAnalysisAsync_ShouldProvide_DetailedCompetitorInsights()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var analysis = await _businessPlanService.GenerateCompetitiveAnalysisAsync(request);

            // Assert
            analysis.Should().NotBeNull();
            analysis.DirectCompetitors.Should().NotBeEmpty();
            analysis.IndirectCompetitors.Should().NotBeEmpty();
            analysis.MarketPositioning.Should().NotBeEmpty();
            analysis.CompetitiveAdvantages.Should().NotBeEmpty();
            analysis.CompetitiveThreats.Should().NotBeEmpty();
            analysis.MarketGaps.Should().NotBeEmpty();
            analysis.DifferentiationStrategy.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GenerateTimelineAsync_ShouldCreate_ImplementationRoadmap()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();

            // Act
            var timeline = await _businessPlanService.GenerateTimelineAsync(request);

            // Assert
            timeline.Should().NotBeNull();
            timeline.Phases.Should().NotBeEmpty();
            timeline.TotalDurationWeeks.Should().BeGreaterThan(0);
            timeline.KeyMilestones.Should().NotBeEmpty();
            timeline.CriticalPath.Should().NotBeEmpty();
            
            // Validate phases have proper structure
            timeline.Phases.Should().OnlyContain(p => !string.IsNullOrEmpty(p.Name));
            timeline.Phases.Should().OnlyContain(p => p.DurationWeeks > 0);
            timeline.Phases.Should().OnlyContain(p => p.KeyTasks.Count > 0);
        }

        [Fact]
        public async Task BusinessPlanResult_ShouldInclude_TimestampAndVersion()
        {
            // Arrange
            var request = CreateValidBusinessPlanRequest();
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var result = await _businessPlanService.GenerateBusinessPlanAsync(request);

            // Assert
            result.Id.Should().NotBeEmpty("Should have unique ID");
            result.GeneratedAt.Should().BeOnOrAfter(beforeGeneration, "Should timestamp generation");
            result.GeneratedAt.Should().BeOnOrBefore(DateTime.UtcNow.AddSeconds(1), "Timestamp should be recent");
            result.Version.Should().NotBeEmpty("Should have version information");
        }

        [Fact]
        public async Task BusinessPlanService_ShouldHandle_IncompleteInputs_Gracefully()
        {
            // Arrange
            var incompleteRequest = new BusinessPlanRequest
            {
                BusinessIdea = "Vague idea",
                ProblemSolved = "",
                TargetMarket = "",
                RevenueModel = ""
            };

            // Act
            var result = await _businessPlanService.GenerateBusinessPlanAsync(incompleteRequest);

            // Assert
            result.Should().NotBeNull("Should handle incomplete inputs gracefully");
            result.ExecutiveSummary.Should().NotBeEmpty("Should provide basic plan even for incomplete inputs");
            result.Recommendations.Should().NotBeEmpty("Should recommend completing missing sections");
        }

        private BusinessPlanRequest CreateValidBusinessPlanRequest()
        {
            return CreateBusinessPlanRequest(
                "AI-powered meal planning app that creates personalized weekly meal plans based on dietary needs and preferences, with automatic grocery lists and nutritional tracking",
                "Parents spend hours each week planning meals and often resort to unhealthy options due to lack of time and meal planning skills",
                "Working parents with children ages 3-12 who value healthy eating but struggle with meal planning and time management",
                "Monthly subscription at $9.99/month for unlimited meal plans, recipes, and grocery list generation"
            );
        }

        private BusinessPlanRequest CreateBusinessPlanRequest(string businessIdea, string problemSolved, string targetMarket, string revenueModel)
        {
            return new BusinessPlanRequest
            {
                BusinessIdea = businessIdea,
                ProblemSolved = problemSolved,
                TargetMarket = targetMarket,
                RevenueModel = revenueModel,
                CompetitiveAdvantages = "Personalized AI recommendations, family-friendly interface, integration with popular grocery stores",
                InitialInvestment = 50000,
                BusinessType = "Software as a Service",
                SubmittedAt = DateTime.UtcNow
            };
        }

        private BusinessPlanResult CreateMockBusinessPlan(int viabilityScore)
        {
            return new BusinessPlanResult
            {
                Id = Guid.NewGuid().ToString(),
                ViabilityScore = viabilityScore,
                ExecutiveSummary = "Mock business plan for testing",
                MarketAnalysis = new MarketAnalysis
                {
                    MarketSize = "$500M",
                    GrowthRate = "15%",
                    TargetSegments = new List<string> { "Primary segment" }
                },
                FinancialProjections = new FinancialProjections
                {
                    YearlyBreakdown = new List<YearlyFinancials>()
                },
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}