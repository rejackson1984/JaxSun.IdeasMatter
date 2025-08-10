using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for managing hub context and navigation state
    /// </summary>
    public interface IHubContextService
    {
        /// <summary>
        /// Gets the current hub context for the user
        /// </summary>
        Task<HubContext> GetCurrentContextAsync();
        
        /// <summary>
        /// Switches to a different hub with validation
        /// </summary>
        Task<bool> SwitchToHubAsync(BusinessHub hub);
        
        /// <summary>
        /// Updates progress for a specific hub
        /// </summary>
        Task UpdateHubProgressAsync(BusinessHub hub, HubProgressStatus progress);
        
        /// <summary>
        /// Marks a milestone as completed in the current hub
        /// </summary>
        Task<bool> CompleteMilestoneAsync(string milestoneId);
        
        /// <summary>
        /// Gets progress status for a specific hub
        /// </summary>
        Task<HubProgressStatus> GetHubProgressAsync(BusinessHub hub);
        
        /// <summary>
        /// Initializes hub context for a new user
        /// </summary>
        Task<HubContext> InitializeUserContextAsync(string userId);
        
        /// <summary>
        /// Event triggered when hub context changes
        /// </summary>
        event EventHandler<HubContextChangedEventArgs>? ContextChanged;
    }
    
    /// <summary>
    /// Event arguments for hub context changes
    /// </summary>
    public class HubContextChangedEventArgs : EventArgs
    {
        public BusinessHub PreviousHub { get; set; }
        public BusinessHub CurrentHub { get; set; }
        public HubContext Context { get; set; } = new();
    }
}