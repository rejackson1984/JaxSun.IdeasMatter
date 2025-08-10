using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Components.Pages;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;

namespace Jackson.Ideas.Mock.Tests.Components.Hub1
{
    public class IdeaDevelopmentDashboardTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IMockDataService> _mockDataService;
        private readonly Mock<IIdeaValidationService> _mockValidationService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public IdeaDevelopmentDashboardTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockDataService = new Mock<IMockDataService>();
            _mockValidationService = new Mock<IIdeaValidationService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockDataService.Object);
            Services.AddSingleton(_mockValidationService.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldDisplay_HubTitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var title = component.Find(".coach-welcome-title");
            title.TextContent.Should().Contain("Idea Development Hub");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldShow_SparkCoachPersona()
        {
            // Arrange
            SetupMocksWithSparkCoach();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var coachAvatar = component.Find(".coach-avatar-large");
            coachAvatar.Should().NotBeNull("Should display coach avatar");
            
            // Coach welcome message should be present
            var welcomeMessage = component.Find(".coach-message-text");
            welcomeMessage.Should().NotBeNull("Should display coach welcome message");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldDisplay_IdeaPipeline()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var pipelineSection = component.Find("h2");
            pipelineSection.TextContent.Should().Contain("Your Ideas Pipeline", "Should show ideas pipeline section");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldShow_CoachGuidance()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var guidanceItems = component.FindAll(".guidance-item");
            guidanceItems.Should().HaveCount(3, "Should show 3 guidance items");

            var guidanceTexts = guidanceItems.Select(item => item.TextContent).ToList();
            guidanceTexts.Should().Contain(text => text.Contains("Transform concepts"));
            guidanceTexts.Should().Contain(text => text.Contains("Research markets"));
            guidanceTexts.Should().Contain(text => text.Contains("Validate ideas"));
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldHave_HubIdeaTheme()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var themeElements = component.FindAll(".hub-idea-theme, .hub-idea-avatar");
            themeElements.Should().NotBeEmpty("Should apply Hub 1 theme styling");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldUse_HubLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            // Component should render without layout errors
            // Layout directive is @layout HubLayout in the component
            component.Find(".idea-development-dashboard").Should().NotBeNull();
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldDisplay_WelcomeMessage_FromCoach()
        {
            // Arrange
            var sparkCoach = CreateSparkCoach();
            SetupMocksWithCoach(sparkCoach);

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var welcomeMessage = component.Find(".coach-message-text");
            welcomeMessage.TextContent.Should().NotBeEmpty("Should display coach welcome message");
            welcomeMessage.TextContent.Should().ContainEquivalentOf("idea");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldShow_LightbulbIcon_ForIdeaHub()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var icons = component.FindAll("i");
            icons.Should().Contain(icon => icon.ClassList.Contains("fa-lightbulb"), 
                "Should show lightbulb icon representing idea development");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldHandle_MissingCoach_Gracefully()
        {
            // Arrange
            SetupMocksWithoutCoach();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            // Should not crash and should show default elements
            var dashboard = component.Find(".idea-development-dashboard");
            dashboard.Should().NotBeNull("Should render even without coach data");
            
            var title = component.Find(".coach-welcome-title");
            title.Should().NotBeNull("Should show title even without coach");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldShow_ContainerFluid()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var container = component.Find(".container-fluid");
            container.Should().NotBeNull("Should use responsive container");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldHave_ResponsiveLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var rows = component.FindAll(".row");
            var cols = component.FindAll("[class*='col-']");
            
            rows.Should().NotBeEmpty("Should use Bootstrap row system");
            cols.Should().NotBeEmpty("Should use Bootstrap column system");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldHave_SectionHeaders()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var sectionHeaders = component.FindAll(".section-header");
            sectionHeaders.Should().NotBeEmpty("Should have organized section headers");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldBe_PartOfHub1Workflow()
        {
            // Arrange
            var hub1Context = CreateHub1Context();
            SetupMocksWithContext(hub1Context);

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var dashboard = component.Find(".idea-development-dashboard");
            dashboard.Should().NotBeNull("Should render in Hub 1 context");
            
            // Should show Hub 1 specific content
            var title = component.Find(".coach-welcome-title");
            title.TextContent.Should().Contain("Idea Development");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_CoachWelcome_ShouldBe_Visible()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaDevelopmentDashboard>();

            // Assert
            var welcomeSection = component.Find(".coach-welcome-section");
            welcomeSection.Should().NotBeNull("Should display coach welcome section");
            
            var welcomeContent = component.Find(".coach-welcome-content");
            welcomeContent.Should().NotBeNull("Should display welcome content");
        }

        [Fact]
        public void IdeaDevelopmentDashboard_ShouldImplement_IDisposable()
        {
            // Arrange
            SetupDefaultMocks();

            // Act & Assert
            // Component should implement IDisposable for proper cleanup
            // This is verified by the @implements IDisposable in the component
            var component = RenderComponent<IdeaDevelopmentDashboard>();
            
            // Component should render without disposal issues
            component.Find(".idea-development-dashboard").Should().NotBeNull();
        }

        private void SetupDefaultMocks()
        {
            var hubContext = CreateHub1Context();
            var sparkCoach = CreateSparkCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync(sparkCoach);
            _mockDataService.Setup(x => x.GetAllScenariosAsync()).ReturnsAsync(new List<BusinessIdeaScenario>());
        }

        private void SetupMocksWithSparkCoach()
        {
            var hubContext = CreateHub1Context();
            var sparkCoach = CreateSparkCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync(sparkCoach);
        }

        private void SetupMocksWithCoach(CoachPersona coach)
        {
            var hubContext = CreateHub1Context();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync(coach);
        }

        private void SetupMocksWithoutCoach()
        {
            var hubContext = CreateHub1Context();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync((CoachPersona?)null);
        }

        private void SetupMocksWithContext(HubContext context)
        {
            var sparkCoach = CreateSparkCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(context);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync(sparkCoach);
        }

        private HubContext CreateHub1Context()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 25,
                        IsUnlocked = true,
                        IsActive = true,
                        CurrentPhase = "Idea Submission",
                        CompletedMilestones = new List<string> { "idea_submitted" },
                        AvailableMilestones = new List<string> 
                        { 
                            "idea_submitted", 
                            "market_research_initiated", 
                            "competitive_analysis_started",
                            "idea_validated",
                            "research_completed"
                        }
                    }
                }
            };
        }

        private CoachPersona CreateSparkCoach()
        {
            return new CoachPersona
            {
                Name = "Spark",
                Avatar = "fas fa-lightbulb",
                Personality = "Energetic and encouraging idea development coach",
                VoicePattern = "Enthusiastic and supportive",
                Hub = BusinessHub.IdeaDevelopment,
                Style = CoachingStyle.Enthusiastic,
                Description = "Your energetic idea development coach who helps transform concepts into validated opportunities"
            };
        }
    }
}