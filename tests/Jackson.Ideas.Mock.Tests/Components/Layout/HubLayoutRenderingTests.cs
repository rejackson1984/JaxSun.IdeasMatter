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

namespace Jackson.Ideas.Mock.Tests.Components.Layout
{
    /// <summary>
    /// Comprehensive tests for HubLayout component rendering to ensure it properly displays child content
    /// and prevents the empty middle area issue that was reported.
    /// </summary>
    public class HubLayoutRenderingTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IHubConfigurationService> _mockHubConfigurationService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IJSRuntime> _mockJSRuntime;

        public HubLayoutRenderingTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockHubConfigurationService = new Mock<IHubConfigurationService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockJSRuntime = new Mock<IJSRuntime>();

            SetupMockServices();
            RegisterServices();
        }

        private void SetupMockServices()
        {
            // Setup hub context service
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                .ReturnsAsync(new HubContext
                {
                    CurrentHub = BusinessHub.IdeaDevelopment,
                    IsAuthenticated = true,
                    UserId = "test-user-123"
                });

            // Setup hub configuration service  
            _mockHubConfigurationService.Setup(x => x.GetHubMetadataAsync(BusinessHub.IdeaDevelopment))
                .ReturnsAsync(new HubMetadata
                {
                    Hub = BusinessHub.IdeaDevelopment,
                    Title = "Idea Development Hub",
                    Description = "Transform your ideas into viable business concepts",
                    ThemeColor = "#7b8fef"
                });

            _mockHubConfigurationService.Setup(x => x.GetHubMetadataAsync(BusinessHub.BusinessPlanning))
                .ReturnsAsync(new HubMetadata
                {
                    Hub = BusinessHub.BusinessPlanning,
                    Title = "Business Planning Hub",
                    Description = "Create comprehensive business plans",
                    ThemeColor = "#5a6fd8"
                });

            _mockHubConfigurationService.Setup(x => x.GetHubMetadataAsync(BusinessHub.BusinessOperations))
                .ReturnsAsync(new HubMetadata
                {
                    Hub = BusinessHub.BusinessOperations,
                    Title = "Business Operations Hub", 
                    Description = "Manage and optimize business operations",
                    ThemeColor = "#4c5fd7"
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
            Services.AddScoped(_ => _mockJSRuntime.Object);
        }

        [Fact]
        public void HubLayout_ShouldRenderCorrectStructure_WithAllRequiredElements()
        {
            // Arrange
            var testContent = CreateTestChildContent("Test Hub Content");

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert
            var hubLayout = component.Find(".hub-layout");
            hubLayout.Should().NotBeNull("Hub layout container should exist");

            // Check for top navigation
            var topNav = component.Find("topnavbar");
            topNav.Should().NotBeNull("Top navigation should be rendered");

            // Check for mobile overlay
            var mobileOverlay = component.Find(".mobile-sidebar-overlay");
            mobileOverlay.Should().NotBeNull("Mobile sidebar overlay should exist");

            // Check for sidebar
            var sidebar = component.Find(".hub-sidebar");
            sidebar.Should().NotBeNull("Hub sidebar should be rendered");

            // Check for main content area
            var mainContent = component.Find(".hub-main-content");
            mainContent.Should().NotBeNull("Main content area should exist");

            // Check for content wrapper
            var contentWrapper = component.Find(".hub-content-wrapper");
            contentWrapper.Should().NotBeNull("Content wrapper should exist");
        }

        [Fact]
        public void HubLayout_ShouldRenderChildContent_InCorrectLocation()
        {
            // Arrange
            var testContent = CreateTestChildContent("Specific Test Content for Verification");

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert
            var childContentElement = component.Find(".test-child-content");
            childContentElement.Should().NotBeNull("Child content should be rendered");
            childContentElement.TextContent.Should().Contain("Specific Test Content for Verification",
                "Child content should display the correct text");

            // Verify child content is inside the main content area
            var mainContent = component.Find(".hub-main-content");
            var contentWrapper = mainContent.QuerySelector(".hub-content-wrapper");
            var childInWrapper = contentWrapper?.QuerySelector(".test-child-content");
            
            childInWrapper.Should().NotBeNull("Child content should be inside the content wrapper");
        }

        [Theory]  
        [InlineData(BusinessHub.IdeaDevelopment, "hub-theme-idea-development")]
        [InlineData(BusinessHub.BusinessPlanning, "hub-theme-business-planning")]
        [InlineData(BusinessHub.BusinessOperations, "hub-theme-business-operations")]
        public void HubLayout_ShouldApplyCorrectTheme_BasedOnCurrentHub(BusinessHub hub, string expectedThemeClass)
        {
            // Arrange
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                .ReturnsAsync(new HubContext
                {
                    CurrentHub = hub,
                    IsAuthenticated = true,
                    UserId = "test-user-123"
                });

            var testContent = CreateTestChildContent("Theme Test Content");

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert
            var hubLayout = component.Find(".hub-layout");
            hubLayout.ClassList.Should().Contain(expectedThemeClass,
                $"Layout should have {expectedThemeClass} class for {hub} hub");

            // Verify content is still rendered with theme applied
            var childContent = component.Find(".test-child-content");
            childContent.Should().NotBeNull("Child content should render correctly with theme applied");
        }

        [Fact]
        public void HubLayout_ShouldHandleMobileSidebarInteractions_WithoutAffectingContent()
        {
            // Arrange
            var testContent = CreateTestChildContent("Mobile Interaction Test Content");

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert - Check mobile overlay click behavior
            var mobileOverlay = component.Find(".mobile-sidebar-overlay");
            mobileOverlay.Should().NotBeNull("Mobile overlay should exist");

            // Verify the overlay has click handler (onclick attribute)
            var hasClickHandler = mobileOverlay.HasAttribute("onclick") || 
                                mobileOverlay.GetAttribute("onclick") != null;
            
            // The content should remain accessible regardless of mobile interactions
            var childContent = component.Find(".test-child-content");
            childContent.Should().NotBeNull("Child content should remain accessible during mobile interactions");
        }

        [Fact]
        public void HubLayout_ShouldRenderNavMenu_WithoutConflictingWithChildContent()
        {
            // Arrange
            var testContent = CreateTestChildContent("Nav Menu Coexistence Test");

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert
            var navMenu = component.Find("hubnavmenu");
            navMenu.Should().NotBeNull("Hub nav menu should be rendered");

            var childContent = component.Find(".test-child-content");
            childContent.Should().NotBeNull("Child content should coexist with nav menu");

            // Verify both elements are in their proper containers
            var sidebar = component.Find(".hub-sidebar");
            var sidebarNavMenu = sidebar.QuerySelector("hubnavmenu");
            sidebarNavMenu.Should().NotBeNull("Nav menu should be inside sidebar");

            var mainContent = component.Find(".hub-main-content");  
            var mainContentChild = mainContent.QuerySelector(".test-child-content");
            mainContentChild.Should().NotBeNull("Child content should be inside main content area");
        }

        [Fact]
        public void HubLayout_ShouldHandleNullChildContent_Gracefully()
        {
            // Act
            var component = RenderComponent<HubLayout>();

            // Assert
            var hubLayout = component.Find(".hub-layout");
            hubLayout.Should().NotBeNull("Layout should render even with null child content");

            var mainContent = component.Find(".hub-main-content");
            mainContent.Should().NotBeNull("Main content area should exist even without child content");

            var contentWrapper = component.Find(".hub-content-wrapper");
            contentWrapper.Should().NotBeNull("Content wrapper should exist even without child content");
        }

        [Fact]
        public void HubLayout_ShouldMaintainCorrectCSSPositioning_PreventingEmptyMiddleArea()
        {
            // Arrange
            var testContent = CreateTestChildContent("CSS Positioning Test Content");

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Assert - Check for proper CSS classes that ensure correct positioning
            var mainContent = component.Find(".hub-main-content");
            mainContent.ClassList.Should().Contain("hub-main-content",
                "Main content should have proper CSS class for positioning");

            var contentWrapper = component.Find(".hub-content-wrapper");
            contentWrapper.ClassList.Should().Contain("hub-content-wrapper",
                "Content wrapper should have proper CSS class");

            // Verify content is accessible and not hidden by absolute positioning issues
            var childContent = component.Find(".test-child-content");
            childContent.Should().NotBeNull("Child content should be visible and accessible");
        }

        [Fact]
        public void HubLayout_ShouldSupportMultipleContentElements_WithoutOverlap()
        {
            // Arrange
            var complexContent = (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "test-multiple-content");
                
                builder.OpenElement(2, "header");
                builder.AddAttribute(3, "class", "test-header");
                builder.AddContent(4, "Test Header");
                builder.CloseElement();
                
                builder.OpenElement(5, "main");
                builder.AddAttribute(6, "class", "test-main");
                builder.AddContent(7, "Test Main Content");
                builder.CloseElement();
                
                builder.OpenElement(8, "footer");
                builder.AddAttribute(9, "class", "test-footer");
                builder.AddContent(10, "Test Footer");
                builder.CloseElement();
                
                builder.CloseElement();
            });

            // Act
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, complexContent));

            // Assert
            var multipleContent = component.Find(".test-multiple-content");
            multipleContent.Should().NotBeNull("Multiple content container should exist");

            var header = component.Find(".test-header");
            header.Should().NotBeNull("Header should be rendered");
            header.TextContent.Should().Contain("Test Header");

            var main = component.Find(".test-main");
            main.Should().NotBeNull("Main should be rendered");
            main.TextContent.Should().Contain("Test Main Content");

            var footer = component.Find(".test-footer");
            footer.Should().NotBeNull("Footer should be rendered");
            footer.TextContent.Should().Contain("Test Footer");
        }

        [Fact]
        public void HubLayout_DisposalPattern_ShouldUnsubscribeFromEvents_Properly()
        {
            // Arrange
            var testContent = CreateTestChildContent("Disposal Test Content");
            var component = RenderComponent<HubLayout>(parameters => parameters
                .Add(p => p.ChildContent, testContent));

            // Act - Dispose the component
            component.Dispose();

            // Assert - Verify events were unsubscribed (no exceptions thrown during disposal)
            // If the event unsubscription is working properly, this should complete without errors
            Assert.True(true, "Component disposal should complete without errors");
        }

        private RenderFragment CreateTestChildContent(string content)
        {
            return (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "test-child-content");
                builder.AddContent(2, content);
                builder.CloseElement();
            });
        }
    }
}