# Jackson.Ideas E2E Tests

This project contains Playwright-based end-to-end tests specifically designed to verify that the empty middle area issue is resolved and that all navigation functionality works correctly.

## Overview

These tests use Microsoft Playwright to automate browser interactions and verify:
- Navigation to `/market-research` and other pages works correctly
- Content loads properly without empty middle areas
- Sidebar navigation functions correctly
- Responsive design works at different viewport sizes
- Layout structure is maintained across page refreshes

## Test Categories

### EmptyMiddleAreaTests
- **MarketResearchPage_ShouldNotHaveEmptyMiddleArea**: Verifies the market research page loads with proper content
- **SidebarNavigation_ShouldWorkCorrectly**: Tests sidebar menu functionality  
- **PageContent_ShouldLoadCompletelyAndNotBeEmpty**: Ensures dynamic content loads properly
- **ResponsiveDesign_ShouldWorkAtDifferentViewportSizes**: Tests mobile/tablet layouts
- **CSSLayout_ShouldHaveCorrectLayoutProperties**: Validates CSS layout properties
- **EndToEndFlow_ShouldWorkFromStartupToMarketResearch**: Tests complete user journey

### NavigationTests
- **SidebarNavigation_AllMenuItems_ShouldWork**: Tests all navigation routes
- **SidebarInteraction_ShouldWorkOnAllViewports**: Tests responsive navigation
- **DirectUrlAccess_ShouldWorkWithoutEmptyAreas**: Tests direct URL navigation
- **PageRefresh_ShouldMaintainContentAndLayout**: Tests page refresh behavior

## Running the Tests

### Prerequisites
1. Install Playwright browsers:
   ```bash
   pwsh bin/Debug/net9.0/playwright.ps1 install
   ```

### Run Tests
```bash
# Run all E2E tests
dotnet test tests/Jackson.Ideas.E2E.Tests/

# Run specific test class
dotnet test tests/Jackson.Ideas.E2E.Tests/ --filter "EmptyMiddleAreaTests"

# Run with visible browser (for debugging)
PLAYWRIGHT_HEADLESS=false dotnet test tests/Jackson.Ideas.E2E.Tests/
```

## Test Configuration

### Application Startup
The tests automatically:
1. Check if the application is running on `localhost:5000`
2. If not running, start the Mock application automatically
3. Wait for the application to be ready before running tests
4. Clean up the application process after tests complete

### Browser Configuration
- Default: Headless Chromium browser
- Viewport: 1920x1080 for desktop tests
- Multiple viewport sizes tested for responsive design
- Screenshots captured for visual verification

### Screenshots
Tests automatically capture screenshots in the following scenarios:
- After each successful page load
- When testing different viewport sizes
- When errors occur (for debugging)
- Before and after page refreshes

Screenshots are saved with descriptive names like:
- `market-research-page-content.png`
- `navigation-market_research.png`
- `sidebar-interaction-375x667.png`

## Test Strategy

### Validation Approach
1. **Structural Validation**: Verify DOM elements exist and are visible
2. **Content Validation**: Ensure pages have actual content, not just loading states
3. **Layout Validation**: Check CSS properties and responsive behavior
4. **Interaction Validation**: Test user interactions like button clicks and navigation
5. **Visual Validation**: Screenshots for manual review if needed

### Empty Middle Area Detection
The tests specifically check for:
- Main content area (`.hub-main-content`) is visible
- Content wrapper (`.hub-content-wrapper`) has child elements
- Specific page elements load correctly
- Loading spinners disappear after content loads
- CSS layout properties are correct (margins, positioning, etc.)

## Troubleshooting

### Common Issues
1. **Application won't start**: Check if port 5000 is available
2. **Tests timeout**: Increase timeout values in test configuration
3. **Content not loading**: Check for JavaScript errors in browser console
4. **Layout issues**: Review CSS properties validation in tests

### Debug Mode
Run tests with visible browser to see what's happening:
```bash
PLAYWRIGHT_HEADLESS=false dotnet test tests/Jackson.Ideas.E2E.Tests/ --filter "MarketResearchPage_ShouldNotHaveEmptyMiddleArea"
```

### Log Output
Tests include detailed console output showing:
- Application startup progress
- Page navigation attempts
- Element visibility checks
- Error messages with context

## Continuous Integration

These tests are designed to:
- Run automatically in CI/CD pipelines
- Fail fast if the empty middle area issue reoccurs
- Provide visual evidence through screenshots
- Validate browser compatibility across different viewport sizes

## Maintenance

When adding new pages or routes:
1. Add the route to `NavigationTests.routesToTest`
2. Implement route-specific content validation
3. Update viewport size tests if needed
4. Add screenshots for visual verification