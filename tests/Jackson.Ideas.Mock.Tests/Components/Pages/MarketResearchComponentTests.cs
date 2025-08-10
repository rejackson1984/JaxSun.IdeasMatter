using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Jackson.Ideas.Mock.Components.Pages;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Models;
using Moq;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Jackson.Ideas.Mock.Tests.Components.Pages
{
    /// <summary>
    /// Comprehensive tests for MarketResearch component to ensure it loads and renders correctly,
    /// preventing the empty middle area issue that was reported.
    /// </summary>
    public class MarketResearchComponentTests : TestContext
    {
        private readonly Mock<IMarketResearchService> _mockMarketResearchService;
        private readonly Mock<IMockDataService> _mockMockDataService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public MarketResearchComponentTests()
        {
            _mockMarketResearchService = new Mock<IMarketResearchService>();
            _mockMockDataService = new Mock<IMockDataService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            SetupMockServices();
            RegisterServices();
        }

        private void SetupMockServices()
        {
            // Setup market research service with comprehensive mock data
            _mockMarketResearchService.Setup(x => x.GetMarketResearchAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateComprehensiveMarketResearchData());

            // Setup mock data service with test scenarios
            _mockMockDataService.Setup(x => x.GetAllScenariosAsync())
                .ReturnsAsync(CreateTestScenarios());

            // Setup navigation manager
            _mockNavigationManager.Setup(x => x.NavigateTo(It.IsAny<string>(), It.IsAny<bool>()))
                .Verifiable();
        }

        private void RegisterServices()
        {
            Services.AddScoped(_ => _mockMarketResearchService.Object);
            Services.AddScoped(_ => _mockMockDataService.Object);
            Services.AddScoped(_ => _mockNavigationManager.Object);
        }

        [Fact]
        public async Task MarketResearch_ShouldLoadAndDisplayContent_OnInitialization()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            
            // Wait for async operations to complete
            await Task.Delay(100);

            // Assert - Check that the main container exists
            var container = component.Find(".container-fluid");
            container.Should().NotBeNull("Main container should exist");

            // Check for page title
            var pageTitle = component.Find("h1");
            pageTitle.Should().NotBeNull("Page title should exist");
            pageTitle.TextContent.Should().Contain("Market Research Analysis", 
                "Page should display the correct title");

            // Check for scenario selector
            var scenarioSelector = component.Find(".card");
            scenarioSelector.Should().NotBeNull("Scenario selector card should exist");
        }

        [Fact]
        public async Task MarketResearch_ShouldDisplayMarketSegmentation_WhenDataLoaded()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow async loading

            // Assert
            var marketSegmentationCard = component.Find("h5:contains('Market Segmentation')");
            marketSegmentationCard.Should().NotBeNull("Market segmentation section should exist");

            // Check for total market size
            var totalMarketSize = component.Find(".fw-bold:contains('$50B')");
            totalMarketSize.Should().NotBeNull("Total market size should be displayed");

            // Check for market segments
            var segments = component.FindAll(".card-title");
            segments.Should().NotBeEmpty("Market segments should be displayed");
        }

        [Fact]
        public async Task MarketResearch_ShouldDisplayCompetitiveAnalysis_WithCompetitorData()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow async loading

            // Assert
            var competitiveAnalysisHeader = component.Find("h5:contains('Competitive Analysis')");
            competitiveAnalysisHeader.Should().NotBeNull("Competitive analysis section should exist");

            // Check for competitors
            var competitorName = component.Find("h6:contains('TechCorp Inc')");
            competitorName.Should().NotBeNull("Competitor information should be displayed");

            // Check for market share badge
            var marketShareBadge = component.Find(".badge:contains('25%')");
            marketShareBadge.Should().NotBeNull("Market share should be displayed");

            // Check for competitive advantages
            var advantages = component.FindAll("li.text-success");
            advantages.Should().NotBeEmpty("Competitive advantages should be listed");
        }

        [Fact]
        public async Task MarketResearch_ShouldDisplayMarketTrends_WithTrendData()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow async loading

            // Assert
            var trendsHeader = component.Find("h5:contains('Market Trends')");
            trendsHeader.Should().NotBeNull("Market trends section should exist");

            // Check for market maturity
            var maturityText = component.Find("strong:contains('Market Maturity')");
            maturityText.Should().NotBeNull("Market maturity should be displayed");

            // Check for emerging trends
            var emergingTrendsHeader = component.Find("h6:contains('Emerging Trends')");
            emergingTrendsHeader.Should().NotBeNull("Emerging trends section should exist");

            var trendsList = component.FindAll("li:contains('AI Integration')");
            trendsList.Should().NotBeEmpty("Specific trends should be listed");
        }

        [Fact]
        public async Task MarketResearch_ShouldDisplayCustomerInsights_WithDetailedData()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow async loading

            // Assert
            var customerInsightsHeader = component.Find("h5:contains('Customer Insights')");
            customerInsightsHeader.Should().NotBeNull("Customer insights section should exist");

            // Check for pain points
            var painPointsHeader = component.Find("h6:contains('Pain Points')");
            painPointsHeader.Should().NotBeNull("Pain points section should exist");

            var painPoints = component.FindAll("li.text-danger");
            painPoints.Should().NotBeEmpty("Pain points should be listed");

            // Check for customer lifetime value
            var ltvElement = component.Find("strong:contains('Customer LTV')");
            ltvElement.Should().NotBeNull("Customer lifetime value should be displayed");
        }

        [Fact]
        public async Task MarketResearch_ShouldDisplayRegulatoryEnvironment_WithComplianceInfo()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow async loading

            // Assert
            var regulatoryHeader = component.Find("h5:contains('Regulatory Environment')");
            regulatoryHeader.Should().NotBeNull("Regulatory environment section should exist");

            // Check for regulatory risk badge
            var riskBadge = component.Find(".badge:contains('Medium')");
            riskBadge.Should().NotBeNull("Regulatory risk should be displayed");

            // Check for key regulations
            var regulationsHeader = component.Find("h6:contains('Key Regulations')");
            regulationsHeader.Should().NotBeNull("Key regulations section should exist");

            var regulations = component.FindAll("li:contains('GDPR')");
            regulations.Should().NotBeEmpty("Specific regulations should be listed");
        }

        [Fact]
        public async Task MarketResearch_ShouldHandleScenarioSelection_AndUpdateContent()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow initial loading

            // Find and click a scenario button
            var scenarioButtons = component.FindAll(".btn-outline-primary");
            scenarioButtons.Should().NotBeEmpty("Scenario buttons should exist");

            var firstScenarioButton = scenarioButtons.First();
            await firstScenarioButton.ClickAsync();

            // Assert
            // Verify that the service was called with the scenario selection
            _mockMarketResearchService.Verify(x => x.GetMarketResearchAsync(It.IsAny<string>()), 
                Times.AtLeast(2), "Market research service should be called when scenario changes");

            // Check that content is still displayed after scenario change
            var container = component.Find(".container-fluid");
            container.Should().NotBeNull("Container should still exist after scenario selection");
        }

        [Fact]
        public async Task MarketResearch_ShouldShowLoadingState_BeforeDataIsAvailable()
        {
            // Arrange - Setup service to delay response
            _mockMarketResearchService.Setup(x => x.GetMarketResearchAsync(It.IsAny<string>()))
                .Returns(Task.Delay(1000).ContinueWith(_ => CreateComprehensiveMarketResearchData()));

            // Act
            var component = RenderComponent<MarketResearch>();

            // Assert - Check for loading spinner initially
            var spinner = component.Find(".spinner-border");
            spinner.Should().NotBeNull("Loading spinner should be displayed initially");

            var loadingText = component.Find("p:contains('Loading market research data')");
            loadingText.Should().NotBeNull("Loading text should be displayed");
        }

        [Fact]
        public async Task MarketResearch_BackToDashboard_ShouldNavigateCorrectly()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow loading

            var backButton = component.Find("button:contains('Back to Dashboard')");
            backButton.Should().NotBeNull("Back to dashboard button should exist");

            await backButton.ClickAsync();

            // Assert
            _mockNavigationManager.Verify(x => x.NavigateTo("/dashboard", false), 
                Times.Once, "Should navigate to dashboard when back button is clicked");
        }

        [Fact]
        public async Task MarketResearch_ShouldHaveResponsiveLayout_WithProperBootstrapClasses()
        {
            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow loading

            // Assert - Check for responsive grid classes
            var rows = component.FindAll(".row");
            rows.Should().NotBeEmpty("Should have Bootstrap row classes for responsive layout");

            var columns = component.FindAll("[class*='col-']");
            columns.Should().NotBeEmpty("Should have Bootstrap column classes for responsive layout");

            // Check for specific responsive classes
            var lgColumns = component.FindAll(".col-lg-4, .col-lg-6");
            lgColumns.Should().NotBeEmpty("Should have large screen column classes");

            var mdColumns = component.FindAll(".col-md-2, .col-md-6");
            mdColumns.Should().NotBeEmpty("Should have medium screen column classes");
        }

        [Fact]
        public async Task MarketResearch_ShouldHandleEmptyScenarios_Gracefully()
        {
            // Arrange - Setup empty scenarios
            _mockMockDataService.Setup(x => x.GetAllScenariosAsync())
                .ReturnsAsync(new List<BusinessIdeaScenario>());

            // Act
            var component = RenderComponent<MarketResearch>();
            await Task.Delay(100); // Allow loading

            // Assert - Should still render the basic structure
            var container = component.Find(".container-fluid");
            container.Should().NotBeNull("Container should exist even with no scenarios");

            var pageTitle = component.Find("h1");
            pageTitle.Should().NotBeNull("Page title should still be displayed");
        }

        private MarketResearchData CreateComprehensiveMarketResearchData()
        {
            return new MarketResearchData
            {
                Segmentation = new MarketSegmentation
                {
                    TotalMarketSize = "$50B",
                    ServicableMarketSize = "$5B",
                    TargetMarketSize = "$500M",
                    GrowthRate = "18%",
                    Segments = new List<MarketSegment>
                    {
                        new MarketSegment
                        {
                            Name = "Enterprise Customers",
                            Demographics = "Large corporations with 1000+ employees",
                            Size = "$200M",
                            PurchasingPower = "High"
                        },
                        new MarketSegment
                        {
                            Name = "SMB Customers",
                            Demographics = "Small to medium businesses, 10-999 employees",
                            Size = "$300M",
                            PurchasingPower = "Medium"
                        }
                    }
                },
                Competition = new CompetitiveAnalysis
                {
                    DirectCompetitors = new List<Competitor>
                    {
                        new Competitor
                        {
                            Name = "TechCorp Inc",
                            MarketShare = "25%",
                            Strengths = "Strong brand recognition, established customer base",
                            Weaknesses = "High pricing, slow innovation",
                            PricingStrategy = "Premium pricing",
                            DifferentiationFactor = "Enterprise focus"
                        },
                        new Competitor
                        {
                            Name = "InnovateSoft",
                            MarketShare = "15%",
                            Strengths = "Innovative features, competitive pricing",
                            Weaknesses = "Limited market presence, smaller team",
                            PricingStrategy = "Value pricing",
                            DifferentiationFactor = "Innovation focus"
                        }
                    },
                    CompetitiveAdvantages = new List<string> 
                    { 
                        "Advanced AI capabilities", 
                        "Superior user experience", 
                        "Flexible pricing models",
                        "24/7 customer support"
                    },
                    MarketGaps = new List<string> 
                    { 
                        "Affordable enterprise solutions", 
                        "Mobile-first platforms",
                        "Industry-specific customizations"
                    }
                },
                Trends = new MarketTrends
                {
                    MarketMaturity = "Growth stage with increasing adoption",
                    EmergingTrends = new List<string> 
                    { 
                        "AI Integration", 
                        "Sustainability focus", 
                        "Remote work optimization",
                        "Data privacy emphasis"
                    },
                    TechnologyTrends = new List<string> 
                    { 
                        "Cloud-native architectures", 
                        "Mobile-first design",
                        "API-first development",
                        "Microservices adoption"
                    },
                    ConsumerBehaviorTrends = new List<string> 
                    { 
                        "Preference for self-service", 
                        "Value-conscious purchasing",
                        "Integration requirements",
                        "Security-first mindset"
                    },
                    RegulatoryTrends = new List<string> 
                    { 
                        "Increased data protection laws", 
                        "Environmental compliance requirements",
                        "Accessibility standards",
                        "Cross-border data restrictions"
                    }
                },
                Customer = new CustomerInsights
                {
                    PainPoints = new List<string> 
                    { 
                        "High implementation costs", 
                        "Complex integration processes",
                        "Limited customization options",
                        "Poor customer support"
                    },
                    Motivations = new List<string> 
                    { 
                        "Operational efficiency", 
                        "Cost reduction",
                        "Competitive advantage",
                        "Scalability needs"
                    },
                    PreferredChannels = new List<string> 
                    { 
                        "Online self-service", 
                        "Direct sales team",
                        "Partner referrals",
                        "Industry events"
                    },
                    PurchaseDecisionFactors = "Price, features, support quality, and vendor reputation",
                    CustomerLifetimeValue = "$25,000",
                    AcquisitionCost = "$2,500"
                },
                Regulatory = new RegulatoryEnvironment
                {
                    RegulatoryRisk = "Medium",
                    KeyRegulations = new List<string> 
                    { 
                        "GDPR (General Data Protection Regulation)", 
                        "SOX (Sarbanes-Oxley Act)",
                        "CCPA (California Consumer Privacy Act)",
                        "HIPAA (Health Insurance Portability and Accountability Act)"
                    },
                    ComplianceRequirements = new List<string> 
                    { 
                        "Data protection and privacy measures", 
                        "Financial reporting transparency",
                        "Audit trail maintenance",
                        "User consent management"
                    },
                    UpcomingChanges = new List<string> 
                    { 
                        "Enhanced privacy regulations", 
                        "AI governance frameworks",
                        "Cross-border data transfer restrictions",
                        "Environmental reporting mandates"
                    }
                }
            };
        }

        private List<BusinessIdeaScenario> CreateTestScenarios()
        {
            return new List<BusinessIdeaScenario>
            {
                new BusinessIdeaScenario
                {
                    Id = "scenario-1",
                    Name = "Tech Startup",
                    Description = "AI-powered productivity platform",
                    Industry = "Technology",
                    Stage = "Concept Development"
                },
                new BusinessIdeaScenario
                {
                    Id = "scenario-2", 
                    Name = "E-commerce",
                    Description = "Sustainable fashion marketplace",
                    Industry = "Retail",
                    Stage = "Market Validation"
                },
                new BusinessIdeaScenario
                {
                    Id = "scenario-3",
                    Name = "FinTech",
                    Description = "Digital banking for small businesses",
                    Industry = "Financial Services",
                    Stage = "MVP Development"
                }
            };
        }
    }
}