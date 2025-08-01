using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for generating comprehensive business plans and financial projections
    /// </summary>
    public interface IBusinessPlanService
    {
        /// <summary>
        /// Generates a comprehensive business plan based on validated idea
        /// </summary>
        Task<BusinessPlanResult> GenerateBusinessPlanAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Creates detailed financial projections for 3-year period
        /// </summary>
        Task<Jackson.Ideas.Mock.Models.FinancialProjections> GenerateFinancialProjectionsAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Develops marketing strategy and customer acquisition plan
        /// </summary>
        Task<Jackson.Ideas.Mock.Models.MarketingStrategy> GenerateMarketingStrategyAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Creates operational plan and business structure
        /// </summary>
        Task<Jackson.Ideas.Mock.Models.OperationalPlan> GenerateOperationalPlanAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Assesses business risks and mitigation strategies
        /// </summary>
        Task<Jackson.Ideas.Mock.Models.RiskAssessment> GenerateRiskAssessmentAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Calculates overall business viability score
        /// </summary>
        Task<int> CalculateBusinessViabilityScoreAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Determines if business plan is ready for operations phase
        /// </summary>
        Task<bool> IsReadyForOperationsHubAsync(BusinessPlanResult businessPlan);
        
        /// <summary>
        /// Gets available business plan templates
        /// </summary>
        Task<List<Jackson.Ideas.Mock.Models.BusinessPlanTemplate>> GetBusinessPlanTemplatesAsync();
        
        /// <summary>
        /// Recommends appropriate funding options
        /// </summary>
        Task<List<FundingOption>> GetFundingOptionsAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Validates business model viability
        /// </summary>
        Task<BusinessModelValidation> ValidateBusinessModelAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Generates competitive analysis for business planning
        /// </summary>
        Task<Jackson.Ideas.Mock.Models.CompetitiveAnalysis> GenerateCompetitiveAnalysisAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Creates implementation timeline and roadmap
        /// </summary>
        Task<Jackson.Ideas.Mock.Models.ImplementationTimeline> GenerateTimelineAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request);
    }
}