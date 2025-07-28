using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for building comprehensive business plans with strategic frameworks
    /// </summary>
    public interface IBusinessPlanBuilderService
    {
        /// <summary>
        /// Creates a new business plan from validated idea
        /// </summary>
        Task<BusinessPlan> CreateBusinessPlanAsync(BusinessPlanRequest request);
        
        /// <summary>
        /// Gets existing business plan by ID
        /// </summary>
        Task<BusinessPlan?> GetBusinessPlanAsync(string planId);
        
        /// <summary>
        /// Updates business plan section
        /// </summary>
        Task<BusinessPlan> UpdateBusinessPlanSectionAsync(string planId, string sectionName, object sectionData);
        
        /// <summary>
        /// Gets business model canvas for the plan
        /// </summary>
        Task<BusinessModelCanvas> GetBusinessModelCanvasAsync(string planId);
        
        /// <summary>
        /// Updates business model canvas
        /// </summary>
        Task<BusinessModelCanvas> UpdateBusinessModelCanvasAsync(string planId, BusinessModelCanvas canvas);
        
        /// <summary>
        /// Generates strategic recommendations
        /// </summary>
        Task<List<StrategicRecommendation>> GetStrategicRecommendationsAsync(string planId);
        
        /// <summary>
        /// Calculates business plan completeness score
        /// </summary>
        Task<int> CalculateCompletenessScoreAsync(string planId);
        
        /// <summary>
        /// Determines if business plan is ready for Hub 3
        /// </summary>
        Task<bool> IsReadyForOperationsAsync(string planId);
        
        /// <summary>
        /// Gets available business plan templates
        /// </summary>
        Task<List<BusinessPlanTemplate>> GetBusinessPlanTemplatesAsync();
        
        /// <summary>
        /// Exports business plan to different formats
        /// </summary>
        Task<BusinessPlanExport> ExportBusinessPlanAsync(string planId, ExportFormat format);
    }
    
    /// <summary>
    /// Request for creating a new business plan
    /// </summary>
    public class BusinessPlanRequest
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
    public class BusinessPlan
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExecutiveSummary ExecutiveSummary { get; set; } = new();
        public MarketAnalysis MarketAnalysis { get; set; } = new();
        public ProductStrategy ProductStrategy { get; set; } = new();
        public MarketingStrategy MarketingStrategy { get; set; } = new();
        public OperationsStrategy OperationsStrategy { get; set; } = new();
        public FinancialStrategy FinancialStrategy { get; set; } = new();
        public RiskAssessment RiskAssessment { get; set; } = new();
        public ImplementationTimeline ImplementationTimeline { get; set; } = new();
        public int CompletenessScore { get; set; }
        public bool ReadyForOperations { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Executive summary section
    /// </summary>
    public class ExecutiveSummary
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
    public class MarketAnalysis
    {
        public string IndustryOverview { get; set; } = string.Empty;
        public string TargetMarket { get; set; } = string.Empty;
        public string MarketSize { get; set; } = string.Empty;
        public string MarketTrends { get; set; } = string.Empty;
        public string CompetitiveAnalysis { get; set; } = string.Empty;
        public string MarketOpportunity { get; set; } = string.Empty;
        public List<CustomerSegment> CustomerSegments { get; set; } = new();
    }
    
    /// <summary>
    /// Product strategy section
    /// </summary>
    public class ProductStrategy
    {
        public string ProductDescription { get; set; } = string.Empty;
        public string DevelopmentPlan { get; set; } = string.Empty;
        public string TechnicalRequirements { get; set; } = string.Empty;
        public string IntellectualProperty { get; set; } = string.Empty;
        public string QualityAssurance { get; set; } = string.Empty;
        public List<ProductFeature> CoreFeatures { get; set; } = new();
        public List<DevelopmentMilestone> Milestones { get; set; } = new();
    }
    
    /// <summary>
    /// Marketing strategy section
    /// </summary>
    public class MarketingStrategy
    {
        public string BrandPositioning { get; set; } = string.Empty;
        public string PricingStrategy { get; set; } = string.Empty;
        public string DistributionChannels { get; set; } = string.Empty;
        public string PromotionalStrategy { get; set; } = string.Empty;
        public string CustomerAcquisition { get; set; } = string.Empty;
        public string CustomerRetention { get; set; } = string.Empty;
        public List<MarketingChannel> Channels { get; set; } = new();
    }
    
    /// <summary>
    /// Operations strategy section
    /// </summary>
    public class OperationsStrategy
    {
        public string OperationalModel { get; set; } = string.Empty;
        public string TechnologyInfrastructure { get; set; } = string.Empty;
        public string QualityControl { get; set; } = string.Empty;
        public string SupplyChain { get; set; } = string.Empty;
        public string Scalability { get; set; } = string.Empty;
        public List<KeyProcess> KeyProcesses { get; set; } = new();
        public List<Resource> RequiredResources { get; set; } = new();
    }
    
    /// <summary>
    /// Financial strategy section
    /// </summary>
    public class FinancialStrategy
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
    public class RiskAssessment
    {
        public List<BusinessRisk> IdentifiedRisks { get; set; } = new();
        public string RiskMitigationStrategy { get; set; } = string.Empty;
        public string ContingencyPlans { get; set; } = string.Empty;
        public string InsuranceRequirements { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Implementation timeline section
    /// </summary>
    public class ImplementationTimeline
    {
        public List<Phase> Phases { get; set; } = new();
        public List<Milestone> KeyMilestones { get; set; } = new();
        public string CriticalPath { get; set; } = string.Empty;
        public string ResourceAllocation { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Business Model Canvas
    /// </summary>
    public class BusinessModelCanvas
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
    public class StrategicRecommendation
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
    public class CustomerSegment
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Characteristics { get; set; } = string.Empty;
    }
    
    public class ProductFeature
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
    
    public class DevelopmentMilestone
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Criteria {  get; set; } = string.Empty;
    }
    
    public class MarketingChannel
    {
        public string Name { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string ExpectedROI { get; set; } = string.Empty;
    }
    
    public class KeyProcess
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Metrics { get; set; } = string.Empty;
    }
    
    public class Resource
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Cost { get; set; } = string.Empty;
    }
    
    public class BusinessRisk
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string Probability { get; set; } = string.Empty;
        public string MitigationStrategy { get; set; } = string.Empty;
    }
    
    public class Phase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Deliverables { get; set; } = new();
    }
    
      
    public class Milestone
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
    }
    
    public class BusinessPlanTemplate
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
    }
    
    public class BusinessPlanExport
    {
        public string Id { get; set; } = string.Empty;
        public ExportFormat Format { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum ExportFormat
    {
        PDF,
        Word,
        PowerPoint,
        Excel
    }
}