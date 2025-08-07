using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Jackson.Ideas.E2E.Tests;

/// <summary>
/// Playwright E2E tests specifically targeting the empty middle area issue.
/// These tests verify that navigation works correctly and content is visible.
/// </summary>
[TestClass]
public class EmptyMiddleAreaTests : PageTest
{
    private const string BaseUrl = "http://localhost:5000";
    private const string MarketResearchUrl = $"{BaseUrl}/market-research";

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Set default timeout for all tests
        Page.SetDefaultTimeout(30000);
        
        // Configure viewport for consistent testing
        await Page.SetViewportSizeAsync(1920, 1080);
    }

    [TestMethod]
    [Description("Verify that the market-research page loads without empty middle area")]
    public async Task MarketResearchPage_ShouldNotHaveEmptyMiddleArea()
    {
        // Navigate to the market research page directly
        await Page.GotoAsync(MarketResearchUrl);

        // Wait for the page to fully load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Verify the page title is correct
        var title = await Page.TitleAsync();
        title.Should().Contain("Market Research");

        // Verify the main content area exists and is visible
        var mainContent = Page.Locator(".hub-main-content");
        await Expect(mainContent).ToBeVisibleAsync();

        // Verify the content wrapper exists and has content
        var contentWrapper = Page.Locator(".hub-content-wrapper");
        await Expect(contentWrapper).ToBeVisibleAsync();
        
        // Verify specific content elements are visible (not empty middle area)
        var pageHeader = Page.Locator("h1:has-text('Market Research Analysis')");
        await Expect(pageHeader).ToBeVisibleAsync();

        // Verify scenario selector is visible
        var scenarioSelector = Page.Locator("text=Select Business Scenario");
        await Expect(scenarioSelector).ToBeVisibleAsync();

        // Verify market segmentation card is visible
        var marketSegmentation = Page.Locator("text=Market Segmentation");
        await Expect(marketSegmentation).ToBeVisibleAsync();

        // Verify competitive analysis card is visible
        var competitiveAnalysis = Page.Locator("text=Competitive Analysis");
        await Expect(competitiveAnalysis).ToBeVisibleAsync();

        // Verify market trends card is visible
        var marketTrends = Page.Locator("text=Market Trends");
        await Expect(marketTrends).ToBeVisibleAsync();

        // Take a screenshot for visual verification
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "market-research-page-content.png",
            FullPage = true
        });
    }

    [TestMethod]
    [Description("Verify sidebar navigation menu items work correctly")]
    public async Task SidebarNavigation_ShouldWorkCorrectly()
    {
        // Start from the market research page
        await Page.GotoAsync(MarketResearchUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify sidebar is visible
        var sidebar = Page.Locator(".hub-sidebar");
        await Expect(sidebar).ToBeVisibleAsync();

        // Verify hub navigation menu is present
        var navMenu = Page.Locator("nav, .nav-menu, .hub-nav-menu").First;
        await Expect(navMenu).ToBeVisibleAsync();

        // Test navigation to dashboard (Back to Dashboard button)
        var backButton = Page.Locator("button:has-text('Back to Dashboard')");
        if (await backButton.IsVisibleAsync())
        {
            await backButton.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Verify we navigated successfully
            var currentUrl = Page.Url;
            currentUrl.Should().Contain("/dashboard");
            
            // Navigate back to market research for other tests
            await Page.GotoAsync(MarketResearchUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    [TestMethod]
    [Description("Verify page content loads properly and is not empty")]
    public async Task PageContent_ShouldLoadCompletelyAndNotBeEmpty()
    {
        await Page.GotoAsync(MarketResearchUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait for dynamic content to load (scenarios and market data)
        await Page.WaitForSelectorAsync("text=Select Business Scenario", new PageWaitForSelectorOptions 
        { 
            Timeout = 10000 
        });

        // Verify scenario buttons are loaded and clickable
        var scenarioButtons = Page.Locator("button[class*='btn']:has-text('')").Filter(new LocatorFilterOptions 
        { 
            HasNotText = "Back to Dashboard" 
        });
        
        var buttonCount = await scenarioButtons.CountAsync();
        buttonCount.Should().BeGreaterThan(0, "Should have scenario buttons loaded");

        // Click first scenario button to test interactivity
        if (buttonCount > 0)
        {
            await scenarioButtons.First.ClickAsync();
            await Page.WaitForTimeoutAsync(2000); // Wait for data to load
        }

        // Verify loading spinner is not permanently visible (content has loaded)
        var loadingSpinner = Page.Locator(".spinner-border");
        var isSpinnerVisible = await loadingSpinner.IsVisibleAsync();
        
        // If spinner is visible, wait for it to disappear
        if (isSpinnerVisible)
        {
            await Expect(loadingSpinner).Not.ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions 
            { 
                Timeout = 15000 
            });
        }

        // Verify actual market data is displayed (not just loading state)
        var marketDataElements = Page.Locator(".card-body").Filter(new LocatorFilterOptions 
        { 
            HasText = "Market Size" 
        });
        await Expect(marketDataElements.First).ToBeVisibleAsync();
    }

    [TestMethod]
    [Description("Test responsive design at different viewport sizes")]
    public async Task ResponsiveDesign_ShouldWorkAtDifferentViewportSizes()
    {
        var viewportSizes = new[]
        {
            new ViewportSize { Width = 1920, Height = 1080 }, // Desktop
            new ViewportSize { Width = 1024, Height = 768 },  // Tablet
            new ViewportSize { Width = 768, Height = 1024 },  // Mobile landscape
            new ViewportSize { Width = 375, Height = 667 }    // Mobile portrait
        };

        foreach (var viewport in viewportSizes)
        {
            await Page.SetViewportSizeAsync(viewport.Width, viewport.Height);
            await Page.GotoAsync(MarketResearchUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify main content is visible at this viewport size
            var mainContent = Page.Locator(".hub-main-content");
            await Expect(mainContent).ToBeVisibleAsync();

            // Verify content wrapper adapts properly
            var contentWrapper = Page.Locator(".hub-content-wrapper");
            await Expect(contentWrapper).ToBeVisibleAsync();

            // Verify header is still visible
            var header = Page.Locator("h1:has-text('Market Research Analysis')");
            await Expect(header).ToBeVisibleAsync();

            // Take screenshot for visual verification at each size
            await Page.ScreenshotAsync(new PageScreenshotOptions 
            { 
                Path = $"market-research-{viewport.Width}x{viewport.Height}.png",
                FullPage = true
            });

            // For mobile sizes, test mobile menu functionality if present
            if (viewport.Width <= 1024)
            {
                var mobileMenuButton = Page.Locator("button:has([class*='hamburger'], [class*='menu-toggle'], [class*='mobile-menu'])");
                if (await mobileMenuButton.IsVisibleAsync())
                {
                    await mobileMenuButton.ClickAsync();
                    await Page.WaitForTimeoutAsync(500);
                    
                    // Verify mobile menu opened
                    var mobileSidebar = Page.Locator(".hub-sidebar.mobile-open, .sidebar.mobile-open");
                    if (await mobileSidebar.IsVisibleAsync())
                    {
                        // Close mobile menu
                        var overlay = Page.Locator(".mobile-sidebar-overlay, .overlay");
                        if (await overlay.IsVisibleAsync())
                        {
                            await overlay.ClickAsync();
                        }
                    }
                }
            }
        }
    }

    [TestMethod]
    [Description("Verify CSS layout properties are correct and prevent empty middle areas")]
    public async Task CSSLayout_ShouldHaveCorrectLayoutProperties()
    {
        await Page.GotoAsync(MarketResearchUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check main content area has correct CSS properties
        var mainContent = Page.Locator(".hub-main-content");
        
        // Verify main content has proper margin-left for sidebar
        var marginLeft = await mainContent.EvaluateAsync("el => window.getComputedStyle(el).marginLeft");
        marginLeft.ToString().Should().NotBe("0px", "Main content should have left margin for sidebar");

        // Verify content wrapper has proper padding
        var contentWrapper = Page.Locator(".hub-content-wrapper");
        var padding = await contentWrapper.EvaluateAsync("el => window.getComputedStyle(el).padding");
        padding.Should().NotBe("0px", "Content wrapper should have padding");

        // Verify layout flexbox is working correctly
        var hubLayout = Page.Locator(".hub-layout");
        var display = await hubLayout.EvaluateAsync("el => window.getComputedStyle(el).display");
        display.Should().Be("flex", "Hub layout should use flexbox");

        // Verify sidebar has correct positioning
        var sidebar = Page.Locator(".hub-sidebar");
        var position = await sidebar.EvaluateAsync("el => window.getComputedStyle(el).position");
        position.Should().Be("fixed", "Sidebar should be fixed positioned");

        var width = await sidebar.EvaluateAsync("el => window.getComputedStyle(el).width");
        width.ToString().Should().Contain("280px", "Sidebar should have correct width");
    }

    [TestMethod]
    [Description("SYSTEM-WIDE VERIFICATION: Test ALL critical pages for empty middle area fix")]
    public async Task SystemWideVerification_AllCriticalPages_ShouldHaveContent()
    {
        // Define all critical pages that should render content in middle area
        var criticalPages = new Dictionary<string, string>
        {
            { "/market-research", "Market Research Analysis" },
            { "/dashboard", "Dashboard" },
            { "/scenarios", "Business Scenario" },
            { "/progress", "Progress" },
            { "/idea-input", "Idea Input" },
            { "/financial-projections", "Financial" },
            { "/business-plan-builder", "Business Plan" },
            { "/action-plan", "Action Plan" },
            { "/operations-dashboard", "Operations" },
            { "/business-model-canvas", "Business Model" }
        };

        var failedPages = new List<string>();
        var successfulPages = new List<string>();

        foreach (var (route, expectedContent) in criticalPages)
        {
            try
            {
                Console.WriteLine($"Testing page: {route}");
                
                // Navigate to the page
                var fullUrl = $"{BaseUrl}{route}";
                await Page.GotoAsync(fullUrl);
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Page.WaitForTimeoutAsync(3000); // Allow dynamic content to load

                // Verify basic layout structure exists
                await VerifyPageLayoutStructure(route);

                // Verify middle area has content
                await VerifyMiddleAreaHasContent(route);

                // Verify page-specific content
                await VerifyPageSpecificContent(route, expectedContent);

                // Take screenshot as evidence
                var routeName = route.Replace("/", "_").Replace("-", "_");
                await Page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = $"system-wide-verification{routeName}.png",
                    FullPage = true
                });

                successfulPages.Add(route);
                Console.WriteLine($"✓ PASSED: {route} - Content verified");
            }
            catch (Exception ex)
            {
                failedPages.Add(route);
                Console.WriteLine($"✗ FAILED: {route} - {ex.Message}");
                
                // Take screenshot of failed state
                var routeName = route.Replace("/", "_").Replace("-", "_");
                await Page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = $"system-wide-FAILED{routeName}.png",
                    FullPage = true
                });
            }
        }

        // Report results
        Console.WriteLine($"\n=== SYSTEM-WIDE VERIFICATION RESULTS ===");
        Console.WriteLine($"SUCCESSFUL PAGES ({successfulPages.Count}):");
        foreach (var page in successfulPages)
        {
            Console.WriteLine($"  ✓ {page}");
        }

        if (failedPages.Any())
        {
            Console.WriteLine($"\nFAILED PAGES ({failedPages.Count}):");
            foreach (var page in failedPages)
            {
                Console.WriteLine($"  ✗ {page}");
            }
        }

        // Assert overall success
        failedPages.Should().BeEmpty($"All critical pages should have content. Failed pages: {string.Join(", ", failedPages)}");
        
        Console.WriteLine($"\n🎉 SUCCESS: All {successfulPages.Count} critical pages have content in middle area!");
        Console.WriteLine("✅ System-wide empty middle area issue has been RESOLVED!");
    }

    private async Task VerifyPageLayoutStructure(string route)
    {
        // Verify main layout components exist
        var hubLayout = Page.Locator(".hub-layout, .layout, .main-layout").First;
        await Expect(hubLayout).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });

        var mainContent = Page.Locator(".hub-main-content, .main-content, .content").First;
        await Expect(mainContent).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });

        var contentWrapper = Page.Locator(".hub-content-wrapper, .content-wrapper, .page-content").First;
        await Expect(contentWrapper).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 5000 });
    }

    private async Task VerifyMiddleAreaHasContent(string route)
    {
        // Find the content wrapper using multiple selectors
        var contentSelectors = new[]
        {
            ".hub-content-wrapper",
            ".content-wrapper", 
            ".page-content",
            ".main-content > div",
            ".hub-main-content > div"
        };

        ILocator? contentWrapper = null;
        foreach (var selector in contentSelectors)
        {
            var element = Page.Locator(selector);
            if (await element.IsVisibleAsync())
            {
                contentWrapper = element;
                break;
            }
        }

        contentWrapper.Should().NotBeNull($"Could not find content wrapper for route {route}");

        // Verify content wrapper has child elements (not empty)
        var childElements = contentWrapper!.Locator("> *");
        var childCount = await childElements.CountAsync();
        childCount.Should().BeGreaterThan(0, $"Content wrapper should have child elements for route {route}. Middle area appears empty!");

        // Verify content wrapper has visible text or interactive elements
        var hasText = await contentWrapper.TextContentAsync();
        var hasInputs = await contentWrapper.Locator("input, button, select, textarea").CountAsync();
        var hasCards = await contentWrapper.Locator(".card, .panel, .box").CountAsync();

        var hasContent = !string.IsNullOrWhiteSpace(hasText) || hasInputs > 0 || hasCards > 0;
        hasContent.Should().BeTrue($"Content wrapper for {route} appears to be empty - no text, inputs, or cards found");
    }

    private async Task VerifyPageSpecificContent(string route, string expectedContent)
    {
        // Look for page-specific content indicators
        switch (route)
        {
            case "/market-research":
                await VerifyElementExists("text=Select Business Scenario, text=Market Research, text=Market Segmentation");
                break;
            case "/dashboard":
                await VerifyElementExists("text=Dashboard, text=Ideas, text=Progress, .dashboard, .card");
                break;
            case "/scenarios":
                await VerifyElementExists("text=Scenario, text=Business, .scenario-card, .scenario");
                break;
            case "/progress":
                await VerifyElementExists("text=Progress, text=Milestone, text=Achievement, .progress");
                break;
            case "/idea-input":
                await VerifyElementExists("text=Idea, text=Description, input, textarea");
                break;
            case "/financial-projections":
                await VerifyElementExists("text=Financial, text=Revenue, text=Projections, text=Expenses");
                break;
            case "/business-plan-builder":
                await VerifyElementExists("text=Business Plan, text=Executive Summary, text=Market");
                break;
            case "/action-plan":
                await VerifyElementExists("text=Action, text=Plan, text=Task, text=Milestone");
                break;
            case "/operations-dashboard":
                await VerifyElementExists("text=Operations, text=Metrics, text=Performance, .dashboard");
                break;
            case "/business-model-canvas":
                await VerifyElementExists("text=Business Model, text=Canvas, text=Value Proposition");
                break;
        }
    }

    private async Task VerifyElementExists(string selectors)
    {
        var selectorArray = selectors.Split(',').Select(s => s.Trim()).ToArray();
        
        foreach (var selector in selectorArray)
        {
            var element = Page.Locator(selector);
            if (await element.IsVisibleAsync())
            {
                return; // Found at least one matching element
            }
        }
        
        // If we get here, none of the selectors matched
        throw new InvalidOperationException($"None of the expected content selectors were found: {selectors}");
    }

    [TestMethod]
    [Description("Test original issue: Market Research page should not have empty middle area")]
    public async Task MarketResearchPage_OriginalIssue_ShouldBeFixed()
    {
        // This test specifically targets the original reported issue
        Console.WriteLine("Testing ORIGINAL ISSUE: Market Research page empty middle area");
        
        // Navigate to the market research page directly
        await Page.GotoAsync(MarketResearchUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(3000); // Allow time for dynamic content
        
        // Take screenshot for evidence
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "ORIGINAL-ISSUE-market-research-fix-verification.png",
            FullPage = true
        });

        // Verify the specific issue is resolved
        var contentWrapper = Page.Locator(".hub-content-wrapper");
        await Expect(contentWrapper).ToBeVisibleAsync();

        // Verify it's not empty
        var childElements = contentWrapper.Locator("> *");
        var childCount = await childElements.CountAsync();
        childCount.Should().BeGreaterThan(0, "ORIGINAL ISSUE: Market Research middle area should NOT be empty!");

        // Verify specific market research content
        var scenarioSelector = Page.Locator("text=Select Business Scenario");
        await Expect(scenarioSelector).ToBeVisibleAsync();

        var marketCards = Page.Locator(".card");
        var cardCount = await marketCards.CountAsync();
        cardCount.Should().BeGreaterThan(0, "Should have market research cards visible");

        Console.WriteLine("✅ ORIGINAL ISSUE RESOLVED: Market Research page now has content in middle area!");
    }

    [TestMethod]
    [Description("Test navigation between all hub pages works without empty areas")]
    public async Task NavigationFlow_AllHubPages_ShouldMaintainContent()
    {
        var navigationFlow = new[]
        {
            "/dashboard",
            "/market-research",
            "/scenarios", 
            "/progress",
            "/financial-projections",
            "/business-plan-builder"
        };

        foreach (var route in navigationFlow)
        {
            Console.WriteLine($"Testing navigation to: {route}");
            
            await Page.GotoAsync($"{BaseUrl}{route}");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(2000);

            // Verify content is present after navigation
            var contentWrapper = Page.Locator(".hub-content-wrapper, .content-wrapper, .page-content").First;
            await Expect(contentWrapper).ToBeVisibleAsync();

            var childCount = await contentWrapper.Locator("> *").CountAsync();
            childCount.Should().BeGreaterThan(0, $"Navigation to {route} should show content, not empty middle area");

            // Test navigation back to dashboard
            var backButton = Page.Locator("button:has-text('Back to Dashboard'), a[href*='/dashboard']");
            if (await backButton.IsVisibleAsync())
            {
                await backButton.ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Page.WaitForTimeoutAsync(1000);
            }
        }

        Console.WriteLine("✅ NAVIGATION TEST PASSED: All hub pages maintain content during navigation");
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        // Any cleanup needed after each test
        await Task.CompletedTask;
    }
}