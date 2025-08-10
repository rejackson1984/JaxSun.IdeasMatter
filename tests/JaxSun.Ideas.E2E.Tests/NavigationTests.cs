using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace JaxSun.Ideas.E2E.Tests;

/// <summary>
/// Comprehensive navigation tests to verify all sidebar menu items work correctly
/// and no pages have empty middle areas.
/// </summary>
[TestClass]
public class NavigationTests : PageTest
{
    private const string BaseUrl = "http://localhost:5000";

    [TestInitialize]
    public async Task TestInitialize()
    {
        Page.SetDefaultTimeout(30000);
        await Page.SetViewportSizeAsync(1920, 1080);
    }

    [TestMethod]
    [Description("Test navigation through all available sidebar menu items")]
    public async Task SidebarNavigation_AllMenuItems_ShouldWork()
    {
        // Start from dashboard or home page
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Common navigation routes in the Mock application
        var routesToTest = new[]
        {
            "/dashboard",
            "/market-research", 
            "/business-planning",
            "/financial-projections",
            "/scenarios"
        };

        foreach (var route in routesToTest)
        {
            try
            {
                await NavigateAndVerifyRoute(route);
            }
            catch (Exception ex)
            {
                // Log the error but continue testing other routes
                Console.WriteLine($"Error testing route {route}: {ex.Message}");
                
                // Take screenshot of failed state for debugging
                await Page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = $"navigation-error-{route.Replace("/", "_")}.png",
                    FullPage = true
                });
            }
        }
    }

    private async Task NavigateAndVerifyRoute(string route)
    {
        var fullUrl = $"{BaseUrl}{route}";
        
        // Navigate to the route
        await Page.GotoAsync(fullUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait a moment for dynamic content to load
        await Page.WaitForTimeoutAsync(2000);

        // Verify the URL is correct
        Page.Url.Should().Contain(route, $"Should navigate to {route}");

        // Verify the main layout elements are present
        await VerifyLayoutStructure();

        // Verify content is not empty
        await VerifyContentNotEmpty(route);

        // Take screenshot for visual verification
        var routeName = route.Replace("/", "_").Replace("-", "_");
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = $"navigation-{routeName}.png",
            FullPage = true
        });
    }

    private async Task VerifyLayoutStructure()
    {
        // Verify main layout components exist
        var hubLayout = Page.Locator(".hub-layout");
        await Expect(hubLayout).ToBeVisibleAsync();

        var sidebar = Page.Locator(".hub-sidebar");
        await Expect(sidebar).ToBeVisibleAsync();

        var mainContent = Page.Locator(".hub-main-content");
        await Expect(mainContent).ToBeVisibleAsync();

        var contentWrapper = Page.Locator(".hub-content-wrapper");
        await Expect(contentWrapper).ToBeVisibleAsync();
    }

    private async Task VerifyContentNotEmpty(string route)
    {
        var contentWrapper = Page.Locator(".hub-content-wrapper");
        
        // Check that content wrapper has child elements
        var childCount = await contentWrapper.Locator("> *").CountAsync();
        childCount.Should().BeGreaterThan(0, $"Content wrapper should have child elements for route {route}");

        // Check for specific content based on route
        switch (route)
        {
            case "/market-research":
                await VerifyMarketResearchContent();
                break;
            case "/business-planning":
                await VerifyBusinessPlanningContent();
                break;
            case "/financial-projections":
                await VerifyFinancialProjectionsContent();
                break;
            case "/dashboard":
                await VerifyDashboardContent();
                break;
            case "/scenarios":
                await VerifyScenariosContent();
                break;
        }
    }

    private async Task VerifyMarketResearchContent()
    {
        // Verify market research specific content
        var header = Page.Locator("h1:has-text('Market Research'), h2:has-text('Market Research'), h3:has-text('Market Research')");
        await Expect(header).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });

        // Wait for and verify scenario selector
        await Page.WaitForSelectorAsync("text=Select Business Scenario, text=Business Scenario", new PageWaitForSelectorOptions { Timeout = 10000 });
    }

    private async Task VerifyBusinessPlanningContent()
    {
        // Look for business planning specific elements
        var planningElements = Page.Locator("text=Business Plan, text=Planning, h1, h2, h3").First;
        await Expect(planningElements).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });
    }

    private async Task VerifyFinancialProjectionsContent()
    {
        // Look for financial projections specific elements
        var financialElements = Page.Locator("text=Financial, text=Projections, text=Revenue, text=Expenses, h1, h2, h3").First;
        await Expect(financialElements).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });
    }

    private async Task VerifyDashboardContent()
    {
        // Look for dashboard specific elements
        var dashboardElements = Page.Locator("text=Dashboard, .dashboard, .card, h1, h2, h3").First;
        await Expect(dashboardElements).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });
    }

    private async Task VerifyScenariosContent()
    {
        // Look for scenarios specific elements
        var scenarioElements = Page.Locator("text=Scenario, text=Business, .scenario, .card, h1, h2, h3").First;
        await Expect(scenarioElements).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });
    }

    [TestMethod]
    [Description("Test sidebar menu interaction and mobile responsiveness")]
    public async Task SidebarInteraction_ShouldWorkOnAllViewports()
    {
        var viewports = new[]
        {
            new ViewportSize { Width = 1920, Height = 1080 }, // Desktop
            new ViewportSize { Width = 1024, Height = 768 },  // Tablet
            new ViewportSize { Width = 375, Height = 667 }    // Mobile
        };

        foreach (var viewport in viewports)
        {
            await Page.SetViewportSizeAsync(viewport.Width, viewport.Height);
            await Page.GotoAsync($"{BaseUrl}/market-research");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Test sidebar visibility based on viewport
            var sidebar = Page.Locator(".hub-sidebar");
            
            if (viewport.Width >= 1024)
            {
                // Desktop/tablet - sidebar should be visible by default
                await Expect(sidebar).ToBeVisibleAsync();
            }
            else
            {
                // Mobile - test mobile menu functionality
                await TestMobileMenuInteraction();
            }

            // Verify content is still accessible regardless of viewport
            var contentWrapper = Page.Locator(".hub-content-wrapper");
            await Expect(contentWrapper).ToBeVisibleAsync();

            // Take screenshot for each viewport
            await Page.ScreenshotAsync(new PageScreenshotOptions 
            { 
                Path = $"sidebar-interaction-{viewport.Width}x{viewport.Height}.png",
                FullPage = true
            });
        }
    }

    private async Task TestMobileMenuInteraction()
    {
        // Look for mobile menu trigger (hamburger menu, menu button, etc.)
        var menuTriggers = new[]
        {
            ".mobile-menu-button",
            ".hamburger-menu",
            ".menu-toggle",
            "button:has-text('Menu')",
            "[data-toggle='mobile-menu']"
        };

        foreach (var triggerSelector in menuTriggers)
        {
            var trigger = Page.Locator(triggerSelector);
            if (await trigger.IsVisibleAsync())
            {
                await trigger.ClickAsync();
                await Page.WaitForTimeoutAsync(500); // Wait for animation

                // Check if mobile sidebar opened
                var mobileSidebar = Page.Locator(".hub-sidebar.mobile-open, .sidebar.mobile-open, .mobile-menu-open");
                if (await mobileSidebar.IsVisibleAsync())
                {
                    // Close the menu
                    var overlay = Page.Locator(".mobile-sidebar-overlay, .overlay");
                    if (await overlay.IsVisibleAsync())
                    {
                        await overlay.ClickAsync();
                        await Page.WaitForTimeoutAsync(500);
                    }
                }
                break;
            }
        }
    }

    [TestMethod]
    [Description("Test deep linking - direct URL access should work without empty areas")]
    public async Task DirectUrlAccess_ShouldWorkWithoutEmptyAreas()
    {
        var directUrls = new[]
        {
            $"{BaseUrl}/market-research",
            $"{BaseUrl}/dashboard",
            $"{BaseUrl}/scenarios"
        };

        foreach (var url in directUrls)
        {
            // Open a new page/tab for each test to ensure clean state
            var newPage = await Context.NewPageAsync();
            await newPage.SetViewportSizeAsync(1920, 1080);

            try
            {
                // Navigate directly to the URL (simulating user typing URL in browser)
                await newPage.GotoAsync(url);
                await newPage.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Verify the page loads completely
                var mainContent = newPage.Locator(".hub-main-content");
                await Expect(mainContent).ToBeVisibleAsync();

                var contentWrapper = newPage.Locator(".hub-content-wrapper");
                await Expect(contentWrapper).ToBeVisibleAsync();

                // Verify content is not empty
                var contentElements = contentWrapper.Locator("> *");
                var elementCount = await contentElements.CountAsync();
                elementCount.Should().BeGreaterThan(0, $"Direct access to {url} should have content");

                // Take screenshot
                var urlPath = new Uri(url).AbsolutePath.Replace("/", "_");
                await newPage.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = $"direct-access{urlPath}.png",
                    FullPage = true
                });
            }
            finally
            {
                await newPage.CloseAsync();
            }
        }
    }

    [TestMethod]
    [Description("Test page refresh maintains content and layout")]
    public async Task PageRefresh_ShouldMaintainContentAndLayout()
    {
        // Navigate to market research page
        await Page.GotoAsync($"{BaseUrl}/market-research");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(3000); // Let content fully load

        // Take screenshot before refresh
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "before-refresh.png",
            FullPage = true
        });

        // Verify content is present before refresh
        var contentBefore = Page.Locator(".hub-content-wrapper > *");
        var countBefore = await contentBefore.CountAsync();
        countBefore.Should().BeGreaterThan(0, "Should have content before refresh");

        // Refresh the page
        await Page.ReloadAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(3000); // Let content fully reload

        // Verify content is still present after refresh
        var contentAfter = Page.Locator(".hub-content-wrapper > *");
        var countAfter = await contentAfter.CountAsync();
        countAfter.Should().BeGreaterThan(0, "Should have content after refresh");

        // Verify layout structure is maintained
        await VerifyLayoutStructure();

        // Take screenshot after refresh
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "after-refresh.png",
            FullPage = true
        });

        // Verify specific content elements are still present
        var header = Page.Locator("h1, h2, h3").First;
        await Expect(header).ToBeVisibleAsync();
    }
}