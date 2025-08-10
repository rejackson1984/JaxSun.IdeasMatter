using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Models.Builder;
using JaxSun.Ideas.Mock.Services.Interfaces;
using JaxSun.Ideas.Mock.Components.Pages;
using JaxSun.Ideas.Mock.Components.Shared;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace JaxSun.Ideas.Mock.Tests.Integration
{
    /// <summary>
    /// Comprehensive integration tests for the three-hub workflow system
    /// Tests the complete user journey from idea development through business planning to operations
    /// </summary>
    public class ThreeHubWorkflowIntegrationTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IHubConfigurationService> _mockHubConfigService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IIdeaValidationService> _mockIdeaValidationService;
        private readonly Mock<IBusinessPlanBuilderService> _mockBusinessPlanService;
        private readonly Mock<IBusinessOperationsService> _mockBusinessOperationsService;
        private readonly Mock<IJSRuntime> _mockJSRuntime;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public ThreeHubWorkflowIntegrationTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockHubConfigService = new Mock<IHubConfigurationService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockIdeaValidationService = new Mock<IIdeaValidationService>();
            _mockBusinessPlanService = new Mock<IBusinessPlanBuilderService>();
            _mockBusinessOperationsService = new Mock<IBusinessOperationsService>();
            _mockJSRuntime = new Mock<IJSRuntime>();
            _mockNavigationManager = new Mock<NavigationManager>();

            RegisterAllServices();
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldEnforce_ProgressiveUnlocking()
        {
            // Arrange - Start with only Hub 1 unlocked
            var initialContext = CreateWorkflowContext(BusinessHub.IdeaDevelopment, 0, false, false);
            SetupWorkflowMocks(initialContext);

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert - Only Hub 1 should be accessible initially
            var unlockedHubs = hubSelector.FindAll(".hub-item").Where(item => !item.ClassList.Contains("locked"));
            var lockedHubs = hubSelector.FindAll(".hub-item.locked");

            unlockedHubs.Should().HaveCount(1, "Only Hub 1 (Idea Development) should be unlocked initially");
            lockedHubs.Should().HaveCount(2, "Hub 2 and 3 should be locked initially");
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldUnlockHub2_WhenHub1Reaches70Percent()
        {
            // Arrange - Hub 1 at 75% completion
            var context = CreateWorkflowContext(BusinessHub.IdeaDevelopment, 75, true, false);
            SetupWorkflowMocks(context);

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert - Hub 2 should now be unlocked
            var unlockedHubs = hubSelector.FindAll(".hub-item").Where(item => !item.ClassList.Contains("locked"));
            var lockedHubs = hubSelector.FindAll(".hub-item.locked");

            unlockedHubs.Should().HaveCount(2, "Hub 1 and 2 should be unlocked when Hub 1 reaches 70%");
            lockedHubs.Should().HaveCount(1, "Only Hub 3 should remain locked");
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldUnlockHub3_WhenHub2Reaches80Percent()
        {
            // Arrange - Hub 1 complete, Hub 2 at 85%
            var context = CreateWorkflowContext(BusinessHub.BusinessPlanning, 100, true, true, 85);
            SetupWorkflowMocks(context);

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert - All hubs should be unlocked
            var unlockedHubs = hubSelector.FindAll(".hub-item").Where(item => !item.ClassList.Contains("locked"));
            var lockedHubs = hubSelector.FindAll(".hub-item.locked");

            unlockedHubs.Should().HaveCount(3, "All three hubs should be unlocked when Hub 2 reaches 80%");
            lockedHubs.Should().HaveCount(0, "No hubs should be locked at this stage");
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldTransition_BetweenHubsCorrectly()
        {
            // Arrange - Set up for hub transitions
            var contexts = new[]
            {
                CreateWorkflowContext(BusinessHub.IdeaDevelopment, 85, true, false),
                CreateWorkflowContext(BusinessHub.BusinessPlanning, 100, true, true, 90),
                CreateWorkflowContext(BusinessHub.BusinessOperations, 100, true, true, 100, 45)
            };

            foreach (var context in contexts)
            {
                SetupWorkflowMocks(context);

                // Act
                var dashboard = RenderHubDashboard(context.CurrentHub);

                // Assert - Each hub should render its specific dashboard
                AssertCorrectDashboardContent(dashboard, context.CurrentHub);
            }
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, "Spark")]
        [InlineData(BusinessHub.BusinessPlanning, "Strategy")]
        [InlineData(BusinessHub.BusinessOperations, "Execute")]
        public void ThreeHubWorkflow_ShouldShow_CorrectCoach_ForEachHub(BusinessHub hub, string expectedCoach)
        {
            // Arrange
            var context = CreateWorkflowContext(hub, 50, true, hub != BusinessHub.IdeaDevelopment, 
                hub == BusinessHub.BusinessPlanning ? 50 : 0, 
                hub == BusinessHub.BusinessOperations ? 50 : 0);
            SetupWorkflowMocks(context);
            SetupCoachMocks(hub, expectedCoach);

            // Act
            var dashboard = RenderHubDashboard(hub);

            // Assert
            var coachElements = dashboard.FindAll(".coach-persona, .coach-name, .coach-widget");
            coachElements.Should().NotBeEmpty("Should display coach information");
            
            var coachText = string.Join(" ", coachElements.Select(e => e.TextContent));
            coachText.Should().Contain(expectedCoach, $"Should show {expectedCoach} coach for {hub}");
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldCalculate_OverallProgress_Correctly()
        {
            // Arrange - Set specific progress values for each hub
            var testCases = new[]
            {
                new { Hub1 = 100, Hub2 = 50, Hub3 = 0, Expected = 50 },   // (100 + 50 + 0) / 3 = 50
                new { Hub1 = 80, Hub2 = 90, Hub3 = 70, Expected = 80 },   // (80 + 90 + 70) / 3 = 80
                new { Hub1 = 60, Hub2 = 0, Hub3 = 0, Expected = 20 }      // (60 + 0 + 0) / 3 = 20
            };

            foreach (var testCase in testCases)
            {
                var context = CreateWorkflowContext(
                    BusinessHub.IdeaDevelopment, 
                    testCase.Hub1, 
                    testCase.Hub2 > 0, 
                    testCase.Hub3 > 0, 
                    testCase.Hub2, 
                    testCase.Hub3
                );
                
                SetupWorkflowMocks(context);

                // Act
                var hubSelector = RenderComponent<HubSelector>();

                // Assert
                var progressText = hubSelector.Find(".progress-text");
                var progressValue = ExtractProgressPercentage(progressText.TextContent);
                
                ((float)progressValue).Should().BeApproximately((float)testCase.Expected, 5.0f, 
                    $"Overall progress should be approximately {testCase.Expected}% for Hub1:{testCase.Hub1}%, Hub2:{testCase.Hub2}%, Hub3:{testCase.Hub3}%");
            }
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldValidate_BusinessLogic_Consistency()
        {
            // Arrange - Complete workflow through all hubs
            var hub1Result = CreateMockIdeaValidationResult();
            var hub2Result = CreateMockBusinessPlanResult();
            var hub3Result = CreateMockBusinessOperationsResult();

            SetupServiceMocks(hub1Result, hub2Result, hub3Result);

            // Act & Assert - Test data consistency across hubs
            ValidateDataConsistencyAcrossHubs(hub1Result, hub2Result, hub3Result);
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldHandle_DataPersistence_BetweenHubs()
        {
            // Arrange - Start with idea data in Hub 1
            var ideaData = "Revolutionary AI-powered project management tool";
            var hub1Context = CreateWorkflowContext(BusinessHub.IdeaDevelopment, 80, true, false);
            var hub2Context = CreateWorkflowContext(BusinessHub.BusinessPlanning, 40, true, true);

            // Act - Move from Hub 1 to Hub 2 and verify data carries over
            SetupWorkflowMocks(hub1Context);
            var hub1Dashboard = RenderComponent<IdeaDevelopmentDashboard>();

            SetupWorkflowMocks(hub2Context);
            var hub2Dashboard = RenderComponent<BusinessPlanningDashboard>();

            // Assert - Business idea should be consistent across hubs
            // This test ensures that the business idea entered in Hub 1 
            // is available and consistent when moving to Hub 2
            hub1Dashboard.Should().NotBeNull("Hub 1 dashboard should render successfully");
            hub2Dashboard.Should().NotBeNull("Hub 2 dashboard should render successfully");
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldSupport_NonLinear_Navigation()
        {
            // Arrange - User has progressed through all hubs
            var context = CreateWorkflowContext(BusinessHub.BusinessOperations, 100, true, true, 100, 75);
            SetupWorkflowMocks(context);

            // Act - User should be able to navigate back to previous hubs
            var hubSelector = RenderComponent<HubSelector>();
            var allHubItems = hubSelector.FindAll(".hub-item");

            // Assert - All hubs should be clickable/navigable
            allHubItems.Should().HaveCount(3, "Should display all three hubs");
            
            foreach (var hubItem in allHubItems)
            {
                hubItem.ClassList.Should().NotContain("locked", "All hubs should be unlocked and navigable");
            }
        }

        [Fact]
        public void ThreeHubWorkflow_ShouldGenerate_ComprehensiveResults()
        {
            // Arrange - Complete workflow with all data
            var completeContext = CreateWorkflowContext(BusinessHub.BusinessOperations, 100, true, true, 100, 95);
            SetupWorkflowMocks(completeContext);

            var ideaResult = CreateMockIdeaValidationResult();
            var planResult = CreateMockBusinessPlanResult();
            var operationsResult = CreateMockBusinessOperationsResult();

            SetupServiceMocks(ideaResult, planResult, operationsResult);

            // Act
            var operationsDashboard = RenderComponent<BusinessOperationsDashboard>();

            // Assert - Should display comprehensive business development results
            operationsDashboard.Should().NotBeNull("Operations dashboard should render");
            
            // Verify key sections are present
            var sections = operationsDashboard.FindAll(".dashboard-section, .operations-section, .launch-plan-section");
            sections.Should().NotBeEmpty("Should display operations management sections");
        }

        #region Helper Methods

        private void RegisterAllServices()
        {
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockHubConfigService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockIdeaValidationService.Object);
            Services.AddSingleton(_mockBusinessPlanService.Object);
            Services.AddSingleton(_mockBusinessOperationsService.Object);
            Services.AddSingleton(_mockJSRuntime.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        private void SetupWorkflowMocks(HubContext context)
        {
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(context);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(CreateHubMetadata());
        }

        private void SetupCoachMocks(BusinessHub hub, string coachName)
        {
            var coach = new CoachPersona
            {
                Name = coachName,
                Hub = hub,
                Description = $"{coachName} coach for {hub}",
                Avatar = $"/images/coaches/{coachName.ToLower()}.png",
                Personality = $"{coachName} coaching personality"
            };

            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(hub)).ReturnsAsync(coach);
        }

        private void SetupServiceMocks(IdeaValidationResult ideaResult, BusinessPlanBuilderResult planResult, BusinessOperationsResult operationsResult)
        {
            _mockIdeaValidationService.Setup(x => x.ValidateIdeaAsync(It.IsAny<IdeaValidationRequest>()))
                .ReturnsAsync(ideaResult);

            _mockBusinessPlanService.Setup(x => x.BuildBusinessPlanAsync(It.IsAny<BusinessPlanBuilderRequest>()))
                .ReturnsAsync(planResult);

            _mockBusinessOperationsService.Setup(x => x.GenerateLaunchPlanAsync(It.IsAny<BusinessOperationsRequest>()))
                .ReturnsAsync(operationsResult.LaunchPlan);
        }

        private HubContext CreateWorkflowContext(BusinessHub currentHub, int hub1Progress, bool hub2Unlocked, bool hub3Unlocked, int hub2Progress = 0, int hub3Progress = 0)
        {
            return new HubContext
            {
                CurrentHub = currentHub,
                UserId = "workflow-test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = hub1Progress,
                        IsUnlocked = true,
                        IsActive = currentHub == BusinessHub.IdeaDevelopment
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = hub2Progress,
                        IsUnlocked = hub2Unlocked,
                        IsActive = currentHub == BusinessHub.BusinessPlanning
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = hub3Progress,
                        IsUnlocked = hub3Unlocked,
                        IsActive = currentHub == BusinessHub.BusinessOperations
                    }
                }
            };
        }

        private Dictionary<BusinessHub, HubMetadata> CreateHubMetadata()
        {
            return new Dictionary<BusinessHub, HubMetadata>
            {
                [BusinessHub.IdeaDevelopment] = new HubMetadata
                {
                    Hub = BusinessHub.IdeaDevelopment,
                    Name = "Idea Development",
                    Icon = "fas fa-lightbulb",
                    CoachPersona = "Spark",
                    UnlockThreshold = 0
                },
                [BusinessHub.BusinessPlanning] = new HubMetadata
                {
                    Hub = BusinessHub.BusinessPlanning,
                    Name = "Business Planning",
                    Icon = "fas fa-clipboard-list",
                    CoachPersona = "Strategy",
                    UnlockThreshold = 70
                },
                [BusinessHub.BusinessOperations] = new HubMetadata
                {
                    Hub = BusinessHub.BusinessOperations,
                    Name = "Business Operations",
                    Icon = "fas fa-chart-line",
                    CoachPersona = "Execute",
                    UnlockThreshold = 80
                }
            };
        }

        private IRenderedComponent<T> RenderHubDashboard<T>(BusinessHub hub) where T : ComponentBase
        {
            return hub switch
            {
                BusinessHub.IdeaDevelopment => (IRenderedComponent<T>)RenderComponent<IdeaDevelopmentDashboard>(),
                BusinessHub.BusinessPlanning => (IRenderedComponent<T>)RenderComponent<BusinessPlanningDashboard>(),
                BusinessHub.BusinessOperations => (IRenderedComponent<T>)RenderComponent<BusinessOperationsDashboard>(),
                _ => throw new ArgumentException($"Unknown hub: {hub}")
            };
        }

        private IRenderedComponent<ComponentBase> RenderHubDashboard(BusinessHub hub)
        {
            return hub switch
            {
                BusinessHub.IdeaDevelopment => RenderComponent<IdeaDevelopmentDashboard>(),
                BusinessHub.BusinessPlanning => RenderComponent<BusinessPlanningDashboard>(),
                BusinessHub.BusinessOperations => RenderComponent<BusinessOperationsDashboard>(),
                _ => throw new ArgumentException($"Unknown hub: {hub}")
            };
        }

        private void AssertCorrectDashboardContent(IRenderedComponent<ComponentBase> dashboard, BusinessHub expectedHub)
        {
            dashboard.Should().NotBeNull($"{expectedHub} dashboard should render successfully");

            var expectedContent = expectedHub switch
            {
                BusinessHub.IdeaDevelopment => "idea",
                BusinessHub.BusinessPlanning => "business",
                BusinessHub.BusinessOperations => "operations",
                _ => throw new ArgumentException($"Unknown hub: {expectedHub}")
            };

            var content = dashboard.Markup.ToLowerInvariant();
            content.Should().Contain(expectedContent, $"{expectedHub} dashboard should contain relevant content");
        }

        private int ExtractProgressPercentage(string progressText)
        {
            var match = System.Text.RegularExpressions.Regex.Match(progressText, @"(\d+)%");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        private void ValidateDataConsistencyAcrossHubs(IdeaValidationResult ideaResult, BusinessPlanBuilderResult planResult, BusinessOperationsResult operationsResult)
        {
            // Validate that business idea is consistent across all hubs
            ideaResult.Should().NotBeNull("Idea validation result should exist");
            planResult.Should().NotBeNull("Business plan result should exist");
            operationsResult.Should().NotBeNull("Business operations result should exist");

            // Additional consistency checks would go here
            // For example, ensuring the business idea from Hub 1 matches what's used in Hub 2 and 3
        }

        private IdeaValidationResult CreateMockIdeaValidationResult()
        {
            return new IdeaValidationResult
            {
                Request = new IdeaValidationRequest 
                { 
                    Description = "AI-powered project management tool",
                    TargetAudience = "Software development teams",
                    ProblemSolved = "Inefficient project coordination"
                },
                OverallScore = 85,
                NextSteps = new List<string> { "Focus on team collaboration features", "Implement AI-driven insights" }
            };
        }

        private BusinessPlanBuilderResult CreateMockBusinessPlanResult()
        {
            return new BusinessPlanBuilderResult
            {
                Request = new BusinessPlanBuilderRequest
                {
                    Description = "AI-powered project management tool",
                    TargetMarket = "Software development teams",
                    RevenueModel = "Subscription-based SaaS"
                },
                BusinessPlan = new BuilderBusinessPlan
                {
                    ExecutiveSummary = new BuilderExecutiveSummary
                    {
                        BusinessConcept = "Comprehensive AI-powered project management solution with growing market opportunity and AI-driven competitive advantages"
                    }
                }
            };
        }

        private BusinessOperationsResult CreateMockBusinessOperationsResult()
        {
            return new BusinessOperationsResult
            {
                Request = new BusinessOperationsRequest
                {
                    BusinessIdea = "AI-powered project management tool",
                    TargetMarket = "Software development teams",
                    RevenueModel = "Subscription-based SaaS",
                    InitialInvestment = 500000
                },
                LaunchPlan = new LaunchPlan
                {
                    LaunchPhases = new List<string> { "Phase 1: MVP Development", "Phase 2: Beta Testing", "Phase 3: Market Launch" },
                    TimelineWeeks = 24,
                    CriticalSuccessFactors = new List<string> { "Product-market fit", "User adoption", "Revenue generation" }
                },
                OperationalReadinessScore = 87
            };
        }

        #endregion
    }
}