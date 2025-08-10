using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Models.Builder;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Services.Mock;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Integration
{
    /// <summary>
    /// Integration tests for service layer interactions and data flow
    /// Tests the complete service chain from idea validation through business planning to operations
    /// </summary>
    public class ServiceIntegrationTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IIdeaValidationService _ideaValidationService;
        private readonly IBusinessPlanBuilderService _businessPlanService;
        private readonly IBusinessOperationsService _businessOperationsService;

        public ServiceIntegrationTests()
        {
            var services = new ServiceCollection();
            RegisterTestServices(services);
            _serviceProvider = services.BuildServiceProvider();

            _ideaValidationService = _serviceProvider.GetRequiredService<IIdeaValidationService>();
            _businessPlanService = _serviceProvider.GetRequiredService<IBusinessPlanBuilderService>();
            _businessOperationsService = _serviceProvider.GetRequiredService<IBusinessOperationsService>();
        }

        [Fact]
        public async Task ServiceIntegration_ShouldComplete_FullWorkflow_FromIdeaToOperations()
        {
            // Arrange - Create a complete business idea
            var businessIdea = "AI-powered personal finance management app that provides intelligent spending insights and automated savings recommendations";
            
            // Act - Step 1: Validate the idea
            var ideaRequest = new IdeaValidationRequest
            {
                Description = businessIdea,
                TargetAudience = "Millennials and Gen Z consumers aged 22-40",
                ProblemSolved = "People struggle to manage their finances effectively and save money consistently",
                RevenueModel = "Freemium with premium subscription tiers"
            };

            var ideaResult = await _ideaValidationService.ValidateIdeaAsync(ideaRequest);

            // Act - Step 2: Build business plan based on validated idea
            var planRequest = new BusinessPlanBuilderRequest
            {
                Description = businessIdea,
                TargetMarket = ideaRequest.TargetAudience,
                RevenueModel = "Freemium with premium subscription tiers"
            };

            var planResult = await _businessPlanService.BuildBusinessPlanAsync(planRequest);

            // Act - Step 3: Generate operations plan based on business plan
            var operationsRequest = new BusinessOperationsRequest
            {
                BusinessIdea = businessIdea,
                BusinessPlan = planResult.BusinessPlan.ExecutiveSummary.BusinessConcept,
                TargetMarket = planRequest.TargetMarket,
                RevenueModel = planRequest.RevenueModel,
                InitialInvestment = 750000,
                BusinessType = "Mobile App/Fintech",
                LaunchTimeline = "18 months to market",
                TargetCustomers = 50000
            };

            var operationsResult = await _businessOperationsService.GenerateLaunchPlanAsync(operationsRequest);

            // Assert - Validate end-to-end workflow
            ValidateIdeaValidationResult(ideaResult, businessIdea);
            ValidateBusinessPlanResult(planResult, businessIdea);
            ValidateOperationsResult(operationsResult);
            ValidateWorkflowConsistency(ideaResult, planResult, operationsResult);
        }

        [Theory]
        [InlineData("E-commerce platform for sustainable products", "Eco-conscious consumers", 500000)]
        [InlineData("Remote team collaboration tool with VR integration", "Distributed software teams", 1000000)]
        [InlineData("AI-powered language learning app for professionals", "Working professionals", 300000)]
        public async Task ServiceIntegration_ShouldHandle_DifferentBusinessTypes(string businessIdea, string targetAudience, decimal investment)
        {
            // Arrange
            var ideaRequest = new IdeaValidationRequest
            {
                Description = businessIdea,
                TargetAudience = targetAudience,
                ProblemSolved = "Market gap identified for this solution",
                RevenueModel = "Subscription-based"
            };

            // Act - Run through complete workflow
            var ideaResult = await _ideaValidationService.ValidateIdeaAsync(ideaRequest);
            
            var planRequest = new BusinessPlanBuilderRequest
            {
                Description = businessIdea,
                TargetMarket = targetAudience,
                RevenueModel = "Subscription-based"
            };
            
            var planResult = await _businessPlanService.BuildBusinessPlanAsync(planRequest);
            
            var operationsRequest = new BusinessOperationsRequest
            {
                BusinessIdea = businessIdea,
                TargetMarket = targetAudience,
                InitialInvestment = investment,
                BusinessType = "Technology"
            };
            
            var operationsResult = await _businessOperationsService.GenerateLaunchPlanAsync(operationsRequest);

            // Assert - All services should handle different business types
            ideaResult.Should().NotBeNull($"Idea validation should work for {businessIdea}");
            ideaResult.OverallScore.Should().BeGreaterThan(0, "Should generate meaningful validation score");
            
            planResult.Should().NotBeNull($"Business planning should work for {businessIdea}");
            planResult.BusinessPlan.ExecutiveSummary.Should().NotBeNull("Should generate executive summary");
            
            operationsResult.Should().NotBeNull($"Operations planning should work for {businessIdea}");
            operationsResult.LaunchPhases.Should().NotBeEmpty("Should generate launch phases");
        }

        [Fact]
        public async Task ServiceIntegration_ShouldCalculate_ConsistentScores_AcrossServices()
        {
            // Arrange - Create test scenarios with expected score ranges
            var testScenarios = new[]
            {
                new { Idea = "Revolutionary blockchain-based social media platform", ExpectedRange = (60, 90) },
                new { Idea = "Simple mobile app for tracking daily water intake", ExpectedRange = (40, 70) },
                new { Idea = "AI-powered autonomous vehicle navigation system", ExpectedRange = (70, 95) }
            };

            foreach (var scenario in testScenarios)
            {
                // Act
                var ideaRequest = new IdeaValidationRequest { Description = scenario.Idea };
                var ideaResult = await _ideaValidationService.ValidateIdeaAsync(ideaRequest);

                var planRequest = new BusinessPlanBuilderRequest { Description = scenario.Idea };
                var planResult = await _businessPlanService.BuildBusinessPlanAsync(planRequest);

                var operationsRequest = new BusinessOperationsRequest { BusinessIdea = scenario.Idea };
                var operationsResult = await _businessOperationsService.CalculateOperationalReadinessScoreAsync(
                    new BusinessOperationsResult { LaunchPlan = await _businessOperationsService.GenerateLaunchPlanAsync(operationsRequest) });

                // Assert - Scores should be within expected ranges and reasonably consistent
                ideaResult.OverallScore.Should().BeInRange(scenario.ExpectedRange.Item1, scenario.ExpectedRange.Item2,
                    $"Idea validation score for '{scenario.Idea}' should be in expected range");

                planResult.CompletenessScore.Should().BeInRange(scenario.ExpectedRange.Item1, scenario.ExpectedRange.Item2,
                    $"Business plan completeness score for '{scenario.Idea}' should be in expected range");

                operationsResult.Should().BeInRange(scenario.ExpectedRange.Item1, scenario.ExpectedRange.Item2,
                    $"Operations readiness score for '{scenario.Idea}' should be in expected range");
            }
        }

        [Fact]
        public async Task ServiceIntegration_ShouldGenerate_ComprehensiveData_ForAllComponents()
        {
            // Arrange
            var comprehensiveRequest = CreateComprehensiveTestRequest();

            // Act - Generate all data components
            var ideaResult = await _ideaValidationService.ValidateIdeaAsync(comprehensiveRequest.IdeaRequest);
            var planResult = await _businessPlanService.BuildBusinessPlanAsync(comprehensiveRequest.PlanRequest);
            
            var launchPlan = await _businessOperationsService.GenerateLaunchPlanAsync(comprehensiveRequest.OperationsRequest);
            var resourcePlan = await _businessOperationsService.GenerateResourcePlanAsync(comprehensiveRequest.OperationsRequest);
            var performanceFramework = await _businessOperationsService.GeneratePerformanceFrameworkAsync(comprehensiveRequest.OperationsRequest);
            var scalingStrategy = await _businessOperationsService.GenerateScalingStrategyAsync(comprehensiveRequest.OperationsRequest);
            var qualityFramework = await _businessOperationsService.GenerateQualityFrameworkAsync(comprehensiveRequest.OperationsRequest);
            var businessIntelligence = await _businessOperationsService.GenerateBusinessIntelligenceAsync(comprehensiveRequest.OperationsRequest);

            // Assert - All components should be comprehensive and interconnected
            ValidateComprehensiveIdeaData(ideaResult);
            ValidateComprehensiveBusinessPlanData(planResult);
            ValidateComprehensiveOperationsData(launchPlan, resourcePlan, performanceFramework, scalingStrategy, qualityFramework, businessIntelligence);
        }

        [Fact]
        public async Task ServiceIntegration_ShouldSupport_ProgressionLogic()
        {
            // Arrange - Test progression thresholds
            var businessIdea = "Cloud-based inventory management system for small retailers";

            // Act - Test Hub 1 completion triggers Hub 2 unlock
            var ideaResult = await _ideaValidationService.ValidateIdeaAsync(new IdeaValidationRequest { Description = businessIdea });
            var hub1ReadyForProgression = await _ideaValidationService.IsReadyForNextHubAsync(ideaResult);

            // Act - Test Hub 2 completion triggers Hub 3 unlock  
            var planResult = await _businessPlanService.BuildBusinessPlanAsync(new BusinessPlanBuilderRequest { Description = businessIdea });
            var hub2ReadyForProgression = planResult.ReadyForOperations;

            // Act - Test Hub 3 readiness for market launch
            var operationsResult = new BusinessOperationsResult
            {
                LaunchPlan = await _businessOperationsService.GenerateLaunchPlanAsync(new BusinessOperationsRequest { BusinessIdea = businessIdea })
            };
            var readyForLaunch = await _businessOperationsService.IsReadyForMarketLaunchAsync(operationsResult);

            // Assert - Progression logic should work correctly
            hub1ReadyForProgression.Should().BeTrue("Hub 1 should be ready for progression with complete idea validation");
            hub2ReadyForProgression.Should().BeTrue("Hub 2 should be ready for progression with complete business plan");
            
            // Market launch readiness depends on operations score
            var operationsScore = await _businessOperationsService.CalculateOperationalReadinessScoreAsync(operationsResult);
            if (operationsScore >= 80)
            {
                readyForLaunch.Should().BeTrue("Should be ready for market launch with high operational readiness");
            }
            else
            {
                readyForLaunch.Should().BeFalse("Should not be ready for market launch with low operational readiness");
            }
        }

        [Fact]
        public async Task ServiceIntegration_ShouldHandle_ServiceErrors_Gracefully()
        {
            // Arrange - Create invalid/edge case requests
            var invalidRequests = new[]
            {
                new IdeaValidationRequest { Description = "" }, // Empty idea
                new IdeaValidationRequest { Description = "a" }, // Too short
                new IdeaValidationRequest { Description = new string('x', 10000) } // Too long
            };

            foreach (var invalidRequest in invalidRequests)
            {
                // Act & Assert - Should handle gracefully without throwing
                var exception = await Record.ExceptionAsync(async () =>
                {
                    var result = await _ideaValidationService.ValidateIdeaAsync(invalidRequest);
                    result.Should().NotBeNull("Should return a result even for invalid input");
                });

                exception.Should().BeNull($"Should handle invalid request gracefully: {invalidRequest.Description}");
            }
        }

        [Fact]
        public async Task ServiceIntegration_ShouldProvide_Templates_AndCompliance()
        {
            // Act - Test template and compliance services
            var operationsTemplates = await _businessOperationsService.GetOperationsTemplatesAsync();
            var complianceRequirements = await _businessOperationsService.GetComplianceRequirementsAsync(
                new BusinessOperationsRequest { BusinessType = "Technology", TargetMarket = "B2B Software" });
            var operationsChecklist = await _businessOperationsService.GenerateOperationsChecklistAsync(
                new BusinessOperationsRequest { BusinessIdea = "Test business idea" });

            // Assert - Should provide comprehensive templates and compliance guidance
            operationsTemplates.Should().NotBeEmpty("Should provide operations templates");
            operationsTemplates.Should().HaveCountGreaterThan(2, "Should have multiple template options");

            complianceRequirements.Should().NotBeEmpty("Should provide compliance requirements");
            complianceRequirements.Should().Contain(req => req.Category == "Legal", "Should include legal requirements");

            operationsChecklist.Should().NotBeNull("Should generate operations checklist");
            operationsChecklist.PreLaunchTasks.Should().NotBeEmpty("Should include pre-launch tasks");
            operationsChecklist.LaunchTasks.Should().NotBeEmpty("Should include launch tasks");
            operationsChecklist.PostLaunchTasks.Should().NotBeEmpty("Should include post-launch tasks");
        }

        #region Helper Methods

        private void RegisterTestServices(IServiceCollection services)
        {
            services.AddScoped<IIdeaValidationService, MockIdeaValidationService>();
            services.AddScoped<IBusinessPlanBuilderService, MockBusinessPlanBuilderService>();
            services.AddScoped<IBusinessOperationsService, MockBusinessOperationsService>();
        }

        private void ValidateIdeaValidationResult(IdeaValidationResult result, string expectedBusinessIdea)
        {
            result.Should().NotBeNull("Idea validation result should not be null");
            result.Request.Description.Should().Be(expectedBusinessIdea, "Business idea should be preserved");
            result.OverallScore.Should().BeInRange(0, 100, "Overall score should be valid percentage");
            result.NextSteps.Should().NotBeEmpty("Should provide next steps");
        }

        private void ValidateBusinessPlanResult(BusinessPlanBuilderResult result, string expectedBusinessIdea)
        {
            result.Should().NotBeNull("Business plan result should not be null");
            result.Request.Description.Should().Be(expectedBusinessIdea, "Business idea should be preserved");
            result.BusinessPlan.ExecutiveSummary.Should().NotBeNull("Should generate executive summary");
            result.BusinessPlan.ExecutiveSummary.BusinessConcept.Should().NotBeEmpty("Should provide business concept");
        }

        private void ValidateOperationsResult(LaunchPlan result)
        {
            result.Should().NotBeNull("Launch plan should not be null");
            result.LaunchPhases.Should().NotBeEmpty("Should provide launch phases");
            result.TimelineWeeks.Should().BeGreaterThan(0, "Should provide realistic timeline");
            result.CriticalSuccessFactors.Should().NotBeEmpty("Should identify success factors");
        }

        private void ValidateWorkflowConsistency(IdeaValidationResult ideaResult, BusinessPlanBuilderResult planResult, LaunchPlan operationsResult)
        {
            // Validate that core business concept is consistent across all stages
            var businessIdea = ideaResult.Request.Description;
            
            planResult.Request.Description.Should().Be(businessIdea, "Business idea should be consistent from idea to plan");
            
            // Additional consistency validations would go here
            // For example, target market, revenue model, etc. should align across stages
        }

        private (IdeaValidationRequest IdeaRequest, BusinessPlanBuilderRequest PlanRequest, BusinessOperationsRequest OperationsRequest) CreateComprehensiveTestRequest()
        {
            var businessIdea = "Comprehensive AI-powered business intelligence platform for SMEs";
            
            return (
                IdeaRequest: new IdeaValidationRequest
                {
                    Description = businessIdea,
                    TargetAudience = "Small and medium enterprises seeking data-driven insights",
                    ProblemSolved = "SMEs lack affordable, comprehensive business intelligence tools",
                    RevenueModel = "Tiered SaaS subscription model"
                },
                PlanRequest: new BusinessPlanBuilderRequest
                {
                    Description = businessIdea,
                    TargetMarket = "SMEs with 10-500 employees",
                    RevenueModel = "Tiered SaaS subscription model"
                },
                OperationsRequest: new BusinessOperationsRequest
                {
                    BusinessIdea = businessIdea,
                    TargetMarket = "SMEs with 10-500 employees",
                    RevenueModel = "Tiered SaaS subscription model",
                    InitialInvestment = 2000000,
                    BusinessType = "B2B Software",
                    LaunchTimeline = "24 months",
                    TargetCustomers = 100000
                }
            );
        }

        private void ValidateComprehensiveIdeaData(IdeaValidationResult result)
        {
            result.Should().NotBeNull();
            result.OverallScore.Should().BeGreaterThan(0);
            result.NextSteps.Should().NotBeEmpty();
            result.MarketOpportunity.Should().NotBeNull();
            result.Competition.Should().NotBeNull();
        }

        private void ValidateComprehensiveBusinessPlanData(BusinessPlanBuilderResult result)
        {
            result.Should().NotBeNull();
            result.BusinessPlan.ExecutiveSummary.Should().NotBeNull();
            result.BusinessPlan.MarketAnalysis.Should().NotBeNull();
            result.BusinessPlan.FinancialStrategy.Should().NotBeNull();
            result.CompletenessScore.Should().BeGreaterThan(0);
        }

        private void ValidateComprehensiveOperationsData(LaunchPlan launchPlan, ResourcePlan resourcePlan, 
            PerformanceFramework performanceFramework, ScalingStrategy scalingStrategy, 
            QualityFramework qualityFramework, BusinessIntelligence businessIntelligence)
        {
            launchPlan.Should().NotBeNull();
            launchPlan.LaunchPhases.Should().NotBeEmpty();

            resourcePlan.Should().NotBeNull();
            resourcePlan.HumanResources.Should().NotBeNull();
            resourcePlan.TechnologyResources.Should().NotBeNull();
            resourcePlan.FinancialResources.Should().NotBeNull();

            performanceFramework.Should().NotBeNull();
            performanceFramework.KPIs.Should().NotBeEmpty();

            scalingStrategy.Should().NotBeNull();
            scalingStrategy.ScalingPhases.Should().NotBeEmpty();

            qualityFramework.Should().NotBeNull();
            qualityFramework.QualityStandards.Should().NotBeEmpty();

            businessIntelligence.Should().NotBeNull();
            businessIntelligence.DataSources.Should().NotBeEmpty();
        }

        #endregion
    }
}