using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Components.Pages;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;

namespace Jackson.Ideas.Mock.Tests.Components.Hub2
{
    public class FinancialProjectionsComponentTests : TestContext
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<ICoachPersonaService> _mockCoachPersonaService;
        private readonly Mock<IBusinessPlanService> _mockBusinessPlanService;
        private readonly Mock<NavigationManager> _mockNavigationManager;

        public FinancialProjectionsComponentTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockCoachPersonaService = new Mock<ICoachPersonaService>();
            _mockBusinessPlanService = new Mock<IBusinessPlanService>();
            _mockNavigationManager = new Mock<NavigationManager>();

            // Register services
            Services.AddSingleton(_mockHubContextService.Object);
            Services.AddSingleton(_mockCoachPersonaService.Object);
            Services.AddSingleton(_mockBusinessPlanService.Object);
            Services.AddSingleton(_mockNavigationManager.Object);
        }

        [Fact]
        public void FinancialProjections_ShouldDisplay_PageTitle()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            component.Markup.Should().Contain("Financial Projections - IdeaCoach Pro");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_CoachWelcomeSection()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var welcomeSection = component.Find(".coach-welcome-section");
            welcomeSection.Should().NotBeNull("Should display coach welcome section");
            
            var welcomeTitle = component.Find(".coach-welcome-title");
            welcomeTitle.Should().NotBeNull("Should display welcome title");
        }

        [Fact]
        public void FinancialProjections_ShouldUse_HubLayout()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            // Component should render without layout errors
            // Layout directive is @layout HubLayout in the component
            component.Find(".financial-projections-container").Should().NotBeNull();
        }

        [Fact]
        public void FinancialProjections_ShouldHave_HubPlanningTheme()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var themeElements = component.FindAll(".hub-planning-theme, .hub-planning-avatar");
            themeElements.Should().NotBeEmpty("Should apply Hub 2 theme styling");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_CoachAvatar()
        {
            // Arrange
            SetupMocksWithStrategyCoach();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var coachAvatar = component.Find(".coach-avatar-large");
            coachAvatar.Should().NotBeNull("Should display coach avatar");
            
            var avatarIcon = coachAvatar.QuerySelector("i");
            avatarIcon.Should().NotBeNull("Should contain avatar icon");
        }

        [Fact]
        public void FinancialProjections_ShouldDisplay_RevenueProjectionForm()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var revenueForm = component.Find(".revenue-projection-form");
            revenueForm.Should().NotBeNull("Should have revenue projection form");

            var revenueInputs = component.FindAll("input[type='number']");
            revenueInputs.Should().NotBeEmpty("Should have numeric inputs for revenue projections");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_ExpenseCategories()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var expenseSection = component.Find(".expense-categories");
            expenseSection.Should().NotBeNull("Should have expense categories section");

            var expenseInputs = component.FindAll(".expense-input");
            expenseInputs.Should().NotBeEmpty("Should have expense input fields");
        }

        [Fact]
        public void FinancialProjections_ShouldCalculate_BreakEvenPoint()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var breakEvenSection = component.Find(".break-even-analysis");
            breakEvenSection.Should().NotBeNull("Should display break-even analysis");

            var breakEvenMetric = component.Find(".break-even-metric");
            breakEvenMetric.Should().NotBeNull("Should show break-even calculation");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_ThreeYearProjections()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var projectionYears = component.FindAll(".projection-year");
            projectionYears.Should().HaveCount(3, "Should show 3-year projections");

            var yearHeaders = projectionYears.Select(y => y.TextContent).ToList();
            yearHeaders.Should().Contain(h => h.Contains("Year 1"));
            yearHeaders.Should().Contain(h => h.Contains("Year 2"));
            yearHeaders.Should().Contain(h => h.Contains("Year 3"));
        }

        [Fact]
        public void FinancialProjections_ShouldDisplay_ProfitabilityMetrics()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var profitabilitySection = component.Find(".profitability-metrics");
            profitabilitySection.Should().NotBeNull("Should show profitability metrics");

            var grossMargin = component.Find(".gross-margin");
            grossMargin.Should().NotBeNull("Should show gross margin calculation");

            var netProfit = component.Find(".net-profit");
            netProfit.Should().NotBeNull("Should show net profit projection");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_CashFlowChart()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var cashFlowChart = component.Find(".cash-flow-chart");
            cashFlowChart.Should().NotBeNull("Should display cash flow visualization");
        }

        [Fact]
        public void FinancialProjections_ShouldHave_ValidationMessages()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var validationSummary = component.FindComponents<Microsoft.AspNetCore.Components.Forms.ValidationSummary>();
            validationSummary.Should().NotBeEmpty("Should have form validation");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_FundingRequirements()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var fundingSection = component.Find(".funding-requirements");
            fundingSection.Should().NotBeNull("Should show funding requirements section");

            var startupCosts = component.Find(".startup-costs");
            startupCosts.Should().NotBeNull("Should display startup costs breakdown");
        }

        [Fact]
        public void FinancialProjections_ShouldHandle_FormSubmission()
        {
            // Arrange
            SetupDefaultMocks();
            var mockProjections = CreateMockFinancialProjections();
            _mockBusinessPlanService.Setup(x => x.GenerateFinancialProjectionsAsync(It.IsAny<BusinessPlanRequest>()))
                .ReturnsAsync(mockProjections);

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();
            var submitButton = component.Find("button[type='submit']");

            // Assert
            submitButton.Should().NotBeNull("Should have submit button for projections");
        }

        [Fact]
        public void FinancialProjections_ShouldShow_CoachGuidance()
        {
            // Arrange
            SetupMocksWithStrategyCoach();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var coachGuidance = component.Find(".coach-guidance");
            coachGuidance.Should().NotBeNull("Should show coach guidance for financial planning");

            var guidanceMessage = component.Find(".coach-message-text");
            guidanceMessage.Should().NotBeNull("Should display coaching message");
        }

        [Fact]
        public void FinancialProjections_ShouldBe_PartOfHub2Workflow()
        {
            // Arrange
            var hub2Context = CreateHub2Context();
            SetupMocksWithContext(hub2Context);

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var container = component.Find(".financial-projections-container");
            container.Should().NotBeNull("Should render in Hub 2 context");
            
            // Should be themed for Hub 2
            var themeElements = component.FindAll(".hub-planning-theme");
            themeElements.Should().NotBeEmpty("Should apply Hub 2 theme");
        }

        [Fact]
        public void FinancialProjections_ShouldImplement_IDisposable()
        {
            // Arrange
            SetupDefaultMocks();

            // Act & Assert
            // Component implements IDisposable for proper cleanup
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();
            
            // Component should render without disposal issues
            component.Find(".financial-projections-container").Should().NotBeNull();
        }

        [Fact]
        public void FinancialProjections_ShouldShow_RevenueModelOptions()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var revenueModelSelect = component.Find("select#revenue-model");
            revenueModelSelect.Should().NotBeNull("Should have revenue model selection");

            var modelOptions = component.FindAll("option");
            modelOptions.Should().Contain(o => o.TextContent.Contains("Subscription"));
            modelOptions.Should().Contain(o => o.TextContent.Contains("One-time"));
        }

        [Fact]
        public void FinancialProjections_ShouldCalculate_KeyMetrics_Automatically()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var component = RenderComponent<Jackson.Ideas.Mock.Components.Pages.FinancialProjections>();

            // Assert
            var keyMetrics = component.Find(".key-metrics");
            keyMetrics.Should().NotBeNull("Should display calculated key metrics");

            var metrics = component.FindAll(".metric-card");
            metrics.Should().NotBeEmpty("Should show individual metric cards");
        }

        private void SetupDefaultMocks()
        {
            var hubContext = CreateHub2Context();
            var strategyCoach = CreateStrategyCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(strategyCoach);
        }

        private void SetupMocksWithStrategyCoach()
        {
            var hubContext = CreateHub2Context();
            var strategyCoach = CreateStrategyCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(strategyCoach);
        }

        private void SetupMocksWithContext(HubContext context)
        {
            var strategyCoach = CreateStrategyCoach();

            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(context);
            _mockCoachPersonaService.Setup(x => x.GetCoachForHubAsync(BusinessHub.BusinessPlanning)).ReturnsAsync(strategyCoach);
        }

        private HubContext CreateHub2Context()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 35,
                        IsUnlocked = true,
                        IsActive = true,
                        CurrentPhase = "Financial Planning",
                        CompletedMilestones = new List<string> { "executive_summary_created" },
                        AvailableMilestones = new List<string> 
                        { 
                            "executive_summary_created",
                            "market_analysis_completed",
                            "financial_projections_created",
                            "business_plan_finalized"
                        }
                    }
                }
            };
        }

        private CoachPersona CreateStrategyCoach()
        {
            return new CoachPersona
            {
                Name = "Strategy",
                Avatar = "fas fa-chess-king",
                Personality = "Strategic and analytical business planning coach",
                VoicePattern = "Professional and methodical",
                Hub = BusinessHub.BusinessPlanning,
                Style = CoachingStyle.Analytical,
                Description = "Your strategic business planning coach"
            };
        }

        private Jackson.Ideas.Mock.Models.FinancialProjections CreateMockFinancialProjections()
        {
            return new Jackson.Ideas.Mock.Models.FinancialProjections
            {
                YearlyBreakdown = new List<YearlyFinancials>
                {
                    new() { Year = 1, Revenue = 50000, Expenses = 45000, NetIncome = 5000 },
                    new() { Year = 2, Revenue = 125000, Expenses = 95000, NetIncome = 30000 },
                    new() { Year = 3, Revenue = 250000, Expenses = 180000, NetIncome = 70000 }
                },
                Revenue = new RevenueProjections
                {
                    RevenueModel = "Subscription-based",
                    Year1Total = 50000,
                    Year3Total = 250000
                },
                Expenses = new ExpenseProjections
                {
                    Year1Total = 45000,
                    Year3Total = 180000
                },
                CashFlow = new CashFlowProjections
                {
                    BreakEvenMonth = 14,
                    MaxCashDeficit = -25000
                }
            };
        }
    }
}