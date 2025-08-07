using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for validating business ideas and providing scoring/analysis
    /// </summary>
    public interface IIdeaValidationService
    {
        /// <summary>
        /// Validates an idea and returns a comprehensive validation result
        /// </summary>
        Task<IdeaValidationResult> ValidateIdeaAsync(IdeaValidationRequest request);
        
        /// <summary>
        /// Gets validation result for a user by ID
        /// </summary>
        Task<IdeaValidationResult?> GetValidationResultAsync(string userId);
        
        /// <summary>
        /// Gets validation criteria for different validation strategies
        /// </summary>
        Task<List<ValidationCriteria>> GetValidationCriteriaAsync(string strategy);
        
        /// <summary>
        /// Calculates an overall validation score for an idea
        /// </summary>
        Task<int> CalculateValidationScoreAsync(IdeaValidationRequest request);
        
        /// <summary>
        /// Gets specific recommendations for improving an idea
        /// </summary>
        Task<List<string>> GetImprovementRecommendationsAsync(IdeaValidationResult result);
        
        /// <summary>
        /// Determines if an idea is ready to advance to the next hub
        /// </summary>
        Task<bool> IsReadyForNextHubAsync(IdeaValidationResult result);
        
        /// <summary>
        /// Gets market research suggestions based on the idea
        /// </summary>
        Task<List<string>> GetMarketResearchSuggestionsAsync(IdeaValidationRequest request);
        
        /// <summary>
        /// Analyzes competitive landscape for the idea
        /// </summary>
        Task<CompetitiveAnalysis> AnalyzeCompetitionAsync(IdeaValidationRequest request);
        
        /// <summary>
        /// Estimates market opportunity size
        /// </summary>
        Task<MarketOpportunity> EstimateMarketOpportunityAsync(IdeaValidationRequest request);
    }
    
    /// <summary>
    /// Request model for idea validation
    /// </summary>
    public class IdeaValidationRequest
    {
        public string Description { get; set; } = string.Empty;
        public string ProblemSolved { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
        public string RevenueModel { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Comprehensive validation result
    /// </summary>
    public class IdeaValidationResult
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public IdeaValidationRequest Request { get; set; } = new();
        public int OverallScore { get; set; }
        public Dictionary<string, int> CategoryScores { get; set; } = new();
        public List<string> Strengths { get; set; } = new();
        public List<string> Weaknesses { get; set; } = new();
        public List<string> Opportunities { get; set; } = new();
        public List<string> Threats { get; set; } = new();
        public List<string> NextSteps { get; set; } = new();
        public bool ReadyForNextHub { get; set; }
        public MarketOpportunity MarketOpportunity { get; set; } = new();
        public CompetitiveAnalysis Competition { get; set; } = new();
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Validation criteria for different strategies
    /// </summary>
    public class ValidationCriteria
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Weight { get; set; }
        public string Category { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Competitive analysis result
    /// </summary>
    public class CompetitiveAnalysis
    {
        public List<Competitor> DirectCompetitors { get; set; } = new();
        public List<Competitor> IndirectCompetitors { get; set; } = new();
        public string CompetitiveLandscape { get; set; } = string.Empty;
        public List<string> CompetitiveAdvantages { get; set; } = new();
        public List<string> MarketGaps { get; set; } = new();
        public string CompetitiveIntensity {  get; set; } = string.Empty;
        public List<string> CompetitiveThreats { get; set; } = new();
        public int CompetitionIntensity { get; set; } // 1-10 scale
    }
    
    /// <summary>
    /// Competitor information
    /// </summary>
    public class Competitor
    {
        public string DifferentiationFactor {  get; set; } = string.Empty;
        public string PricingStrategy {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Strengths { get; set; } = string.Empty;
        public string Weaknesses { get; set; } = string.Empty;
        public string MarketShare { get; set; } = string.Empty;
        public bool IsDirect { get; set; }
    }
    
    /// <summary>
    /// Market opportunity analysis
    /// </summary>
    public class MarketOpportunity
    {
        public string MarketSize { get; set; } = string.Empty;
        public string GrowthRate { get; set; } = string.Empty;
        public string Trends { get; set; } = string.Empty;
        public List<string> KeyDrivers { get; set; } = new();
        public List<string> Barriers { get; set; } = new();
        public int OpportunityScore { get; set; } // 1-100 scale
        public string SizeCategory { get; set; } = string.Empty; // Niche, Medium, Large
    }
}