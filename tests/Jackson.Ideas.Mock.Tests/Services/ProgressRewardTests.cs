using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Services
{
    /// <summary>
    /// Tests for progress reward system that celebrates user achievements and milestones
    /// Tests the milestone celebration system that provides positive reinforcement during business development
    /// </summary>
    public class ProgressRewardTests
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IGamificationService> _mockGamificationService;
        private readonly Mock<ICelebrationService> _mockCelebrationService;
        private readonly ProgressRewardService _progressRewardService;

        public ProgressRewardTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockGamificationService = new Mock<IGamificationService>();
            _mockCelebrationService = new Mock<ICelebrationService>();
            
            _progressRewardService = new ProgressRewardService(
                _mockHubContextService.Object,
                _mockNotificationService.Object,
                _mockGamificationService.Object,
                _mockCelebrationService.Object
            );
        }

        [Theory]
        [InlineData(25, RewardTier.Bronze, "Quarter Complete")]
        [InlineData(50, RewardTier.Silver, "Halfway Hero")]
        [InlineData(75, RewardTier.Gold, "Excellence Achieved")]
        [InlineData(100, RewardTier.Platinum, "Perfect Completion")]
        public async Task CalculateProgressReward_ShouldReturn_AppropriateReward_ForCompletionLevel(int completionPercentage, RewardTier expectedTier, string expectedName)
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, completionPercentage);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var reward = await _progressRewardService.CalculateProgressRewardAsync(userId, BusinessHub.IdeaDevelopment);

            // Assert
            reward.Should().NotBeNull("Should generate reward for progress");
            reward.Tier.Should().Be(expectedTier, $"Should assign {expectedTier} tier for {completionPercentage}% completion");
            reward.Name.Should().Contain(expectedName, "Should have appropriate reward name");
        }

        [Fact]
        public async Task TriggerMilestoneCelebration_ShouldSend_PersonalizedCelebration()
        {
            // Arrange
            var userId = "test-user";
            var milestone = new ProgressMilestone
            {
                Type = MilestoneType.HubCompletion,
                Name = "Idea Development Complete",
                Description = "Successfully completed Hub 1 with 75% score",
                Points = 100,
                UnlockedFeatures = new List<string> { "Business Planning Hub", "Advanced Analytics" }
            };

            // Act
            await _progressRewardService.TriggerMilestoneCelebrationAsync(userId, milestone);

            // Assert
            _mockCelebrationService.Verify(x => x.TriggerCelebrationAnimationAsync(
                userId, 
                It.Is<CelebrationType>(c => c == CelebrationType.Fireworks)), 
                Times.Once, "Should trigger celebration animation");

            _mockNotificationService.Verify(x => x.SendAchievementNotificationAsync(
                userId, 
                It.IsAny<Achievement>()), 
                Times.Once, "Should send achievement notification");
        }

        [Fact]
        public async Task EvaluateStreakRewards_ShouldProvide_BonusRewards_ForConsistency()
        {
            // Arrange
            var userId = "test-user";
            var streak = new ProgressStreak
            {
                Type = StreakType.Daily,
                Count = 7,
                StartDate = DateTime.UtcNow.AddDays(-7),
                IsActive = true
            };

            _mockGamificationService.Setup(x => x.GetActiveStreaksAsync(userId))
                .ReturnsAsync(new List<ProgressStreak> { streak });

            // Act
            var streakRewards = await _progressRewardService.EvaluateStreakRewardsAsync(userId);

            // Assert
            streakRewards.Should().NotBeEmpty("Should provide streak rewards");
            streakRewards.Should().Contain(r => r.Type == RewardType.Special, "Should include special bonus");
            streakRewards.Should().Contain(r => r.BonusMultiplier > 1.0, "Should provide bonus multiplier");
        }

        [Fact]
        public async Task GenerateProgressSummary_ShouldCreate_ComprehensiveReport()
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateMultiHubProgress();
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);
            _mockGamificationService.Setup(x => x.GetUserAchievementsAsync(userId))
                .ReturnsAsync(CreateSampleAchievements());

            // Act
            var summary = await _progressRewardService.GenerateProgressSummaryAsync(userId);

            // Assert
            summary.Should().NotBeNull("Should generate progress summary");
            summary.TotalScore.Should().BeGreaterThan(0, "Should calculate total score");
            summary.CompletedMilestones.Should().NotBeEmpty("Should list completed milestones");
            summary.NextMilestone.Should().NotBeNull("Should identify next milestone");
            summary.OverallProgress.Should().BeInRange(0, 100, "Should calculate overall progress percentage");
        }

        [Theory]
        [InlineData(MilestoneType.FirstIdea, CelebrationType.Confetti)]
        [InlineData(MilestoneType.HubCompletion, CelebrationType.Fireworks)]
        [InlineData(MilestoneType.PerfectScore, CelebrationType.GoldenConfetti)]
        [InlineData(MilestoneType.StreakAchievement, CelebrationType.Sparkles)]
        public async Task DetermineCelebrationType_ShouldMatch_MilestoneToAnimation(MilestoneType milestoneType, CelebrationType expectedCelebration)
        {
            // Arrange
            var milestone = new ProgressMilestone { Type = milestoneType };

            // Act
            var celebrationType = await _progressRewardService.DetermineCelebrationTypeAsync(milestone);

            // Assert
            celebrationType.Should().Be(expectedCelebration, $"Should use {expectedCelebration} for {milestoneType}");
        }

        [Fact]
        public async Task TrackRewardHistory_ShouldMaintain_UserRewardLog()
        {
            // Arrange
            var userId = "test-user";
            var reward = new ProgressReward
            {
                Name = "Hub Completion Bonus",
                Points = 150,
                Tier = RewardTier.Gold,
                Type = RewardType.Milestone
            };

            // Act
            await _progressRewardService.TrackRewardHistoryAsync(userId, reward);

            // Assert
            _mockGamificationService.Verify(x => x.RecordRewardAsync(
                userId, 
                It.Is<ProgressReward>(r => r.Name == "Hub Completion Bonus" && r.Points == 150)), 
                Times.Once, "Should record reward in history");
        }

        [Fact]
        public async Task CalculateTimeBasedBonus_ShouldProvide_SpeedBonuses()
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 75);
            var completionTime = TimeSpan.FromHours(2); // Completed quickly
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var timeBonus = await _progressRewardService.CalculateTimeBonusAsync(userId, BusinessHub.IdeaDevelopment, completionTime);

            // Assert
            timeBonus.Should().NotBeNull("Should calculate time bonus");
            timeBonus.BonusPercentage.Should().BeGreaterThan(0, "Should provide bonus for quick completion");
            timeBonus.BonusReason.Should().Contain("speed", "Should explain speed bonus");
        }

        [Fact]
        public async Task GetUpcomingRewards_ShouldPreview_AvailableRewards()
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.BusinessPlanning, 60);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var upcomingRewards = await _progressRewardService.GetUpcomingRewardsAsync(userId);

            // Assert
            upcomingRewards.Should().NotBeEmpty("Should preview upcoming rewards");
            upcomingRewards.Should().Contain(r => r.RequiredProgress > 60, "Should show rewards requiring more progress");
            upcomingRewards.Should().BeInAscendingOrder(r => r.RequiredProgress, "Should be ordered by progress requirement");
        }

        [Fact]
        public async Task ProcessBatchRewards_ShouldHandle_MultipleSimultaneousRewards()
        {
            // Arrange
            var userId = "test-user";
            var rewards = new List<ProgressReward>
            {
                new ProgressReward { Name = "Completion Bonus", Points = 100, Type = RewardType.Milestone },
                new ProgressReward { Name = "Speed Bonus", Points = 50, Type = RewardType.Special },
                new ProgressReward { Name = "Quality Bonus", Points = 75, Type = RewardType.Special }
            };

            // Act
            var batchResult = await _progressRewardService.ProcessBatchRewardsAsync(userId, rewards);

            // Assert
            batchResult.Should().NotBeNull("Should process batch rewards");
            batchResult.TotalPoints.Should().Be(225, "Should sum all reward points");
            batchResult.ProcessedRewards.Should().HaveCount(3, "Should process all rewards");
            
            _mockCelebrationService.Verify(x => x.TriggerBatchCelebrationAsync(
                userId, 
                It.Is<List<ProgressReward>>(r => r.Count == 3)), 
                Times.Once, "Should trigger batch celebration");
        }

        [Fact]
        public async Task CreatePersonalizedMessage_ShouldGenerate_ContextualCongratulations()
        {
            // Arrange
            var userId = "test-user";
            var milestone = new ProgressMilestone
            {
                Type = MilestoneType.HubCompletion,
                Name = "Business Planning Complete",
                Achievement = "Completed comprehensive business plan with 85% score"
            };

            // Act
            var message = await _progressRewardService.CreatePersonalizedMessageAsync(userId, milestone);

            // Assert
            message.Should().NotBeEmpty("Should generate personalized message");
            message.Should().Contain("congratulations", "Should be congratulatory");
            message.Should().Contain("business plan", "Should reference specific achievement");
            message.Should().NotContain("error", "Should not contain error messages");
        }

        [Fact]
        public async Task HandleConcurrentRewards_ShouldBe_ThreadSafe()
        {
            // Arrange - Multiple users achieving rewards simultaneously
            var userIds = Enumerable.Range(1, 10).Select(i => $"user{i}").ToArray();
            var milestones = userIds.Select(id => new ProgressMilestone
            {
                Type = MilestoneType.HubCompletion,
                Name = $"Milestone for {id}",
                Points = 100
            });

            // Act - Process rewards for all users concurrently
            var tasks = userIds.Zip(milestones).Select(async pair =>
                await _progressRewardService.TriggerMilestoneCelebrationAsync(pair.First, pair.Second));

            await Task.WhenAll(tasks);

            // Assert - All celebrations should be triggered
            _mockCelebrationService.Verify(x => x.TriggerCelebrationAnimationAsync(
                It.IsAny<string>(), 
                It.IsAny<CelebrationType>()), 
                Times.Exactly(10), "Should trigger celebrations for all users");
        }

        #region Helper Methods

        private HubContext CreateHubContext(BusinessHub currentHub, int completionPercentage)
        {
            return new HubContext
            {
                CurrentHub = currentHub,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = currentHub == BusinessHub.IdeaDevelopment ? completionPercentage : 100,
                        IsUnlocked = true,
                        IsActive = currentHub == BusinessHub.IdeaDevelopment
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = currentHub == BusinessHub.BusinessPlanning ? completionPercentage : 0,
                        IsUnlocked = currentHub != BusinessHub.IdeaDevelopment,
                        IsActive = currentHub == BusinessHub.BusinessPlanning
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = currentHub == BusinessHub.BusinessOperations ? completionPercentage : 0,
                        IsUnlocked = currentHub == BusinessHub.BusinessOperations,
                        IsActive = currentHub == BusinessHub.BusinessOperations
                    }
                }
            };
        }

        private HubContext CreateMultiHubProgress()
        {
            return new HubContext
            {
                CurrentHub = BusinessHub.BusinessPlanning,
                UserId = "test-user",
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 100,
                        IsUnlocked = true,
                        IsActive = false
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 65,
                        IsUnlocked = true,
                        IsActive = true
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false
                    }
                }
            };
        }

        private List<UserAchievement> CreateSampleAchievements()
        {
            return new List<UserAchievement>
            {
                new UserAchievement { Name = "First Idea", Points = 50, EarnedAt = DateTime.UtcNow.AddDays(-5) },
                new UserAchievement { Name = "Idea Master", Points = 100, EarnedAt = DateTime.UtcNow.AddDays(-3) },
                new UserAchievement { Name = "Planning Beginner", Points = 75, EarnedAt = DateTime.UtcNow.AddDays(-1) }
            };
        }

        #endregion
    }

    /// <summary>
    /// Progress reward service for managing milestone celebrations and progress rewards
    /// This would be the actual service implementation being tested
    /// </summary>
    public class ProgressRewardService
    {
        private readonly IHubContextService _hubContextService;
        private readonly INotificationService _notificationService;
        private readonly IGamificationService _gamificationService;
        private readonly ICelebrationService _celebrationService;

        public ProgressRewardService(
            IHubContextService hubContextService,
            INotificationService notificationService,
            IGamificationService gamificationService,
            ICelebrationService celebrationService)
        {
            _hubContextService = hubContextService;
            _notificationService = notificationService;
            _gamificationService = gamificationService;
            _celebrationService = celebrationService;
        }

        public async Task<ProgressReward> CalculateProgressRewardAsync(string userId, BusinessHub hub)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var progress = context.Progress[hub];
            
            var tier = progress.CompletionPercentage switch
            {
                >= 100 => RewardTier.Platinum,
                >= 75 => RewardTier.Gold,
                >= 50 => RewardTier.Silver,
                >= 25 => RewardTier.Bronze,
                _ => RewardTier.None
            };

            var name = progress.CompletionPercentage switch
            {
                >= 100 => "Perfect Completion",
                >= 75 => "Excellence Achieved",
                >= 50 => "Halfway Hero",
                >= 25 => "Quarter Complete",
                _ => "Getting Started"
            };

            return new ProgressReward
            {
                Name = name,
                Tier = tier,
                Points = progress.CompletionPercentage,
                Type = RewardType.Milestone,
                EarnedAt = DateTime.UtcNow
            };
        }

        public async Task TriggerMilestoneCelebrationAsync(string userId, ProgressMilestone milestone)
        {
            var celebrationType = await DetermineCelebrationTypeAsync(milestone);
            
            await _celebrationService.TriggerCelebrationAnimationAsync(userId, celebrationType);
            await _notificationService.SendAchievementNotificationAsync(userId, new Achievement());
        }

        public async Task<List<ProgressReward>> EvaluateStreakRewardsAsync(string userId)
        {
            var streaks = await _gamificationService.GetActiveStreaksAsync(userId);
            var rewards = new List<ProgressReward>();

            foreach (var streak in streaks)
            {
                if (streak.Count >= 7) // Weekly streak
                {
                    rewards.Add(new ProgressReward
                    {
                        Name = "Consistency Champion",
                        Type = RewardType.Special,
                        Points = streak.Count * 10,
                        BonusMultiplier = 1.5,
                        Tier = RewardTier.Gold
                    });
                }
            }

            return rewards;
        }

        public async Task<ProgressSummary> GenerateProgressSummaryAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var achievements = await _gamificationService.GetUserAchievementsAsync(userId);
            
            var totalScore = achievements.Sum(a => a.Points);
            var overallProgress = context.Progress.Values.Average(p => p.CompletionPercentage);
            
            return new ProgressSummary
            {
                UserId = userId,
                TotalScore = totalScore,
                OverallProgress = (int)overallProgress,
                CompletedMilestones = achievements.Select(a => a.Name).ToList(),
                NextMilestone = GetNextMilestone(context),
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<CelebrationType> DetermineCelebrationTypeAsync(ProgressMilestone milestone)
        {
            return milestone.Type switch
            {
                MilestoneType.FirstIdea => CelebrationType.Confetti,
                MilestoneType.HubCompletion => CelebrationType.Fireworks,
                MilestoneType.PerfectScore => CelebrationType.GoldenConfetti,
                MilestoneType.StreakAchievement => CelebrationType.Sparkles,
                _ => CelebrationType.Confetti
            };
        }

        public async Task TrackRewardHistoryAsync(string userId, ProgressReward reward)
        {
            await _gamificationService.RecordRewardAsync(userId, reward);
        }

        public async Task<TimeBonus> CalculateTimeBonusAsync(string userId, BusinessHub hub, TimeSpan completionTime)
        {
            // Award speed bonus for quick completion
            var expectedTime = TimeSpan.FromHours(4); // Expected time to complete hub
            var speedRatio = expectedTime.TotalHours / completionTime.TotalHours;
            
            if (speedRatio > 1.5) // Completed significantly faster
            {
                return new TimeBonus
                {
                    BonusPercentage = Math.Min((speedRatio - 1) * 100, 50), // Cap at 50% bonus
                    BonusReason = $"Completed {speedRatio:F1}x faster than average speed bonus",
                    EarnedAt = DateTime.UtcNow
                };
            }

            return new TimeBonus { BonusPercentage = 0, BonusReason = "Standard completion time" };
        }

        public async Task<List<UpcomingReward>> GetUpcomingRewardsAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var currentProgress = context.Progress[context.CurrentHub].CompletionPercentage;
            
            var upcomingRewards = new List<UpcomingReward>();
            
            for (int threshold = 25; threshold <= 100; threshold += 25)
            {
                if (threshold > currentProgress)
                {
                    upcomingRewards.Add(new UpcomingReward
                    {
                        Name = $"{threshold}% Completion Reward",
                        RequiredProgress = threshold,
                        EstimatedPoints = threshold,
                        RewardType = RewardType.Milestone
                    });
                }
            }

            return upcomingRewards.OrderBy(r => r.RequiredProgress).ToList();
        }

        public async Task<BatchRewardResult> ProcessBatchRewardsAsync(string userId, List<ProgressReward> rewards)
        {
            var totalPoints = rewards.Sum(r => r.Points);
            
            foreach (var reward in rewards)
            {
                await TrackRewardHistoryAsync(userId, reward);
            }

            await _celebrationService.TriggerBatchCelebrationAsync(userId, rewards);

            return new BatchRewardResult
            {
                TotalPoints = totalPoints,
                ProcessedRewards = rewards,
                ProcessedAt = DateTime.UtcNow
            };
        }

        public async Task<string> CreatePersonalizedMessageAsync(string userId, ProgressMilestone milestone)
        {
            var baseMessage = $"🎉 Congratulations! You've achieved {milestone.Name}!";
            var achievementDetail = $"Your accomplishment: {milestone.Achievement}";
            var encouragement = "Keep up the excellent work on your business development journey!";

            return $"{baseMessage} {achievementDetail} {encouragement}";
        }

        private string GetNextMilestone(HubContext context)
        {
            var currentHub = context.Progress[context.CurrentHub];
            
            if (currentHub.CompletionPercentage < 25) return "25% Hub Completion";
            if (currentHub.CompletionPercentage < 50) return "50% Hub Completion";
            if (currentHub.CompletionPercentage < 75) return "75% Hub Completion";
            if (currentHub.CompletionPercentage < 100) return "Hub Completion";
            
            return "Next Hub Unlock";
        }
    }

    #region Supporting Models

    public class ProgressReward
    {
        public string Name { get; set; } = string.Empty;
        public RewardTier Tier { get; set; }
        public int Points { get; set; }
        public RewardType Type { get; set; }
        public double BonusMultiplier { get; set; } = 1.0;
        public DateTime EarnedAt { get; set; }
    }

    public class ProgressMilestone
    {
        public MilestoneType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Achievement { get; set; } = string.Empty;
        public int Points { get; set; }
        public List<string> UnlockedFeatures { get; set; } = new();
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProgressStreak
    {
        public StreakType Type { get; set; }
        public int Count { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProgressSummary
    {
        public string UserId { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public int OverallProgress { get; set; }
        public List<string> CompletedMilestones { get; set; } = new();
        public string NextMilestone { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }

    public class UserAchievement
    {
        public string Name { get; set; } = string.Empty;
        public int Points { get; set; }
        public DateTime EarnedAt { get; set; }
    }

    public class TimeBonus
    {
        public double BonusPercentage { get; set; }
        public string BonusReason { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; }
    }

    public class UpcomingReward
    {
        public string Name { get; set; } = string.Empty;
        public int RequiredProgress { get; set; }
        public int EstimatedPoints { get; set; }
        public RewardType RewardType { get; set; }
    }

    public class BatchRewardResult
    {
        public int TotalPoints { get; set; }
        public List<ProgressReward> ProcessedRewards { get; set; } = new();
        public DateTime ProcessedAt { get; set; }
    }

    public enum RewardTier
    {
        None,
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    // RewardType enum defined in GamificationServiceTests.cs

    public enum MilestoneType
    {
        FirstIdea,
        HubCompletion,
        PerfectScore,
        StreakAchievement,
        QualityAchievement
    }

    public enum CelebrationType
    {
        Confetti,
        Fireworks,
        GoldenConfetti,
        Sparkles,
        Rainbow
    }

    // StreakType enum defined in GamificationServiceTests.cs

    #endregion

    #region Service Interfaces

    public interface IGamificationService
    {
        Task<List<ProgressStreak>> GetActiveStreaksAsync(string userId);
        Task<List<UserAchievement>> GetUserAchievementsAsync(string userId);
        Task RecordRewardAsync(string userId, ProgressReward reward);
    }

    public interface ICelebrationService
    {
        Task TriggerCelebrationAnimationAsync(string userId, CelebrationType type);
        Task TriggerBatchCelebrationAsync(string userId, List<ProgressReward> rewards);
    }

    // INotificationService interface defined in GamificationServiceTests.cs

    #endregion
}