namespace Jackson.Ideas.Mock.Models;

public class LaunchedBusiness
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Industry { get; set; } = "";
    public DateTime LaunchDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Paused, Scaling
    public string BusinessType { get; set; } = ""; // E-commerce, Service, SaaS, etc.
    public string LogoUrl { get; set; } = "";
    
    // Current Performance Metrics
    public BusinessMetrics CurrentMetrics { get; set; } = new();
    public List<BusinessMetrics> HistoricalMetrics { get; set; } = new();
    
    // Marketing & Sales Data
    public MarketingPerformance Marketing { get; set; } = new();
    public SalesPerformance Sales { get; set; } = new();
    
    // Operational Data
    public CustomerSupportMetrics Support { get; set; } = new();
    public WebsiteAnalytics Website { get; set; } = new();
    
    // Growth & Opportunities
    public List<GrowthOpportunity> Opportunities { get; set; } = new();
    public List<ActionItem> ActionItems { get; set; } = new();
    
    // Educational Content
    public BusinessEducation Education { get; set; } = new();
}

public class BusinessMetrics
{
    public DateTime Period { get; set; } = DateTime.UtcNow;
    public decimal Revenue { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin => Revenue > 0 ? (Profit / Revenue) * 100 : 0;
    public int CustomerCount { get; set; }
    public decimal CustomerAcquisitionCost { get; set; }
    public decimal CustomerLifetimeValue { get; set; }
    public decimal MonthlyGrowthRate { get; set; }
    public decimal ChurnRate { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageOrderValue => OrderCount > 0 ? Revenue / OrderCount : 0;
}

public class MarketingPerformance
{
    public Dictionary<string, ChannelMetrics> Channels { get; set; } = new();
    public decimal TotalMarketingSpend { get; set; }
    public decimal MarketingROI { get; set; }
    public int TotalLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public string BestPerformingChannel { get; set; } = "";
    public List<CampaignResult> RecentCampaigns { get; set; } = new();
}

public class ChannelMetrics
{
    public string ChannelName { get; set; } = "";
    public decimal Spend { get; set; }
    public int Clicks { get; set; }
    public int Conversions { get; set; }
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;
    public decimal CostPerAcquisition { get; set; }
    public int Revenue { get; set; }
    public decimal ROI => Spend > 0 ? (Revenue - Spend) / Spend * 100 : 0;
}

public class CampaignResult
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Spend { get; set; }
    public int Reach { get; set; }
    public int Conversions { get; set; }
    public decimal ROI { get; set; }
    public string Status { get; set; } = "";
}

public class SalesPerformance
{
    public decimal ConversionRate { get; set; }
    public List<SalesTrend> DailyTrends { get; set; } = new();
    public List<SalesTrend> WeeklyTrends { get; set; } = new();
    public List<SalesTrend> MonthlyTrends { get; set; } = new();
    public decimal RepeatCustomerRate { get; set; }
    public int AverageOrdersPerCustomer { get; set; }
    public Dictionary<string, decimal> RevenueByProduct { get; set; } = new();
}

public class SalesTrend
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
    public int NewCustomers { get; set; }
    public decimal ConversionRate { get; set; }
}

public class CustomerSupportMetrics
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public double AverageResponseTimeHours { get; set; }
    public double AverageResolutionTimeHours { get; set; }
    public decimal CustomerSatisfactionScore { get; set; }
    public List<SupportIssue> CommonIssues { get; set; } = new();
    public int TicketsThisWeek { get; set; }
    public int TicketsLastWeek { get; set; }
}

public class SupportIssue
{
    public string Category { get; set; } = "";
    public int Count { get; set; }
    public string Description { get; set; } = "";
    public decimal ImpactScore { get; set; }
}

public class WebsiteAnalytics
{
    public int MonthlyVisitors { get; set; }
    public int DailyVisitors { get; set; }
    public decimal BounceRate { get; set; }
    public double AverageSessionDuration { get; set; }
    public int PageViews { get; set; }
    public decimal ConversionRate { get; set; }
    public string Status { get; set; } = "Live"; // Live, Under Maintenance, Down, Not Yet Launched
    public decimal UptimePercentage { get; set; } = 99.9m;
    public string Url { get; set; } = "";
    public List<TrafficSource> TrafficSources { get; set; } = new();
    public List<PopularPage> PopularPages { get; set; } = new();
}

public class TrafficSource
{
    public string Source { get; set; } = "";
    public int Visitors { get; set; }
    public decimal Percentage { get; set; }
    public decimal ConversionRate { get; set; }
}

public class PopularPage
{
    public string Path { get; set; } = "";
    public string Title { get; set; } = "";
    public int Views { get; set; }
    public double AverageTimeOnPage { get; set; }
    public decimal ConversionRate { get; set; }
}

public class GrowthOpportunity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = ""; // Marketing, Product, Operations, etc.
    public decimal EstimatedImpact { get; set; } // Revenue impact
    public string RiskLevel { get; set; } = ""; // Low, Medium, High
    public string Effort { get; set; } = ""; // Easy, Medium, Hard
    public int Priority { get; set; } = 1; // 1-5 scale
    public DateTime IdentifiedDate { get; set; } = DateTime.UtcNow;
    public bool IsImplemented { get; set; } = false;
}

public class ActionItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "Medium"; // High, Medium, Low
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Category { get; set; } = "";
    public bool IsOverdue => DateTime.UtcNow > DueDate && Status != "Completed";
    public string EstimatedTime { get; set; } = ""; // "30 minutes", "2 hours", etc.
}

public class BusinessEducation
{
    public Dictionary<string, EducationalTooltip> Tooltips { get; set; } = new();
    public List<BusinessTip> Tips { get; set; } = new();
    public Dictionary<string, BenchmarkData> Benchmarks { get; set; } = new();
    public List<Achievement> Achievements { get; set; } = new();
}

public class EducationalTooltip
{
    public string Term { get; set; } = "";
    public string Definition { get; set; } = "";
    public string Example { get; set; } = "";
    public string WhyItMatters { get; set; } = "";
    public List<string> TipsToImprove { get; set; } = new();
}

public class BusinessTip
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Category { get; set; } = "";
    public string Difficulty { get; set; } = ""; // Beginner, Intermediate, Advanced
    public string EstimatedImpact { get; set; } = ""; // Low, Medium, High
    public bool IsPersonalized { get; set; } = false;
}

public class BenchmarkData
{
    public string Metric { get; set; } = "";
    public decimal YourValue { get; set; }
    public decimal IndustryAverage { get; set; }
    public decimal TopPerformers { get; set; }
    public string Interpretation { get; set; } = "";
    public bool IsGoodPerformance => YourValue >= IndustryAverage;
}

public class Achievement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public DateTime AchievedDate { get; set; }
    public bool IsNewlyAchieved { get; set; } = false;
    public string Category { get; set; } = "";
}