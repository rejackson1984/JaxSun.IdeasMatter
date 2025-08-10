using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for managing hub configurations and metadata
    /// </summary>
    public interface IHubConfigurationService
    {
        /// <summary>
        /// Gets metadata for a specific hub
        /// </summary>
        Task<HubMetadata> GetHubMetadataAsync(BusinessHub hub);
        
        /// <summary>
        /// Gets metadata for all hubs
        /// </summary>
        Task<Dictionary<BusinessHub, HubMetadata>> GetAllHubMetadataAsync();
        
        /// <summary>
        /// Gets the default hub for new users
        /// </summary>
        BusinessHub GetDefaultHub();
        
        /// <summary>
        /// Determines if a hub should be unlocked based on progress
        /// </summary>
        Task<bool> ShouldUnlockHubAsync(BusinessHub hub, HubContext context);
        
        /// <summary>
        /// Gets the required milestones to unlock a specific hub
        /// </summary>
        Task<List<string>> GetUnlockRequirementsAsync(BusinessHub hub);
        
        /// <summary>
        /// Validates if a hub transition is allowed
        /// </summary>
        Task<bool> CanTransitionToHubAsync(BusinessHub fromHub, BusinessHub toHub, HubContext context);
    }
}