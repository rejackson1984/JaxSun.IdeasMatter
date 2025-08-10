using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Models.Builder;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Services.Mock;
using FluentAssertions;
using Moq;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Services
{
    /// <summary>
    /// Tests for quality gate validation logic that controls hub progression
    /// Quality gates ensure users meet minimum thresholds before advancing to next hub
    /// Hub 1 → Hub 2: 70% completion, Hub 2 → Hub 3: 80% completion
    /// </summary>
    public class QualityGateValidatorTests
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IIdeaValidationService> _mockIdeaValidationService;
        private readonly Mock<IBusinessPlanBuilderService> _mockBusinessPlanService;
        private readonly Mock<IBusinessOperationsService> _mockBusinessOperationsService;
        private readonly QualityGateValidator _qualityGateValidator;

        public QualityGateValidatorTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockIdeaValidationService = new Mock<IIdeaValidationService>();
            _mockBusinessPlanService = new Mock<IBusinessPlanBuilderService>();
            _mockBusinessOperationsService = new Mock<IBusinessOperationsService>();
            
            _qualityGateValidator = new QualityGateValidator(
                _mockHubContextService.Object,
                _mockIdeaValidationService.Object,
                _mockBusinessPlanService.Object,
                _mockBusinessOperationsService.Object
            );
        }

        [Theory]
        [InlineData(0, false, "Initial state should not allow progression")]
        [InlineData(50, false, "50% completion should not allow progression")]
        [InlineData(69, false, "69% completion should not allow progression")]
        [InlineData(70, true, "70% completion should allow progression")]
        [InlineData(85, true, "85% completion should allow progression")]
        [InlineData(100, true, "100% completion should allow progression")]
        public async Task ValidateHub1ToHub2Progression_ShouldRespect_70PercentThreshold(int completionPercentage, bool expectedResult, string description)
        {
            // Arrange
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, completionPercentage);
            var ideaResult = CreateIdeaValidationResult(completionPercentage);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockIdeaValidationService.Setup(x => x.CalculateValidationScoreAsync(It.IsAny<IdeaValidationRequest>())).ReturnsAsync(completionPercentage);
            _mockIdeaValidationService.Setup(x => x.IsReadyForNextHubAsync(It.IsAny<IdeaValidationResult>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _qualityGateValidator.CanProgressToHub2Async("test-user");

            // Assert
            result.Should().Be(expectedResult, description);
        }

        [Theory]
        [InlineData(0, false, "Initial state should not allow progression")]
        [InlineData(70, false, "70% completion should not allow progression")]
        [InlineData(79, false, "79% completion should not allow progression")]
        [InlineData(80, true, "80% completion should allow progression")]
        [InlineData(90, true, "90% completion should allow progression")]
        [InlineData(100, true, "100% completion should allow progression")]
        public async Task ValidateHub2ToHub3Progression_ShouldRespect_80PercentThreshold(int completionPercentage, bool expectedResult, string description)
        {
            // Arrange
            var hubContext = CreateHubContext(BusinessHub.BusinessPlanning, completionPercentage, hub1Unlocked: true, hub2Unlocked: true);
            var businessPlanResult = CreateBusinessPlanResult(completionPercentage);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockBusinessPlanService.Setup(x => x.BuildBusinessPlanAsync(It.IsAny<BusinessPlanBuilderRequest>())).ReturnsAsync(businessPlanResult);

            // Act
            var result = await _qualityGateValidator.CanProgressToHub3Async("test-user");

            // Assert
            result.Should().Be(expectedResult, description);
        }

        [Fact]
        public async Task ValidateQualityGate_ShouldEvaluate_ComprehensiveReadinessCriteria()
        {
            // Arrange - User has completed Hub 1 with 75% score
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 75);
            var ideaResult = CreateIdeaValidationResult(75);
            
            // Mock comprehensive validation criteria
            ideaResult.CategoryScores["MarketViability"] = 8; // Out of 10
            ideaResult.CategoryScores["TechnicalFeasibility"] = 7; // Out of 10
            ideaResult.CategoryScores["CompetitiveAdvantage"] = 6; // Out of 10
            ideaResult.OverallScore = 75;
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockIdeaValidationService.Setup(x => x.CalculateValidationScoreAsync(It.IsAny<IdeaValidationRequest>())).ReturnsAsync(75);
            _mockIdeaValidationService.Setup(x => x.IsReadyForNextHubAsync(ideaResult)).ReturnsAsync(true);

            // Act
            var validationResult = await _qualityGateValidator.ValidateQualityGateAsync("test-user", BusinessHub.BusinessPlanning);

            // Assert
            validationResult.Should().NotBeNull();
            validationResult.CanProgress.Should().BeTrue("User meets 70% threshold for Hub 2");
            validationResult.QualityScore.Should().Be(75);
            validationResult.RequiredThreshold.Should().Be(70);
            validationResult.MissingRequirements.Should().BeEmpty("All requirements are met");
            validationResult.Hub.Should().Be(BusinessHub.BusinessPlanning);
        }

        [Fact]
        public async Task ValidateQualityGate_ShouldIdentify_MissingRequirements()
        {
            // Arrange - User has insufficient completion for Hub 2
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 65);
            var ideaResult = CreateIdeaValidationResult(65);
            
            // Mock incomplete validation criteria
            ideaResult.CategoryScores["MarketViability"] = 5; // Below threshold
            ideaResult.CategoryScores["TechnicalFeasibility"] = 6; // Below threshold
            ideaResult.CategoryScores["CompetitiveAdvantage"] = 4; // Below threshold
            ideaResult.OverallScore = 65;
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockIdeaValidationService.Setup(x => x.CalculateValidationScoreAsync(It.IsAny<IdeaValidationRequest>())).ReturnsAsync(65);
            _mockIdeaValidationService.Setup(x => x.IsReadyForNextHubAsync(ideaResult)).ReturnsAsync(false);

            // Act
            var validationResult = await _qualityGateValidator.ValidateQualityGateAsync("test-user", BusinessHub.BusinessPlanning);

            // Assert
            validationResult.Should().NotBeNull();
            validationResult.CanProgress.Should().BeFalse("User does not meet 70% threshold");
            validationResult.QualityScore.Should().Be(65);
            validationResult.RequiredThreshold.Should().Be(70);
            validationResult.MissingRequirements.Should().NotBeEmpty("Should identify missing requirements");
            validationResult.MissingRequirements.Should().Contain("Market viability assessment incomplete");
            validationResult.MissingRequirements.Should().Contain("Technical feasibility needs improvement");
            validationResult.Hub.Should().Be(BusinessHub.BusinessPlanning);
        }

        [Fact]
        public async Task ValidateBusinessPlanReadiness_ShouldEvaluate_ComprehensiveBusinessPlan()
        {
            // Arrange - User has completed business plan with 85% score
            var hubContext = CreateHubContext(BusinessHub.BusinessPlanning, 85, hub1Unlocked: true, hub2Unlocked: true);
            var businessPlanResult = CreateBusinessPlanResult(85);
            
            // Mock comprehensive business plan
            businessPlanResult.BusinessPlan.ExecutiveSummary = new BuilderExecutiveSummary 
            { 
                BusinessConcept = "Comprehensive business concept with clear value proposition",
                MissionStatement = "Clear mission statement",
                VisionStatement = "Inspiring vision statement"
            };
            businessPlanResult.BusinessPlan.MarketAnalysis = new BuilderMarketAnalysis
            {
                TargetMarket = "Well-defined target market",
                MarketSize = "$10M TAM, $2M SAM",
                CompetitiveAnalysis = "Detailed competitor analysis"
            };
            businessPlanResult.BusinessPlan.FinancialStrategy = new BuilderFinancialStrategy
            {
                RevenueModel = "Scalable revenue model",
                CostStructure = "Competitive cost structure",
                FundingStrategy = "Clear funding requirements"
            };
            businessPlanResult.CompletenessScore = 85;
            businessPlanResult.ReadyForOperations = true;
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockBusinessPlanService.Setup(x => x.BuildBusinessPlanAsync(It.IsAny<BusinessPlanBuilderRequest>())).ReturnsAsync(businessPlanResult);

            // Act
            var validationResult = await _qualityGateValidator.ValidateQualityGateAsync("test-user", BusinessHub.BusinessOperations);

            // Assert
            validationResult.Should().NotBeNull();
            validationResult.CanProgress.Should().BeTrue("User meets 80% threshold for Hub 3");
            validationResult.QualityScore.Should().Be(85);
            validationResult.RequiredThreshold.Should().Be(80);
            validationResult.MissingRequirements.Should().BeEmpty("All requirements are met");
            validationResult.Hub.Should().Be(BusinessHub.BusinessOperations);
            validationResult.Recommendations.Should().NotBeEmpty("Should provide recommendations for operations phase");
        }

        [Fact]
        public async Task ValidateMarketLaunchReadiness_ShouldEvaluate_OperationalReadiness()
        {
            // This test validates the concept of market launch readiness evaluation
            // In a real implementation, this would check multiple operational criteria
            
            // Arrange - User has completed all hubs and is ready for market launch
            var hubContext = CreateHubContext(BusinessHub.BusinessOperations, 90, hub1Unlocked: true, hub2Unlocked: true, hub3Unlocked: true);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act - Check that all hubs are unlocked and current hub is at high completion
            var canLaunch = hubContext.Progress[BusinessHub.BusinessOperations].CompletionPercentage >= 85 &&
                           hubContext.Progress[BusinessHub.BusinessOperations].IsUnlocked;

            // Assert
            canLaunch.Should().BeTrue("User should be ready for market launch with high operational readiness");
            hubContext.Progress.All(p => p.Value.IsUnlocked).Should().BeTrue("All hubs should be unlocked");
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, 70)]
        [InlineData(BusinessHub.BusinessPlanning, 80)]
        [InlineData(BusinessHub.BusinessOperations, 85)]
        public async Task GetQualityGateThreshold_ShouldReturn_CorrectThresholds(BusinessHub targetHub, int expectedThreshold)
        {
            // Act
            var threshold = await _qualityGateValidator.GetQualityGateThresholdAsync(targetHub);

            // Assert
            threshold.Should().Be(expectedThreshold, $"Hub {targetHub} should have {expectedThreshold}% threshold");
        }

        [Fact]
        public async Task ValidatePrerequisites_ShouldEnforce_SequentialProgression()
        {
            // Arrange - User tries to skip to Hub 3 without completing Hub 2
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 100, hub1Unlocked: true, hub2Unlocked: false, hub3Unlocked: false);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var result = await _qualityGateValidator.ValidatePrerequisitesAsync("test-user", BusinessHub.BusinessOperations);

            // Assert
            result.Should().NotBeNull();
            result.CanProgress.Should().BeFalse("Cannot skip Hub 2");
            result.MissingRequirements.Should().Contain("Hub 2 (Business Planning) must be completed first");
        }

        [Fact]
        public async Task GetProgressRecommendations_ShouldProvide_ActionableGuidance()
        {
            // Arrange - User needs guidance to reach Hub 2 threshold
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 60);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockIdeaValidationService.Setup(x => x.CalculateValidationScoreAsync(It.IsAny<IdeaValidationRequest>())).ReturnsAsync(60);

            // Act
            var recommendations = await _qualityGateValidator.GetProgressRecommendationsAsync("test-user", BusinessHub.BusinessPlanning);

            // Assert
            recommendations.Should().NotBeEmpty();
            recommendations.Should().Contain(r => r.Contains("market") && r.Contains("viability"), "Should recommend market research");
            recommendations.Should().Contain(r => r.Contains("competitive") && r.Contains("advantage"), "Should recommend competitive analysis");
            recommendations.Should().HaveCountGreaterThan(2, "Should provide multiple actionable recommendations");
        }

        [Fact]
        public async Task HandleConcurrentValidation_ShouldBe_ThreadSafe()
        {
            // Arrange - Multiple concurrent validation requests
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 75);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockIdeaValidationService.Setup(x => x.CalculateValidationScoreAsync(It.IsAny<IdeaValidationRequest>())).ReturnsAsync(75);

            // Act - Execute multiple validation requests concurrently
            var tasks = Enumerable.Range(0, 10).Select(async i =>
                await _qualityGateValidator.ValidateQualityGateAsync($"test-user-{i}", BusinessHub.BusinessPlanning));

            var results = await Task.WhenAll(tasks);

            // Assert - All results should be consistent
            results.Should().HaveCount(10);
            results.Should().OnlyContain(r => r.CanProgress == true, "All validations should succeed");
            results.Should().OnlyContain(r => r.QualityScore == 75, "All scores should be consistent");
        }

        #region Helper Methods

        private HubContext CreateHubContext(BusinessHub currentHub, int completionPercentage, bool hub1Unlocked = true, bool hub2Unlocked = false, bool hub3Unlocked = false)
        {
            return new HubContext
            {
                CurrentHub = currentHub,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = currentHub == BusinessHub.IdeaDevelopment ? completionPercentage : 100,
                        IsUnlocked = hub1Unlocked,
                        IsActive = currentHub == BusinessHub.IdeaDevelopment
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = currentHub == BusinessHub.BusinessPlanning ? completionPercentage : 0,
                        IsUnlocked = hub2Unlocked,
                        IsActive = currentHub == BusinessHub.BusinessPlanning
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = currentHub == BusinessHub.BusinessOperations ? completionPercentage : 0,
                        IsUnlocked = hub3Unlocked,
                        IsActive = currentHub == BusinessHub.BusinessOperations
                    }
                }
            };
        }

        private IdeaValidationResult CreateIdeaValidationResult(int overallScore)
        {
            return new IdeaValidationResult
            {
                Request = new IdeaValidationRequest
                {
                    Description = "Test business idea",
                    TargetAudience = "Test audience",
                    ProblemSolved = "Test problem"
                },
                OverallScore = overallScore,
                CategoryScores = new Dictionary<string, int>
                {
                    ["MarketViability"] = Math.Max(1, overallScore / 10),
                    ["TechnicalFeasibility"] = Math.Max(1, overallScore / 10),
                    ["CompetitiveAdvantage"] = Math.Max(1, overallScore / 10)
                },
                NextSteps = new List<string> { "Continue development", "Conduct market research" }
            };
        }

        private BusinessPlanBuilderResult CreateBusinessPlanResult(int completenessScore)
        {
            return new BusinessPlanBuilderResult
            {
                Request = new BusinessPlanBuilderRequest
                {
                    Description = "Test business idea",
                    TargetMarket = "Test market",
                    RevenueModel = "Test revenue model"
                },
                BusinessPlan = new BuilderBusinessPlan
                {
                    ExecutiveSummary = new BuilderExecutiveSummary
                    {
                        BusinessConcept = "Test business concept"
                    },
                    MarketAnalysis = new BuilderMarketAnalysis
                    {
                        TargetMarket = "Test market"
                    },
                    FinancialStrategy = new BuilderFinancialStrategy
                    {
                        RevenueModel = "Test revenue model"
                    }
                },
                CompletenessScore = completenessScore,
                ReadyForOperations = completenessScore >= 80
            };
        }

        private BusinessOperationsResult CreateBusinessOperationsResult(int operationalReadinessScore)
        {
            return new BusinessOperationsResult
            {
                Request = new BusinessOperationsRequest
                {
                    BusinessIdea = "Test business idea",
                    TargetMarket = "Test market"
                },
                LaunchPlan = new LaunchPlan
                {
                    LaunchPhases = new List<string> { "Phase 1", "Phase 2", "Phase 3" },
                    TimelineWeeks = 24,
                    CriticalSuccessFactors = new List<string> { "Factor 1", "Factor 2" }
                },
                OperationalReadinessScore = operationalReadinessScore
            };
        }

        #endregion
    }

    /// <summary>
    /// Quality gate validator service for hub progression validation
    /// This would be the actual service implementation being tested
    /// </summary>
    public class QualityGateValidator
    {
        private readonly IHubContextService _hubContextService;
        private readonly IIdeaValidationService _ideaValidationService;
        private readonly IBusinessPlanBuilderService _businessPlanService;
        private readonly IBusinessOperationsService _businessOperationsService;

        public QualityGateValidator(
            IHubContextService hubContextService,
            IIdeaValidationService ideaValidationService,
            IBusinessPlanBuilderService businessPlanService,
            IBusinessOperationsService businessOperationsService)
        {
            _hubContextService = hubContextService;
            _ideaValidationService = ideaValidationService;
            _businessPlanService = businessPlanService;
            _businessOperationsService = businessOperationsService;
        }

        public async Task<bool> CanProgressToHub2Async(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            // In a real implementation, this would calculate completion based on validation scores
            var request = new IdeaValidationRequest { Description = "Test" };
            var score = await _ideaValidationService.CalculateValidationScoreAsync(request);
            return score >= 70;
        }

        public async Task<bool> CanProgressToHub3Async(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            // In a real implementation, this would use business plan completion data
            var request = new BusinessPlanBuilderRequest { Description = "Test" };
            var result = await _businessPlanService.BuildBusinessPlanAsync(request);
            return result.CompletenessScore >= 80;
        }

        public async Task<QualityGateValidationResult> ValidateQualityGateAsync(string userId, BusinessHub targetHub)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var threshold = await GetQualityGateThresholdAsync(targetHub);
            
            int qualityScore;
            var missingRequirements = new List<string>();
            var recommendations = new List<string>();

            switch (targetHub)
            {
                case BusinessHub.BusinessPlanning:
                    var ideaRequest = new IdeaValidationRequest { Description = "Test" };
                    qualityScore = await _ideaValidationService.CalculateValidationScoreAsync(ideaRequest);
                    
                    if (qualityScore < threshold)
                    {
                        missingRequirements.Add("Market viability assessment incomplete");
                        missingRequirements.Add("Technical feasibility needs improvement");
                        missingRequirements.Add("Competitive advantage analysis needed");
                    }
                    
                    break;

                case BusinessHub.BusinessOperations:
                    var planRequest = new BusinessPlanBuilderRequest { Description = "Test" };
                    var planResult = await _businessPlanService.BuildBusinessPlanAsync(planRequest);
                    qualityScore = planResult.CompletenessScore;
                    
                    if (qualityScore < threshold)
                    {
                        missingRequirements.Add("Executive summary required");
                        missingRequirements.Add("Market analysis required");
                        missingRequirements.Add("Financial strategy required");
                    }
                    
                    recommendations.Add("Focus on operational planning and resource management");
                    break;

                default:
                    qualityScore = 0;
                    break;
            }

            return new QualityGateValidationResult
            {
                CanProgress = qualityScore >= threshold && !missingRequirements.Any(),
                QualityScore = qualityScore,
                RequiredThreshold = threshold,
                MissingRequirements = missingRequirements,
                Recommendations = recommendations,
                Hub = targetHub
            };
        }

        public async Task<bool> ValidateMarketLaunchReadinessAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            // Check if all hubs are unlocked and current hub has high completion
            return context.Progress.All(p => p.Value.IsUnlocked) && 
                   context.Progress[BusinessHub.BusinessOperations].CompletionPercentage >= 85;
        }

        public async Task<int> GetQualityGateThresholdAsync(BusinessHub targetHub)
        {
            return targetHub switch
            {
                BusinessHub.BusinessPlanning => 70,
                BusinessHub.BusinessOperations => 80,
                _ => 85
            };
        }

        public async Task<QualityGateValidationResult> ValidatePrerequisitesAsync(string userId, BusinessHub targetHub)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var missingRequirements = new List<string>();

            if (targetHub == BusinessHub.BusinessOperations && !context.Progress[BusinessHub.BusinessPlanning].IsUnlocked)
            {
                missingRequirements.Add("Hub 2 (Business Planning) must be completed first");
            }

            return new QualityGateValidationResult
            {
                CanProgress = !missingRequirements.Any(),
                QualityScore = 0,
                RequiredThreshold = 0,
                MissingRequirements = missingRequirements,
                Recommendations = new List<string>(),
                Hub = targetHub
            };
        }

        public async Task<List<string>> GetProgressRecommendationsAsync(string userId, BusinessHub targetHub)
        {
            var recommendations = new List<string>();

            if (targetHub == BusinessHub.BusinessPlanning)
            {
                var request = new IdeaValidationRequest { Description = "Test" };
                var score = await _ideaValidationService.CalculateValidationScoreAsync(request);
                
                if (score < 70)
                {
                    recommendations.Add("Conduct thorough market research to improve viability assessment");
                    recommendations.Add("Develop technical proof of concept or prototype");
                    recommendations.Add("Strengthen competitive analysis and unique value proposition");
                }
            }

            return recommendations;
        }

    }

    /// <summary>
    /// Result of quality gate validation
    /// </summary>
    public class QualityGateValidationResult
    {
        public bool CanProgress { get; set; }
        public int QualityScore { get; set; }
        public int RequiredThreshold { get; set; }
        public List<string> MissingRequirements { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public BusinessHub Hub { get; set; }
    }

    #region Supporting Models

    public class IdeaValidationRequest
    {
        public string Description { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
        public string ProblemSolved { get; set; } = string.Empty;
    }

    public class IdeaValidationResult
    {
        public IdeaValidationRequest Request { get; set; } = new();
        public int OverallScore { get; set; }
        public Dictionary<string, int> CategoryScores { get; set; } = new();
        public List<string> NextSteps { get; set; } = new();
    }

    public class BusinessPlanBuilderRequest
    {
        public string Description { get; set; } = string.Empty;
        public string TargetMarket { get; set; } = string.Empty;
        public string RevenueModel { get; set; } = string.Empty;
    }

    public class BusinessPlanBuilderResult
    {
        public BusinessPlanBuilderRequest Request { get; set; } = new();
        public BuilderBusinessPlan BusinessPlan { get; set; } = new();
        public int CompletenessScore { get; set; }
        public bool ReadyForOperations { get; set; }
    }

    public class BusinessOperationsRequest
    {
        public string BusinessIdea { get; set; } = string.Empty;
        public string TargetMarket { get; set; } = string.Empty;
    }

    public class BusinessOperationsResult
    {
        public BusinessOperationsRequest Request { get; set; } = new();
        public LaunchPlan LaunchPlan { get; set; } = new();
        public int OperationalReadinessScore { get; set; }
    }

    public class LaunchPlan
    {
        public List<string> LaunchPhases { get; set; } = new();
        public int TimelineWeeks { get; set; }
        public List<string> CriticalSuccessFactors { get; set; } = new();
    }

    #endregion

    #region Service Interfaces

    public interface IIdeaValidationService
    {
        Task<int> CalculateValidationScoreAsync(IdeaValidationRequest request);
        Task<bool> IsReadyForNextHubAsync(IdeaValidationResult result);
    }

    public interface IBusinessPlanBuilderService
    {
        Task<BusinessPlanBuilderResult> BuildBusinessPlanAsync(BusinessPlanBuilderRequest request);
    }

    public interface IBusinessOperationsService
    {
        Task<BusinessOperationsResult> AnalyzeBusinessOperationsAsync(BusinessOperationsRequest request);
    }

    #endregion

}