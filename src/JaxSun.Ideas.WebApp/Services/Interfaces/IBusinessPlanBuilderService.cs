using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Models.Builder;

namespace JaxSun.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for building comprehensive business plans with strategic frameworks
    /// </summary>
    public interface IBusinessPlanBuilderService
    {
        /// <summary>
        /// Creates a new business plan from validated idea
        /// </summary>
        Task<BuilderBusinessPlan> CreateBusinessPlanAsync(BusinessPlanBuilderRequest request);
        
        /// <summary>
        /// Builds a comprehensive business plan with analysis and recommendations
        /// </summary>
        Task<BusinessPlanBuilderResult> BuildBusinessPlanAsync(BusinessPlanBuilderRequest request);
        
        /// <summary>
        /// Gets existing business plan by ID
        /// </summary>
        Task<BuilderBusinessPlan?> GetBusinessPlanAsync(string planId);
        
        /// <summary>
        /// Updates business plan section
        /// </summary>
        Task<BuilderBusinessPlan> UpdateBusinessPlanSectionAsync(string planId, string sectionName, object sectionData);
        
        /// <summary>
        /// Gets business model canvas for the plan
        /// </summary>
        Task<BuilderBusinessModelCanvas> GetBusinessModelCanvasAsync(string planId);
        
        /// <summary>
        /// Updates business model canvas
        /// </summary>
        Task<BuilderBusinessModelCanvas> UpdateBusinessModelCanvasAsync(string planId, BuilderBusinessModelCanvas canvas);
        
        /// <summary>
        /// Generates strategic recommendations
        /// </summary>
        Task<List<BuilderStrategicRecommendation>> GetStrategicRecommendationsAsync(string planId);
        
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
        Task<List<BuilderBusinessPlanTemplate>> GetBusinessPlanTemplatesAsync();
        
        /// <summary>
        /// Exports business plan to different formats
        /// </summary>
        Task<BuilderBusinessPlanExport> ExportBusinessPlanAsync(string planId, Models.Builder.ExportFormat format);
    }
}
