using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Models.Builder;
using JaxSun.Ideas.Mock.Services.Interfaces;

namespace JaxSun.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Validates quality gates for hub progression based on PRD requirements
    /// Hub 1 → Hub 2: 70% completion threshold
    /// Hub 2 → Hub 3: 80% completion threshold
    /// </summary>
    public class QualityGateValidator
    {
        private readonly IHubContextService _hubContextService;
        private readonly IIdeaValidationService _ideaValidationService;
        private readonly IBusinessPlanBuilderService _businessPlanService;
        private readonly IBusinessOperationsService _businessOperationsService;

        // Quality gate thresholds as defined in PRD
        private const int HUB1_TO_HUB2_THRESHOLD = 70;
        private const int HUB2_TO_HUB3_THRESHOLD = 80;

        public QualityGateValidator(
            IHubContextService hubContextService,
            IIdeaValidationService ideaValidationService,
            IBusinessPlanBuilderService businessPlanService,
            IBusinessOperationsService businessOperationsService)
        {
            _hubContextService = hubContextService;
            _ideaValidationService = ideaValidationService;
            _businessPlanService = businessPlanService;
            _businessOperationsService = businessOperationsService;
        }

        /// <summary>
        /// Validates if a user can progress from current hub to target hub
        /// </summary>
        public async Task<QualityGateResult> ValidateProgressionAsync(BusinessHub fromHub, BusinessHub toHub, HubContext context)
        {
            var result = new QualityGateResult
            {
                FromHub = fromHub,
                ToHub = toHub,
                IsValid = false,
                ValidationErrors = new List<string>()
            };

            // Get current hub progress
            var hubProgress = await _hubContextService.GetHubProgressAsync(fromHub);
            result.CurrentCompletionPercentage = hubProgress.CompletionPercentage;

            // Validate progression based on hub transition
            switch ((fromHub, toHub))
            {
                case (BusinessHub.IdeaDevelopment, BusinessHub.BusinessPlanning):
                    result = await ValidateHub1ToHub2Progression(result, context);
                    break;
                    
                case (BusinessHub.BusinessPlanning, BusinessHub.BusinessOperations):
                    result = await ValidateHub2ToHub3Progression(result, context);
                    break;
                    
                default:
                    result.ValidationErrors.Add($"Invalid hub progression from {fromHub} to {toHub}");
                    break;
            }

            return result;
        }

        /// <summary>
        /// Validates progression from Hub 1 (Idea Development) to Hub 2 (Business Planning)
        /// Requires 70% completion threshold
        /// </summary>
        public async Task<QualityGateResult> ValidateHub1ToHub2Progression(QualityGateResult result, HubContext context)
        {
            var ideaProgress = context.Progress[BusinessHub.IdeaDevelopment];
            result.CurrentCompletionPercentage = ideaProgress.CompletionPercentage;
            result.RequiredThreshold = HUB1_TO_HUB2_THRESHOLD;

            // Check completion percentage threshold
            if (ideaProgress.CompletionPercentage < HUB1_TO_HUB2_THRESHOLD)
            {
                result.ValidationErrors.Add($"Idea Development completion is {ideaProgress.CompletionPercentage}%. Required: {HUB1_TO_HUB2_THRESHOLD}%");
                return result;
            }

            // Check required milestones
            var requiredMilestones = new[] { "idea_submitted", "idea_validated", "market_research_initiated" };
            var missingMilestones = requiredMilestones.Except(ideaProgress.CompletedMilestones).ToList();
            
            if (missingMilestones.Any())
            {
                result.ValidationErrors.Add($"Missing required milestones: {string.Join(", ", missingMilestones)}");
                return result;
            }

            // Validate idea validation results if service available
            try
            {
                var ideaValidationResult = await _ideaValidationService.GetValidationResultAsync(context.UserId);
                if (ideaValidationResult != null && ideaValidationResult.OverallScore < 70)
                {
                    result.ValidationErrors.Add($"Idea validation score ({ideaValidationResult.OverallScore}/100) is below required threshold (70)");
                    return result;
                }
            }
            catch
            {
                // Service unavailable - skip validation
            }

            result.IsValid = true;
            result.Message = "Hub 1 to Hub 2 progression validated successfully";
            return result;
        }

        /// <summary>
        /// Validates progression from Hub 2 (Business Planning) to Hub 3 (Business Operations)
        /// Requires 80% completion threshold
        /// </summary>
        public async Task<QualityGateResult> ValidateHub2ToHub3Progression(QualityGateResult result, HubContext context)
        {
            var planningProgress = context.Progress[BusinessHub.BusinessPlanning];
            result.CurrentCompletionPercentage = planningProgress.CompletionPercentage;
            result.RequiredThreshold = HUB2_TO_HUB3_THRESHOLD;

            // Check completion percentage threshold
            if (planningProgress.CompletionPercentage < HUB2_TO_HUB3_THRESHOLD)
            {
                result.ValidationErrors.Add($"Business Planning completion is {planningProgress.CompletionPercentage}%. Required: {HUB2_TO_HUB3_THRESHOLD}%");
                return result;
            }

            // Check required milestones
            var requiredMilestones = new[] { "business_model_defined", "financial_projections_created", "strategic_plan_completed" };
            var missingMilestones = requiredMilestones.Except(planningProgress.CompletedMilestones).ToList();
            
            if (missingMilestones.Any())
            {
                result.ValidationErrors.Add($"Missing required milestones: {string.Join(", ", missingMilestones)}");
                return result;
            }

            // Validate business plan completeness if service available
            try
            {
                var businessPlanResult = await _businessPlanService.GetBusinessPlanAsync(context.UserId);
                if (businessPlanResult != null && businessPlanResult.CompletenessScore < 80)
                {
                    result.ValidationErrors.Add($"Business plan completeness score ({businessPlanResult.CompletenessScore}%) is below required threshold (80%)");
                    return result;
                }
            }
            catch
            {
                // Service unavailable - skip validation
            }

            result.IsValid = true;
            result.Message = "Hub 2 to Hub 3 progression validated successfully";
            return result;
        }

        /// <summary>
        /// Gets the completion percentage required for progression from the specified hub
        /// </summary>
        public int GetRequiredCompletionPercentage(BusinessHub fromHub, BusinessHub toHub)
        {
            return (fromHub, toHub) switch
            {
                (BusinessHub.IdeaDevelopment, BusinessHub.BusinessPlanning) => HUB1_TO_HUB2_THRESHOLD,
                (BusinessHub.BusinessPlanning, BusinessHub.BusinessOperations) => HUB2_TO_HUB3_THRESHOLD,
                _ => 100 // Default to 100% for unknown transitions
            };
        }

        /// <summary>
        /// Checks if a user meets all quality gate requirements for the specified hub transition
        /// </summary>
        public async Task<bool> CanProgressToHubAsync(BusinessHub fromHub, BusinessHub toHub, HubContext context)
        {
            var result = await ValidateProgressionAsync(fromHub, toHub, context);
            return result.IsValid;
        }
    }

    /// <summary>
    /// Result of quality gate validation
    /// </summary>
    public class QualityGateResult
    {
        public BusinessHub FromHub { get; set; }
        public BusinessHub ToHub { get; set; }
        public bool IsValid { get; set; }
        public int CurrentCompletionPercentage { get; set; }
        public int RequiredThreshold { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new();
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    }
}