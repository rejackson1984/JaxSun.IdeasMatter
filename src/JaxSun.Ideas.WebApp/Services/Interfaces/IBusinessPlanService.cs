using JaxSun.Ideas.WebApp.Models;

namespace JaxSun.Ideas.WebApp.Services.Interfaces
{
    /// <summary>
    /// Service for generating comprehensive business plans and financial projections
    /// </summary>
    public interface IBusinessPlanService
    {
        /// <summary>
        /// Generates a comprehensive business plan based on validated idea
        /// </summary>
        Task<BusinessPlanResult> GenerateBusinessPlanAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Creates detailed financial projections for 3-year period
        /// </summary>
        Task<JaxSun.Ideas.WebApp.Models.FinancialProjections> GenerateFinancialProjectionsAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Develops marketing strategy and customer acquisition plan
        /// </summary>
        Task<JaxSun.Ideas.WebApp.Models.MarketingStrategy> GenerateMarketingStrategyAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Creates operational plan and business structure
        /// </summary>
        Task<JaxSun.Ideas.WebApp.Models.OperationalPlan> GenerateOperationalPlanAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Assesses business risks and mitigation strategies
        /// </summary>
        Task<JaxSun.Ideas.WebApp.Models.RiskAssessment> GenerateRiskAssessmentAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Calculates overall business viability score
        /// </summary>
        Task<int> CalculateBusinessViabilityScoreAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Determines if business plan is ready for operations phase
        /// </summary>
        Task<bool> IsReadyForOperationsHubAsync(BusinessPlanResult businessPlan);
        
        /// <summary>
        /// Gets available business plan templates
        /// </summary>
        Task<List<JaxSun.Ideas.WebApp.Models.BusinessPlanTemplate>> GetBusinessPlanTemplatesAsync();
        
        /// <summary>
        /// Recommends appropriate funding options
        /// </summary>
        Task<List<FundingOption>> GetFundingOptionsAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Validates business model viability
        /// </summary>
        Task<BusinessModelValidation> ValidateBusinessModelAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Generates competitive analysis for business planning
        /// </summary>
        Task<JaxSun.Ideas.WebApp.Models.CompetitiveAnalysis> GenerateCompetitiveAnalysisAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
        
        /// <summary>
        /// Creates implementation timeline and roadmap
        /// </summary>
        Task<JaxSun.Ideas.WebApp.Models.ImplementationTimeline> GenerateTimelineAsync(JaxSun.Ideas.WebApp.Models.BusinessPlanRequest request);
    }
}