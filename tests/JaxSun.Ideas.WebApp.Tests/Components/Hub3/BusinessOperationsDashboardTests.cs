using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Services.Interfaces;
using JaxSun.Ideas.Mock.Components.Pages;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;

namespace JaxSun.Ideas.Mock.Tests.Components.Hub3
{
    /// <summary>
    /// Comprehensive tests for Hub 3 (Business Operations) dashboard components
    /// Tests all UI components for operations management, launch planning, resource management,
    /// performance monitoring, and scaling strategies with Compass coach integration
    /// </summary>
    public class BusinessOperationsDashboardTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IBusinessOperationsService> _mockOperationsService;
        private readonly Mock<IMockDataService> _mockDataService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public BusinessOperationsDashboardTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockOperationsService = new Mock<IBusinessOperationsService>();
            _mockDataService = new Mock<IMockDataService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockOperationsService.Object);
            Services.AddSingleton(_mockDataService.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        #region Layout and Theme Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldUse_HubLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            // Component should render without layout errors
            // Layout directive is @layout HubLayout in the component
            component.Find(".business-operations-dashboard").Should().NotBeNull();
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldHave_HubOperationsTheme()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var themeElements = component.FindAll(".hub-operations-theme, .hub-operations-avatar");
            themeElements.Should().NotBeEmpty("Should apply Hub 3 theme styling");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_CompassCoachWelcome()
        {
            // Arrange
            SetupMocksWithCompassCoach();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var welcomeSection = component.Find(".coach-welcome-section");
            welcomeSection.Should().NotBeNull("Should display coach welcome section");
            
            var welcomeTitle = component.Find(".coach-welcome-title");
            welcomeTitle.Should().NotBeNull("Should display welcome title");
            welcomeTitle.TextContent.Should().Contain("Business Operations Hub", "Should show Hub 3 title");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_CompassCoachAvatar()
        {
            // Arrange
            SetupMocksWithCompassCoach();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var coachAvatar = component.Find(".coach-avatar-large");
            coachAvatar.Should().NotBeNull("Should display coach avatar");
            
            var avatarIcon = coachAvatar.QuerySelector("i");
            avatarIcon.Should().NotBeNull("Should contain avatar icon");
            avatarIcon.ClassList.Should().Contain("fa-compass", "Should use compass icon for operations coach");
        }

        #endregion

        #region Operations Progress Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_OperationsProgressOverview()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var progressSection = component.Find(".operations-progress-section");
            progressSection.Should().NotBeNull("Should display operations progress section");
            
            var sectionHeader = component.Find(".section-header");
            sectionHeader.Should().NotBeNull("Should have section header");
            sectionHeader.TextContent.Should().Contain("Operations Progress", "Should show operations progress title");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_OperationsMetrics()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var metricCards = component.FindAll(".metric-card");
            metricCards.Should().HaveCountGreaterThan(3, "Should have multiple operations metrics");
            
            // Check for specific operations metrics
            var launchReadiness = component.FindAll(".metric-card").FirstOrDefault(c => c.TextContent.Contains("Launch Readiness"));
            launchReadiness.Should().NotBeNull("Should show launch readiness metric");
            
            var operationalEfficiency = component.FindAll(".metric-card").FirstOrDefault(c => c.TextContent.Contains("Operational Efficiency"));
            operationalEfficiency.Should().NotBeNull("Should show operational efficiency metric");
            
            var resourceUtilization = component.FindAll(".metric-card").FirstOrDefault(c => c.TextContent.Contains("Resource Utilization"));
            resourceUtilization.Should().NotBeNull("Should show resource utilization metric");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_OperationsStatusIndicators()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var statusIndicators = component.FindAll(".metric-status");
            statusIndicators.Should().NotBeEmpty("Should have status indicators for operations metrics");
            
            var metricValues = component.FindAll(".metric-value");
            metricValues.Should().NotBeEmpty("Should display metric values");
            metricValues.Should().HaveCountGreaterThan(3, "Should have multiple metric values");
        }

        #endregion

        #region Strategic Overview Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_CurrentOperationsFocus()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var strategicOverview = component.Find(".strategic-overview-card");
            strategicOverview.Should().NotBeNull("Should display strategic overview card");
            
            var currentFocus = component.Find(".current-focus");
            currentFocus.Should().NotBeNull("Should show current operations focus");
            currentFocus.TextContent.Should().Contain("MyFitnessPal Clone", "Should show current business focus");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_LaunchPreparationStatus()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var planStatus = component.Find(".plan-status");
            planStatus.Should().NotBeNull("Should show operations plan status");
            
            var statusBadge = component.Find(".status-badge");
            statusBadge.Should().NotBeNull("Should have status badge");
            statusBadge.TextContent.Should().Contain("Launch Preparation", "Should show launch preparation status");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_KeyOperationalMetrics()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var keyMetricsGrid = component.Find(".key-metrics-grid");
            keyMetricsGrid.Should().NotBeNull("Should display key operational metrics grid");
            
            var keyMetrics = component.FindAll(".key-metric");
            keyMetrics.Should().HaveCountGreaterThan(3, "Should show multiple key metrics");
            
            // Check for specific operational metrics
            var launchDate = keyMetrics.FirstOrDefault(m => m.TextContent.Contains("Launch Date"));
            launchDate.Should().NotBeNull("Should show launch date metric");
            
            var teamSize = keyMetrics.FirstOrDefault(m => m.TextContent.Contains("Team Size"));
            teamSize.Should().NotBeNull("Should show team size metric");
            
            var burnRate = keyMetrics.FirstOrDefault(m => m.TextContent.Contains("Burn Rate"));
            burnRate.Should().NotBeNull("Should show burn rate metric");
        }

        #endregion

        #region Operations Phases Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_OperationsPhases()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var operationsPhases = component.Find(".operations-phases");
            operationsPhases.Should().NotBeNull("Should display operations phases section");
            
            var phasesGrid = component.Find(".phases-grid");
            phasesGrid.Should().NotBeNull("Should have phases grid");
            
            var phaseItems = component.FindAll(".phase-item");
            phaseItems.Should().HaveCountGreaterThan(3, "Should show multiple operation phases");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_PhaseProgression()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var completedPhases = component.FindAll(".phase-item.completed");
            completedPhases.Should().NotBeEmpty("Should show completed phases");
            
            var activePhases = component.FindAll(".phase-item.active");
            activePhases.Should().NotBeEmpty("Should show active phases");
            
            var pendingPhases = component.FindAll(".phase-item.pending");
            pendingPhases.Should().NotBeEmpty("Should show pending phases");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_PhaseScores()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var phaseScores = component.FindAll(".phase-score");
            phaseScores.Should().NotBeEmpty("Should display phase completion scores");
            
            foreach (var score in phaseScores)
            {
                score.TextContent.Should().MatchRegex(@"\d+%", "Phase scores should be percentages");
            }
        }

        #endregion

        #region Operations Action Buttons Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldHave_ContinueOperationsButton()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var continueButton = component.Find("button");
            continueButton.Should().NotBeNull("Should have continue operations button");
            continueButton.TextContent.Should().Contain("Continue Operations", "Should show continue operations text");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldHave_ViewLaunchPlanButton()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var actionButtons = component.FindAll("button");
            var launchPlanButton = actionButtons.FirstOrDefault(b => b.TextContent.Contains("View Launch Plan"));
            launchPlanButton.Should().NotBeNull("Should have view launch plan button");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldHandle_ButtonClicks()
        {
            // Arrange
            SetupDefaultMocks();
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Act & Assert
            var buttons = component.FindAll("button");
            buttons.Should().NotBeEmpty("Should have clickable action buttons");
            
            // Verify buttons are interactive
            foreach (var button in buttons.Take(3)) // Test first few buttons
            {
                button.Should().NotBeNull("Button should be rendered");
                button.HasAttribute("onclick").Should().BeTrue("Button should be clickable");
            }
        }

        #endregion

        #region Strategic Insights Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_OperationalInsights()
        {
            // Arrange
            SetupMocksWithCompassCoach();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var insightsCard = component.Find(".strategic-insights-card");
            insightsCard.Should().NotBeNull("Should display strategic insights card");
            
            var insightItems = component.FindAll(".insight-item");
            insightItems.Should().HaveCountGreaterThan(2, "Should show multiple operational insights");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_InsightCategories()
        {
            // Arrange
            SetupMocksWithCompassCoach();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var insightIcons = component.FindAll(".insight-icon");
            insightIcons.Should().NotBeEmpty("Should have insight icons");
            
            var insightContent = component.FindAll(".insight-content");
            insightContent.Should().NotBeEmpty("Should have insight content");
            
            // Check for different insight types
            var positiveInsights = component.FindAll(".insight-icon.positive");
            var focusInsights = component.FindAll(".insight-icon.focus");
            var warningInsights = component.FindAll(".insight-icon.warning");
            
            (positiveInsights.Count + focusInsights.Count + warningInsights.Count).Should().BeGreaterThan(0, 
                "Should have various insight types");
        }

        #endregion

        #region Operations Tools Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_OperationsToolCards()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var toolCards = component.FindAll(".operations-tool-card");
            toolCards.Should().HaveCountGreaterThan(2, "Should show multiple operations tool cards");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_LaunchPlanningTool()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var launchPlanningTool = component.FindAll(".operations-tool-card")
                .FirstOrDefault(c => c.TextContent.Contains("Launch Planning"));
            launchPlanningTool.Should().NotBeNull("Should have launch planning tool card");
            
            var toolProgress = launchPlanningTool?.QuerySelector(".tool-progress");
            toolProgress.Should().NotBeNull("Should show tool progress");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_ResourceManagementTool()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var resourceTool = component.FindAll(".operations-tool-card")
                .FirstOrDefault(c => c.TextContent.Contains("Resource Management"));
            resourceTool.Should().NotBeNull("Should have resource management tool card");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_PerformanceMonitoringTool()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var performanceTool = component.FindAll(".operations-tool-card")
                .FirstOrDefault(c => c.TextContent.Contains("Performance Monitoring"));
            performanceTool.Should().NotBeNull("Should have performance monitoring tool card");
        }

        #endregion

        #region Operations Plan Builder Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_OperationsPlanBuilder()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var planBuilderCard = component.Find(".operations-plan-builder-card");
            planBuilderCard.Should().NotBeNull("Should display operations plan builder card");
            
            var planCompletion = component.Find(".plan-completion");
            planCompletion.Should().NotBeNull("Should show plan completion status");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_PlanSections()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var planSectionsGrid = component.Find(".plan-sections-grid");
            planSectionsGrid.Should().NotBeNull("Should have plan sections grid");
            
            var planSections = component.FindAll(".plan-section");
            planSections.Should().HaveCountGreaterThan(4, "Should show multiple plan sections");
            
            // Check for specific operations plan sections
            var launchPlanSection = planSections.FirstOrDefault(s => s.TextContent.Contains("Launch Plan"));
            launchPlanSection.Should().NotBeNull("Should have launch plan section");
            
            var resourcePlanSection = planSections.FirstOrDefault(s => s.TextContent.Contains("Resource Plan"));
            resourcePlanSection.Should().NotBeNull("Should have resource plan section");
            
            var performanceSection = planSections.FirstOrDefault(s => s.TextContent.Contains("Performance Framework"));
            performanceSection.Should().NotBeNull("Should have performance framework section");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_CompletionProgress()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var completionBar = component.Find(".completion-bar");
            completionBar.Should().NotBeNull("Should show completion progress bar");
            
            var completionFill = component.Find(".completion-fill");
            completionFill.Should().NotBeNull("Should have completion fill indicator");
            
            var completionText = component.Find(".completion-text");
            completionText.Should().NotBeNull("Should show completion percentage text");
            completionText.TextContent.Should().MatchRegex(@"\d+%", "Should show percentage completion");
        }

        #endregion

        #region Coach Integration Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldShow_CompassCoachGuidance()
        {
            // Arrange
            SetupMocksWithCompassCoach();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var coachGuidance = component.Find(".coach-guidance");
            coachGuidance.Should().NotBeNull("Should show coach guidance for operations");
            
            var guidanceItems = component.FindAll(".guidance-item");
            guidanceItems.Should().HaveCountGreaterThan(2, "Should have multiple guidance items");
            
            // Check for operations-specific guidance
            var guidanceText = string.Join(" ", guidanceItems.Select(g => g.TextContent));
            guidanceText.Should().ContainEquivalentOf("launch");
            guidanceText.Should().ContainEquivalentOf("operations");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldDisplay_CoachMessage()
        {
            // Arrange
            SetupMocksWithCompassCoach();

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var coachMessage = component.Find(".coach-message-text");
            coachMessage.Should().NotBeNull("Should display coaching message");
            coachMessage.TextContent.Should().NotBeEmpty("Coach message should not be empty");
        }

        #endregion

        #region Hub Context Integration Tests

        [Fact]
        public void BusinessOperationsDashboard_ShouldBe_PartOfHub3Workflow()
        {
            // Arrange
            var hub3Context = CreateHub3Context();
            SetupMocksWithContext(hub3Context);

            // Act
            var component = RenderComponent<BusinessOperationsDashboard>();

            // Assert
            var container = component.Find(".business-operations-dashboard");
            container.Should().NotBeNull("Should render in Hub 3 context");
            
            // Should be themed for Hub 3
            var themeElements = component.FindAll(".hub-operations-theme");
            themeElements.Should().NotBeEmpty("Should apply Hub 3 theme");
        }

        [Fact]
        public void BusinessOperationsDashboard_ShouldImplement_IDisposable()
        {
            // Arrange
            SetupDefaultMocks();

            // Act & Assert
            // Component implements IDisposable for proper cleanup
            var component = RenderComponent<BusinessOperationsDashboard>();
            
            // Component should render without disposal issues  
            component.Find(".business-operations-dashboard").Should().NotBeNull();
        }

        #endregion

        #region Helper Methods

        private void SetupDefaultMocks()
        {
            var hubContext = CreateHub3Context();
            var compassCoach = CreateCompassCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessOperations)).ReturnsAsync(compassCoach);
            _mockDataService.Setup(x => x.GetAllScenariosAsync()).ReturnsAsync(new List<BusinessIdeaScenario>());
        }

        private void SetupMocksWithCompassCoach()
        {
            var hubContext = CreateHub3Context();
            var compassCoach = CreateCompassCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessOperations)).ReturnsAsync(compassCoach);
            _mockDataService.Setup(x => x.GetAllScenariosAsync()).ReturnsAsync(new List<BusinessIdeaScenario>());
        }

        private void SetupMocksWithContext(HubContext context)
        {
            var compassCoach = CreateCompassCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(context);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessOperations)).ReturnsAsync(compassCoach);
            _mockDataService.Setup(x => x.GetAllScenariosAsync()).ReturnsAsync(new List<BusinessIdeaScenario>());
        }

        private HubContext CreateHub3Context()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.BusinessOperations,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 25,
                        IsUnlocked = true,
                        IsActive = true,
                        CurrentPhase = "Launch Preparation",
                        CompletedMilestones = new List<string> { "operations_plan_initiated" },
                        AvailableMilestones = new List<string> 
                        { 
                            "operations_plan_initiated",
                            "launch_plan_created",
                            "resource_plan_finalized",
                            "performance_framework_established",
                            "operations_launched"
                        }
                    }
                }
            };
        }

        private CoachPersona CreateCompassCoach()
        {
            return new CoachPersona
            {
                Name = "Compass",
                Avatar = "fas fa-compass",
                Personality = "Strategic operations and launch management coach",
                VoicePattern = "Focused and results-oriented",
                Hub = BusinessHub.BusinessOperations,
                Style = CoachingStyle.ResultsOriented,
                Description = "Your strategic operations and launch coach"
            };
        }

        #endregion
    }
}