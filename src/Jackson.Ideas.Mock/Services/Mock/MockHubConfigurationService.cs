using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Mock implementation of hub configuration service with predefined hub metadata
    /// </summary>
    public class MockHubConfigurationService : IHubConfigurationService
    {
        private readonly Dictionary<BusinessHub, HubMetadata> _hubMetadata;
        
        public MockHubConfigurationService()
        {
            _hubMetadata = InitializeHubMetadata();
        }
        
        public Task<HubMetadata> GetHubMetadataAsync(BusinessHub hub)
        {
            return Task.FromResult(_hubMetadata.GetValueOrDefault(hub, new HubMetadata()));
        }
        
        public Task<Dictionary<BusinessHub, HubMetadata>> GetAllHubMetadataAsync()
        {
            return Task.FromResult(_hubMetadata);
        }
        
        public BusinessHub GetDefaultHub()
        {
            return BusinessHub.IdeaDevelopment;
        }
        
        public Task<bool> ShouldUnlockHubAsync(BusinessHub hub, HubContext context)
        {
            switch (hub)
            {
                case BusinessHub.IdeaDevelopment:
                    return Task.FromResult(true); // Always unlocked
                    
                case BusinessHub.BusinessPlanning:
                    // Unlock when Hub 1 is at least 70% complete
                    if (context.Progress.ContainsKey(BusinessHub.IdeaDevelopment))
                    {
                        var hub1Progress = context.Progress[BusinessHub.IdeaDevelopment];
                        return Task.FromResult(hub1Progress.CompletionPercentage >= 70);
                    }
                    return Task.FromResult(false);
                    
                case BusinessHub.BusinessOperations:
                    // Unlock when Hub 2 is at least 80% complete
                    if (context.Progress.ContainsKey(BusinessHub.BusinessPlanning))
                    {
                        var hub2Progress = context.Progress[BusinessHub.BusinessPlanning];
                        return Task.FromResult(hub2Progress.CompletionPercentage >= 80);
                    }
                    return Task.FromResult(false);
                    
                default:
                    return Task.FromResult(false);
            }
        }
        
        public Task<List<string>> GetUnlockRequirementsAsync(BusinessHub hub)
        {
            var metadata = _hubMetadata.GetValueOrDefault(hub, new HubMetadata());
            return Task.FromResult(metadata.RequiredMilestones);
        }
        
        public Task<bool> CanTransitionToHubAsync(BusinessHub fromHub, BusinessHub toHub, HubContext context)
        {
            // Can always go to a lower-numbered hub
            if ((int)toHub <= (int)fromHub)
                return Task.FromResult(true);
                
            // For higher-numbered hubs, check if they're unlocked
            return Task.FromResult(context.CanAccessHub(toHub));
        }
        
        private Dictionary<BusinessHub, HubMetadata> InitializeHubMetadata()
        {
            return new Dictionary<BusinessHub, HubMetadata>
            {
                [BusinessHub.IdeaDevelopment] = new HubMetadata
                {
                    Hub = BusinessHub.IdeaDevelopment,
                    Name = "Idea Development",
                    Description = "Discover, develop, and validate your business ideas with AI-powered research and validation tools.",
                    Icon = "fas fa-lightbulb",
                    PrimaryColor = "#7b8fef",
                    SecondaryColor = "#22d3ee",
                    GradientColors = "linear-gradient(135deg, #7b8fef 0%, #22d3ee 100%)",
                    CoachPersona = "Spark",
                    Features = new List<string>
                    {
                        "AI-Powered Idea Submission Wizard",
                        "Comprehensive Market Research",
                        "Idea Validation Workflow",
                        "Competitive Landscape Analysis",
                        "Trend Analysis & Opportunity Sizing",
                        "Achievement & Badge System"
                    },
                    RequiredMilestones = new List<string>(),
                    UnlockThreshold = 0
                },
                
                [BusinessHub.BusinessPlanning] = new HubMetadata
                {
                    Hub = BusinessHub.BusinessPlanning,
                    Name = "Business Planning",
                    Description = "Transform your validated idea into a comprehensive business plan with strategic frameworks and solution design.",
                    Icon = "fas fa-clipboard-list",
                    PrimaryColor = "#5a6fd8",
                    SecondaryColor = "#3b82f6",
                    GradientColors = "linear-gradient(135deg, #5a6fd8 0%, #3b82f6 100%)",
                    CoachPersona = "Strategy",
                    Features = new List<string>
                    {
                        "Business Plan Builder",
                        "Strategic Planning Modules",
                        "Financial Modeling & Projections",
                        "Technical Architecture Planning",
                        "Feature Specification Tools",
                        "Development Roadmap Creation"
                    },
                    RequiredMilestones = new List<string>
                    {
                        "idea_validated",
                        "market_research_complete",
                        "competitive_analysis_done"
                    },
                    UnlockThreshold = 10
                },
                
                [BusinessHub.BusinessOperations] = new HubMetadata
                {
                    Hub = BusinessHub.BusinessOperations,
                    Name = "Business Operations",
                    Description = "Launch and scale your business with executive dashboards, growth analytics, and CEO development tools.",
                    Icon = "fas fa-chart-line",
                    PrimaryColor = "#4c5fd7",
                    SecondaryColor = "#059669",
                    GradientColors = "linear-gradient(135deg, #4c5fd7 0%, #059669 100%)",
                    CoachPersona = "Execute",
                    Features = new List<string>
                    {
                        "Executive KPI Dashboard",
                        "Customer Analytics Suite",
                        "Growth Metrics & Forecasting",
                        "Go-to-Market Execution Tools",
                        "Brand Development Tools",
                        "CEO Development Program"
                    },
                    RequiredMilestones = new List<string>
                    {
                        "business_plan_complete",
                        "financial_model_validated",
                        "technical_architecture_defined",
                        "development_roadmap_created"
                    },
                    UnlockThreshold = 10
                }
            };
        }
    }
}