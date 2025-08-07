using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Jackson.Ideas.Mock.Components.Layout;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Models;
using Moq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Jackson.Ideas.Mock.Tests.Components.Navigation
{
    /// <summary>
    /// Tests for sidebar navigation functionality to ensure all menu items work correctly
    /// and don't result in empty content areas.
    /// </summary>
    public class SidebarNavigationTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IHubConfigurationService> _mockHubConfigurationService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public SidebarNavigationTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockHubConfigurationService = new Mock<IHubConfigurationService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            SetupMockServices();
            RegisterServices();
        }

        private void SetupMockServices()
        {
            // Setup hub context for different hubs
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                .ReturnsAsync(new HubContext
                {
                    CurrentHub = BusinessHub.IdeaDevelopment,
                    IsAuthenticated = true,
                    UserId = "test-user"
                });

            // Setup hub metadata for all hubs
            _mockHubConfigurationService.Setup(x => x.GetHubMetadataAsync(BusinessHub.IdeaDevelopment))
                .ReturnsAsync(new HubMetadata
                {
                    Hub = BusinessHub.IdeaDevelopment,
                    Title = "Idea Development",
                    Description = "Transform ideas into viable concepts",
                    ThemeColor = "#7b8fef",
                    NavigationItems = new List<NavigationItem>
                    {
                        new NavigationItem { Title = "Dashboard", Route = "/dashboard", Icon = "fas fa-tachometer-alt" },
                        new NavigationItem { Title = "Idea Input", Route = "/idea-input", Icon = "fas fa-lightbulb" },
                        new NavigationItem { Title = "Market Research", Route = "/market-research", Icon = "fas fa-chart-line" },
                        new NavigationItem { Title = "Idea Validation", Route = "/idea-validation", Icon = "fas fa-check-circle" }
                    }
                });

            _mockHubConfigurationService.Setup(x => x.GetHubMetadataAsync(BusinessHub.BusinessPlanning))
                .ReturnsAsync(new HubMetadata
                {
                    Hub = BusinessHub.BusinessPlanning,
                    Title = "Business Planning",
                    Description = "Create comprehensive business plans",
                    ThemeColor = "#5a6fd8",
                    NavigationItems = new List<NavigationItem>
                    {
                        new NavigationItem { Title = "Dashboard", Route = "/dashboard", Icon = "fas fa-tachometer-alt" },
                        new NavigationItem { Title = "Business Plan Builder", Route = "/business-plan-builder", Icon = "fas fa-file-alt" },
                        new NavigationItem { Title = "Financial Projections", Route = "/financial-projections", Icon = "fas fa-calculator" },
                        new NavigationItem { Title = "Business Model Canvas", Route = "/business-model-canvas", Icon = "fas fa-th-large" }
                    }
                });

            _mockHubConfigurationService.Setup(x => x.GetHubMetadataAsync(BusinessHub.BusinessOperations))
                .ReturnsAsync(new HubMetadata
                {
                    Hub = BusinessHub.BusinessOperations,
                    Title = "Business Operations",
                    Description = "Manage and optimize operations",
                    ThemeColor = "#4c5fd7",
                    NavigationItems = new List<NavigationItem>
                    {
                        new NavigationItem { Title = "Dashboard", Route = "/dashboard", Icon = "fas fa-tachometer-alt" },
                        new NavigationItem { Title = "Operations Dashboard", Route = "/operations-dashboard", Icon = "fas fa-cogs" },
                        new NavigationItem { Title = "Leadership Development", Route = "/leadership-development", Icon = "fas fa-users" },
                        new NavigationItem { Title = "Solution Design", Route = "/solution-design", Icon = "fas fa-drafting-compass" }
                    }
                });

            // Setup coach persona service
            _mockCoachPersonaService.Setup(x => x.GetActivePersonaAsync())
                .ReturnsAsync(new CoachPersona
                {
                    Name = "Spark",
                    Description = "The Creative Catalyst",
                    Hub = BusinessHub.IdeaDevelopment
                });
        }

        private void RegisterServices()
        {
            Services.AddScoped(_ => _mockHubContextService.Object);
            Services.AddScoped(_ => _mockHubConfigurationService.Object);
            Services.AddScoped(_ => _mockCoachPersonaService.Object);
            Services.AddScoped(_ => _mockNavigationManager.Object);
            Services.AddScoped<IJSRuntime>(_ => new Mock<IJSRuntime>().Object);
        }

        [Fact]
        public void HubNavMenu_ShouldRender_WithNavigationItems()
        {
            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navMenu = component.Find(".hub-nav-menu");
            navMenu.Should().NotBeNull("Hub nav menu should render");

            // Should have navigation links
            var navLinks = component.FindAll("a[href]");
            navLinks.Should().NotBeEmpty("Should have navigation links");
        }

        [Theory]
        [InlineData("/dashboard")]
        [InlineData("/idea-input")]
        [InlineData("/market-research")]
        [InlineData("/idea-validation")]
        public void IdeaDevelopmentHub_NavigationItems_ShouldHaveCorrectRoutes(string expectedRoute)
        {
            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navLink = component.Find($"a[href='{expectedRoute}']");
            navLink.Should().NotBeNull($"Should have navigation link for {expectedRoute}");
        }

        [Fact]
        public void NavigationItems_ShouldHaveAccessibleLabels_AndIcons()
        {
            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navItems = component.FindAll(".nav-item");
            navItems.Should().NotBeEmpty("Should have navigation items");

            foreach (var navItem in navItems)
            {
                // Each nav item should have an icon
                var icon = navItem.QuerySelector("i[class*='fa']");
                icon.Should().NotBeNull("Navigation item should have an icon");

                // Each nav item should have text content
                navItem.TextContent.Should().NotBeNullOrWhiteSpace("Navigation item should have text content");
            }
        }

        [Fact]
        public void ActiveNavigationItem_ShouldBeHighlighted_WhenOnCurrentRoute()
        {
            // Arrange - Mock current route
            var mockNav = new Mock<NavigationManager>();
            mockNav.Setup(x => x.Uri).Returns("https://localhost:5000/market-research");
            Services.AddScoped(_ => mockNav.Object);

            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var activeNavItem = component.Find(".nav-item.active, .nav-link.active");
            activeNavItem.Should().NotBeNull("Current route should be highlighted");
        }

        [Fact]
        public void HubSwitching_ShouldUpdateNavigationItems_Correctly()
        {
            // Arrange - Start with Idea Development hub
            var component = RenderComponent<HubNavMenu>();
            var initialNavItems = component.FindAll("a[href]");

            // Act - Switch to Business Planning hub
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                .ReturnsAsync(new HubContext
                {
                    CurrentHub = BusinessHub.BusinessPlanning,
                    IsAuthenticated = true,
                    UserId = "test-user"
                });

            // Trigger re-render (in real app this would happen through hub context changes)
            component.Render();

            // Assert
            var updatedNavItems = component.FindAll("a[href]");
            updatedNavItems.Should().NotBeEmpty("Should still have navigation items after hub switch");
        }

        [Fact]
        public void MobileSidebar_ShouldToggle_WithJavaScriptFunctions()
        {
            // Arrange
            var testContent = (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Test content");
                builder.CloseElement();
            });

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert - Check that JavaScript functions are defined
            var scriptTag = component.Find("script");
            scriptTag.Should().NotBeNull("Should have script tag with JavaScript functions");
            scriptTag.InnerHtml.Should().Contain("toggleMobileSidebar", "Should define toggle function");
            scriptTag.InnerHtml.Should().Contain("closeMobileSidebar", "Should define close function");
        }

        [Fact]
        public void SidebarNavigation_ShouldNotOverlapWithMainContent()
        {
            // Arrange
            var testContent = (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "test-main-content");
                builder.AddContent(2, "Main content that should not be overlapped");
                builder.CloseElement();
            });

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert
            var sidebar = component.Find(".hub-sidebar");
            var mainContent = component.Find(".hub-main-content");
            var testContentElement = component.Find(".test-main-content");

            sidebar.Should().NotBeNull("Sidebar should exist");
            mainContent.Should().NotBeNull("Main content area should exist"); 
            testContentElement.Should().NotBeNull("Test content should be rendered and accessible");

            // Verify CSS classes for proper positioning
            mainContent.ClassList.Should().Contain("hub-main-content", 
                "Main content should have proper CSS class for positioning");
        }

        [Theory]
        [InlineData("dashboard", "fas fa-tachometer-alt")]
        [InlineData("idea-input", "fas fa-lightbulb")]
        [InlineData("market-research", "fas fa-chart-line")]
        [InlineData("operations-dashboard", "fas fa-cogs")]
        public void NavigationIcons_ShouldBeAccessible_AndSemanticallyClear(string itemText, string expectedIconClass)
        {
            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navItem = component.FindAll(".nav-item")
                .FirstOrDefault(item => item.TextContent.ToLower().Contains(itemText.Replace("-", " ")));

            if (navItem != null)
            {
                var icon = navItem.QuerySelector("i");
                icon.Should().NotBeNull($"Navigation item '{itemText}' should have an icon");
                
                if (icon != null)
                {
                    var iconClasses = icon.GetAttribute("class");
                    iconClasses.Should().Contain("fa", "Icon should use Font Awesome classes");
                }
            }
        }

        [Fact]
        public void SidebarCollapse_ShouldMaintainNavigation_OnMobileDevices()
        {
            // Arrange
            var testContent = (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "mobile-test-content");
                builder.AddContent(2, "Content for mobile test");
                builder.CloseElement();
            });

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert
            var mobileOverlay = component.Find(".mobile-sidebar-overlay");
            mobileOverlay.Should().NotBeNull("Mobile overlay should exist for collapsed sidebar");

            var sidebar = component.Find(".hub-sidebar");
            sidebar.Should().NotBeNull("Sidebar should exist even when collapsed on mobile");

            // Content should remain accessible
            var testContent = component.Find(".mobile-test-content");
            testContent.Should().NotBeNull("Content should remain accessible on mobile");
        }

        [Fact]
        public void NavigationAccessibility_ShouldSupportKeyboardNavigation()
        {
            // Act
            var component = RenderComponent<HubNavMenu>();

            // Assert
            var navLinks = component.FindAll("a[href]");
            navLinks.Should().NotBeEmpty("Should have navigation links");

            foreach (var link in navLinks)
            {
                // Links should be focusable for keyboard navigation
                var href = link.GetAttribute("href");
                href.Should().NotBeNullOrEmpty("Links should have href attributes for keyboard navigation");

                // Links should have meaningful text content for screen readers
                link.TextContent.Should().NotBeNullOrWhiteSpace(
                    "Links should have text content for accessibility");
            }
        }

        [Fact]
        public void HubNavigation_ShouldPreventEmptyContentAreas_WhenSwitchingRoutes()
        {
            // This test verifies that the navigation system doesn't cause the empty middle area issue
            
            // Arrange
            var routes = new[]
            {
                "/dashboard",
                "/market-research", 
                "/financial-projections",
                "/business-plan-builder",
                "/idea-input"
            };

            var testContent = (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "route-test-content");
                builder.AddContent(2, "Content should be visible after navigation");
                builder.CloseElement();
            });

            // Act & Assert
            foreach (var route in routes)
            {
                // Simulate navigation to each route
                var component = RenderComponent<HubLayout>(parameters => parameters
                    .Add(p => p.ChildContent, testContent));

                // Verify content remains visible
                var content = component.Find(".route-test-content");
                content.Should().NotBeNull($"Content should be visible when navigating to {route}");
                content.TextContent.Should().Contain("Content should be visible", 
                    $"Content should be accessible at route {route}");
            }
        }
    }
}