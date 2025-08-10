namespace Jackson.Ideas.Mock.Models
{
    /// <summary>
    /// Comprehensive business plan result containing all generated business planning components
    /// This is the composite result returned by IBusinessPlanService for Hub 2 operations
    /// </summary>
    public class BusinessPlanResult
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public BusinessPlanRequest Request { get; set; } = new();
        public string ExecutiveSummary { get; set; } = string.Empty;
        public string BusinessCategory { get; set; } = string.Empty;
        public MarketAnalysis MarketAnalysis { get; set; } = new();
        public CompetitiveAnalysis CompetitiveAnalysis { get; set; } = new();
        public MarketingStrategy MarketingStrategy { get; set; } = new();
        public OperationalPlan OperationalPlan { get; set; } = new();
        public FinancialProjections FinancialProjections { get; set; } = new();
        public RiskAssessment RiskAssessment { get; set; } = new();
        public FundingRequirements FundingRequirements { get; set; } = new();
        public int ViabilityScore { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0";
        public bool ReadyForOperations { get; set; }
        public int CompletenessScore { get; set; }
    }

    /// <summary>
    /// Business plan request model - simplified version for IBusinessPlanService
    /// Note: There's also a BusinessPlanRequest in IBusinessPlanBuilderService with different fields
    /// </summary>
    public class BusinessPlanRequest
    {
        public string BusinessIdea { get; set; } = string.Empty;
        public string ProblemSolved { get; set; } = string.Empty;
        public string TargetMarket { get; set; } = string.Empty;
        public string RevenueModel { get; set; } = string.Empty;
        public string CompetitiveAdvantages { get; set; } = string.Empty;
        public decimal InitialInvestment { get; set; }
        public string BusinessType { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Market analysis component for business planning
    /// </summary>
    public class MarketAnalysis
    {
        public string MarketSize { get; set; } = string.Empty;
        public string GrowthRate { get; set; } = string.Empty;
        public List<string> TargetSegments { get; set; } = new();
        public string MarketTrends { get; set; } = string.Empty;
        public List<string> KeyDrivers { get; set; } = new();
        public List<string> MarketBarriers { get; set; } = new();
    }

    /// <summary>
    /// Competitive analysis component
    /// </summary>
    public class CompetitiveAnalysis
    {
        public List<Competitor> DirectCompetitors { get; set; } = new();
        public List<Competitor> IndirectCompetitors { get; set; } = new();
        public string MarketPositioning { get; set; } = string.Empty;
        public List<string> CompetitiveAdvantages { get; set; } = new();
        public List<string> CompetitiveThreats { get; set; } = new();
        public string DifferentiationStrategy { get; set; } = string.Empty;
        public List<string> MarketGaps { get; set; } = new();
        public string CompetitiveIntensity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Competitor information
    /// </summary>
    public class Competitor
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MarketShare { get; set; } = string.Empty;
        public List<string> Strengths { get; set; } = new();
        public List<string> Weaknesses { get; set; } = new();
        public string Positioning { get; set; } = string.Empty;
        public string PricingStrategy { get; set; } = string.Empty;
        public string DifferentiationFactor { get; set; } = string.Empty;
        public bool IsDirect { get; set; } = true;
    }

    /// <summary>
    /// Marketing strategy component
    /// </summary>
    public class MarketingStrategy
    {
        public string TargetAudience { get; set; } = string.Empty;
        public List<string> TargetMarketSegments { get; set; } = new();
        public string ValueProposition { get; set; } = string.Empty;
        public string PricingStrategy { get; set; } = string.Empty;
        public List<string> MarketingChannels { get; set; } = new();
        public string CustomerAcquisitionStrategy { get; set; } = string.Empty;
        public string CustomerRetentionStrategy { get; set; } = string.Empty;
        public BrandingStrategy BrandingStrategy { get; set; } = new();
        public decimal MarketingBudget { get; set; }
    }

    /// <summary>
    /// Branding strategy details
    /// </summary>
    public class BrandingStrategy
    {
        public string BrandIdentity { get; set; } = string.Empty;
        public string BrandPersonality { get; set; } = string.Empty;
        public string BrandPromise { get; set; } = string.Empty;
        public string BrandPosition { get; set; } = string.Empty;
        public List<string> BrandValues { get; set; } = new();
        public string VisualIdentity { get; set; } = string.Empty;
        public string VoiceAndTone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Operational plan component
    /// </summary>
    public class OperationalPlan
    {
        public string BusinessModel { get; set; } = string.Empty;
        public string OperationalStructure { get; set; } = string.Empty;
        public List<string> TechnologyRequirements { get; set; } = new();
        public StaffingPlan StaffingPlan { get; set; } = new();
        public List<string> KeyProcesses { get; set; } = new();
        public string QualityControl { get; set; } = string.Empty;
        public string SupplyChainManagement { get; set; } = string.Empty;
        public string ScalingStrategy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Staffing plan details
    /// </summary>
    public class StaffingPlan
    {
        public List<string> KeyRoles { get; set; } = new();
        public string HiringTimeline { get; set; } = string.Empty;
        public decimal StaffingBudget { get; set; }
        public List<string> SkillRequirements { get; set; } = new();
        public string OrganizationalStructure { get; set; } = string.Empty;
        public string CompensationStrategy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Risk assessment component
    /// </summary>
    public class RiskAssessment
    {
        public List<string> MarketRisks { get; set; } = new();
        public List<string> CompetitiveRisks { get; set; } = new();
        public List<string> FinancialRisks { get; set; } = new();
        public List<string> OperationalRisks { get; set; } = new();
        public List<string> TechnicalRisks { get; set; } = new();
        public List<string> RegulatoryRisks { get; set; } = new();
        public List<string> MitigationStrategies { get; set; } = new();
        public string OverallRiskLevel { get; set; } = string.Empty;
    }

    /// <summary>
    /// Profitability analysis details
    /// </summary>
    public class ProfitabilityAnalysis
    {
        public decimal GrossMarginPercentage { get; set; }
        public decimal OperatingMarginPercentage { get; set; }
        public decimal NetMarginPercentage { get; set; }
        public string ProfitabilityTimeline { get; set; } = string.Empty;
        public List<string> RevenueDrivers { get; set; } = new();
        public List<string> CostDrivers { get; set; } = new();
        public string ScenarioAnalysis { get; set; } = string.Empty;
    }

    /// <summary>
    /// Break-even analysis details
    /// </summary>
    public class BreakEvenAnalysis
    {
        public int BreakEvenPointMonths { get; set; }
        public decimal BreakEvenRevenue { get; set; }
        public int BreakEvenCustomerCount { get; set; }
        public decimal FixedCosts { get; set; }
        public decimal VariableCostPerUnit { get; set; }
        public decimal ContributionMargin { get; set; }
        public string SensitivityAnalysis { get; set; } = string.Empty;
    }

    /// <summary>
    /// Implementation phase for timeline planning
    /// </summary>
    public class ImplementationPhase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationWeeks { get; set; }
        public List<string> KeyTasks { get; set; } = new();
        public List<string> Deliverables { get; set; } = new();
        public List<string> ResourceRequirements { get; set; } = new();
        public List<string> Dependencies { get; set; } = new();
        public string SuccessCriteria { get; set; } = string.Empty;
    }

    /// <summary>
    /// Implementation timeline component
    /// </summary>
    public class ImplementationTimeline
    {
        public List<ImplementationPhase> Phases { get; set; } = new();
        public int TotalDurationWeeks { get; set; }
        public string CriticalPath { get; set; } = string.Empty;
        public List<string> KeyMilestones { get; set; } = new();
        public string ResourceAllocation { get; set; } = string.Empty;
        public string RiskFactors { get; set; } = string.Empty;
    }

    /// <summary>
    /// Business plan template for different industries
    /// </summary>
    public class BusinessPlanTemplate
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string TemplateType { get; set; } = string.Empty;
        public List<string> Sections { get; set; } = new();
    }

   
}