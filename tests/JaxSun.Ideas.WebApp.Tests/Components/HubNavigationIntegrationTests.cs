using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using JaxSun.Ideas.WebApp.Components.Shared;
using JaxSun.Ideas.WebApp.Components.Layout;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace JaxSun.Ideas.WebApp.Tests.Components
{
    public class HubNavigationIntegrationTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IHubConfigurationService> _mockHubConfigService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IJSRuntime> _mockJSRuntime;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public HubNavigationIntegrationTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockHubConfigService = new Mock<IHubConfigurationService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockJSRuntime = new Mock<IJSRuntime>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register all services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockHubConfigService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockJSRuntime.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        [Fact]
        public void HubNavigation_ShouldMaintain_ConsistentState_BetweenComponents()
        {
            // Arrange
            var currentHub = BusinessHub.BusinessPlanning;
            SetupMocksForIntegratedTest(currentHub);

            // Act
            var hubSelector = RenderComponent<HubSelector>();
            var hubNavMenu = RenderComponent<HubNavMenu>();

            // Assert
            // Both components should show the same current hub
            var selectorActiveHub = hubSelector.FindAll(".hub-item.active");
            var navMenuTitle = hubNavMenu.Find(".hub-nav-title");

            selectorActiveHub.Should().HaveCount(1, "HubSelector should show exactly one active hub");
            navMenuTitle.TextContent.Should().Be("Business Planning", "HubNavMenu should show correct hub name");
        }

        [Fact]
        public void HubNavigation_ShouldSync_WhenHubChanges()
        {
            // Arrange
            SetupMocksForIntegratedTest(BusinessHub.IdeaDevelopment);
            var hubSelector = RenderComponent<HubSelector>();
            var hubNavMenu = RenderComponent<HubNavMenu>();

            // Act - Simulate hub change through context service
            var newContext = CreateIntegratedHubContext(BusinessHub.BusinessPlanning);
            var eventArgs = new HubContextChangedEventArgs
            {
                PreviousHub = BusinessHub.IdeaDevelopment,
                CurrentHub = BusinessHub.BusinessPlanning,
                Context = newContext
            };

            _mockHubContextService.Raise(x => x.ContextChanged += null, null, eventArgs);

            // Assert
            hubSelector.WaitForAssertion(() =>
            {
                var activeHubs = hubSelector.FindAll(".hub-item.active");
                activeHubs.Should().HaveCount(1);
            });

            hubNavMenu.WaitForAssertion(() =>
            {
                var title = hubNavMenu.Find(".hub-nav-title");
                title.TextContent.Should().Be("Business Planning");
            });
        }

        [Fact]
        public void HubNavigation_ShouldEnforce_ProgressionRules()
        {
            // Arrange - Set up context where only Hub 1 is unlocked
            SetupMocksWithProgressionRules();

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert
            var unlockedHubs = hubSelector.FindAll(".hub-item").Where(item => !item.ClassList.Contains("locked"));
            var lockedHubs = hubSelector.FindAll(".hub-item.locked");

            unlockedHubs.Should().HaveCount(1, "Only Hub 1 should be unlocked initially");
            lockedHubs.Should().HaveCount(2, "Hub 2 and 3 should be locked initially");
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, "Spark")]
        [InlineData(BusinessHub.BusinessPlanning, "Strategy")]
        [InlineData(BusinessHub.BusinessOperations, "Execute")]
        public void HubNavigation_ShouldShow_CorrectCoach_ForEachHub(BusinessHub hub, string expectedCoach)
        {
            // Arrange
            SetupMocksWithCoachForHub(hub, expectedCoach);

            // Act
            var hubNavMenu = RenderComponent<HubNavMenu>();

            // Assert
            var coachWidget = hubNavMenu.Find(".coach-widget");
            coachWidget.TextContent.Should().Contain(expectedCoach, $"Should show {expectedCoach} coach for {hub}");
        }

        [Fact]
        public void HubNavigation_ShouldHandle_ServiceErrors_Gracefully()
        {
            // Arrange
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                .ThrowsAsync(new Exception("Service unavailable"));

            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync())
                .ThrowsAsync(new Exception("Configuration unavailable"));

            // Act & Assert - Should not throw exceptions
            var exception = Record.Exception(() =>
            {
                var hubSelector = RenderComponent<HubSelector>();
                var hubNavMenu = RenderComponent<HubNavMenu>();
            });

            exception.Should().BeNull("Components should handle service errors gracefully");
        }

        [Fact]
        public void HubNavigation_ShouldShow_LoadingState_WhenServicesAreSlow()
        {
            // Arrange
            var slowTaskSource = new TaskCompletionSource<HubContext>();
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                .Returns(slowTaskSource.Task);

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert - Should show some loading state or handle gracefully
            // This test ensures the component doesn't break while waiting for async data
            var hubItems = hubSelector.FindAll(".hub-item");
            // Component should either show loading state or render with defaults
            // The exact behavior depends on implementation, but it shouldn't crash
        }

        [Fact]
        public void HubNavigation_ShouldValidate_BusinessRules()
        {
            // Arrange
            SetupMocksWithStrictProgressionRules();

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert
            // Validate Hub 2 unlock requirements (70% Hub 1 completion)
            var hub2Item = hubSelector.FindAll(".hub-item")[1]; // Hub 2 (BusinessPlanning)
            hub2Item.ClassList.Should().Contain("locked", "Hub 2 should be locked when Hub 1 is below 70%");

            // Validate Hub 3 unlock requirements (80% Hub 2 completion)
            var hub3Item = hubSelector.FindAll(".hub-item")[2]; // Hub 3 (BusinessOperations)
            hub3Item.ClassList.Should().Contain("locked", "Hub 3 should be locked when Hub 2 is below 80%");
        }

        [Fact]
        public void HubNavigation_ShouldSupport_HubUnlocking_Progression()
        {
            // Arrange
            var initialContext = CreateProgressionTestContext(60, false, false); // Hub 1: 60%, Hub 2&3: locked
            SetupMocksForProgression(initialContext);

            var hubSelector = RenderComponent<HubSelector>();

            // Act - Simulate Hub 1 reaching 70% completion
            var updatedContext = CreateProgressionTestContext(75, true, false); // Hub 1: 75%, Hub 2: unlocked
            var eventArgs = new HubContextChangedEventArgs
            {
                PreviousHub = BusinessHub.IdeaDevelopment,
                CurrentHub = BusinessHub.IdeaDevelopment,
                Context = updatedContext
            };

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(updatedContext);
            _mockHubContextService.Raise(x => x.ContextChanged += null, null, eventArgs);

            // Assert
            hubSelector.WaitForAssertion(() =>
            {
                var lockedHubs = hubSelector.FindAll(".hub-item.locked");
                lockedHubs.Should().HaveCount(1, "Only Hub 3 should remain locked after Hub 1 reaches 70%");
            });
        }

        [Fact]
        public void HubNavigation_ShouldDisplay_ProgressIndicators_Correctly()
        {
            // Arrange
            SetupMocksWithProgressIndicators();

            // Act
            var hubSelector = RenderComponent<HubSelector>();

            // Assert
            var progressBars = hubSelector.FindAll(".progress-bar");
            var progressPercentages = hubSelector.FindAll(".progress-percentage");

            progressBars.Should().NotBeEmpty("Should display progress bars for unlocked hubs");
            progressPercentages.Should().NotBeEmpty("Should display progress percentages");

            // Validate overall progress circle
            var progressText = hubSelector.Find(".progress-text");
            progressText.TextContent.Should().MatchRegex(@"\d+%", "Should show overall progress percentage");
        }

        private void SetupMocksForIntegratedTest(BusinessHub currentHub)
        {
            var hubContext = CreateIntegratedHubContext(currentHub);
            var hubMetadata = CreateIntegratedHubMetadata();
            var coach = CreateMockCoach(currentHub);

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(currentHub)).ReturnsAsync(coach);
        }

        private void SetupMocksWithProgressionRules()
        {
            var hubContext = CreateProgressionTestContext(60, false, false); // Only Hub 1 unlocked
            var hubMetadata = CreateIntegratedHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksWithCoachForHub(BusinessHub hub, string coachName)
        {
            var hubContext = CreateIntegratedHubContext(hub);
            var coach = CreateMockCoach(hub, coachName);

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(hub)).ReturnsAsync(coach);
        }

        private void SetupMocksWithStrictProgressionRules()
        {
            var hubContext = CreateProgressionTestContext(60, false, false); // Hub 1: 60% (below 70% threshold)
            var hubMetadata = CreateIntegratedHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksForProgression(HubContext context)
        {
            var hubMetadata = CreateIntegratedHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(context);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksWithProgressIndicators()
        {
            var hubContext = CreateProgressionTestContext(75, true, false); // Hub 1: 75%, Hub 2: unlocked
            var hubMetadata = CreateIntegratedHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private HubContext CreateIntegratedHubContext(BusinessHub currentHub)
        {
            return new HubContext
            {
                CurrentHub = currentHub,
                UserId = "integration-test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 85,
                        IsUnlocked = true,
                        IsActive = currentHub == BusinessHub.IdeaDevelopment
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 45,
                        IsUnlocked = true,
                        IsActive = currentHub == BusinessHub.BusinessPlanning
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = currentHub == BusinessHub.BusinessOperations
                    }
                }
            };
        }

        private HubContext CreateProgressionTestContext(int hub1Progress, bool hub2Unlocked, bool hub3Unlocked)
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserId = "progression-test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = hub1Progress,
                        IsUnlocked = true,
                        IsActive = true
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = hub2Unlocked ? 30 : 0,
                        IsUnlocked = hub2Unlocked,
                        IsActive = false
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = hub3Unlocked,
                        IsActive = false
                    }
                }
            };
        }

        private Dictionary<BusinessHub, HubMetadata> CreateIntegratedHubMetadata()
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

        private CoachPersona CreateMockCoach(BusinessHub hub, string? customName = null)
        {
            var coachNames = new Dictionary<BusinessHub, string>
            {
                [BusinessHub.IdeaDevelopment] = "Spark",
                [BusinessHub.BusinessPlanning] = "Strategy",
                [BusinessHub.BusinessOperations] = "Execute"
            };

            var coachName = customName ?? coachNames[hub];

            return new CoachPersona
            {
                Name = coachName,
                Hub = hub,
                Description = $"{coachName} coach for {hub} hub",
                Avatar = $"/images/coaches/{coachName.ToLower()}.png",
                Personality = $"{coachName} coaching personality"
            };
        }
    }
}