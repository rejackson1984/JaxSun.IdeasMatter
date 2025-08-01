using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Mock;
using FluentAssertions;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Services
{
    public class HubConfigurationServiceTests
    {
        private readonly MockHubConfigurationService _hubConfigService;

        public HubConfigurationServiceTests()
        {
            _hubConfigService = new MockHubConfigurationService();
        }

        [Fact]
        public async Task GetHubMetadataAsync_ShouldReturn_ValidMetadata_ForHub1()
        {
            // Act
            var metadata = await _hubConfigService.GetHubMetadataAsync(BusinessHub.IdeaDevelopment);

            // Assert
            metadata.Should().NotBeNull();
            metadata.Hub.Should().Be(BusinessHub.IdeaDevelopment);
            metadata.Name.Should().Be("Idea Development");
            metadata.Description.Should().NotBeNullOrEmpty();
            metadata.Icon.Should().Be("fas fa-lightbulb");
            metadata.PrimaryColor.Should().NotBeNullOrEmpty();
            metadata.CoachPersona.Should().Be("Spark");
            metadata.Features.Should().NotBeEmpty();
            metadata.UnlockThreshold.Should().Be(0); // Hub 1 has no unlock threshold
        }

        [Fact]
        public async Task GetHubMetadataAsync_ShouldReturn_ValidMetadata_ForHub2()
        {
            // Act
            var metadata = await _hubConfigService.GetHubMetadataAsync(BusinessHub.BusinessPlanning);

            // Assert
            metadata.Should().NotBeNull();
            metadata.Hub.Should().Be(BusinessHub.BusinessPlanning);
            metadata.Name.Should().Be("Business Planning");
            metadata.Description.Should().NotBeNullOrEmpty();
            metadata.Icon.Should().Be("fas fa-clipboard-list");
            metadata.CoachPersona.Should().Be("Strategy");
            metadata.Features.Should().NotBeEmpty();
            metadata.RequiredMilestones.Should().NotBeEmpty();
            metadata.UnlockThreshold.Should().Be(70); // Hub 2 unlocks at 70% Hub 1 completion
        }

        [Fact]
        public async Task GetHubMetadataAsync_ShouldReturn_ValidMetadata_ForHub3()
        {
            // Act
            var metadata = await _hubConfigService.GetHubMetadataAsync(BusinessHub.BusinessOperations);

            // Assert
            metadata.Should().NotBeNull();
            metadata.Hub.Should().Be(BusinessHub.BusinessOperations);
            metadata.Name.Should().Be("Business Operations");
            metadata.Description.Should().NotBeNullOrEmpty();
            metadata.Icon.Should().Be("fas fa-chart-line");
            metadata.CoachPersona.Should().Be("Execute");
            metadata.Features.Should().NotBeEmpty();
            metadata.RequiredMilestones.Should().NotBeEmpty();
            metadata.UnlockThreshold.Should().Be(80); // Hub 3 unlocks at 80% Hub 2 completion
        }

        [Fact]
        public async Task GetAllHubMetadataAsync_ShouldReturn_AllThreeHubs()
        {
            // Act
            var allMetadata = await _hubConfigService.GetAllHubMetadataAsync();

            // Assert
            allMetadata.Should().NotBeNull();
            allMetadata.Should().HaveCount(3);
            allMetadata.Should().ContainKey(BusinessHub.IdeaDevelopment);
            allMetadata.Should().ContainKey(BusinessHub.BusinessPlanning);
            allMetadata.Should().ContainKey(BusinessHub.BusinessOperations);
        }

        [Fact]
        public void GetDefaultHub_ShouldReturn_IdeaDevelopment()
        {
            // Act
            var defaultHub = _hubConfigService.GetDefaultHub();

            // Assert
            defaultHub.Should().Be(BusinessHub.IdeaDevelopment);
        }

        [Fact]
        public async Task ShouldUnlockHubAsync_Hub1_ShouldAlways_ReturnTrue()
        {
            // Arrange
            var context = new HubContext();

            // Act
            var shouldUnlock = await _hubConfigService.ShouldUnlockHubAsync(BusinessHub.IdeaDevelopment, context);

            // Assert
            shouldUnlock.Should().BeTrue("Hub 1 should always be unlocked");
        }

        [Theory]
        [InlineData(70, true)]
        [InlineData(69, false)]
        [InlineData(100, true)]
        [InlineData(0, false)]
        public async Task ShouldUnlockHubAsync_Hub2_ShouldDependOn_Hub1Completion(int hub1Completion, bool expectedUnlock)
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = hub1Completion,
                        IsUnlocked = true
                    }
                }
            };

            // Act
            var shouldUnlock = await _hubConfigService.ShouldUnlockHubAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            shouldUnlock.Should().Be(expectedUnlock, 
                $"Hub 2 should {(expectedUnlock ? "be unlocked" : "remain locked")} when Hub 1 is {hub1Completion}% complete");
        }

        [Theory]
        [InlineData(80, true)]
        [InlineData(79, false)]
        [InlineData(100, true)]
        [InlineData(0, false)]
        public async Task ShouldUnlockHubAsync_Hub3_ShouldDependOn_Hub2Completion(int hub2Completion, bool expectedUnlock)
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = hub2Completion,
                        IsUnlocked = true
                    }
                }
            };

            // Act
            var shouldUnlock = await _hubConfigService.ShouldUnlockHubAsync(BusinessHub.BusinessOperations, context);

            // Assert
            shouldUnlock.Should().Be(expectedUnlock, 
                $"Hub 3 should {(expectedUnlock ? "be unlocked" : "remain locked")} when Hub 2 is {hub2Completion}% complete");
        }

        [Fact]
        public async Task ShouldUnlockHubAsync_Hub2_ShouldReturnFalse_WhenHub1DoesNotExist()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>()
            };

            // Act
            var shouldUnlock = await _hubConfigService.ShouldUnlockHubAsync(BusinessHub.BusinessPlanning, context);

            // Assert
            shouldUnlock.Should().BeFalse("Hub 2 should not unlock when Hub 1 progress doesn't exist");
        }

        [Fact]
        public async Task ShouldUnlockHubAsync_Hub3_ShouldReturnFalse_WhenHub2DoesNotExist()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>()
            };

            // Act
            var shouldUnlock = await _hubConfigService.ShouldUnlockHubAsync(BusinessHub.BusinessOperations, context);

            // Assert
            shouldUnlock.Should().BeFalse("Hub 3 should not unlock when Hub 2 progress doesn't exist");
        }

        [Fact]
        public async Task GetUnlockRequirementsAsync_ShouldReturn_EmptyList_ForHub1()
        {
            // Act
            var requirements = await _hubConfigService.GetUnlockRequirementsAsync(BusinessHub.IdeaDevelopment);

            // Assert
            requirements.Should().BeEmpty("Hub 1 has no unlock requirements");
        }

        [Fact]
        public async Task GetUnlockRequirementsAsync_ShouldReturn_RequiredMilestones_ForHub2()
        {
            // Act
            var requirements = await _hubConfigService.GetUnlockRequirementsAsync(BusinessHub.BusinessPlanning);

            // Assert
            requirements.Should().NotBeEmpty("Hub 2 should have unlock requirements");
            requirements.Should().Contain("idea_validated");
            requirements.Should().Contain("market_research_complete");
            requirements.Should().Contain("competitive_analysis_done");
        }

        [Fact]
        public async Task GetUnlockRequirementsAsync_ShouldReturn_RequiredMilestones_ForHub3()
        {
            // Act
            var requirements = await _hubConfigService.GetUnlockRequirementsAsync(BusinessHub.BusinessOperations);

            // Assert
            requirements.Should().NotBeEmpty("Hub 3 should have unlock requirements");
            requirements.Should().Contain("business_plan_complete");
            requirements.Should().Contain("financial_model_validated");
            requirements.Should().Contain("technical_architecture_defined");
            requirements.Should().Contain("development_roadmap_created");
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, BusinessHub.IdeaDevelopment, true)]
        [InlineData(BusinessHub.BusinessPlanning, BusinessHub.IdeaDevelopment, true)]
        [InlineData(BusinessHub.BusinessOperations, BusinessHub.BusinessPlanning, true)]
        [InlineData(BusinessHub.BusinessOperations, BusinessHub.IdeaDevelopment, true)]
        public async Task CanTransitionToHubAsync_ShouldAllow_BackwardTransitions(BusinessHub fromHub, BusinessHub toHub, bool expectedResult)
        {
            // Arrange
            var context = new HubContext();

            // Act
            var canTransition = await _hubConfigService.CanTransitionToHubAsync(fromHub, toHub, context);

            // Assert
            canTransition.Should().Be(expectedResult, "Should allow transitions to lower-numbered hubs");
        }

        [Fact]
        public async Task CanTransitionToHubAsync_ShouldDependOn_UnlockStatus_ForForwardTransitions()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = true
                    }
                }
            };

            // Act
            var canTransition = await _hubConfigService.CanTransitionToHubAsync(
                BusinessHub.IdeaDevelopment, 
                BusinessHub.BusinessPlanning, 
                context);

            // Assert
            canTransition.Should().BeTrue("Should allow transition to unlocked higher-numbered hub");
        }

        [Fact]
        public async Task CanTransitionToHubAsync_ShouldPrevent_TransitionToLockedHub()
        {
            // Arrange
            var context = new HubContext
            {
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        IsUnlocked = false
                    }
                }
            };

            // Act
            var canTransition = await _hubConfigService.CanTransitionToHubAsync(
                BusinessHub.IdeaDevelopment, 
                BusinessHub.BusinessPlanning, 
                context);

            // Assert
            canTransition.Should().BeFalse("Should prevent transition to locked higher-numbered hub");
        }

        [Fact]
        public async Task HubMetadata_ShouldHave_UniqueCoachPersonas()
        {
            // Act
            var allMetadata = await _hubConfigService.GetAllHubMetadataAsync();

            // Assert
            var coachPersonas = allMetadata.Values.Select(m => m.CoachPersona).ToList();
            coachPersonas.Should().OnlyHaveUniqueItems("Each hub should have a unique coach persona");
            coachPersonas.Should().Contain("Spark", "Strategy", "Execute");
        }

        [Fact]
        public async Task HubMetadata_ShouldHave_UniqueIcons()
        {
            // Act
            var allMetadata = await _hubConfigService.GetAllHubMetadataAsync();

            // Assert
            var icons = allMetadata.Values.Select(m => m.Icon).ToList();
            icons.Should().OnlyHaveUniqueItems("Each hub should have a unique icon");
            icons.Should().Contain("fas fa-lightbulb", "fas fa-clipboard-list", "fas fa-chart-line");
        }

        [Fact]
        public async Task HubMetadata_Features_ShouldBe_HubSpecific()
        {
            // Act
            var allMetadata = await _hubConfigService.GetAllHubMetadataAsync();

            // Assert
            var hub1Features = allMetadata[BusinessHub.IdeaDevelopment].Features;
            var hub2Features = allMetadata[BusinessHub.BusinessPlanning].Features;
            var hub3Features = allMetadata[BusinessHub.BusinessOperations].Features;

            // Hub 1 should focus on idea development
            hub1Features.Should().Contain(f => f.Contains("Idea") || f.Contains("Market Research") || f.Contains("Validation"));

            // Hub 2 should focus on business planning
            hub2Features.Should().Contain(f => f.Contains("Business Plan") || f.Contains("Strategic") || f.Contains("Financial"));

            // Hub 3 should focus on operations
            hub3Features.Should().Contain(f => f.Contains("KPI") || f.Contains("Analytics") || f.Contains("Growth") || f.Contains("CEO"));
        }
    }
}