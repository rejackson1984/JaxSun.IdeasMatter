namespace Jackson.Ideas.Mock.Models;

public class TimelineModifications
{
    public WorkSchedule Schedule { get; set; } = WorkSchedule.FullTime;
    public int HoursPerWeek { get; set; } = 40;
    public DateTime? PreferredLaunchDate { get; set; }
    public int LaunchTimelineMonths { get; set; } = 6;
    public List<MilestoneAdjustment> MilestoneChanges { get; set; } = new();
    public TimeBuffer BufferStrategy { get; set; } = TimeBuffer.Standard;
    public bool AllowWeekendWork { get; set; } = false;
    public bool AllowEveningWork { get; set; } = true;
}

public class ResourceModifications
{
    public FundingApproach FundingStrategy { get; set; } = FundingApproach.Bootstrap;
    public decimal InitialBudget { get; set; }
    public decimal MaxBudget { get; set; }
    public TeamBuildingStrategy TeamStrategy { get; set; } = TeamBuildingStrategy.SoloFounder;
    public int MaxTeamSize { get; set; } = 1;
    public bool UseContractors { get; set; } = false;
    public bool UseFreelancers { get; set; } = false;
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> AvailableSkills { get; set; } = new();
}

public class MarketModifications
{
    public GoToMarketStrategy MarketingStrategy { get; set; } = GoToMarketStrategy.DigitalFirst;
    public CompetitionStrategy CompetitionApproach { get; set; } = CompetitionStrategy.DirectCompetition;
    public MarketScope TargetScope { get; set; } = MarketScope.Local;
    public CustomerSegment PrimarySegment { get; set; } = CustomerSegment.B2C;
    public List<string> GeographicFocus { get; set; } = new();
    public PricingStrategy PricingApproach { get; set; } = PricingStrategy.Competitive;
    public decimal? TargetPricePoint { get; set; }
}

public class FinancialModifications
{
    public RevenueModel RevenueStrategy { get; set; } = RevenueModel.OneTime;
    public decimal Year1RevenueTarget { get; set; }
    public decimal Year3RevenueTarget { get; set; }
    public GrowthStrategy GrowthApproach { get; set; } = GrowthStrategy.Organic;
    public ProfitabilityGoal ProfitGoal { get; set; } = ProfitabilityGoal.BreakEven;
    public int BreakEvenMonths { get; set; } = 12;
    public bool ReinvestProfits { get; set; } = true;
}

public class OperationalModifications
{
    public BusinessModel OperatingModel { get; set; } = BusinessModel.Traditional;
    public WorkLocation LocationStrategy { get; set; } = WorkLocation.Remote;
    public ScalabilityPlan ScalingStrategy { get; set; } = ScalabilityPlan.Manual;
    public QualityStandard QualityLevel { get; set; } = QualityStandard.High;
    public CustomerServiceLevel ServiceLevel { get; set; } = CustomerServiceLevel.Standard;
    public List<string> OperationalPriorities { get; set; } = new();
}

// Supporting Data Models
public class BusinessTimeline
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime TargetLaunchDate { get; set; }
    public List<BusinessMilestone> Milestones { get; set; } = new();
    public int TotalDurationMonths { get; set; }
}

public class BusinessResources
{
    public decimal TotalBudget { get; set; }
    public int TeamSize { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public List<ResourceAllocation> Allocations { get; set; } = new();
}

public class MarketStrategy
{
    public string TargetMarket { get; set; } = "";
    public string ValueProposition { get; set; } = "";
    public List<string> MarketingChannels { get; set; } = new();
    public CompetitiveAdvantage CompetitiveEdge { get; set; } = new();
}

public class FinancialPlan
{
    public decimal StartupCost { get; set; }
    public decimal OperatingCost { get; set; }
    public RevenueProjections RevenueProjections { get; set; } = new();
    public CashFlowProjections CashFlow { get; set; } = new();
}

public class OperationalPlan
{
    public string BusinessModel { get; set; } = "";
    public List<string> KeyProcesses { get; set; } = new();
    public List<string> ResourceRequirements { get; set; } = new();
    public string ScalingStrategy { get; set; } = "";
}

public class RiskAssessment
{
    public List<string> Risks { get; set; } = new();
    public List<string> MitigationStrategies { get; set; } = new();
    public decimal OverallRiskScore { get; set; }
}

// Supporting Classes
public class MilestoneAdjustment
{
    public string MilestoneId { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime OriginalDate { get; set; }
    public DateTime NewDate { get; set; }
    public string Reason { get; set; } = "";
    public List<string> Dependencies { get; set; } = new();
}

public class BusinessMilestone
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime TargetDate { get; set; }
    public int Priority { get; set; } = 1;
    public bool IsCompleted { get; set; } = false;
    public List<string> Dependencies { get; set; } = new();
}

public class ResourceAllocation
{
    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public string Description { get; set; } = "";
}

public class CompetitiveAdvantage
{
    public string Description { get; set; } = "";
    public List<string> KeyDifferentiators { get; set; } = new();
    public string SustainabilityFactor { get; set; } = "";
}

// Enums
public enum WorkSchedule
{
    FullTime,        // 40+ hours/week
    PartTime,        // 20-30 hours/week
    SideHustle,      // 10-15 hours/week
    WeekendWarrior,  // Weekends only
    Custom           // User-defined schedule
}

public enum TimeBuffer
{
    Aggressive,      // Minimal buffer time
    Standard,        // 20% buffer
    Conservative,    // 40% buffer
    Cautious         // 60% buffer
}

public enum FundingApproach
{
    Bootstrap,       // Self-funded
    InvestorReady,   // Seeking investment
    LoanBased,       // Traditional financing
    Crowdfunding,    // Community funded
    Hybrid           // Multiple sources
}

public enum TeamBuildingStrategy
{
    SoloFounder,     // Work alone
    CoFounder,       // Partner-based
    HireEarly,       // Build team quickly
    ContractorBased, // Use contractors
    OutsourcingHeavy // Heavy outsourcing
}

public enum GoToMarketStrategy
{
    LocalFirst,      // Local expansion
    DigitalFirst,    // Online focused
    NetworkBased,    // Relationship driven
    ContentMarketing, // Content focused
    PaidAdvertising   // Ad-driven growth
}

public enum CompetitionStrategy
{
    DirectCompetition, // Head-to-head
    BlueOcean,        // Uncontested market
    MarketDisruption, // Disruptive innovation
    Collaborative,    // Partnership focused
    NicheSpecialist   // Specialized focus
}

public enum MarketScope
{
    Local,
    Regional,
    National,
    International,
    Global
}

public enum CustomerSegment
{
    B2B,    // Business to Business
    B2C,    // Business to Consumer
    B2G,    // Business to Government
    Hybrid  // Multiple segments
}

public enum PricingStrategy
{
    Premium,     // High-end pricing
    Competitive, // Market-rate pricing
    Penetration, // Low introductory pricing
    Value,       // Value-based pricing
    Freemium     // Free with premium options
}

public enum RevenueModel
{
    OneTime,     // Single purchase
    Subscription, // Recurring revenue
    Commission,  // Transaction-based
    Advertising, // Ad-supported
    Hybrid       // Multiple models
}

public enum GrowthStrategy
{
    Organic,     // Natural growth
    Acquisition, // Growth through acquisition
    Partnership, // Strategic partnerships
    Franchise,   // Franchising model
    Licensing    // Licensing strategy
}

public enum ProfitabilityGoal
{
    BreakEven,   // Cover costs
    Moderate,    // 10-20% profit
    High,        // 20%+ profit
    Growth,      // Reinvest everything
    Exit         // Build for sale
}

public enum BusinessModel
{
    Traditional, // Standard business model
    Platform,    // Platform-based
    Marketplace, // Marketplace model
    SaaS,        // Software as a Service
    Subscription, // Subscription-based
    OnDemand     // On-demand service
}

public enum WorkLocation
{
    Remote,      // Fully remote
    Office,      // Physical office
    Hybrid,      // Mix of remote/office
    Coworking,   // Shared workspace
    Home         // Home-based
}

public enum ScalabilityPlan
{
    Manual,      // Manual scaling
    Automated,   // Technology-driven
    Systematic,  // Process-driven
    Franchised,  // Franchise model
    Licensed     // Licensing model
}

public enum QualityStandard
{
    Basic,       // Minimum viable
    Standard,    // Industry standard
    High,        // Above average
    Premium,     // Premium quality
    Luxury       // Luxury standard
}

public enum CustomerServiceLevel
{
    Basic,       // Minimal support
    Standard,    // Regular support
    Premium,     // High-touch support
    Concierge    // White-glove service
}