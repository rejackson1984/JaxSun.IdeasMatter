using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces;

public interface ILaunchedBusinessService
{
    // Core business management
    Task<List<LaunchedBusiness>> GetUserBusinessesAsync(string userId);
    Task<LaunchedBusiness?> GetBusinessByIdAsync(string businessId);
    Task<LaunchedBusiness> CreateBusinessAsync(LaunchBusinessRequest request);
    Task<bool> UpdateBusinessAsync(string businessId, UpdateBusinessRequest request);
    Task<bool> DeleteBusinessAsync(string businessId);

    // Metrics and analytics
    Task<List<BusinessMetric>> GetBusinessMetricsAsync(string businessId, DateTime startDate, DateTime endDate);
    Task<BusinessMetric> AddBusinessMetricAsync(string businessId, BusinessMetric metric);
    Task<PortfolioSummary> GetPortfolioSummaryAsync(string userId);
    Task<List<BusinessAnalytics>> GetPortfolioAnalyticsAsync(string userId, int months = 12);

    // Business updates and communication
    Task<List<BusinessUpdate>> GetBusinessUpdatesAsync(string businessId);
    Task<BusinessUpdate> AddBusinessUpdateAsync(string businessId, BusinessUpdate update);

    // Comparison and benchmarking
    Task<BusinessComparison> CompareBusinesesAsync(List<string> businessIds);
    Task<List<LaunchedBusiness>> GetSimilarBusinessesAsync(string businessId);

    // Legacy methods for backward compatibility
    Task<List<LaunchedBusiness>> GetAllLaunchedBusinessesAsync();
    Task<LaunchedBusiness?> GetLaunchedBusinessByIdAsync(string id);
    Task<Dictionary<string, EducationalTooltip>> GetEducationalTooltipsAsync();
    Task<List<BusinessTip>> GetPersonalizedTipsAsync(string businessId);
    Task<Dictionary<string, BenchmarkData>> GetBenchmarkDataAsync(string businessId);
    Task<List<Achievement>> GetRecentAchievementsAsync(string businessId);
}

// Supporting request/response models
public class LaunchBusinessRequest
{
    public string UserId { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public string Industry { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public string BusinessPlanId { get; set; } = "";
    public string IdeaScenarioId { get; set; } = "";
}

public class UpdateBusinessRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public decimal? MonthlyRevenue { get; set; }
    public double? GrowthRate { get; set; }
    public int? CustomerCount { get; set; }
}

public class BusinessMetric
{
    public string Id { get; set; } = "";
    public string BusinessId { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int Customers { get; set; }
    public double ConversionRate { get; set; }
    public double CustomerSatisfaction { get; set; }
    public decimal MarketingSpend { get; set; }
    public decimal OperatingCosts { get; set; }
}

public class PortfolioSummary
{
    public string UserId { get; set; } = "";
    public int TotalBusinesses { get; set; }
    public int ActiveBusinesses { get; set; }
    public decimal TotalMonthlyRevenue { get; set; }
    public double AverageGrowthRate { get; set; }
    public int TotalCustomers { get; set; }
    public decimal PortfolioValue { get; set; }
    public LaunchedBusiness? TopPerformer { get; set; }
    public List<BusinessUpdate> RecentUpdates { get; set; } = new();
}

public class BusinessUpdate
{
    public string Id { get; set; } = "";
    public string BusinessId { get; set; } = "";
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string Priority { get; set; } = "";
    public string Author { get; set; } = "";
    public bool IsPublic { get; set; }
}

public class BusinessAnalytics
{
    public string BusinessId { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public string Industry { get; set; } = "";
    public List<MonthlyData> MonthlyData { get; set; } = new();
    public Dictionary<string, object> KeyMetrics { get; set; } = new();
    public int PerformanceScore { get; set; }
    public string TrendAnalysis { get; set; } = "";
}

public class MonthlyData
{
    public DateTime Month { get; set; }
    public decimal Revenue { get; set; }
    public int Customers { get; set; }
    public double ConversionRate { get; set; }
    public double CustomerSatisfaction { get; set; }
}

public class BusinessComparison
{
    public List<string> BusinessIds { get; set; } = new();
    public Dictionary<string, Dictionary<string, object>> ComparisonMetrics { get; set; } = new();
    public Dictionary<string, object> BenchmarkData { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}