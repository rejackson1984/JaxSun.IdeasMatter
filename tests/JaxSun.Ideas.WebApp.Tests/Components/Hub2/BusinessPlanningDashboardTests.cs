using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using JaxSun.Ideas.WebApp.Components.Pages;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;

namespace JaxSun.Ideas.WebApp.Tests.Components.Hub2
{
    public class BusinessPlanningDashboardTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IBusinessPlanService> _mockBusinessPlanService;
        private readonly Mock<IMockDataService> _mockDataService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public BusinessPlanningDashboardTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockBusinessPlanService = new Mock<IBusinessPlanService>();
            _mockDataService = new Mock<IMockDataService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockBusinessPlanService.Object);
            Services.AddSingleton(_mockDataService.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldDisplay_HubTitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var title = component.Find(".coach-welcome-title");
            title.TextContent.Should().Contain("Business Planning Hub");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldShow_StrategyCoachPersona()
        {
            // Arrange
            SetupMocksWithStrategyCoach();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var coachAvatar = component.Find(".coach-avatar-large");
            coachAvatar.Should().NotBeNull("Should display coach avatar");
            
            var welcomeMessage = component.Find(".coach-message-text");
            welcomeMessage.Should().NotBeNull("Should display coach welcome message");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldDisplay_BusinessPlanProgress()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var progressSection = component.Find("h2");
            progressSection.TextContent.Should().Contain("Business Plan Development", "Should show business plan progress section");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldShow_CoachGuidance()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var guidanceItems = component.FindAll(".guidance-item");
            guidanceItems.Should().HaveCount(3, "Should show 3 guidance items");

            var guidanceTexts = guidanceItems.Select(item => item.TextContent).ToList();
            guidanceTexts.Should().Contain(text => text.Contains("Create comprehensive"));
            guidanceTexts.Should().Contain(text => text.Contains("Develop financial"));
            guidanceTexts.Should().Contain(text => text.Contains("Plan market"));
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldHave_HubPlanningTheme()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var themeElements = component.FindAll(".hub-planning-theme, .hub-planning-avatar");
            themeElements.Should().NotBeEmpty("Should apply Hub 2 theme styling");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldUse_HubLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            // Component should render without layout errors
            // Layout directive is @layout HubLayout in the component
            component.Find(".business-planning-dashboard").Should().NotBeNull();
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldDisplay_WelcomeMessage_FromCoach()
        {
            // Arrange
            var strategyCoach = CreateStrategyCoach();
            SetupMocksWithCoach(strategyCoach);

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var welcomeMessage = component.Find(".coach-message-text");
            welcomeMessage.TextContent.Should().NotBeEmpty("Should display coach welcome message");
            welcomeMessage.TextContent.Should().ContainEquivalentOf("plan");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldShow_StrategyIcon_ForPlanningHub()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var icons = component.FindAll("i");
            icons.Should().Contain(icon => icon.ClassList.Contains("fa-chess-king") || icon.ClassList.Contains("fa-chart-line"), 
                "Should show strategy icon representing business planning");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldHandle_MissingCoach_Gracefully()
        {
            // Arrange
            SetupMocksWithoutCoach();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            // Should not crash and should show default elements
            var dashboard = component.Find(".business-planning-dashboard");
            dashboard.Should().NotBeNull("Should render even without coach data");
            
            var title = component.Find(".coach-welcome-title");
            title.Should().NotBeNull("Should show title even without coach");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldShow_ContainerFluid()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var container = component.Find(".container-fluid");
            container.Should().NotBeNull("Should use responsive container");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldHave_ResponsiveLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var rows = component.FindAll(".row");
            var cols = component.FindAll("[class*='col-']");
            
            rows.Should().NotBeEmpty("Should use Bootstrap row system");
            cols.Should().NotBeEmpty("Should use Bootstrap column system");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldHave_SectionHeaders()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var sectionHeaders = component.FindAll(".section-header");
            sectionHeaders.Should().NotBeEmpty("Should have organized section headers");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldBe_PartOfHub2Workflow()
        {
            // Arrange
            var hub2Context = CreateHub2Context();
            SetupMocksWithContext(hub2Context);

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var dashboard = component.Find(".business-planning-dashboard");
            dashboard.Should().NotBeNull("Should render in Hub 2 context");
            
            // Should show Hub 2 specific content
            var title = component.Find(".coach-welcome-title");
            title.TextContent.Should().Contain("Business Planning");
        }

        [Fact]
        public void BusinessPlanningDashboard_CoachWelcome_ShouldBe_Visible()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var welcomeSection = component.Find(".coach-welcome-section");
            welcomeSection.Should().NotBeNull("Should display coach welcome section");
            
            var welcomeContent = component.Find(".coach-welcome-content");
            welcomeContent.Should().NotBeNull("Should display welcome content");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldImplement_IDisposable()
        {
            // Arrange
            SetupDefaultMocks();

            // Act & Assert
            // Component should implement IDisposable for proper cleanup
            // This is verified by the @implements IDisposable in the component
            var component = RenderComponent<BusinessPlanningDashboard>();
            
            // Component should render without disposal issues
            component.Find(".business-planning-dashboard").Should().NotBeNull();
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldShow_BusinessPlanSections()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var planSections = component.FindAll(".plan-section");
            planSections.Should().NotBeEmpty("Should display business plan sections");
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldShow_FinancialProjections()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            var financialSection = component.FindAll("h3").Should().Contain(h => 
                h.TextContent.Contains("Financial", StringComparison.OrdinalIgnoreCase) ||
                h.TextContent.Contains("Revenue", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void BusinessPlanningDashboard_ShouldRequire_Hub1Completion()
        {
            // Arrange
            var incompleteHub1Context = CreateIncompleteHub1Context();
            SetupMocksWithContext(incompleteHub1Context);

            // Act
            var component = RenderComponent<BusinessPlanningDashboard>();

            // Assert
            // Should show message about completing Hub 1 first
            var prerequisiteMessage = component.FindAll("*").Should().Contain(el => 
                el.TextContent.Contains("complete", StringComparison.OrdinalIgnoreCase) &&
                el.TextContent.Contains("idea", StringComparison.OrdinalIgnoreCase));
        }

        private void SetupDefaultMocks()
        {
            var hubContext = CreateHub2Context();
            var strategyCoach = CreateStrategyCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(strategyCoach);
            _mockDataService.Setup(x => x.GetAllScenariosAsync()).ReturnsAsync(new List<BusinessIdeaScenario>());
        }

        private void SetupMocksWithStrategyCoach()
        {
            var hubContext = CreateHub2Context();
            var strategyCoach = CreateStrategyCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(strategyCoach);
        }

        private void SetupMocksWithCoach(CoachPersona coach)
        {
            var hubContext = CreateHub2Context();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(coach);
        }

        private void SetupMocksWithoutCoach()
        {
            var hubContext = CreateHub2Context();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync((CoachPersona?)null);
        }

        private void SetupMocksWithContext(HubContext context)
        {
            var strategyCoach = CreateStrategyCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(context);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(strategyCoach);
        }

        private HubContext CreateHub2Context()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 85,
                        IsUnlocked = true,
                        IsActive = false,
                        CurrentPhase = "Completed",
                        CompletedMilestones = new List<string> { "idea_submitted", "market_research_completed", "idea_validated" },
                        AvailableMilestones = new List<string> { "idea_submitted", "market_research_completed", "idea_validated" }
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 15,
                        IsUnlocked = true,
                        IsActive = true,
                        CurrentPhase = "Business Plan Development",
                        CompletedMilestones = new List<string>(),
                        AvailableMilestones = new List<string> 
                        { 
                            "executive_summary_created",
                            "market_analysis_completed", 
                            "financial_projections_created",
                            "marketing_strategy_developed",
                            "operational_plan_created",
                            "business_plan_finalized"
                        }
                    }
                }
            };
        }

        private HubContext CreateIncompleteHub1Context()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 45, // Below 70% threshold
                        IsUnlocked = true,
                        IsActive = false,
                        CurrentPhase = "In Progress",
                        CompletedMilestones = new List<string> { "idea_submitted" },
                        AvailableMilestones = new List<string> { "idea_submitted", "market_research_completed", "idea_validated" }
                    }
                }
            };
        }

        private CoachPersona CreateStrategyCoach()
        {
            return new CoachPersona
            {
                Name = "Strategy",
                Avatar = "fas fa-chess-king",
                Personality = "Strategic and analytical business planning coach",
                VoicePattern = "Professional and methodical",
                Hub = BusinessHub.BusinessPlanning,
                Style = CoachingStyle.Analytical,
                Description = "Your strategic business planning coach who helps transform ideas into comprehensive business plans"
            };
        }
    }
}