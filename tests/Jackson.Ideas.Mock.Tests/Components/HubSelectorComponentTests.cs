using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Components.Shared;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.JSInterop;

namespace Jackson.Ideas.Mock.Tests.Components
{
    public class HubSelectorComponentTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IHubConfigurationService> _mockHubConfigService;
        private readonly Mock<IJSRuntime> _mockJSRuntime;

        public HubSelectorComponentTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockHubConfigService = new Mock<IHubConfigurationService>();
            _mockJSRuntime = new Mock<IJSRuntime>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockHubConfigService.Object);
            Services.AddSingleton(_mockJSRuntime.Object);
        }

        [Fact]
        public void HubSelector_ShouldRender_BusinessHubsTitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var title = component.Find(".selector-title");
            title.TextContent.Should().Be("Business Hubs");
        }

        [Fact]
        public void HubSelector_ShouldDisplay_AllThreeHubs()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var hubItems = component.FindAll(".hub-item");
            hubItems.Should().HaveCount(3, "Should display all three business hubs");
        }

        [Fact]
        public void HubSelector_ShouldShow_CorrectHubNames()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var hubNames = component.FindAll(".hub-name");
            var nameTexts = hubNames.Select(n => n.TextContent).ToList();
            
            nameTexts.Should().Contain("Idea Development");
            nameTexts.Should().Contain("Business Planning");
            nameTexts.Should().Contain("Business Operations");
        }

        [Fact]
        public void HubSelector_ShouldDisplay_CorrectHubIcons()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var hubIcons = component.FindAll(".hub-icon i");
            var iconClasses = hubIcons.Select(i => i.ClassList).ToList();
            
            iconClasses.Should().Contain(classes => classes.Contains("fa-lightbulb"));
            iconClasses.Should().Contain(classes => classes.Contains("fa-clipboard-list"));
            iconClasses.Should().Contain(classes => classes.Contains("fa-chart-line"));
        }

        [Fact]
        public void HubSelector_ShouldMark_CurrentHubAsActive()
        {
            // Arrange
            SetupMocksWithCurrentHub(BusinessHub.BusinessPlanning);

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var activeHubItems = component.FindAll(".hub-item.active");
            activeHubItems.Should().HaveCount(1, "Only one hub should be marked as active");
        }

        [Fact]
        public void HubSelector_ShouldShow_ActiveIndicator_ForCurrentHub()
        {
            // Arrange
            SetupMocksWithCurrentHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var activeIndicators = component.FindAll(".active-indicator");
            activeIndicators.Should().HaveCount(1, "Should show active indicator for current hub only");
        }

        [Fact]
        public void HubSelector_ShouldMark_LockedHubs_AsLocked()
        {
            // Arrange
            SetupMocksWithLockedHubs();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var lockedHubItems = component.FindAll(".hub-item.locked");
            lockedHubItems.Should().HaveCountGreaterThan(0, "Should show locked hubs as locked");
        }

        [Fact]
        public void HubSelector_ShouldDisplay_ProgressPercentages()
        {
            // Arrange
            SetupMocksWithProgress();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var progressPercentages = component.FindAll(".progress-percentage");
            progressPercentages.Should().NotBeEmpty("Should display progress percentages for unlocked hubs");
        }

        [Fact]
        public void HubSelector_ShouldShow_UnlockRequirements_ForLockedHubs()
        {
            // Arrange
            SetupMocksWithLockedHubs();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var lockIndicators = component.FindAll(".lock-indicator");
            lockIndicators.Should().NotBeEmpty("Should show unlock requirements for locked hubs");
        }

        [Fact]
        public void HubSelector_ShouldDisplay_OverallProgress()
        {
            // Arrange
            SetupMocksWithProgress();

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var progressText = component.Find(".progress-text");
            progressText.TextContent.Should().EndWith("%", "Should display overall progress percentage");
        }

        [Fact]
        public void HubSelector_ShouldCall_SwitchToHubAsync_OnHubClick()
        {
            // Arrange
            SetupDefaultMocks();
            var component = RenderComponent<HubSelector>();

            // Act
            var firstUnlockedHub = component.FindAll(".hub-item").First(item => !item.ClassList.Contains("locked"));
            firstUnlockedHub.Click();

            // Assert
            _mockHubContextService.Verify(x => x.SwitchToHubAsync(It.IsAny<BusinessHub>()), Times.Once);
        }

        [Fact]
        public void HubSelector_ShouldNot_AllowClick_OnLockedHub()
        {
            // Arrange
            SetupMocksWithLockedHubs();
            var component = RenderComponent<HubSelector>();

            // Act
            var lockedHubs = component.FindAll(".hub-item.locked");
            if (lockedHubs.Any())
            {
                lockedHubs.First().Click();
            }

            // Assert
            // Should not call SwitchToHubAsync for locked hubs
            _mockHubContextService.Verify(x => x.SwitchToHubAsync(It.IsAny<BusinessHub>()), Times.Never);
        }

        [Fact]
        public void HubSelector_ShouldShow_TransitionIndicator_WhenSwitching()
        {
            // Arrange
            SetupDefaultMocks();
            _mockHubContextService.Setup(x => x.SwitchToHubAsync(It.IsAny<BusinessHub>()))
                .ReturnsAsync(true);

            var component = RenderComponent<HubSelector>();

            // Act
            var unlockedHub = component.FindAll(".hub-item").First(item => !item.ClassList.Contains("locked"));
            unlockedHub.Click();

            // Assert - This test validates the transition state is shown
            // The actual implementation should show a spinner or loading indicator
            component.FindAll(".transition-indicator, .spinner").Should().NotBeEmpty();
        }

        [Fact]
        public void HubSelector_ShouldHandle_HubContextChangedEvent()
        {
            // Arrange
            SetupDefaultMocks();
            var component = RenderComponent<HubSelector>();

            // Act - Simulate hub context change event
            var eventArgs = new HubContextChangedEventArgs
            {
                PreviousHub = BusinessHub.IdeaDevelopment,
                CurrentHub = BusinessHub.BusinessPlanning,
                Context = CreateMockHubContext(BusinessHub.BusinessPlanning)
            };

            _mockHubContextService.Raise(x => x.ContextChanged += null, null, eventArgs);

            // Assert
            // Component should re-render and update the active hub
            component.WaitForAssertion(() =>
            {
                var activeHubItems = component.FindAll(".hub-item.active");
                activeHubItems.Should().HaveCount(1);
            });
        }

        [Theory]
        [InlineData(BusinessHub.BusinessPlanning, 70)]
        [InlineData(BusinessHub.BusinessOperations, 80)]
        public void HubSelector_ShouldShow_CorrectUnlockThreshold_ForLockedHubs(BusinessHub hub, int expectedThreshold)
        {
            // Arrange
            SetupMocksWithSpecificUnlockThreshold(hub, expectedThreshold);

            // Act
            var component = RenderComponent<HubSelector>();

            // Assert
            var lockIndicators = component.FindAll(".lock-indicator");
            var thresholdText = lockIndicators.FirstOrDefault()?.TextContent;
            thresholdText.Should().Contain($"{expectedThreshold}%", $"Should show {expectedThreshold}% unlock threshold for {hub}");
        }

        private void SetupDefaultMocks()
        {
            var hubContext = CreateMockHubContext(BusinessHub.IdeaDevelopment);
            var hubMetadata = CreateMockHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksWithCurrentHub(BusinessHub currentHub)
        {
            var hubContext = CreateMockHubContext(currentHub);
            var hubMetadata = CreateMockHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksWithLockedHubs()
        {
            var hubContext = CreateMockHubContextWithLockedHubs();
            var hubMetadata = CreateMockHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksWithProgress()
        {
            var hubContext = CreateMockHubContextWithProgress();
            var hubMetadata = CreateMockHubMetadata();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private void SetupMocksWithSpecificUnlockThreshold(BusinessHub hub, int threshold)
        {
            var hubContext = CreateMockHubContextWithLockedHubs();
            var hubMetadata = CreateMockHubMetadata();
            hubMetadata[hub].UnlockThreshold = threshold;

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockHubConfigService.Setup(x => x.GetAllHubMetadataAsync()).ReturnsAsync(hubMetadata);
        }

        private HubContext CreateMockHubContext(BusinessHub currentHub)
        {
            return new HubContext
            {
                CurrentHub = currentHub,
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

        private HubContext CreateMockHubContextWithLockedHubs()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 60,
                        IsUnlocked = true,
                        IsActive = true
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false
                    }
                }
            };
        }

        private HubContext CreateMockHubContextWithProgress()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 75,
                        IsUnlocked = true,
                        IsActive = false
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 50,
                        IsUnlocked = true,
                        IsActive = true
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false
                    }
                }
            };
        }

        private Dictionary<BusinessHub, HubMetadata> CreateMockHubMetadata()
        {
            return new Dictionary<BusinessHub, HubMetadata>
            {
                [BusinessHub.IdeaDevelopment] = new HubMetadata
                {
                    Hub = BusinessHub.IdeaDevelopment,
                    Name = "Idea Development",
                    Icon = "fas fa-lightbulb",
                    UnlockThreshold = 0
                },
                [BusinessHub.BusinessPlanning] = new HubMetadata
                {
                    Hub = BusinessHub.BusinessPlanning,
                    Name = "Business Planning",
                    Icon = "fas fa-clipboard-list",
                    UnlockThreshold = 70
                },
                [BusinessHub.BusinessOperations] = new HubMetadata
                {
                    Hub = BusinessHub.BusinessOperations,
                    Name = "Business Operations",
                    Icon = "fas fa-chart-line",
                    UnlockThreshold = 80
                }
            };
        }
    }
}