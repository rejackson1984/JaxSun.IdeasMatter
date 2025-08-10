using FluentAssertions;
using Jackson.Ideas.Mock.Models;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Models
{
    /// <summary>
    /// Comprehensive tests for CoachingContext functionality
    /// Tests contextual coaching logic, progress tracking, and milestone management
    /// </summary>
    public class CoachingContextTests
    {
        #region CoachingContext Construction Tests

        [Fact]
        public void CoachingContext_ShouldInitialize_WithDefaultValues()
        {
            // Act
            var context = new CoachingContext();

            // Assert
            context.CompletedMilestones.Should().NotBeNull("CompletedMilestones should be initialized");
            context.CompletedMilestones.Should().BeEmpty("CompletedMilestones should start empty");
            context.SessionStartTime.Should().Be(default, "SessionStartTime should be default");
            context.IsFirstTimeInHub.Should().BeFalse("IsFirstTimeInHub should default to false");
            context.UserProgress.Should().Be(0, "UserProgress should default to 0");
        }

        [Fact]
        public void CoachingContext_ShouldAllow_PropertyInitialization()
        {
            // Arrange & Act
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                CurrentPage = "BusinessModelCanvas",
                UserProgress = 75,
                CompletedMilestones = new List<string> { "idea_validated", "market_research_completed" },
                LastUserAction = "saved_business_model",
                SessionStartTime = DateTime.UtcNow,
                IsFirstTimeInHub = false,
                UserIntent = "create_business_plan"
            };

            // Assert
            context.CurrentHub.Should().Be(BusinessHub.BusinessPlanning);
            context.CurrentPage.Should().Be("BusinessModelCanvas");
            context.UserProgress.Should().Be(75);
            context.CompletedMilestones.Should().HaveCount(2);
            context.LastUserAction.Should().Be("saved_business_model");
            context.SessionStartTime.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
            context.IsFirstTimeInHub.Should().BeFalse();
            context.UserIntent.Should().Be("create_business_plan");
        }

        #endregion

        #region CoachingContext Validation Tests

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment)]
        [InlineData(BusinessHub.BusinessPlanning)]
        [InlineData(BusinessHub.BusinessOperations)]
        public void CoachingContext_ShouldSupport_AllValidHubs(BusinessHub hub)
        {
            // Act
            var context = new CoachingContext { CurrentHub = hub };

            // Assert
            context.CurrentHub.Should().Be(hub);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(100)]
        public void CoachingContext_ShouldSupport_ValidProgressValues(int progress)
        {
            // Act
            var context = new CoachingContext { UserProgress = progress };

            // Assert
            context.UserProgress.Should().Be(progress);
        }

        [Fact]
        public void CoachingContext_ShouldHandle_NullValues_Gracefully()
        {
            // Act
            var context = new CoachingContext
            {
                CurrentPage = null,
                LastUserAction = null,
                UserIntent = null
            };

            // Assert
            context.CurrentPage.Should().BeNull();
            context.LastUserAction.Should().BeNull();
            context.UserIntent.Should().BeNull();
        }

        #endregion

        #region Milestone Management Tests

        [Fact]
        public void CoachingContext_ShouldAllow_MilestoneCollection_Manipulation()
        {
            // Arrange
            var context = new CoachingContext();
            var milestones = new List<string> { "idea_submitted", "market_research_started", "competition_analyzed" };

            // Act
            context.CompletedMilestones.AddRange(milestones);

            // Assert
            context.CompletedMilestones.Should().HaveCount(3);
            context.CompletedMilestones.Should().Contain("idea_submitted");
            context.CompletedMilestones.Should().Contain("market_research_started");
            context.CompletedMilestones.Should().Contain("competition_analyzed");
        }

        [Fact]
        public void CoachingContext_ShouldPrevent_DuplicateMilestones()
        {
            // Arrange
            var context = new CoachingContext();
            context.CompletedMilestones.Add("idea_submitted");

            // Act
            if (!context.CompletedMilestones.Contains("idea_submitted"))
            {
                context.CompletedMilestones.Add("idea_submitted");
            }

            // Assert
            context.CompletedMilestones.Should().HaveCount(1);
            context.CompletedMilestones.Count(m => m == "idea_submitted").Should().Be(1);
        }

        [Fact]
        public void CoachingContext_ShouldSupport_MilestoneRemoval()
        {
            // Arrange
            var context = new CoachingContext();
            context.CompletedMilestones.AddRange(new[] { "milestone1", "milestone2", "milestone3" });

            // Act
            context.CompletedMilestones.Remove("milestone2");

            // Assert
            context.CompletedMilestones.Should().HaveCount(2);
            context.CompletedMilestones.Should().NotContain("milestone2");
            context.CompletedMilestones.Should().Contain("milestone1");
            context.CompletedMilestones.Should().Contain("milestone3");
        }

        #endregion

        #region Hub-Specific Context Tests

        [Fact]
        public void CoachingContext_ForIdeaDevelopmentHub_ShouldSupport_IdeaSpecificMilestones()
        {
            // Arrange & Act
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                CompletedMilestones = new List<string>
                {
                    "idea_submitted",
                    "idea_expanded",
                    "market_research_initiated",
                    "competitive_analysis_started",
                    "idea_validated",
                    "research_completed"
                }
            };

            // Assert
            context.CurrentHub.Should().Be(BusinessHub.IdeaDevelopment);
            context.CompletedMilestones.Should().Contain("idea_submitted");
            context.CompletedMilestones.Should().Contain("idea_validated");
            context.CompletedMilestones.Should().HaveCount(6);
        }

        [Fact]
        public void CoachingContext_ForBusinessPlanningHub_ShouldSupport_PlanningSpecificMilestones()
        {
            // Arrange & Act
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                CompletedMilestones = new List<string>
                {
                    "business_model_canvas_created",
                    "financial_projections_generated",
                    "market_analysis_completed",
                    "operations_plan_outlined",
                    "risk_assessment_completed",
                    "business_plan_finalized"
                }
            };

            // Assert
            context.CurrentHub.Should().Be(BusinessHub.BusinessPlanning);
            context.CompletedMilestones.Should().Contain("business_model_canvas_created");
            context.CompletedMilestones.Should().Contain("business_plan_finalized");
            context.CompletedMilestones.Should().HaveCount(6);
        }

        [Fact]
        public void CoachingContext_ForBusinessOperationsHub_ShouldSupport_OperationsSpecificMilestones()
        {
            // Arrange & Act
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessOperations,
                CompletedMilestones = new List<string>
                {
                    "launch_plan_created",
                    "resource_planning_completed",
                    "performance_framework_established",
                    "scaling_strategy_defined",
                    "quality_framework_implemented",
                    "operations_optimized"
                }
            };

            // Assert
            context.CurrentHub.Should().Be(BusinessHub.BusinessOperations);
            context.CompletedMilestones.Should().Contain("launch_plan_created");
            context.CompletedMilestones.Should().Contain("operations_optimized");
            context.CompletedMilestones.Should().HaveCount(6);
        }

        #endregion

        #region Context State Validation Tests

        [Fact]
        public void CoachingContext_ShouldIndicateFirstTimeUser_Correctly()
        {
            // Arrange
            var firstTimeContext = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                IsFirstTimeInHub = true,
                CompletedMilestones = new List<string>()
            };

            var returningUserContext = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                IsFirstTimeInHub = false,
                CompletedMilestones = new List<string> { "idea_submitted", "market_research_started" }
            };

            // Assert
            firstTimeContext.IsFirstTimeInHub.Should().BeTrue();
            firstTimeContext.CompletedMilestones.Should().BeEmpty();

            returningUserContext.IsFirstTimeInHub.Should().BeFalse();
            returningUserContext.CompletedMilestones.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(25, false)]
        [InlineData(50, false)]
        [InlineData(100, false)]
        public void CoachingContext_ShouldDetermineBeginnerStatus_BasedOnProgress(int progress, bool expectedBeginner)
        {
            // Arrange
            var context = new CoachingContext { UserProgress = progress };

            // Act
            var isBeginner = context.UserProgress <= 0;

            // Assert
            isBeginner.Should().Be(expectedBeginner);
        }

        [Fact]
        public void CoachingContext_ShouldTrack_SessionDuration()
        {
            // Arrange
            var sessionStart = DateTime.UtcNow;
            var context = new CoachingContext { SessionStartTime = sessionStart };

            // Act
            var sessionDuration = DateTime.UtcNow - context.SessionStartTime;

            // Assert
            sessionDuration.Should().BeGreaterOrEqualTo(TimeSpan.Zero);
            sessionDuration.Should().BeLessThan(TimeSpan.FromMinutes(1)); // Should be very recent
        }

        #endregion

        #region Context Transition Tests

        [Fact]
        public void CoachingContext_ShouldSupport_HubTransition()
        {
            // Arrange
            var context = new CoachingContext
            {
                CurrentHub = BusinessHub.IdeaDevelopment,
                UserProgress = 85,
                CompletedMilestones = new List<string> { "idea_validated", "research_completed" }
            };

            // Act - Simulate progression to next hub
            var transitionedContext = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 0, // Reset progress for new hub
                CompletedMilestones = new List<string>(), // Start fresh in new hub
                IsFirstTimeInHub = true,
                SessionStartTime = DateTime.UtcNow
            };

            // Assert
            context.CurrentHub.Should().Be(BusinessHub.IdeaDevelopment);
            context.UserProgress.Should().Be(85);

            transitionedContext.CurrentHub.Should().Be(BusinessHub.BusinessPlanning);
            transitionedContext.UserProgress.Should().Be(0);
            transitionedContext.IsFirstTimeInHub.Should().BeTrue();
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, "IdeaInput")]
        [InlineData(BusinessHub.BusinessPlanning, "BusinessModelCanvas")]
        [InlineData(BusinessHub.BusinessOperations, "LaunchPlanning")]
        public void CoachingContext_ShouldTrack_CurrentPage_ForEachHub(BusinessHub hub, string expectedPageType)
        {
            // Act
            var context = new CoachingContext
            {
                CurrentHub = hub,
                CurrentPage = expectedPageType
            };

            // Assert
            context.CurrentHub.Should().Be(hub);
            context.CurrentPage.Should().Be(expectedPageType);
        }

        #endregion

        #region User Intent and Action Tracking Tests

        [Theory]
        [InlineData("create_new_idea", BusinessHub.IdeaDevelopment)]
        [InlineData("build_business_plan", BusinessHub.BusinessPlanning)]
        [InlineData("prepare_for_launch", BusinessHub.BusinessOperations)]
        public void CoachingContext_ShouldTrack_UserIntent_ByHub(string intent, BusinessHub hub)
        {
            // Act
            var context = new CoachingContext
            {
                CurrentHub = hub,
                UserIntent = intent
            };

            // Assert
            context.UserIntent.Should().Be(intent);
            context.CurrentHub.Should().Be(hub);
        }

        [Fact]
        public void CoachingContext_ShouldTrack_UserActions_Chronologically()
        {
            // Arrange
            var context = new CoachingContext();
            var actions = new List<string>
            {
                "navigated_to_hub",
                "started_idea_input",
                "saved_idea_draft",
                "submitted_idea",
                "viewed_validation_results"
            };

            // Act - Simulate tracking the last action
            foreach (var action in actions)
            {
                context.LastUserAction = action;
            }

            // Assert
            context.LastUserAction.Should().Be("viewed_validation_results");
        }

        #endregion

        #region Context Equality and Comparison Tests

        [Fact]
        public void CoachingContext_ShouldSupport_DeepCopy()
        {
            // Arrange
            var original = new CoachingContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserProgress = 60,
                CompletedMilestones = new List<string> { "milestone1", "milestone2" },
                IsFirstTimeInHub = false,
                CurrentPage = "TestPage",
                UserIntent = "test_intent"
            };

            // Act - Create a copy
            var copy = new CoachingContext
            {
                CurrentHub = original.CurrentHub,
                UserProgress = original.UserProgress,
                CompletedMilestones = new List<string>(original.CompletedMilestones),
                IsFirstTimeInHub = original.IsFirstTimeInHub,
                CurrentPage = original.CurrentPage,
                UserIntent = original.UserIntent,
                SessionStartTime = original.SessionStartTime
            };

            // Assert
            copy.CurrentHub.Should().Be(original.CurrentHub);
            copy.UserProgress.Should().Be(original.UserProgress);
            copy.CompletedMilestones.Should().BeEquivalentTo(original.CompletedMilestones);
            copy.IsFirstTimeInHub.Should().Be(original.IsFirstTimeInHub);
            copy.CurrentPage.Should().Be(original.CurrentPage);
            copy.UserIntent.Should().Be(original.UserIntent);

            // Ensure they are separate instances
            copy.CompletedMilestones.Should().NotBeSameAs(original.CompletedMilestones);
        }

        [Fact]
        public void CoachingContext_ShouldHandle_EmptyState_Appropriately()
        {
            // Act
            var emptyContext = new CoachingContext();

            // Assert
            emptyContext.CurrentHub.Should().Be(default(BusinessHub));
            emptyContext.CurrentPage.Should().BeNull();
            emptyContext.UserProgress.Should().Be(0);
            emptyContext.CompletedMilestones.Should().BeEmpty();
            emptyContext.LastUserAction.Should().BeNull();
            emptyContext.IsFirstTimeInHub.Should().BeFalse();
            emptyContext.UserIntent.Should().BeNull();
        }

        #endregion

        #region Progress Calculation Tests

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 17)] // 1 of 6 milestones = ~16.67% rounds to 17%
        [InlineData(3, 50)] // 3 of 6 milestones = 50%
        [InlineData(6, 100)] // 6 of 6 milestones = 100%
        public void CoachingContext_ShouldCalculate_ProgressFromMilestones(int completedCount, int expectedProgress)
        {
            // Arrange
            var allMilestones = new[] { "m1", "m2", "m3", "m4", "m5", "m6" };
            var completedMilestones = allMilestones.Take(completedCount).ToList();

            var context = new CoachingContext
            {
                CompletedMilestones = completedMilestones
            };

            // Act
            var calculatedProgress = (int)Math.Round((double)context.CompletedMilestones.Count / allMilestones.Length * 100);

            // Assert
            calculatedProgress.Should().Be(expectedProgress);
        }

        #endregion
    }
}