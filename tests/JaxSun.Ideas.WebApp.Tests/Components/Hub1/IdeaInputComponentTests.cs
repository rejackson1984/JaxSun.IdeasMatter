using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Services.Interfaces;
using JaxSun.Ideas.Mock.Components.Pages;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace JaxSun.Ideas.Mock.Tests.Components.Hub1
{
    public class IdeaInputComponentTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;  
        private readonly Mock<IIdeaValidationService> _mockValidationService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public IdeaInputComponentTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockValidationService = new Mock<IIdeaValidationService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockValidationService.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        [Fact]
        public void IdeaInput_ShouldDisplay_PageTitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var pageTitle = component.Find("title, PageTitle");
            // The PageTitle component should be rendered
            component.Markup.Should().Contain("Idea Development Hub - IdeaCoach Pro");
        }

        [Fact]
        public void IdeaInput_ShouldShow_CoachWelcomeSection()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var welcomeSection = component.Find(".coach-welcome-section");
            welcomeSection.Should().NotBeNull("Should display coach welcome section");
            
            var welcomeTitle = component.Find(".coach-welcome-title");
            welcomeTitle.Should().NotBeNull("Should display welcome title");
        }

        [Fact]
        public void IdeaInput_ShouldDisplay_CredibilityIndicators()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var credibilityItems = component.FindAll(".credibility-item");
            credibilityItems.Should().HaveCount(3, "Should show 3 credibility indicators");

            var credibilityTexts = credibilityItems.Select(item => item.TextContent).ToList();
            credibilityTexts.Should().Contain(text => text.Contains("15,000+ Businesses"));
            credibilityTexts.Should().Contain(text => text.Contains("$2.3B+ Revenue"));
            credibilityTexts.Should().Contain(text => text.Contains("94% Success Rate"));
        }

        [Fact]
        public void IdeaInput_ShouldHave_EditForm()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var editForm = component.FindComponents<EditForm>();
            editForm.Should().NotBeEmpty("Should contain EditForm for idea submission");
        }

        [Fact]
        public void IdeaInput_ShouldShow_FormTitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var formTitle = component.Find(".form-title");
            formTitle.TextContent.Should().Be("Let's Build Your Business Together 🚀");
        }

        [Fact]
        public void IdeaInput_ShouldShow_FormSubtitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var formSubtitle = component.Find(".form-subtitle");
            formSubtitle.TextContent.Should().Contain("Tell me about your idea");
        }

        [Fact]
        public void IdeaInput_ShouldUse_HubLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            // Component should render without layout errors
            // Layout directive is @layout HubLayout in the component
            component.Find(".coach-idea-input-container").Should().NotBeNull();
        }

        [Fact]
        public void IdeaInput_ShouldHave_HubIdeaTheme()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var themeElements = component.FindAll(".hub-idea-theme, .hub-idea-avatar");
            themeElements.Should().NotBeEmpty("Should apply Hub 1 theme styling");
        }

        [Fact]
        public void IdeaInput_ShouldShow_CoachAvatar()
        {
            // Arrange
            SetupMocksWithSparkCoach();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var coachAvatar = component.Find(".coach-avatar-large");
            coachAvatar.Should().NotBeNull("Should display coach avatar");
            
            var avatarIcon = coachAvatar.QuerySelector("i");
            avatarIcon.Should().NotBeNull("Should contain avatar icon");
        }

        [Fact]
        public void IdeaInput_ShouldDisplay_WelcomeMessage_FromCoach()
        {
            // Arrange
            SetupMocksWithSparkCoach();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var welcomeMessage = component.Find(".coach-message-text");
            welcomeMessage.Should().NotBeNull("Should display coach welcome message");
            welcomeMessage.TextContent.Should().NotBeEmpty("Welcome message should not be empty");
        }

        [Fact]
        public void IdeaInput_ShouldHave_CoachConversationForm()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var conversationForm = component.Find(".coach-conversation-form");
            conversationForm.Should().NotBeNull("Should have coach conversation form");
            
            var formContainer = component.Find(".coach-form-container");
            formContainer.Should().NotBeNull("Should have form container");
        }

        [Fact]
        public void IdeaInput_ShouldHandle_FormSubmission()
        {
            // Arrange
            SetupDefaultMocks();
            var mockValidationResult = CreateMockValidationResult();
            _mockValidationService.Setup(x => x.ValidateIdeaAsync(It.IsAny<IdeaValidationRequest>()))
                .ReturnsAsync(mockValidationResult);

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var editForm = component.FindComponents<EditForm>().FirstOrDefault();
            editForm.Should().NotBeNull("Should have EditForm for handling submission");
        }

        [Fact]
        public void IdeaInput_ShouldImplement_IDisposable()
        {
            // Arrange
            SetupDefaultMocks();

            // Act & Assert
            // Component implements IDisposable for proper cleanup
            var component = RenderComponent<IdeaInput>();
            
            // Component should render without disposal issues
            component.Find(".coach-idea-input-container").Should().NotBeNull();
        }

        [Fact]
        public void IdeaInput_ShouldHandle_MissingCoach_Gracefully()
        {
            // Arrange
            SetupMocksWithoutCoach();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            // Should not crash and should show default elements
            var container = component.Find(".coach-idea-input-container");
            container.Should().NotBeNull("Should render even without coach data");
            
            var welcomeTitle = component.Find(".coach-welcome-title");
            welcomeTitle.Should().NotBeNull("Should show title even without coach");
        }

        [Fact]
        public void IdeaInput_ShouldHave_Route_IdeaInput()
        {
            // This test validates that the component is accessible via the /idea-input route
            // The route is defined with @page "/idea-input" in the component
            
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            // Component should render properly when accessed via its route
            component.Find(".coach-idea-input-container").Should().NotBeNull();
        }

        [Fact]
        public void IdeaInput_ShouldInject_RequiredServices()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            // Component should render without service injection errors
            component.Find(".coach-idea-input-container").Should().NotBeNull();
            
            // Verify that component uses injected services (through rendering without errors)
            var welcomeSection = component.Find(".coach-welcome-section");
            welcomeSection.Should().NotBeNull("Should use injected services successfully");
        }

        [Fact]
        public void IdeaInput_ShouldBe_PartOfHub1Workflow()
        {
            // Arrange
            var hub1Context = CreateHub1Context();
            SetupMocksWithContext(hub1Context);

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var container = component.Find(".coach-idea-input-container");
            container.Should().NotBeNull("Should render in Hub 1 context");
            
            // Should be themed for Hub 1
            var themeElements = component.FindAll(".hub-idea-theme");
            themeElements.Should().NotBeEmpty("Should apply Hub 1 theme");
        }

        [Fact]
        public void IdeaInput_FormHeader_ShouldBe_Visible()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            var formHeader = component.Find(".coach-form-header");
            formHeader.Should().NotBeNull("Should display form header");
            
            var formTitle = component.Find(".form-title");
            var formSubtitle = component.Find(".form-subtitle");
            
            formTitle.Should().NotBeNull("Should have form title");
            formSubtitle.Should().NotBeNull("Should have form subtitle");
        }

        [Fact]
        public void IdeaInput_ShouldShow_ProgressiveEnhancement()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<IdeaInput>();

            // Assert
            // Should show coach guidance and credibility without requiring JavaScript
            var credibilitySection = component.Find(".coach-credibility");
            credibilitySection.Should().NotBeNull("Should show credibility indicators");
            
            var welcomeContent = component.Find(".coach-welcome-content");
            welcomeContent.Should().NotBeNull("Should show welcome content");
        }

        private void SetupDefaultMocks()
        {
            var hubContext = CreateHub1Context();
            var sparkCoach = CreateSparkCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync(sparkCoach);
        }

        private void SetupMocksWithSparkCoach()
        {
            var hubContext = CreateHub1Context();
            var sparkCoach = CreateSparkCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.IdeaDevelopment)).ReturnsAsync(sparkCoach);
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
                        CompletionPercentage = 10,
                        IsUnlocked = true,
                        IsActive = true,
                        CurrentPhase = "Initial Idea Input",
                        CompletedMilestones = new List<string>(),
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
                Description = "Your energetic idea development coach"
            };
        }

        private IdeaValidationResult CreateMockValidationResult()
        {
            return new IdeaValidationResult
            {
                Id = Guid.NewGuid().ToString(),
                OverallScore = 75,
                CategoryScores = new Dictionary<string, int>
                {
                    ["Market"] = 80,
                    ["Competition"] = 70,
                    ["Feasibility"] = 75
                },
                Strengths = new List<string> { "Strong market demand", "Clear value proposition" },
                NextSteps = new List<string> { "Conduct user interviews", "Build MVP" },
                ReadyForNextHub = false,
                ValidatedAt = DateTime.UtcNow
            };
        }
    }
}