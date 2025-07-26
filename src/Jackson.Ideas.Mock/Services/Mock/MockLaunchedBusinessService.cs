using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock;

public class MockLaunchedBusinessService : ILaunchedBusinessService
{
    private readonly List<LaunchedBusiness> _launchedBusinesses;
    private readonly Dictionary<string, EducationalTooltip> _tooltips;

    public MockLaunchedBusinessService()
    {
        _launchedBusinesses = GenerateMockLaunchedBusinesses();
        _tooltips = GenerateEducationalTooltips();
    }

    public async Task<List<LaunchedBusiness>> GetAllLaunchedBusinessesAsync()
    {
        await Task.Delay(100);
        return _launchedBusinesses;
    }

    public async Task<LaunchedBusiness?> GetLaunchedBusinessByIdAsync(string id)
    {
        await Task.Delay(50);
        return _launchedBusinesses.FirstOrDefault(b => b.Id == id);
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
        return business?.Education.Tips ?? new List<BusinessTip>();
    }

    public async Task<Dictionary<string, BenchmarkData>> GetBenchmarkDataAsync(string businessId)
    {
        await Task.Delay(50);
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        return business?.Education.Benchmarks ?? new Dictionary<string, BenchmarkData>();
    }

    public async Task<List<Achievement>> GetRecentAchievementsAsync(string businessId)
    {
        await Task.Delay(50);
        var business = _launchedBusinesses.FirstOrDefault(b => b.Id == businessId);
        return business?.Education.Achievements.Where(a => a.AchievedDate >= DateTime.UtcNow.AddDays(-30)).ToList() ?? new List<Achievement>();
    }

    private static List<LaunchedBusiness> GenerateMockLaunchedBusinesses()
    {
        return new List<LaunchedBusiness>
        {
            new LaunchedBusiness
            {
                Id = "craft-corner-store",
                Name = "Craft Corner",
                Description = "Handmade jewelry and accessories sold through Instagram and local markets",
                Industry = "E-commerce",
                LaunchDate = DateTime.UtcNow.AddMonths(-8),
                Status = "Active",
                BusinessType = "E-commerce",
                LogoUrl = "/images/craft-corner-logo.png",
                CurrentMetrics = new BusinessMetrics
                {
                    Revenue = 3200,
                    Profit = 1920,
                    CustomerCount = 145,
                    CustomerAcquisitionCost = 12,
                    CustomerLifetimeValue = 85,
                    MonthlyGrowthRate = 15.3m,
                    ChurnRate = 8.2m,
                    OrderCount = 67
                },
                Marketing = new MarketingPerformance
                {
                    TotalMarketingSpend = 480,
                    MarketingROI = 566.7m,
                    TotalLeads = 89,
                    ConversionRate = 16.2m,
                    BestPerformingChannel = "Instagram",
                    Channels = new Dictionary<string, ChannelMetrics>
                    {
                        ["Instagram"] = new ChannelMetrics
                        {
                            ChannelName = "Instagram",
                            Spend = 280,
                            Clicks = 1240,
                            Conversions = 45,
                            CostPerAcquisition = 6.22m,
                            Revenue = 2150
                        },
                        ["TikTok"] = new ChannelMetrics
                        {
                            ChannelName = "TikTok",
                            Spend = 120,
                            Clicks = 890,
                            Conversions = 18,
                            CostPerAcquisition = 6.67m,
                            Revenue = 720
                        },
                        ["Local Markets"] = new ChannelMetrics
                        {
                            ChannelName = "Local Markets",
                            Spend = 80,
                            Clicks = 0,
                            Conversions = 12,
                            CostPerAcquisition = 6.67m,
                            Revenue = 330
                        }
                    },
                    RecentCampaigns = new List<CampaignResult>
                    {
                        new CampaignResult
                        {
                            Name = "Valentine's Day Collection",
                            Type = "Instagram Ads",
                            StartDate = DateTime.UtcNow.AddDays(-14),
                            EndDate = DateTime.UtcNow.AddDays(-1),
                            Spend = 150,
                            Reach = 2400,
                            Conversions = 28,
                            ROI = 320,
                            Status = "Completed"
                        }
                    }
                },
                Sales = new SalesPerformance
                {
                    ConversionRate = 16.2m,
                    RepeatCustomerRate = 34.5m,
                    AverageOrdersPerCustomer = 2,
                    RevenueByProduct = new Dictionary<string, decimal>
                    {
                        ["Earrings"] = 1280,
                        ["Necklaces"] = 960,
                        ["Bracelets"] = 640,
                        ["Rings"] = 320
                    },
                    MonthlyTrends = GenerateMonthlySalesTrends(3200, 8)
                },
                Support = new CustomerSupportMetrics
                {
                    TotalTickets = 12,
                    OpenTickets = 2,
                    AverageResponseTimeHours = 3.2,
                    AverageResolutionTimeHours = 8.5,
                    CustomerSatisfactionScore = 4.7m,
                    TicketsThisWeek = 3,
                    TicketsLastWeek = 5,
                    CommonIssues = new List<SupportIssue>
                    {
                        new SupportIssue { Category = "Shipping Delays", Count = 5, Description = "Orders arriving later than expected", ImpactScore = 6.5m },
                        new SupportIssue { Category = "Size Questions", Count = 4, Description = "Customers asking about jewelry sizing", ImpactScore = 3.2m },
                        new SupportIssue { Category = "Custom Orders", Count = 3, Description = "Requests for personalized pieces", ImpactScore = 8.9m }
                    }
                },
                Website = new WebsiteAnalytics
                {
                    MonthlyVisitors = 2840,
                    DailyVisitors = 95,
                    BounceRate = 42.3m,
                    AverageSessionDuration = 2.8,
                    PageViews = 8520,
                    ConversionRate = 5.1m,
                    TrafficSources = new List<TrafficSource>
                    {
                        new TrafficSource { Source = "Instagram", Visitors = 1420, Percentage = 50.0m, ConversionRate = 8.2m },
                        new TrafficSource { Source = "Direct", Visitors = 710, Percentage = 25.0m, ConversionRate = 12.5m },
                        new TrafficSource { Source = "TikTok", Visitors = 426, Percentage = 15.0m, ConversionRate = 4.1m },
                        new TrafficSource { Source = "Google", Visitors = 284, Percentage = 10.0m, ConversionRate = 6.8m }
                    }
                },
                Opportunities = new List<GrowthOpportunity>
                {
                    new GrowthOpportunity
                    {
                        Title = "Launch Email Newsletter",
                        Description = "Start collecting emails and sending weekly updates. Similar businesses see 25% revenue increase!",
                        Category = "Marketing",
                        EstimatedImpact = 800,
                        RiskLevel = "Low",
                        Effort = "Easy",
                        Priority = 5
                    },
                    new GrowthOpportunity
                    {
                        Title = "Add Subscription Box",
                        Description = "Monthly jewelry subscription could create recurring revenue. Industry average: $45/month.",
                        Category = "Product",
                        EstimatedImpact = 1350,
                        RiskLevel = "Medium",
                        Effort = "Medium",
                        Priority = 4
                    }
                },
                ActionItems = new List<ActionItem>
                {
                    new ActionItem
                    {
                        Title = "Reply to Customer Reviews",
                        Description = "3 new reviews need responses - great for SEO and customer relationships!",
                        Priority = "High",
                        DueDate = DateTime.UtcNow.AddDays(1),
                        Category = "Customer Service",
                        EstimatedTime = "30 minutes"
                    },
                    new ActionItem
                    {
                        Title = "Post This Week's Instagram Content",
                        Description = "You have 5 photos ready to post. Consistent posting = more engagement!",
                        Priority = "Medium",
                        DueDate = DateTime.UtcNow.AddDays(2),
                        Category = "Marketing",
                        EstimatedTime = "1 hour"
                    }
                },
                Education = new BusinessEducation
                {
                    Benchmarks = new Dictionary<string, BenchmarkData>
                    {
                        ["ConversionRate"] = new BenchmarkData
                        {
                            Metric = "Conversion Rate",
                            YourValue = 16.2m,
                            IndustryAverage = 12.8m,
                            TopPerformers = 22.1m,
                            Interpretation = "You're doing better than average! Keep focusing on Instagram."
                        },
                        ["CustomerAcquisitionCost"] = new BenchmarkData
                        {
                            Metric = "Customer Acquisition Cost",
                            YourValue = 12,
                            IndustryAverage = 18,
                            TopPerformers = 8,
                            Interpretation = "Great job keeping costs low! Your Instagram strategy is working."
                        }
                    },
                    Tips = new List<BusinessTip>
                    {
                        new BusinessTip
                        {
                            Title = "Post at Peak Times",
                            Content = "Your Instagram posts get 3x more engagement when posted at 7-9 PM. Try scheduling your content for these hours!",
                            Category = "Marketing",
                            Difficulty = "Beginner",
                            EstimatedImpact = "Medium",
                            IsPersonalized = true
                        }
                    },
                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Title = "First $1K Month! 🎉",
                            Description = "You hit your first $1,000 revenue month! This is a huge milestone for any side hustle.",
                            Icon = "💰",
                            AchievedDate = DateTime.UtcNow.AddMonths(-3),
                            Category = "Revenue"
                        },
                        new Achievement
                        {
                            Title = "100 Happy Customers",
                            Description = "You now have over 100 customers who love your products! Word of mouth is your best marketing.",
                            Icon = "👥",
                            AchievedDate = DateTime.UtcNow.AddDays(-15),
                            Category = "Customers",
                            IsNewlyAchieved = true
                        }
                    }
                }
            },
            new LaunchedBusiness
            {
                Id = "fresh-fitness-app",
                Name = "FreshFit",
                Description = "Personalized workout app with nutrition tracking for busy professionals",
                Industry = "Health & Fitness",
                LaunchDate = DateTime.UtcNow.AddMonths(-4),
                Status = "Scaling",
                BusinessType = "SaaS",
                LogoUrl = "/images/freshfit-logo.png",
                CurrentMetrics = new BusinessMetrics
                {
                    Revenue = 1850,
                    Profit = 1295,
                    CustomerCount = 78,
                    CustomerAcquisitionCost = 18,
                    CustomerLifetimeValue = 120,
                    MonthlyGrowthRate = 28.7m,
                    ChurnRate = 12.5m,
                    OrderCount = 78 // Subscription count
                },
                Marketing = new MarketingPerformance
                {
                    TotalMarketingSpend = 320,
                    MarketingROI = 478.1m,
                    TotalLeads = 156,
                    ConversionRate = 22.4m,
                    BestPerformingChannel = "TikTok",
                    Channels = new Dictionary<string, ChannelMetrics>
                    {
                        ["TikTok"] = new ChannelMetrics
                        {
                            ChannelName = "TikTok",
                            Spend = 180,
                            Clicks = 2100,
                            Conversions = 42,
                            CostPerAcquisition = 4.29m,
                            Revenue = 1260
                        },
                        ["Google Ads"] = new ChannelMetrics
                        {
                            ChannelName = "Google Ads",
                            Spend = 140,
                            Clicks = 680,
                            Conversions = 21,
                            CostPerAcquisition = 6.67m,
                            Revenue = 630
                        }
                    }
                },
                Sales = new SalesPerformance
                {
                    ConversionRate = 22.4m,
                    RepeatCustomerRate = 87.5m, // High for subscription
                    AverageOrdersPerCustomer = 1, // Subscription model
                    MonthlyTrends = GenerateMonthlySalesTrends(1850, 4)
                },
                Support = new CustomerSupportMetrics
                {
                    TotalTickets = 18,
                    OpenTickets = 1,
                    AverageResponseTimeHours = 2.1,
                    AverageResolutionTimeHours = 6.3,
                    CustomerSatisfactionScore = 4.6m,
                    TicketsThisWeek = 4,
                    TicketsLastWeek = 6,
                    CommonIssues = new List<SupportIssue>
                    {
                        new SupportIssue { Category = "Login Issues", Count = 8, Description = "Users having trouble logging in", ImpactScore = 7.8m },
                        new SupportIssue { Category = "Feature Requests", Count = 6, Description = "Users asking for new workout types", ImpactScore = 5.4m },
                        new SupportIssue { Category = "Billing Questions", Count = 4, Description = "Questions about subscription pricing", ImpactScore = 6.1m }
                    }
                },
                Website = new WebsiteAnalytics
                {
                    MonthlyVisitors = 4200,
                    DailyVisitors = 140,
                    BounceRate = 35.8m,
                    AverageSessionDuration = 4.2,
                    PageViews = 12600,
                    ConversionRate = 8.3m
                },
                Opportunities = new List<GrowthOpportunity>
                {
                    new GrowthOpportunity
                    {
                        Title = "Partner with Influencers",
                        Description = "Fitness influencers in your niche have 50K+ engaged followers. Average cost: $200/post.",
                        Category = "Marketing",
                        EstimatedImpact = 1200,
                        RiskLevel = "Low",
                        Effort = "Easy",
                        Priority = 5
                    },
                    new GrowthOpportunity
                    {
                        Title = "Add Premium Tier",
                        Description = "35% of users would pay extra for 1-on-1 coaching. Potential: $49/month premium tier.",
                        Category = "Product",
                        EstimatedImpact = 2450,
                        RiskLevel = "Medium",
                        Effort = "Hard",
                        Priority = 4
                    }
                },
                ActionItems = new List<ActionItem>
                {
                    new ActionItem
                    {
                        Title = "Fix Login Bug",
                        Description = "8 users reported login issues. This is affecting user experience and retention!",
                        Priority = "High",
                        DueDate = DateTime.UtcNow,
                        Category = "Technical",
                        EstimatedTime = "2 hours"
                    },
                    new ActionItem
                    {
                        Title = "Create TikTok Content Calendar",
                        Description = "Plan next month's TikTok posts. Consistency = more followers and customers!",
                        Priority = "Medium",
                        DueDate = DateTime.UtcNow.AddDays(3),
                        Category = "Marketing",
                        EstimatedTime = "3 hours"
                    }
                },
                Education = new BusinessEducation
                {
                    Benchmarks = new Dictionary<string, BenchmarkData>
                    {
                        ["ChurnRate"] = new BenchmarkData
                        {
                            Metric = "Monthly Churn Rate",
                            YourValue = 12.5m,
                            IndustryAverage = 15.2m,
                            TopPerformers = 8.1m,
                            Interpretation = "Better than average! Keep focusing on user engagement to reduce churn further."
                        },
                        ["ConversionRate"] = new BenchmarkData
                        {
                            Metric = "Conversion Rate",
                            YourValue = 22.4m,
                            IndustryAverage = 18.3m,
                            TopPerformers = 28.9m,
                            Interpretation = "Excellent conversion rate! Your TikTok strategy is really working."
                        }
                    },
                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Title = "50 Subscribers! 🚀",
                            Description = "You've reached 50 paying subscribers! Your app is solving real problems for people.",
                            Icon = "📱",
                            AchievedDate = DateTime.UtcNow.AddDays(-10),
                            Category = "Subscribers",
                            IsNewlyAchieved = true
                        }
                    }
                }
            }
        };
    }

    private static List<SalesTrend> GenerateMonthlySalesTrends(decimal currentRevenue, int monthsBack)
    {
        var trends = new List<SalesTrend>();
        var random = new Random(42); // Fixed seed for consistent data
        
        for (int i = monthsBack; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            var growthFactor = (decimal)Math.Pow(1.15, monthsBack - i); // 15% monthly growth
            var baseRevenue = currentRevenue / growthFactor;
            var variationFactor = 1 + (decimal)(random.NextDouble() - 0.5) * 0.2m; // ±10% variation
            
            trends.Add(new SalesTrend
            {
                Date = date,
                Revenue = Math.Round(baseRevenue * variationFactor, 2),
                Orders = (int)(baseRevenue * variationFactor / 30), // Assuming $30 average order
                NewCustomers = (int)(baseRevenue * variationFactor / 45), // Assuming some repeat customers
                ConversionRate = 15 + (decimal)(random.NextDouble() * 10) // 15-25% range
            });
        }
        
        return trends;
    }

    private static Dictionary<string, EducationalTooltip> GenerateEducationalTooltips()
    {
        return new Dictionary<string, EducationalTooltip>
        {
            ["Revenue"] = new EducationalTooltip
            {
                Term = "Revenue",
                Definition = "All the money coming into your business from sales",
                Example = "If you sold 50 items at $20 each, your revenue is $1,000",
                WhyItMatters = "Revenue shows how much your customers value your product and how well your marketing is working",
                TipsToImprove = new List<string> { "Increase your prices", "Sell more products", "Find new customers", "Create bundles or packages" }
            },
            ["ProfitMargin"] = new EducationalTooltip
            {
                Term = "Profit Margin",
                Definition = "The percentage of revenue you keep as profit after paying all costs",
                Example = "If you earn $1,000 and spend $600, your profit margin is 40%",
                WhyItMatters = "Higher profit margins mean your business is more efficient and sustainable",
                TipsToImprove = new List<string> { "Negotiate better prices with suppliers", "Reduce unnecessary expenses", "Increase prices if possible", "Focus on higher-margin products" }
            },
            ["CustomerAcquisitionCost"] = new EducationalTooltip
            {
                Term = "Customer Acquisition Cost (CAC)",
                Definition = "How much you spend to get each new customer",
                Example = "If you spend $200 on ads and get 10 new customers, your CAC is $20",
                WhyItMatters = "Lower CAC means you can grow more efficiently and profitably",
                TipsToImprove = new List<string> { "Improve your ad targeting", "Use referral programs", "Focus on organic social media", "Optimize your conversion rate" }
            },
            ["ConversionRate"] = new EducationalTooltip
            {
                Term = "Conversion Rate",
                Definition = "The percentage of visitors who become customers",
                Example = "If 100 people visit your store and 5 buy something, your conversion rate is 5%",
                WhyItMatters = "Higher conversion rates mean you're better at turning interest into sales",
                TipsToImprove = new List<string> { "Improve product photos", "Write better descriptions", "Add customer reviews", "Make checkout easier", "Offer guarantees" }
            },
            ["ChurnRate"] = new EducationalTooltip
            {
                Term = "Churn Rate",
                Definition = "The percentage of customers who stop buying from you each month",
                Example = "If you have 100 customers and 10 stop buying, your churn rate is 10%",
                WhyItMatters = "Lower churn means customers are happier and your business is more stable",
                TipsToImprove = new List<string> { "Follow up with customers", "Ask for feedback", "Improve product quality", "Create loyalty programs", "Send helpful content" }
            },
            ["CustomerLifetimeValue"] = new EducationalTooltip
            {
                Term = "Customer Lifetime Value (LTV)",
                Definition = "The total amount a customer will spend with your business over time",
                Example = "If customers spend $30 per order and order 4 times, their LTV is $120",
                WhyItMatters = "Higher LTV means you can spend more to acquire customers and still be profitable",
                TipsToImprove = new List<string> { "Create repeat purchase incentives", "Upsell and cross-sell", "Improve customer service", "Build strong relationships", "Launch subscription products" }
            }
        };
    }
}