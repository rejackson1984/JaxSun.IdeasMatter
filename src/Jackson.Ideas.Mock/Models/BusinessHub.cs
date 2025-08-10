namespace Jackson.Ideas.Mock.Models
{
    /// <summary>
    /// Defines the three main business development hubs in the platform
    /// </summary>
    public enum BusinessHub
    {
        /// <summary>
        /// Hub 1: Idea Development & Validation - For discovering, developing, and validating business ideas
        /// </summary>
        IdeaDevelopment = 1,
        
        /// <summary>
        /// Hub 2: Business Planning & Solution Development - For strategic planning and solution design
        /// </summary>
        BusinessPlanning = 2,
        
        /// <summary>
        /// Hub 3: Business Operations & Growth - For operations management and scaling
        /// </summary>
        BusinessOperations = 3
    }

    /// <summary>
    /// Represents the progress status of a user within a specific hub
    /// </summary>
    public class HubProgressStatus
    {
        public BusinessHub Hub { get; set; }
        public int CompletionPercentage { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        public List<string> CompletedMilestones { get; set; } = new();
        public List<string> AvailableMilestones { get; set; } = new();
        public string CurrentPhase { get; set; } = string.Empty;
    }

    /// <summary>
    /// Contains metadata and configuration for each hub
    /// </summary>
    public class HubMetadata
    {
        public BusinessHub Hub { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string GradientColors { get; set; } = string.Empty;
        public string CoachPersona { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
        public List<string> RequiredMilestones { get; set; } = new();
        public int UnlockThreshold { get; set; }
    }

    /// <summary>
    /// Represents the current hub context for a user session
    /// </summary>
    public class HubContext
    {
        public BusinessHub CurrentHub { get; set; }
        public BusinessHub? PreviousHub { get; set; }
        public Dictionary<BusinessHub, HubProgressStatus> Progress { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public DateTime SessionStartedAt { get; set; }
        public bool CanSwitchHubs { get; set; } = true;
        
        /// <summary>
        /// Determines if the user can access a specific hub based on their progress
        /// </summary>
        public bool CanAccessHub(BusinessHub hub)
        {
            return true;
            if (hub == BusinessHub.IdeaDevelopment)
                return true; // Hub 1 is always accessible
                
            if (!Progress.ContainsKey(hub))
                return false;

            return Progress[hub].IsUnlocked;
        }
        
        /// <summary>
        /// Gets the overall completion percentage across all accessible hubs
        /// </summary>
        public int GetOverallCompletion()
        {
            var accessibleHubs = Progress.Values.Where(p => p.IsUnlocked).ToList();
            if (!accessibleHubs.Any())
                return 0;
                
            return (int)accessibleHubs.Average(p => p.CompletionPercentage);
        }
    }
}