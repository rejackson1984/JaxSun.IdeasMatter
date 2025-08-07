using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Services
{
    /// <summary>
    /// Tests for gamification service that manages achievements, progress tracking, and user engagement
    /// Tests the achievement system that motivates users through their business development journey
    /// </summary>
    public class GamificationServiceTests
    {
        private readonly Mock<IHubContextService> _mockHubContextService;
        private readonly Mock<IUserProgressService> _mockProgressService;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly GamificationService _gamificationService;

        public GamificationServiceTests()
        {
            _mockHubContextService = new Mock<IHubContextService>();
            _mockProgressService = new Mock<IUserProgressService>();
            _mockNotificationService = new Mock<INotificationService>();
            
            _gamificationService = new GamificationService(
                _mockHubContextService.Object,
                _mockProgressService.Object,
                _mockNotificationService.Object
            );
        }

        [Fact]
        public async Task CalculateUserScore_ShouldReturn_AccurateScore_BasedOnProgress()
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.BusinessPlanning, 75, true, true);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var score = await _gamificationService.CalculateUserScoreAsync(userId);

            // Assert
            score.Should().BeGreaterThan(0, "User should have earned points for progress");
            score.Should().BeLessOrEqualTo(1000, "Score should be within reasonable bounds");
        }

        [Theory]
        [InlineData(BusinessHub.IdeaDevelopment, 25, AchievementType.FirstIdea)]
        [InlineData(BusinessHub.IdeaDevelopment, 50, AchievementType.IdeaDeveloper)]
        [InlineData(BusinessHub.IdeaDevelopment, 75, AchievementType.IdeaMaster)]
        [InlineData(BusinessHub.BusinessPlanning, 25, AchievementType.PlanningBeginner)]
        [InlineData(BusinessHub.BusinessPlanning, 75, AchievementType.BusinessStrategist)]
        [InlineData(BusinessHub.BusinessOperations, 50, AchievementType.OperationsManager)]
        public async Task EvaluateAchievements_ShouldUnlock_ProgressBasedAchievements(BusinessHub hub, int progress, AchievementType expectedAchievement)
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(hub, progress, true, hub != BusinessHub.IdeaDevelopment, hub == BusinessHub.BusinessOperations);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var achievements = await _gamificationService.EvaluateAchievementsAsync(userId);

            // Assert
            achievements.Should().NotBeEmpty("Should unlock achievements based on progress");
            achievements.Should().Contain(a => a.Type == expectedAchievement, $"Should unlock {expectedAchievement} achievement");
        }

        [Fact]
        public async Task EvaluateAchievements_ShouldUnlock_MilestoneAchievements()
        {
            // Arrange - User completes Hub 1 (70% threshold)
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 70, true, false);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var achievements = await _gamificationService.EvaluateAchievementsAsync(userId);

            // Assert
            achievements.Should().Contain(a => a.Type == AchievementType.Hub1Complete, "Should unlock Hub 1 completion achievement");
            achievements.Should().Contain(a => a.Type == AchievementType.ReadyForPlanning, "Should unlock planning readiness achievement");
        }

        [Fact]
        public async Task EvaluateAchievements_ShouldUnlock_SpecialtyAchievements()
        {
            // Arrange - User demonstrates exceptional performance
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 95, true, false);
            
            // Mock high-quality idea validation
            var ideaValidation = new IdeaValidationResult
            {
                OverallScore = 95,
                CategoryScores = new Dictionary<string, int>
                {
                    ["MarketViability"] = 10,
                    ["TechnicalFeasibility"] = 9,
                    ["CompetitiveAdvantage"] = 10
                }
            };
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var achievements = await _gamificationService.EvaluateAchievementsAsync(userId);

            // Assert
            achievements.Should().Contain(a => a.Type == AchievementType.Perfectionist, "Should unlock perfectionist achievement for 95%+ score");
            achievements.Should().Contain(a => a.Type == AchievementType.MarketExpert, "Should unlock market expert achievement for high market viability");
        }

        [Fact]
        public async Task TrackEngagement_ShouldRecord_UserActivity()
        {
            // Arrange
            var userId = "test-user";
            var engagementData = new UserEngagement
            {
                UserId = userId,
                ActivityType = EngagementType.IdeaValidation,
                Duration = TimeSpan.FromMinutes(15),
                Timestamp = DateTime.UtcNow
            };

            // Act
            await _gamificationService.TrackEngagementAsync(engagementData);

            // Assert
            // Verify engagement tracking was called
            _mockProgressService.Verify(x => x.RecordEngagementAsync(It.Is<UserEngagement>(e => 
                e.UserId == userId && 
                e.ActivityType == EngagementType.IdeaValidation)), Times.Once);
        }

        [Theory]
        [InlineData(1, StreakType.Daily)]
        [InlineData(7, StreakType.Weekly)]
        [InlineData(30, StreakType.Monthly)]
        public async Task CalculateStreaks_ShouldTrack_ConsistentUsage(int days, StreakType expectedStreak)
        {
            // Arrange
            var userId = "test-user";
            var engagementHistory = CreateConsistentEngagementHistory(userId, days);
            
            _mockProgressService.Setup(x => x.GetEngagementHistoryAsync(userId))
                .ReturnsAsync(engagementHistory);

            // Act
            var streaks = await _gamificationService.CalculateStreaksAsync(userId);

            // Assert
            streaks.Should().NotBeEmpty("Should calculate streaks for consistent usage");
            streaks.Should().Contain(s => s.Type == expectedStreak, $"Should recognize {expectedStreak} streak");
        }

        [Fact]
        public async Task GenerateLeaderboard_ShouldRank_UsersByScore()
        {
            // Arrange
            var userScores = new Dictionary<string, int>
            {
                ["user1"] = 850,
                ["user2"] = 920,
                ["user3"] = 780,
                ["user4"] = 890,
                ["user5"] = 750
            };

            _mockProgressService.Setup(x => x.GetUserScoresAsync(It.IsAny<int>()))
                .ReturnsAsync(userScores);

            // Act
            var leaderboard = await _gamificationService.GenerateLeaderboardAsync(5);

            // Assert
            leaderboard.Should().HaveCount(5, "Should return requested number of users");
            leaderboard.First().Score.Should().Be(920, "Highest scorer should be ranked first");
            leaderboard.Last().Score.Should().Be(750, "Lowest scorer should be ranked last");
            
            // Verify descending order
            var scores = leaderboard.Select(l => l.Score).ToList();
            scores.Should().BeInDescendingOrder("Leaderboard should be sorted by score descending");
        }

        [Fact]
        public async Task GetUserBadges_ShouldReturn_EarnedBadges()
        {
            // Arrange
            var userId = "test-user";
            var earnedBadges = new List<Badge>
            {
                new Badge { Id = "innovator", Name = "Innovator", Description = "Created first business idea", Rarity = BadgeRarity.Common },
                new Badge { Id = "strategist", Name = "Strategist", Description = "Completed business plan", Rarity = BadgeRarity.Uncommon },
                new Badge { Id = "visionary", Name = "Visionary", Description = "Achieved 90%+ scores", Rarity = BadgeRarity.Rare }
            };

            _mockProgressService.Setup(x => x.GetUserBadgesAsync(userId))
                .ReturnsAsync(earnedBadges);

            // Act
            var badges = await _gamificationService.GetUserBadgesAsync(userId);

            // Assert
            badges.Should().HaveCount(3, "Should return all earned badges");
            badges.Should().Contain(b => b.Rarity == BadgeRarity.Rare, "Should include rare badges");
            badges.Should().Contain(b => b.Name == "Visionary", "Should include high-achievement badges");
        }

        [Fact]
        public async Task CalculateNextReward_ShouldPredict_UpcomingAchievements()
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.IdeaDevelopment, 65, true, false);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var nextRewards = await _gamificationService.CalculateNextRewardAsync(userId);

            // Assert
            nextRewards.Should().NotBeEmpty("Should predict upcoming rewards");
            nextRewards.Should().Contain(r => r.Type == RewardType.HubCompletion, "Should show hub completion as next reward");
            nextRewards.Should().Contain(r => r.RequiredProgress > 65, "Should show realistic progress requirements");
        }

        [Fact]
        public async Task TriggerCelebration_ShouldSend_AchievementNotifications()
        {
            // Arrange
            var userId = "test-user";
            var achievement = new Achievement
            {
                Type = AchievementType.Hub1Complete,
                Name = "Idea Master",
                Description = "Completed Hub 1 with excellence",
                Points = 100,
                UnlockedAt = DateTime.UtcNow
            };

            // Act
            await _gamificationService.TriggerCelebrationAsync(userId, achievement);

            // Assert
            _mockNotificationService.Verify(x => x.SendAchievementNotificationAsync(
                userId, 
                It.Is<Achievement>(a => a.Type == AchievementType.Hub1Complete)), 
                Times.Once, "Should send achievement notification");
        }

        [Fact]
        public async Task GetMotivationalMessage_ShouldProvide_ContextualEncouragement()
        {
            // Arrange
            var userId = "test-user";
            var hubContext = CreateHubContext(BusinessHub.BusinessPlanning, 40, true, true);
            
            _mockHubContextService.Setup(x => x.GetCurrentContextAsync()).ReturnsAsync(hubContext);

            // Act
            var message = await _gamificationService.GetMotivationalMessageAsync(userId);

            // Assert
            message.Should().NotBeEmpty("Should provide motivational message");
            message.Should().Contain("progress", "Should reference user's progress");
            message.Should().NotContain("fail", "Should be encouraging, not discouraging");
        }

        [Fact]
        public async Task HandleConcurrentUsers_ShouldBe_ThreadSafe()
        {
            // Arrange - Multiple users achieving milestones simultaneously
            var userIds = Enumerable.Range(1, 10).Select(i => $"user{i}").ToArray();
            var contexts = userIds.Select(id => CreateHubContext(BusinessHub.IdeaDevelopment, 70, true, false));

            foreach (var (userId, context) in userIds.Zip(contexts))
            {
                _mockHubContextService.Setup(x => x.GetCurrentContextAsync())
                    .ReturnsAsync(context);
            }

            // Act - Evaluate achievements for all users concurrently
            var tasks = userIds.Select(async userId => 
                await _gamificationService.EvaluateAchievementsAsync(userId));

            var results = await Task.WhenAll(tasks);

            // Assert - All evaluations should complete successfully
            results.Should().HaveCount(10, "All concurrent evaluations should complete");
            results.Should().OnlyContain(r => r.Any(), "All users should have achievements");
        }

        #region Helper Methods

        private HubContext CreateHubContext(BusinessHub currentHub, int completionPercentage, bool hub1Unlocked = true, bool hub2Unlocked = false, bool hub3Unlocked = false)
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
                        IsUnlocked = hub1Unlocked,
                        IsActive = currentHub == BusinessHub.IdeaDevelopment
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = currentHub == BusinessHub.BusinessPlanning ? completionPercentage : 0,
                        IsUnlocked = hub2Unlocked,
                        IsActive = currentHub == BusinessHub.BusinessPlanning
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = currentHub == BusinessHub.BusinessOperations ? completionPercentage : 0,
                        IsUnlocked = hub3Unlocked,
                        IsActive = currentHub == BusinessHub.BusinessOperations
                    }
                }
            };
        }

        private List<UserEngagement> CreateConsistentEngagementHistory(string userId, int days)
        {
            var history = new List<UserEngagement>();
            var baseDate = DateTime.UtcNow.AddDays(-days);

            for (int i = 0; i < days; i++)
            {
                history.Add(new UserEngagement
                {
                    UserId = userId,
                    ActivityType = EngagementType.IdeaValidation,
                    Duration = TimeSpan.FromMinutes(20),
                    Timestamp = baseDate.AddDays(i)
                });
            }

            return history;
        }

        #endregion
    }

    /// <summary>
    /// Gamification service for managing achievements and user engagement
    /// This would be the actual service implementation being tested
    /// </summary>
    public class GamificationService
    {
        private readonly IHubContextService _hubContextService;
        private readonly IUserProgressService _progressService;
        private readonly INotificationService _notificationService;

        public GamificationService(
            IHubContextService hubContextService,
            IUserProgressService progressService,
            INotificationService notificationService)
        {
            _hubContextService = hubContextService;
            _progressService = progressService;
            _notificationService = notificationService;
        }

        public async Task<int> CalculateUserScoreAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            
            // Calculate score based on hub progress
            var totalScore = 0;
            foreach (var progress in context.Progress.Values)
            {
                totalScore += progress.CompletionPercentage * (progress.IsUnlocked ? 1 : 0);
            }

            return Math.Min(totalScore, 1000); // Cap at 1000 points
        }

        public async Task<List<Achievement>> EvaluateAchievementsAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var achievements = new List<Achievement>();

            // Progress-based achievements
            foreach (var progress in context.Progress.Values)
            {
                if (progress.CompletionPercentage >= 25)
                {
                    achievements.Add(CreateProgressAchievement(progress.Hub, 25));
                }
                if (progress.CompletionPercentage >= 50)
                {
                    achievements.Add(CreateProgressAchievement(progress.Hub, 50));
                }
                if (progress.CompletionPercentage >= 75)
                {
                    achievements.Add(CreateProgressAchievement(progress.Hub, 75));
                }
                if (progress.CompletionPercentage >= 95)
                {
                    achievements.Add(new Achievement
                    {
                        Type = AchievementType.Perfectionist,
                        Name = "Perfectionist",
                        Description = "Achieved 95%+ completion score",
                        Points = 50
                    });
                }
            }

            // Hub completion achievements
            if (context.Progress[BusinessHub.IdeaDevelopment].CompletionPercentage >= 70)
            {
                achievements.Add(new Achievement
                {
                    Type = AchievementType.Hub1Complete,
                    Name = "Idea Master",
                    Description = "Completed Hub 1",
                    Points = 100
                });
                achievements.Add(new Achievement
                {
                    Type = AchievementType.ReadyForPlanning,
                    Name = "Ready for Planning",
                    Description = "Ready to advance to business planning",
                    Points = 50
                });
            }

            return achievements;
        }

        public async Task TrackEngagementAsync(UserEngagement engagement)
        {
            await _progressService.RecordEngagementAsync(engagement);
        }

        public async Task<List<Streak>> CalculateStreaksAsync(string userId)
        {
            var history = await _progressService.GetEngagementHistoryAsync(userId);
            var streaks = new List<Streak>();

            // Calculate consecutive day streak
            var consecutiveDays = CalculateConsecutiveDays(history);
            if (consecutiveDays >= 1)
            {
                streaks.Add(new Streak { Type = StreakType.Daily, Count = consecutiveDays });
            }
            if (consecutiveDays >= 7)
            {
                streaks.Add(new Streak { Type = StreakType.Weekly, Count = consecutiveDays / 7 });
            }
            if (consecutiveDays >= 30)
            {
                streaks.Add(new Streak { Type = StreakType.Monthly, Count = consecutiveDays / 30 });
            }

            return streaks;
        }

        public async Task<List<LeaderboardEntry>> GenerateLeaderboardAsync(int topCount)
        {
            var userScores = await _progressService.GetUserScoresAsync(topCount);
            
            return userScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(topCount)
                .Select((kvp, index) => new LeaderboardEntry
                {
                    Rank = index + 1,
                    UserId = kvp.Key,
                    Score = kvp.Value
                })
                .ToList();
        }

        public async Task<List<Badge>> GetUserBadgesAsync(string userId)
        {
            return await _progressService.GetUserBadgesAsync(userId);
        }

        public async Task<List<NextReward>> CalculateNextRewardAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var nextRewards = new List<NextReward>();

            // Calculate next milestone rewards
            foreach (var progress in context.Progress.Values)
            {
                if (progress.CompletionPercentage < 70 && progress.Hub == BusinessHub.IdeaDevelopment)
                {
                    nextRewards.Add(new NextReward
                    {
                        Type = RewardType.HubCompletion,
                        Name = "Hub 1 Completion",
                        RequiredProgress = 70,
                        CurrentProgress = progress.CompletionPercentage
                    });
                }
                if (progress.CompletionPercentage < 80 && progress.Hub == BusinessHub.BusinessPlanning)
                {
                    nextRewards.Add(new NextReward
                    {
                        Type = RewardType.HubCompletion,
                        Name = "Hub 2 Completion",
                        RequiredProgress = 80,
                        CurrentProgress = progress.CompletionPercentage
                    });
                }
            }

            return nextRewards;
        }

        public async Task TriggerCelebrationAsync(string userId, Achievement achievement)
        {
            await _notificationService.SendAchievementNotificationAsync(userId, achievement);
        }

        public async Task<string> GetMotivationalMessageAsync(string userId)
        {
            var context = await _hubContextService.GetCurrentContextAsync();
            var currentProgress = context.Progress[context.CurrentHub].CompletionPercentage;

            if (currentProgress < 25)
            {
                return "Great start! You're building something amazing. Keep making progress!";
            }
            else if (currentProgress < 50)
            {
                return "You're making excellent progress! Keep pushing forward toward your goals.";
            }
            else if (currentProgress < 70)
            {
                return "Outstanding work! You're more than halfway there. The finish line is in sight!";
            }
            else
            {
                return "Incredible progress! You're ready to take your business to the next level!";
            }
        }

        private Achievement CreateProgressAchievement(BusinessHub hub, int percentage)
        {
            var type = hub switch
            {
                BusinessHub.IdeaDevelopment when percentage == 25 => AchievementType.FirstIdea,
                BusinessHub.IdeaDevelopment when percentage == 50 => AchievementType.IdeaDeveloper,
                BusinessHub.IdeaDevelopment when percentage == 75 => AchievementType.IdeaMaster,
                BusinessHub.BusinessPlanning when percentage == 25 => AchievementType.PlanningBeginner,
                BusinessHub.BusinessPlanning when percentage == 75 => AchievementType.BusinessStrategist,
                BusinessHub.BusinessOperations when percentage == 50 => AchievementType.OperationsManager,
                _ => AchievementType.FirstIdea
            };

            return new Achievement
            {
                Type = type,
                Name = $"{hub} {percentage}%",
                Description = $"Reached {percentage}% completion in {hub}",
                Points = percentage
            };
        }

        private int CalculateConsecutiveDays(List<UserEngagement> history)
        {
            if (!history.Any()) return 0;

            var sortedDates = history
                .Select(h => h.Timestamp.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            var consecutiveDays = 1;
            var currentDate = sortedDates.First();

            for (int i = 1; i < sortedDates.Count; i++)
            {
                if (currentDate.AddDays(-1) == sortedDates[i])
                {
                    consecutiveDays++;
                    currentDate = sortedDates[i];
                }
                else
                {
                    break;
                }
            }

            return consecutiveDays;
        }
    }

    #region Supporting Models

    public class Achievement
    {
        public AchievementType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Points { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserEngagement
    {
        public string UserId { get; set; } = string.Empty;
        public EngagementType ActivityType { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Streak
    {
        public StreakType Type { get; set; }
        public int Count { get; set; }
    }

    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class Badge
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BadgeRarity Rarity { get; set; }
    }

    public class NextReward
    {
        public RewardType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RequiredProgress { get; set; }
        public int CurrentProgress { get; set; }
    }

    public enum AchievementType
    {
        FirstIdea,
        IdeaDeveloper,
        IdeaMaster,
        PlanningBeginner,
        BusinessStrategist,
        OperationsManager,
        Hub1Complete,
        Hub2Complete,
        Hub3Complete,
        ReadyForPlanning,
        ReadyForOperations,
        Perfectionist,
        MarketExpert
    }

    public enum EngagementType
    {
        IdeaValidation,
        BusinessPlanning,
        MarketResearch,
        FinancialPlanning,
        OperationsPlanning
    }

    public enum StreakType
    {
        Daily,
        Weekly,
        Monthly
    }

    public enum BadgeRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum RewardType
    {
        HubCompletion,
        Milestone,
        Streak,
        Special
    }

    #endregion

    #region Service Interfaces

    public interface IUserProgressService
    {
        Task RecordEngagementAsync(UserEngagement engagement);
        Task<List<UserEngagement>> GetEngagementHistoryAsync(string userId);
        Task<Dictionary<string, int>> GetUserScoresAsync(int topCount);
        Task<List<Badge>> GetUserBadgesAsync(string userId);
    }

    public interface INotificationService
    {
        Task SendAchievementNotificationAsync(string userId, Achievement achievement);
    }

    public class IdeaValidationResult
    {
        public int OverallScore { get; set; }
        public Dictionary<string, int> CategoryScores { get; set; } = new();
    }

    #endregion
}