namespace JaxSun.Ideas.WebApp.Models.Builder
{
    /// <summary>
    /// Request for creating a new business plan
    /// </summary>
    public class BusinessPlanBuilderRequest
    {
        public string IdeaId { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProblemStatement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string TargetMarket { get; set; } = string.Empty;
        public string RevenueModel { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Comprehensive business plan structure
    /// </summary>  
    public class BuilderBusinessPlan
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BuilderExecutiveSummary ExecutiveSummary { get; set; } = new();
        public BuilderMarketAnalysis MarketAnalysis { get; set; } = new();
        public BuilderProductStrategy ProductStrategy { get; set; } = new();
        public BuilderMarketingStrategy MarketingStrategy { get; set; } = new();
        public BuilderOperationsStrategy OperationsStrategy { get; set; } = new();
        public BuilderFinancialStrategy FinancialStrategy { get; set; } = new();
        public BuilderRiskAssessment RiskAssessment { get; set; } = new();
        public BuilderImplementationTimeline ImplementationTimeline { get; set; } = new();
        public int CompletenessScore { get; set; }
        public bool ReadyForOperations { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Executive summary section
    /// </summary>
    public class BuilderExecutiveSummary
    {
        public string BusinessConcept { get; set; } = string.Empty;
        public string MissionStatement { get; set; } = string.Empty;
        public string VisionStatement { get; set; } = string.Empty;
        public string ValueProposition { get; set; } = string.Empty;
        public string KeySuccessFactors { get; set; } = string.Empty;
        public string FundingRequirements { get; set; } = string.Empty;
        public string ExpectedReturns { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Market analysis section
    /// </summary>
    public class BuilderMarketAnalysis
    {
        public string IndustryOverview { get; set; } = string.Empty;
        public string TargetMarket { get; set; } = string.Empty;
        public string MarketSize { get; set; } = string.Empty;
        public string MarketTrends { get; set; } = string.Empty;
        public string CompetitiveAnalysis { get; set; } = string.Empty;
        public string MarketOpportunity { get; set; } = string.Empty;
        public List<BuilderCustomerSegment> CustomerSegments { get; set; } = new();
    }
    
    /// <summary>
    /// Product strategy section
    /// </summary>
    public class BuilderProductStrategy
    {
        public string ProductDescription { get; set; } = string.Empty;
        public string DevelopmentPlan { get; set; } = string.Empty;
        public string TechnicalRequirements { get; set; } = string.Empty;
        public string IntellectualProperty { get; set; } = string.Empty;
        public string QualityAssurance { get; set; } = string.Empty;
        public List<BuilderProductFeature> CoreFeatures { get; set; } = new();
        public List<BuilderDevelopmentMilestone> Milestones { get; set; } = new();
    }
    
    /// <summary>
    /// Marketing strategy section
    /// </summary>
    public class BuilderMarketingStrategy
    {
        public string BrandPositioning { get; set; } = string.Empty;
        public string PricingStrategy { get; set; } = string.Empty;
        public string DistributionChannels { get; set; } = string.Empty;
        public string PromotionalStrategy { get; set; } = string.Empty;
        public string CustomerAcquisition { get; set; } = string.Empty;
        public string CustomerRetention { get; set; } = string.Empty;
        public List<BuilderMarketingChannel> Channels { get; set; } = new();
    }
    
    /// <summary>
    /// Operations strategy section
    /// </summary>
    public class BuilderOperationsStrategy
    {
        public string OperationalModel { get; set; } = string.Empty;
        public string TechnologyInfrastructure { get; set; } = string.Empty;
        public string QualityControl { get; set; } = string.Empty;
        public string SupplyChain { get; set; } = string.Empty;
        public string Scalability { get; set; } = string.Empty;
        public List<BuilderKeyProcess> KeyProcesses { get; set; } = new();
        public List<BuilderResource> RequiredResources { get; set; } = new();
    }
    
    /// <summary>
    /// Financial strategy section
    /// </summary>
    public class BuilderFinancialStrategy
    {
        public string RevenueModel { get; set; } = string.Empty;
        public string CostStructure { get; set; } = string.Empty;
        public string FundingStrategy { get; set; } = string.Empty;
        public string FinancialProjections { get; set; } = string.Empty;
        public string BreakEvenAnalysis { get; set; } = string.Empty;
        public string CashFlowManagement { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Risk assessment section
    /// </summary>
    public class BuilderRiskAssessment
    {
        public List<BuilderBusinessRisk> IdentifiedRisks { get; set; } = new();
        public string RiskMitigationStrategy { get; set; } = string.Empty;
        public string ContingencyPlans { get; set; } = string.Empty;
        public string InsuranceRequirements { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Implementation timeline section
    /// </summary>
    public class BuilderImplementationTimeline
    {
        public List<BuilderPhase> Phases { get; set; } = new();
        public List<BuilderMilestone> KeyMilestones { get; set; } = new();
        public string CriticalPath { get; set; } = string.Empty;
        public string ResourceAllocation { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Business Model Canvas
    /// </summary>
    public class BuilderBusinessModelCanvas
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BusinessPlanId { get; set; } = string.Empty;
        public List<string> KeyPartners { get; set; } = new();
        public List<string> KeyActivities { get; set; } = new();
        public List<string> KeyResources { get; set; } = new();
        public List<string> ValuePropositions { get; set; } = new();
        public List<string> CustomerRelationships { get; set; } = new();
        public List<string> Channels { get; set; } = new();
        public List<string> CustomerSegments { get; set; } = new();
        public List<string> CostStructure { get; set; } = new();
        public List<string> RevenueStreams { get; set; } = new();
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Strategic recommendation
    /// </summary>
    public class BuilderStrategicRecommendation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Impact { get; set; } = string.Empty;
        public string Implementation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    // Supporting classes
    public class BuilderCustomerSegment
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Characteristics { get; set; } = string.Empty;
    }
    
    public class BuilderProductFeature
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
    
    public class BuilderDevelopmentMilestone
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Criteria {  get; set; } = string.Empty;
    }
    
    public class BuilderMarketingChannel
    {
        public string Name { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string ExpectedROI { get; set; } = string.Empty;
    }
    
    public class BuilderKeyProcess
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Metrics { get; set; } = string.Empty;
    }
    
    public class BuilderResource
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Cost { get; set; } = string.Empty;
    }
    
    public class BuilderBusinessRisk
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string Probability { get; set; } = string.Empty;
        public string MitigationStrategy { get; set; } = string.Empty;
    }
    
    public class BuilderPhase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Deliverables { get; set; } = new();
    }
      
    public class BuilderMilestone
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
    }
    
    public class BuilderBusinessPlanTemplate
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
    }
    
    public class BuilderBusinessPlanExport
    {
        public string Id { get; set; } = string.Empty;
        public ExportFormat Format { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Result wrapper for business plan builder operations
    /// Contains the generated business plan along with request metadata and analysis
    /// </summary>
    public class BusinessPlanBuilderResult
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public BusinessPlanBuilderRequest Request { get; set; } = new();
        public BuilderBusinessPlan BusinessPlan { get; set; } = new();
        public int CompletenessScore { get; set; }
        public bool ReadyForOperations { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public List<BuilderStrategicRecommendation> StrategicRecommendations { get; set; } = new();
        public string ValidationSummary { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0";
        public BuilderBusinessModelCanvas? BusinessModelCanvas { get; set; }
    }

    public enum ExportFormat
    {
        PDF,
        Word,
        PowerPoint,
        Excel
    }
}