namespace JaxSun.Ideas.Mock.Models
{
    /// <summary>
    /// Represents a complete business model canvas with all nine sections
    /// </summary>
    public class BusinessModelCanvas
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string BusinessPlanId { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        
        // Core Business Model Canvas Sections
        public List<string> KeyPartners { get; set; } = new();
        public List<string> KeyActivities { get; set; } = new();
        public List<string> KeyResources { get; set; } = new();
        public List<string> ValuePropositions { get; set; } = new();
        public List<string> CustomerRelationships { get; set; } = new();
        public List<string> Channels { get; set; } = new();
        public List<string> CustomerSegments { get; set; } = new();
        public List<string> CostStructure { get; set; } = new();
        public List<string> RevenueStreams { get; set; } = new();
        
        // Metadata
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string LastModifiedBy { get; set; } = string.Empty;
        public int Version { get; set; } = 1;
        public string Status { get; set; } = "Draft"; // Draft, InProgress, Completed, Archived
        
        // Collaboration
        public List<string> Collaborators { get; set; } = new();
        public string ShareLink { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;
        
        // Tags and Notes
        public List<string> Tags { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
        
        // Analytics
        public int ViewCount { get; set; } = 0;
        public int EditCount { get; set; } = 0;
        public DateTime LastViewedAt { get; set; } = DateTime.UtcNow;
    }
}