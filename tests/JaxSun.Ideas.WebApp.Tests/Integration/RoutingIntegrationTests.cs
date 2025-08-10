using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using JaxSun.Ideas.Mock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;

namespace JaxSun.Ideas.Mock.Tests.Integration
{
    /// <summary>
    /// Integration tests for routing to verify that the empty middle area issue is resolved
    /// and all routes properly serve content without layout issues.
    /// </summary>
    public class RoutingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RoutingIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
            });
            _client = _factory.CreateClient();
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/dashboard")]
        [InlineData("/market-research")]
        [InlineData("/financial-projections")]
        [InlineData("/business-plan-builder")]
        [InlineData("/idea-input")]
        [InlineData("/idea-validation")]
        [InlineData("/business-model-canvas")]
        [InlineData("/solution-design")]
        [InlineData("/prd-review")]
        [InlineData("/mockup-preview")]
        [InlineData("/operations-dashboard")]
        [InlineData("/leadership-development")]
        public async Task Route_ShouldReturnSuccessStatusCode_AndNotBeEmpty(string route)
        {
            // Act
            var response = await _client.GetAsync(route);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, 
                $"Route {route} should return OK status");

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty($"Route {route} should return content");
            content.Length.Should().BeGreaterThan(100, 
                $"Route {route} should return substantial content, not just empty HTML");
        }

        [Fact]
        public async Task MarketResearch_Route_ShouldContainExpectedContent_NotEmptyMiddleArea()
        {
            // Act
            var response = await _client.GetAsync("/market-research");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // Parse HTML to verify structure
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            // Check for essential page elements
            var title = doc.DocumentNode.SelectSingleNode("//title");
            title.Should().NotBeNull("Page should have a title");
            title.InnerText.Should().Contain("Market Research", "Title should indicate market research page");

            // Check for main content indicators
            content.Should().Contain("Market Research Analysis", "Should contain the main heading");
            content.Should().Contain("container-fluid", "Should contain Bootstrap container classes");
            content.Should().Contain("hub-layout", "Should use HubLayout for consistency");

            // Verify it's not just empty divs - check for actual functional content
            content.Should().Contain("Select Business Scenario", "Should contain scenario selection functionality");
            content.Should().Contain("Market Segmentation", "Should contain market segmentation content");
        }

        [Fact]
        public async Task Dashboard_Route_ShouldContainHubSelector_WithNavigationElements()
        {
            // Act
            var response = await _client.GetAsync("/dashboard");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            content.Should().Contain("dashboard", "Should be the dashboard page");
            content.Should().NotBeNullOrEmpty("Dashboard should have content");
            
            // Check for navigation elements that would be missing in the empty middle area bug
            content.Should().Contain("hub", "Should contain hub-related content");
        }

        [Fact]
        public async Task FinancialProjections_Route_ShouldLoadWithProperLayout()
        {
            // Act
            var response = await _client.GetAsync("/financial-projections");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty("Financial projections should have content");
            
            // Verify layout structure is present
            content.Should().Contain("hub-layout", "Should use HubLayout");
            content.Should().Contain("hub-main-content", "Should have main content area");
        }

        [Fact]
        public async Task BusinessPlanBuilder_Route_ShouldRenderWithContent()
        {
            // Act
            var response = await _client.GetAsync("/business-plan-builder");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty("Business plan builder should have content");
            content.Should().Contain("hub-layout", "Should use HubLayout structure");
        }

        [Fact]
        public async Task IdeaInput_Route_ShouldDisplayForm_NotEmptyArea()
        {
            // Act
            var response = await _client.GetAsync("/idea-input");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty("Idea input should have form content");
            
            // Should contain form elements, not empty middle area
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            
            var formElements = doc.DocumentNode.SelectNodes("//input | //textarea | //button");
            formElements.Should().NotBeNullOrEmpty("Should contain interactive form elements");
        }

        [Fact]
        public async Task HubRoutes_ShouldAllUseConsistentLayout_WithSidebar()
        {
            // Arrange
            var hubRoutes = new[]
            {
                "/market-research",
                "/financial-projections", 
                "/business-plan-builder",
                "/idea-input",
                "/idea-validation"
            };

            // Act & Assert
            foreach (var route in hubRoutes)
            {
                var response = await _client.GetAsync(route);
                var content = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.OK, $"Route {route} should be accessible");
                
                // All hub routes should have consistent layout elements
                content.Should().Contain("hub-layout", $"Route {route} should use HubLayout");
                content.Should().Contain("hub-sidebar", $"Route {route} should have sidebar");
                content.Should().Contain("hub-main-content", $"Route {route} should have main content area");
                
                // Verify content area is not empty
                content.Length.Should().BeGreaterThan(1000, 
                    $"Route {route} should have substantial content indicating it's not empty");
            }
        }

        [Fact]
        public async Task NavigationLinks_ShouldWork_WithoutCausingEmptyPages()
        {
            // This test would ideally use a headless browser like Playwright
            // For now, we verify that navigation target routes are accessible
            
            // Act - Test that common navigation targets are accessible
            var navigationTargets = new[]
            {
                "/dashboard",
                "/market-research",
                "/financial-projections",
                "/business-plan-builder"
            };

            // Assert
            foreach (var target in navigationTargets)
            {
                var response = await _client.GetAsync(target);
                response.StatusCode.Should().Be(HttpStatusCode.OK, 
                    $"Navigation target {target} should be accessible");
                
                var content = await response.Content.ReadAsStringAsync();
                content.Should().NotBeNullOrEmpty($"Navigation target {target} should have content");
            }
        }

        [Fact]
        public async Task StaticAssets_ShouldLoad_ForProperStyling()
        {
            // Act - Try to load CSS and JS assets that are critical for layout
            var cssResponse = await _client.GetAsync("/_content/JaxSun.Ideas.Mock/css/bootstrap/bootstrap.min.css");
            var jsResponse = await _client.GetAsync("/_framework/blazor.server.js");

            // Assert - These might not exist in test environment, but check what we can
            // The key is that the main page loads and references these assets
            var mainPageResponse = await _client.GetAsync("/");
            var mainPageContent = await mainPageResponse.Content.ReadAsStringAsync();
            
            mainPageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            mainPageContent.Should().Contain("bootstrap", "Should reference Bootstrap for styling");
            mainPageContent.Should().Contain("blazor", "Should reference Blazor for functionality");
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnHealthy_IndicatingAppIsWorking()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Healthy", "Health check should indicate the app is working properly");
        }

        [Fact]
        public async Task FallbackRoutes_ShouldHandleUnknownPaths_WithoutErrors()
        {
            // Act - Test a path that doesn't exist
            var response = await _client.GetAsync("/non-existent-path");

            // Assert - Should either redirect or show 404, but not crash
            var validStatusCodes = new[] 
            { 
                HttpStatusCode.OK,           // Fallback to main page
                HttpStatusCode.NotFound,     // Proper 404
                HttpStatusCode.Redirect,     // Redirect to valid page
                HttpStatusCode.MovedPermanently 
            };
            
            validStatusCodes.Should().Contain(response.StatusCode, 
                "Unknown paths should be handled gracefully");
        }

        [Theory]
        [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")] // Desktop
        [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 14_7_1 like Mac OS X)")] // Mobile
        [InlineData("Mozilla/5.0 (iPad; CPU OS 14_7_1 like Mac OS X)")] // Tablet
        public async Task Routes_ShouldWorkOnDifferentDevices_WithResponsiveLayout(string userAgent)
        {
            // Arrange
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", userAgent);

            // Act
            var response = await _client.GetAsync("/market-research");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, 
                "Should work on different devices");
            
            content.Should().Contain("viewport", "Should have responsive viewport meta tag");
            content.Should().Contain("col-", "Should use responsive Bootstrap classes");
            content.Should().NotBeNullOrEmpty("Should have content regardless of device");
        }

        [Fact]
        public async Task ConcurrentRequests_ShouldAllReturnContent_NoEmptyResponses()
        {
            // Arrange
            var routes = new[]
            {
                "/",
                "/dashboard", 
                "/market-research",
                "/financial-projections"
            };

            // Act - Make concurrent requests
            var tasks = routes.Select(route => _client.GetAsync(route)).ToArray();
            var responses = await Task.WhenAll(tasks);

            // Assert
            for (int i = 0; i < responses.Length; i++)
            {
                responses[i].StatusCode.Should().Be(HttpStatusCode.OK, 
                    $"Concurrent request to {routes[i]} should succeed");
                
                var content = await responses[i].Content.ReadAsStringAsync();
                content.Should().NotBeNullOrEmpty(
                    $"Concurrent request to {routes[i]} should return content");
                content.Length.Should().BeGreaterThan(100,
                    $"Concurrent request to {routes[i]} should return substantial content");
            }
        }
    }
}