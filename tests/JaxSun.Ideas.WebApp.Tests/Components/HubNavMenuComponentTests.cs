using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using JaxSun.Ideas.WebApp.Components.Layout;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;

namespace JaxSun.Ideas.WebApp.Tests.Components
{
    public class HubNavMenuComponentTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public HubNavMenuComponentTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        [Fact]
        public void HubNavMenu_ShouldDisplay_CurrentHubHeader()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubHeader = component.Find(".current-hub-nav-header");
            hubHeader.Should().NotBeNull("Should display current hub header");
        }

        [Fact]
        public void HubNavMenu_ShouldShow_CorrectHubIcon_ForIdeaDevelopment()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubIcon = component.Find(".hub-nav-icon");
            hubIcon.ClassList.Should().Contain("fa-lightbulb", "Should show lightbulb icon for Idea Development hub");
        }

        [Fact]
        public void HubNavMenu_ShouldShow_CorrectHubIcon_ForBusinessPlanning()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessPlanning);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubIcon = component.Find(".hub-nav-icon");
            hubIcon.ClassList.Should().Contain("fa-clipboard-list", "Should show clipboard icon for Business Planning hub");
        }

        [Fact]
        public void HubNavMenu_ShouldShow_CorrectHubIcon_ForBusinessOperations()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessOperations);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubIcon = component.Find(".hub-nav-icon");
            hubIcon.ClassList.Should().Contain("fa-chart-line", "Should show chart icon for Business Operations hub");
        }

        [Fact]
        public void HubNavMenu_ShouldDisplay_CorrectHubName_ForIdeaDevelopment()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubTitle = component.Find(".hub-nav-title");
            hubTitle.TextContent.Should().Be("Idea Development", "Should display correct hub name");
        }

        [Fact]
        public void HubNavMenu_ShouldDisplay_CorrectHubName_ForBusinessPlanning()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessPlanning);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubTitle = component.Find(".hub-nav-title");
            hubTitle.TextContent.Should().Be("Business Planning", "Should display correct hub name");
        }

        [Fact]
        public void HubNavMenu_ShouldDisplay_CorrectHubName_ForBusinessOperations()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessOperations);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubTitle = component.Find(".hub-nav-title");
            hubTitle.TextContent.Should().Be("Business Operations", "Should display correct hub name");
        }

        [Fact]
        public void HubNavMenu_ShouldRender_IdeaDevelopmentNavigation_WhenInHub1()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navItems = component.FindAll(".hub-sidebar-nav a, .hub-sidebar-nav .nav-link");
            navItems.Should().NotBeEmpty("Should render navigation items for Idea Development hub");
            
            // Should contain Idea Development specific navigation
            var navTexts = navItems.Select(n => n.TextContent.ToLower()).ToList();
            navTexts.Should().Contain(text => text.Contains("idea") || text.Contains("research") || text.Contains("validation"),
                "Should contain Idea Development specific navigation items");
        }

        [Fact]
        public void HubNavMenu_ShouldRender_BusinessPlanningNavigation_WhenInHub2()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessPlanning);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navItems = component.FindAll(".hub-sidebar-nav a, .hub-sidebar-nav .nav-link");
            navItems.Should().NotBeEmpty("Should render navigation items for Business Planning hub");
            
            // Should contain Business Planning specific navigation
            var navTexts = navItems.Select(n => n.TextContent.ToLower()).ToList();
            navTexts.Should().Contain(text => text.Contains("business") || text.Contains("plan") || text.Contains("strategy"),
                "Should contain Business Planning specific navigation items");
        }

        [Fact]
        public void HubNavMenu_ShouldRender_BusinessOperationsNavigation_WhenInHub3()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessOperations);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navItems = component.FindAll(".hub-sidebar-nav a, .hub-sidebar-nav .nav-link");
            navItems.Should().NotBeEmpty("Should render navigation items for Business Operations hub");
            
            // Should contain Business Operations specific navigation
            var navTexts = navItems.Select(n => n.TextContent.ToLower()).ToList();
            navTexts.Should().Contain(text => text.Contains("operations") || text.Contains("kpi") || text.Contains("analytics"),
                "Should contain Business Operations specific navigation items");
        }

        [Fact]
        public void HubNavMenu_ShouldDisplay_CoachWidget()
        {
            // Arrange
            SetupMocksForHubWithCoach(BusinessHub.IdeaDevelopment, "Spark");

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var coachWidget = component.Find(".coach-widget");
            coachWidget.Should().NotBeNull("Should display coach widget");
        }

        [Fact]
        public void HubNavMenu_ShouldShow_CorrectCoachPersona_ForIdeaDevelopment()
        {
            // Arrange
            SetupMocksForHubWithCoach(BusinessHub.IdeaDevelopment, "Spark");

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var coachWidget = component.Find(".coach-widget");
            coachWidget.TextContent.Should().Contain("Spark", "Should show Spark coach for Idea Development hub");
        }

        [Fact]
        public void HubNavMenu_ShouldShow_CorrectCoachPersona_ForBusinessPlanning()
        {
            // Arrange
            SetupMocksForHubWithCoach(BusinessHub.BusinessPlanning, "Strategy");

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var coachWidget = component.Find(".coach-widget");
            coachWidget.TextContent.Should().Contain("Strategy", "Should show Strategy coach for Business Planning hub");
        }

        [Fact]
        public void HubNavMenu_ShouldShow_CorrectCoachPersona_ForBusinessOperations()
        {
            // Arrange
            SetupMocksForHubWithCoach(BusinessHub.BusinessOperations, "Execute");

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var coachWidget = component.Find(".coach-widget");
            coachWidget.TextContent.Should().Contain("Execute", "Should show Execute coach for Business Operations hub");
        }

        [Fact]
        public void HubNavMenu_ShouldApply_CorrectHeaderClass_ForCurrentHub()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.BusinessPlanning);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubHeader = component.Find(".current-hub-nav-header");
            hubHeader.ClassList.Should().Contain(cls => cls.Contains("business-planning") || cls.Contains("hub-2"),
                "Should apply hub-specific CSS class to header");
        }

        [Fact]
        public void HubNavMenu_ShouldUpdate_WhenHubContextChanges()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);
            var component = RenderComponent<HubNavMenu>();

            // Act - Simulate hub context change
            var newContext = CreateMockHubContext(BusinessHub.BusinessPlanning);
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(newContext);
            
            var eventArgs = new HubContextChangedEventArgs
            {
                PreviousHub = BusinessHub.IdeaDevelopment,
                CurrentHub = BusinessHub.BusinessPlanning,
                Context = newContext
            };

            _mockHubContextService.Raise(x => x.ContextChanged += null, null, eventArgs);

            // Assert
            component.WaitForAssertion(() =>
            {
                var hubTitle = component.Find(".hub-nav-title");
                hubTitle.TextContent.Should().Be("Business Planning");
            });
        }

        [Fact]
        public void HubNavMenu_ShouldRender_NavigationSubtitle()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var subtitle = component.Find(".hub-nav-subtitle");
            subtitle.TextContent.Should().Be("Navigation", "Should display 'Navigation' subtitle");
        }

        [Fact]
        public void HubNavMenu_ShouldHave_SidebarHeader()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var sidebarHeader = component.Find(".sidebar-header");
            sidebarHeader.Should().NotBeNull("Should have sidebar header section");
        }

        [Fact]
        public void HubNavMenu_ShouldHave_HubSpecificNavigation()
        {
            // Arrange
            SetupMocksForHub(BusinessHub.IdeaDevelopment);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var hubNav = component.Find(".hub-sidebar-nav");
            hubNav.Should().NotBeNull("Should have hub-specific navigation section");
        }

        [Fact]
        public void HubNavMenu_ShouldHave_CoachSection()
        {
            // Arrange
            SetupMocksForHubWithCoach(BusinessHub.IdeaDevelopment, "Spark");

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var coachSection = component.Find(".hub-coach-section");
            coachSection.Should().NotBeNull("Should have coach section");
        }

        private void SetupMocksForHub(BusinessHub hub)
        {
            var hubContext = CreateMockHubContext(hub);
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
        }

        private void SetupMocksForHubWithCoach(BusinessHub hub, string coachPersona)
        {
            var hubContext = CreateMockHubContext(hub);
            var coach = new CoachPersona
            {
                Name = coachPersona,
                Hub = hub,
                Description = $"{coachPersona} coach for {hub}"
            };

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(hub)).ReturnsAsync(coach);
        }

        private HubContext CreateMockHubContext(BusinessHub currentHub)
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
                        IsUnlocked = true,
                        IsActive = currentHub == BusinessHub.IdeaDevelopment
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = currentHub != BusinessHub.IdeaDevelopment,
                        IsActive = currentHub == BusinessHub.BusinessPlanning
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        IsUnlocked = currentHub == BusinessHub.BusinessOperations,
                        IsActive = currentHub == BusinessHub.BusinessOperations
                    }
                }
            };
        }
    }
}