using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;

namespace JaxSun.Ideas.WebApp.Services.Mock
{
    /// <summary>
    /// Mock implementation of hub context service for demo purposes
    /// </summary>
    public class MockHubContextService : IHubContextService
    {
        private readonly IHubConfigurationService _hubConfigurationService;
        private HubContext _currentContext;
        
        public event EventHandler<HubContextChangedEventArgs>? ContextChanged;
        
        public MockHubContextService(IHubConfigurationService hubConfigurationService)
        {
            _hubConfigurationService = hubConfigurationService;
            _currentContext = InitializeDemoContext();
        }
        
        public Task<HubContext> GetCurrentContextAsync()
        {
            return Task.FromResult(_currentContext);
        }
        
        public async Task<bool> SwitchToHubAsync(BusinessHub hub)
        {
            var canSwitch = await _hubConfigurationService.CanTransitionToHubAsync(_currentContext.CurrentHub, hub, _currentContext);
            
            if (!canSwitch)
                return false;
                
            var previousHub = _currentContext.CurrentHub;
            _currentContext.PreviousHub = previousHub;
            _currentContext.CurrentHub = hub;
            
            // Update last accessed time
            if (_currentContext.Progress.ContainsKey(hub))
            {
                _currentContext.Progress[hub].LastAccessedAt = DateTime.UtcNow;
                _currentContext.Progress[hub].IsActive = true;
            }
            
            // Mark previous hub as inactive
            if (_currentContext.Progress.ContainsKey(previousHub))
            {
                _currentContext.Progress[previousHub].IsActive = false;
            }
            
            // Trigger context changed event
            ContextChanged?.Invoke(this, new HubContextChangedEventArgs
            {
                PreviousHub = previousHub,
                CurrentHub = hub,
                Context = _currentContext
            });
            
            return true;
        }
        
        public async Task UpdateHubProgressAsync(BusinessHub hub, HubProgressStatus progress)
        {
            _currentContext.Progress[hub] = progress;
            
            // Check if this progress unlocks other hubs
            await CheckAndUnlockHubsAsync();
        }
        
        public async Task<bool> CompleteMilestoneAsync(string milestoneId)
        {
            var currentHubProgress = _currentContext.Progress[_currentContext.CurrentHub];
            
            if (!currentHubProgress.CompletedMilestones.Contains(milestoneId))
            {
                currentHubProgress.CompletedMilestones.Add(milestoneId);
                
                // Update completion percentage based on completed milestones
                var totalMilestones = currentHubProgress.AvailableMilestones.Count;
                if (totalMilestones > 0)
                {
                    currentHubProgress.CompletionPercentage = 
                        (currentHubProgress.CompletedMilestones.Count * 100) / totalMilestones;
                }
                else
                {
                    // Fallback: assume at least 5 milestones if none defined
                    currentHubProgress.CompletionPercentage = Math.Min(100, 
                        currentHubProgress.CompletionPercentage + 20);
                }
                
                await CheckAndUnlockHubsAsync();
                return true;
            }
            
            return false;
        }
        
        public Task<HubProgressStatus> GetHubProgressAsync(BusinessHub hub)
        {
            return Task.FromResult(_currentContext.Progress.GetValueOrDefault(hub, new HubProgressStatus
            {
                Hub = hub,
                CompletionPercentage = 0,
                IsUnlocked = hub == BusinessHub.IdeaDevelopment,
                IsActive = false
            }));
        }
        
        public Task<HubContext> InitializeUserContextAsync(string userId)
        {
            var context = new HubContext
            {
                UserId = userId,
                CurrentHub = BusinessHub.IdeaDevelopment,
                SessionStartedAt = DateTime.UtcNow,
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 0,
                        IsUnlocked = true,
                        IsActive = true,
                        LastAccessedAt = DateTime.UtcNow,
                        AvailableMilestones = new List<string>
                        {
                            "idea_submitted",
                            "market_research_initiated",
                            "competitive_analysis_started",
                            "idea_validated",
                            "research_completed"
                        }
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false,
                        AvailableMilestones = new List<string>
                        {
                            "business_model_defined",
                            "financial_projections_created",
                            "strategic_plan_completed",
                            "technical_architecture_planned",
                            "development_roadmap_created"
                        }
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false,
                        AvailableMilestones = new List<string>
                        {
                            "kpi_dashboard_configured",
                            "launch_plan_executed",
                            "customer_analytics_implemented",
                            "growth_metrics_tracked",
                            "ceo_development_started"
                        }
                    }
                }
            };
            
            _currentContext = context;
            return Task.FromResult(context);
        }
        
        private HubContext InitializeDemoContext()
        {
            // Initialize with some demo progress to showcase the hub system
            return new HubContext
            {
                UserId = "demo-user",
                CurrentHub = BusinessHub.IdeaDevelopment,
                SessionStartedAt = DateTime.UtcNow,
                Progress = new Dictionary<BusinessHub, HubProgressStatus>
                {
                    [BusinessHub.IdeaDevelopment] = new HubProgressStatus
                    {
                        Hub = BusinessHub.IdeaDevelopment,
                        CompletionPercentage = 85,
                        IsUnlocked = true,
                        IsActive = true,
                        LastAccessedAt = DateTime.UtcNow,
                        CompletedMilestones = new List<string> 
                        { 
                            "idea_submitted", 
                            "market_research_initiated", 
                            "competitive_analysis_started",
                            "idea_validated"
                        },
                        AvailableMilestones = new List<string>
                        {
                            "idea_submitted",
                            "market_research_initiated",
                            "competitive_analysis_started",
                            "idea_validated",
                            "research_completed"
                        },
                        CurrentPhase = "Research Validation"
                    },
                    [BusinessHub.BusinessPlanning] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessPlanning,
                        CompletionPercentage = 45,
                        IsUnlocked = false, // Fix: Should start locked for tests
                        IsActive = false,
                        LastAccessedAt = DateTime.UtcNow.AddHours(-2),
                        CompletedMilestones = new List<string> 
                        { 
                            "business_model_defined", 
                            "financial_projections_created" 
                        },
                        AvailableMilestones = new List<string>
                        {
                            "business_model_defined",
                            "financial_projections_created",
                            "strategic_plan_completed",
                            "technical_architecture_planned",
                            "development_roadmap_created"
                        },
                        CurrentPhase = "Strategic Planning"
                    },
                    [BusinessHub.BusinessOperations] = new HubProgressStatus
                    {
                        Hub = BusinessHub.BusinessOperations,
                        CompletionPercentage = 0,
                        IsUnlocked = false,
                        IsActive = false,
                        AvailableMilestones = new List<string>
                        {
                            "kpi_dashboard_configured",
                            "launch_plan_executed",
                            "customer_analytics_implemented",
                            "growth_metrics_tracked",
                            "ceo_development_started"
                        },
                        CurrentPhase = "Locked"
                    }
                }
            };
        }
        
        private async Task CheckAndUnlockHubsAsync()
        {
            foreach (var hub in Enum.GetValues<BusinessHub>())
            {
                if (!_currentContext.Progress.ContainsKey(hub) || _currentContext.Progress[hub].IsUnlocked)
                    continue;
                    
                var shouldUnlock = await _hubConfigurationService.ShouldUnlockHubAsync(hub, _currentContext);
                if (shouldUnlock)
                {
                    _currentContext.Progress[hub].IsUnlocked = true;
                }
            }
        }
    }
}