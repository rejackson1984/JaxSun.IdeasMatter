using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;

namespace JaxSun.Ideas.WebApp.Services.Mock;

public class MockLaunchedBusinessService : ILaunchedBusinessService
{
    private readonly List<LaunchedBusiness> _launchedBusinesses;
    private readonly List<BusinessMetric> _businessMetrics;
    private readonly List<BusinessUpdate> _businessUpdates;
    private readonly Dictionary<string, EducationalTooltip> _tooltips;

    public MockLaunchedBusinessService()
    {
        _launchedBusinesses = GenerateMockBusinesses();
        _businessMetrics = GenerateMockMetrics();
        _businessUpdates = GenerateMockUpdates();
        _tooltips = GenerateEducationalTooltips();
    }

    // Core business management
    public Task<List<LaunchedBusiness>> GetUserBusinessesAsync(string userId)
    {
        // For demo purposes, return all businesses since LaunchedBusiness doesn't have UserId
        // In a real implementation, you would filter by user
        var userBusinesses = _launchedBusinesses.ToList();
        return Task.FromResult(userBusinesses);
    }

    public Task<LaunchedBusiness?> GetBusinessByIdAsync(string businessId)
    {
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        return Task.FromResult(business);
    }

    public Task<LaunchedBusiness> CreateBusinessAsync(LaunchBusinessRequest request)
    {
        var business = new LaunchedBusiness
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.BusinessName,
            Industry = request.Industry,
            Description = request.Description,
            Status = "Planning",
            LaunchDate = DateTime.UtcNow,
            CurrentMetrics = new BusinessMetrics
            {
                Revenue = 0,
                CustomerCount = 0,
                MonthlyGrowthRate = 0
            }
        };

        _launchedBusinesses.Add(business);
        return Task.FromResult(business);
    }

    public Task<bool> UpdateBusinessAsync(string businessId, UpdateBusinessRequest request)
    {
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        if (business == null) return Task.FromResult(false);

        if (!string.IsNullOrEmpty(request.Name))
            business.Name = request.Name;
        
        if (!string.IsNullOrEmpty(request.Description))
            business.Description = request.Description;
        
        if (!string.IsNullOrEmpty(request.Status))
            business.Status = request.Status;
        
        if (request.MonthlyRevenue.HasValue)
            business.CurrentMetrics.Revenue = request.MonthlyRevenue.Value;
        
        if (request.GrowthRate.HasValue)
            business.CurrentMetrics.MonthlyGrowthRate = (decimal)request.GrowthRate.Value;
        
        if (request.CustomerCount.HasValue)
            business.CurrentMetrics.CustomerCount = request.CustomerCount.Value;

        // LaunchedBusiness doesn't have LastUpdated property
        return Task.FromResult(true);
    }

    public Task<bool> DeleteBusinessAsync(string businessId)
    {
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        if (business == null) return Task.FromResult(false);

        _launchedBusinesses.Remove(business);
        return Task.FromResult(true);
    }

    // Metrics and analytics
    public Task<List<BusinessMetric>> GetBusinessMetricsAsync(string businessId, DateTime startDate, DateTime endDate)
    {
        var metrics = _businessMetrics
            .Where(m => m.BusinessId == businessId && m.Date >= startDate && m.Date <= endDate)
            .OrderBy(m => m.Date)
            .ToList();
        
        return Task.FromResult(metrics);
    }

    public Task<BusinessMetric> AddBusinessMetricAsync(string businessId, BusinessMetric metric)
    {
        metric.Id = Guid.NewGuid().ToString();
        metric.BusinessId = businessId;
        _businessMetrics.Add(metric);
        return Task.FromResult(metric);
    }

    public Task<PortfolioSummary> GetPortfolioSummaryAsync(string userId)
    {
        // For demo purposes, return all businesses since LaunchedBusiness doesn't have UserId
        var userBusinesses = _launchedBusinesses.ToList();
        var activeBusinesses = userBusinesses.Where(b => b.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)).ToList();
        
        var summary = new PortfolioSummary
        {
            UserId = userId,
            TotalBusinesses = userBusinesses.Count,
            ActiveBusinesses = activeBusinesses.Count,
            TotalMonthlyRevenue = userBusinesses.Sum(b => b.CurrentMetrics.Revenue),
            AverageGrowthRate = (double)(userBusinesses.Count != 0 ? userBusinesses.Average(b => b.CurrentMetrics.MonthlyGrowthRate) : 0),
            TotalCustomers = userBusinesses.Sum(b => b.CurrentMetrics.CustomerCount),
            PortfolioValue = userBusinesses.Sum(b => b.CurrentMetrics.Revenue * 12), // Simple valuation
            TopPerformer = userBusinesses.OrderByDescending(b => b.CurrentMetrics.Revenue).FirstOrDefault(),
            RecentUpdates = _businessUpdates
                .Where(u => userBusinesses.Any(b => b.Id == u.BusinessId))
                .OrderByDescending(u => u.Date)
                .Take(5)
                .ToList()
        };

        return Task.FromResult(summary);
    }

    public Task<List<BusinessAnalytics>> GetPortfolioAnalyticsAsync(string userId, int months = 12)
    {
        // For demo purposes, return all businesses since LaunchedBusiness doesn't have UserId
        var userBusinesses = _launchedBusinesses.ToList();
        var analytics = new List<BusinessAnalytics>();

        foreach (var business in userBusinesses)
        {
            var businessAnalytics = new BusinessAnalytics
            {
                BusinessId = business.Id,
                BusinessName = business.Name,
                Industry = business.Industry,
                MonthlyData = GenerateMonthlyData(business.Id, months),
                KeyMetrics = new Dictionary<string, object>
                {
                    { "Revenue", business.CurrentMetrics.Revenue },
                    { "Customers", business.CurrentMetrics.CustomerCount },
                    { "GrowthRate", business.CurrentMetrics.MonthlyGrowthRate },
                    { "ConversionRate", Random.Shared.NextDouble() * 5 + 2 }, // 2-7%
                    { "CustomerSatisfaction", Random.Shared.NextDouble() * 2 + 3 } // 3-5
                },
                PerformanceScore = CalculatePerformanceScore(business),
                TrendAnalysis = GenerateTrendAnalysis(business)
            };

            analytics.Add(businessAnalytics);
        }

        return Task.FromResult(analytics);
    }

    // Business updates and communication
    public Task<List<BusinessUpdate>> GetBusinessUpdatesAsync(string businessId)
    {
        var updates = _businessUpdates
            .Where(u => u.BusinessId == businessId)
            .OrderByDescending(u => u.Date)
            .ToList();
        
        return Task.FromResult(updates);
    }

    public Task<BusinessUpdate> AddBusinessUpdateAsync(string businessId, BusinessUpdate update)
    {
        update.Id = Guid.NewGuid().ToString();
        update.BusinessId = businessId;
        update.Date = DateTime.UtcNow;
        _businessUpdates.Add(update);
        return Task.FromResult(update);
    }

    // Comparison and benchmarking
    public Task<BusinessComparison> CompareBusinesesAsync(List<string> businessIds)
    {
        var businesses = _launchedBusinesses.Where(b => businessIds.Contains(b.Id)).ToList();
        var comparison = new BusinessComparison
        {
            BusinessIds = businessIds,
            ComparisonMetrics = new Dictionary<string, Dictionary<string, object>>(),
            BenchmarkData = GenerateBenchmarkData(),
            Recommendations = GenerateComparisonRecommendations(businesses)
        };

        foreach (var business in businesses)
        {
            comparison.ComparisonMetrics[business.Id] = new Dictionary<string, object>
            {
                { "Name", business.Name },
                { "Revenue", business.CurrentMetrics.Revenue },
                { "Customers", business.CurrentMetrics.CustomerCount },
                { "GrowthRate", business.CurrentMetrics.MonthlyGrowthRate },
                { "Industry", business.Industry },
                { "Status", business.Status }
            };
        }

        return Task.FromResult(comparison);
    }

    public Task<List<LaunchedBusiness>> GetSimilarBusinessesAsync(string businessId)
    {
        var targetBusiness = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        if (targetBusiness == null) return Task.FromResult(new List<LaunchedBusiness>());

        var similarBusinesses = _launchedBusinesses
            .Where(b => b.Id != businessId && b.Industry == targetBusiness.Industry)
            .Take(5)
            .ToList();

        return Task.FromResult(similarBusinesses);
    }

    // Legacy methods for backward compatibility
    public async Task<List<LaunchedBusiness>> GetAllLaunchedBusinessesAsync()
    {
        await Task.Delay(100);
        return _launchedBusinesses.ToList();
    }

    public async Task<LaunchedBusiness?> GetLaunchedBusinessByIdAsync(string id)
    {
        await Task.Delay(50);
        return GetBusinessByIdAsync(id).Result;
    }

    public async Task<Dictionary<string, EducationalTooltip>> GetEducationalTooltipsAsync()
    {
        await Task.Delay(50);
        return _tooltips;
    }

    public async Task<List<BusinessTip>> GetPersonalizedTipsAsync(string businessId)
    {
        await Task.Delay(50);
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        var tips = new List<BusinessTip>();

        if (business?.CurrentMetrics.MonthlyGrowthRate < 5)
        {
            tips.Add(new BusinessTip
            {
                Title = "Boost Growth Rate",
                Content = "Consider implementing customer referral programs to accelerate growth",
                Category = "Growth",
                EstimatedImpact = "High"
            });
        }

        if (business?.CurrentMetrics.CustomerCount < 100)
        {
            tips.Add(new BusinessTip
            {
                Title = "Customer Acquisition",
                Content = "Focus on digital marketing channels to reach more potential customers",
                Category = "Marketing",
                EstimatedImpact = "Medium"
            });
        }

        return tips;
    }

    public async Task<Dictionary<string, BenchmarkData>> GetBenchmarkDataAsync(string businessId)
    {
        await Task.Delay(50);
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        var benchmarks = new Dictionary<string, BenchmarkData>();

        if (business != null)
        {
            benchmarks["industry_revenue"] = new BenchmarkData
            {
                Metric = "Industry Average Revenue",
                IndustryAverage = 35000,
                YourValue = business.CurrentMetrics.Revenue,
                Interpretation = business.CurrentMetrics.Revenue > 35000 ? "Above average performance" : "Below average performance"
            };

            benchmarks["industry_growth"] = new BenchmarkData
            {
                Metric = "Industry Growth Rate",
                IndustryAverage = 8.5m,
                YourValue = business.CurrentMetrics.MonthlyGrowthRate,
                Interpretation = business.CurrentMetrics.MonthlyGrowthRate > 8.5m ? "Above average performance" : "Below average performance"
            };
        }

        return benchmarks;
    }

    public async Task<List<Achievement>> GetRecentAchievementsAsync(string businessId)
    {
        await Task.Delay(50);
        var achievements = new List<Achievement>
        {
            new Achievement
            {
                Title = "Revenue Milestone",
                Description = "Reached $50K monthly revenue",
                AchievedDate = DateTime.UtcNow.AddDays(-5),
                Icon = "💰",
                Category = "Revenue"
            },
            new Achievement
            {
                Title = "Customer Growth",
                Description = "Acquired 100+ new customers this month",
                AchievedDate = DateTime.UtcNow.AddDays(-12),
                Icon = "👥",
                Category = "Growth"
            }
        };

        return achievements;
    }

    // Private helper methods
    private List<LaunchedBusiness> GenerateMockBusinesses()
    {
        return new List<LaunchedBusiness>
        {
            new LaunchedBusiness
            {
                Id = "bus-001",
                Name = "TechFlow Solutions",
                Industry = "Software Development",
                Description = "Custom software solutions for small businesses",
                Status = "Active",
                LaunchDate = DateTime.UtcNow.AddMonths(-8),
                CurrentMetrics = new BusinessMetrics
                {
                    Revenue = 47250,
                    MonthlyGrowthRate = 12.5m,
                    CustomerCount = 1247
                }
            },
            new LaunchedBusiness
            {
                Id = "bus-002",
                Name = "EcoClean Pro",
                Industry = "Environmental Services", 
                Description = "Eco-friendly cleaning services for commercial properties",
                Status = "Active",
                LaunchDate = DateTime.UtcNow.AddMonths(-3),
                CurrentMetrics = new BusinessMetrics
                {
                    Revenue = 12800,
                    MonthlyGrowthRate = 25.3m,
                    CustomerCount = 342
                }
            }
        };
    }

    private List<BusinessMetric> GenerateMockMetrics()
    {
        var metrics = new List<BusinessMetric>();
        var startDate = DateTime.UtcNow.AddMonths(-6);

        for (int i = 0; i < 180; i++) // 6 months of daily data
        {
            var date = startDate.AddDays(i);
            metrics.Add(new BusinessMetric
            {
                Id = Guid.NewGuid().ToString(),
                BusinessId = "bus-001",
                Date = date,
                Revenue = 1500 + Random.Shared.Next(-200, 400),
                Customers = 40 + Random.Shared.Next(-5, 15),
                ConversionRate = 3.2 + Random.Shared.NextDouble() * 2,
                CustomerSatisfaction = 4.5 + Random.Shared.NextDouble() * 0.5,
                MarketingSpend = 500 + Random.Shared.Next(-100, 200),
                OperatingCosts = 800 + Random.Shared.Next(-100, 150)
            });
        }

        return metrics;
    }

    private List<BusinessUpdate> GenerateMockUpdates()
    {
        return new List<BusinessUpdate>
        {
            new BusinessUpdate
            {
                Id = "upd-001",
                BusinessId = "bus-001",
                Type = "Revenue",
                Title = "Monthly Revenue Target Exceeded",
                Description = "Achieved 105% of monthly revenue target with strong Q4 performance",
                Date = DateTime.UtcNow.AddDays(-3),
                Priority = "High",
                Author = "System",
                IsPublic = true
            },
            new BusinessUpdate
            {
                Id = "upd-002",
                BusinessId = "bus-001",
                Type = "Customer",
                Title = "Customer Satisfaction Survey Results",
                Description = "Latest customer satisfaction survey shows 4.8/5 average rating",
                Date = DateTime.UtcNow.AddDays(-7),
                Priority = "Medium",
                Author = "Marketing Team",
                IsPublic = true
            }
        };
    }

    private List<MonthlyData> GenerateMonthlyData(string businessId, int months)
    {
        var data = new List<MonthlyData>();
        var startDate = DateTime.UtcNow.AddMonths(-months);

        for (int i = 0; i < months; i++)
        {
            var month = startDate.AddMonths(i);
            data.Add(new MonthlyData
            {
                Month = month,
                Revenue = 30000 + Random.Shared.Next(-5000, 15000),
                Customers = 800 + Random.Shared.Next(-100, 300),
                ConversionRate = 3.0 + Random.Shared.NextDouble() * 2,
                CustomerSatisfaction = 4.0 + Random.Shared.NextDouble()
            });
        }

        return data;
    }

    private int CalculatePerformanceScore(LaunchedBusiness business)
    {
        var revenueScore = Math.Min(business.CurrentMetrics.Revenue / 1000, 50); // Max 50 points for revenue
        var growthScore = Math.Min(business.CurrentMetrics.MonthlyGrowthRate * 2, 30); // Max 30 points for growth
        var customerScore = Math.Min(business.CurrentMetrics.CustomerCount / 50, 20); // Max 20 points for customers

        return (int)(revenueScore + growthScore + customerScore);
    }

    private string GenerateTrendAnalysis(LaunchedBusiness business)
    {
        if (business.CurrentMetrics.MonthlyGrowthRate > 15)
            return "Strong upward trend with accelerating growth momentum";
        else if (business.CurrentMetrics.MonthlyGrowthRate > 5)
            return "Steady growth trajectory with positive market indicators";
        else if (business.CurrentMetrics.MonthlyGrowthRate > 0)
            return "Modest growth with opportunities for optimization";
        else
            return "Declining performance requiring strategic intervention";
    }

    private Dictionary<string, object> GenerateBenchmarkData()
    {
        return new Dictionary<string, object>
        {
            { "industry_avg_revenue", 35000 },
            { "industry_avg_growth", 8.5 },
            { "industry_avg_customers", 1000 },
            { "top_quartile_revenue", 75000 },
            { "top_quartile_growth", 20.0 }
        };
    }

    private List<string> GenerateComparisonRecommendations(List<LaunchedBusiness> businesses)
    {
        var recommendations = new List<string>();

        if (businesses.Any(b => b.CurrentMetrics.MonthlyGrowthRate > 15))
        {
            recommendations.Add("Consider scaling successful growth strategies across underperforming businesses");
        }

        if (businesses.Any(b => b.CurrentMetrics.Revenue > 50000))
        {
            recommendations.Add("Leverage high-revenue business models for portfolio expansion");
        }

        recommendations.Add("Focus on customer acquisition for businesses with less than 500 customers");
        recommendations.Add("Implement cross-portfolio synergies to reduce operational costs");

        return recommendations;
    }

    private Dictionary<string, EducationalTooltip> GenerateEducationalTooltips()
    {
        return new Dictionary<string, EducationalTooltip>
        {
            ["revenue"] = new EducationalTooltip { Term = "Monthly Revenue", Definition = "Total income generated by your business each month" },
            ["growth"] = new EducationalTooltip { Term = "Growth Rate", Definition = "Percentage increase in key metrics month-over-month" },
            ["customers"] = new EducationalTooltip { Term = "Customer Count", Definition = "Total number of active customers using your product or service" },
            ["conversion"] = new EducationalTooltip { Term = "Conversion Rate", Definition = "Percentage of visitors who become paying customers" }
        };
    }
}