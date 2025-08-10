using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for managing business model canvas creation and collaboration
    /// </summary>
    public interface IBusinessModelCanvasService
    {
        /// <summary>
        /// Creates a new business model canvas from business plan
        /// </summary>
        Task<BusinessModelCanvas> CreateCanvasAsync(CanvasCreationRequest request);
        
        /// <summary>
        /// Gets existing business model canvas by ID
        /// </summary>
        Task<BusinessModelCanvas?> GetCanvasAsync(string canvasId);
        
        /// <summary>
        /// Updates a specific canvas section
        /// </summary>
        Task<BusinessModelCanvas> UpdateCanvasSectionAsync(string canvasId, string sectionName, List<string> items);
        
        /// <summary>
        /// Gets canvas templates for different business types
        /// </summary>
        Task<List<CanvasTemplate>> GetCanvasTemplatesAsync();
        
        /// <summary>
        /// Applies a template to an existing canvas
        /// </summary>
        Task<BusinessModelCanvas> ApplyTemplateAsync(string canvasId, string templateId);
        
        /// <summary>
        /// Gets strategic insights and recommendations for the canvas
        /// </summary>
        Task<List<CanvasInsight>> GetCanvasInsightsAsync(string canvasId);
        
        /// <summary>
        /// Validates canvas completeness and provides feedback
        /// </summary>
        Task<CanvasValidation> ValidateCanvasAsync(string canvasId);
        
        /// <summary>
        /// Exports canvas to different formats
        /// </summary>
        Task<CanvasExport> ExportCanvasAsync(string canvasId, ExportFormat format);
        
        /// <summary>
        /// Creates a collaborative session for team canvas editing
        /// </summary>
        Task<CollaborationSession> CreateCollaborationSessionAsync(string canvasId);
        
        /// <summary>
        /// Gets canvas version history
        /// </summary>
        Task<List<CanvasVersion>> GetCanvasVersionsAsync(string canvasId);
    }
    
    /// <summary>
    /// Request for creating a new business model canvas
    /// </summary>
    public class CanvasCreationRequest
    {
        public string BusinessPlanId { get; set; } = string.Empty;
        public string CanvasName { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public bool UseAIAssistance { get; set; } = true;
    }
    
    /// <summary>
    /// Canvas template for different business models
    /// </summary>
    public class CanvasTemplate
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public BusinessModelCanvas TemplateCanvas { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string PreviewImage { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Strategic insight for canvas improvement
    /// </summary>
    public class CanvasInsight
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Strength, Weakness, Opportunity, Threat, Recommendation
        public int Priority { get; set; }
        public string Impact { get; set; } = string.Empty;
        public string ActionItem { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Canvas validation result
    /// </summary>
    public class CanvasValidation
    {
        public string CanvasId { get; set; } = string.Empty;
        public int CompletenessScore { get; set; }
        public bool IsValid { get; set; }
        public List<ValidationIssue> Issues { get; set; } = new();
        public List<string> Strengths { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Canvas validation issue
    /// </summary>
    public class ValidationIssue
    {
        public string Section { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // Critical, Warning, Info
        public string Message { get; set; } = string.Empty;
        public string Suggestion { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Canvas export result
    /// </summary>
    public class CanvasExport
    {
        public string Id { get; set; } = string.Empty;
        public string CanvasId { get; set; } = string.Empty;
        public ExportFormat Format { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Collaboration session for team editing
    /// </summary>
    public class CollaborationSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CanvasId { get; set; } = string.Empty;
        public string SessionName { get; set; } = string.Empty;
        public List<Collaborator> Participants { get; set; } = new();
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } = true;
        public string ShareLink { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Canvas collaborator
    /// </summary>
    public class Collaborator
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Owner, Editor, Viewer
        public string Color { get; set; } = string.Empty; // For cursor/highlight colors
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsOnline { get; set; }
    }
    
    /// <summary>
    /// Canvas version for history tracking
    /// </summary>
    public class CanvasVersion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CanvasId { get; set; } = string.Empty;
        public int VersionNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public BusinessModelCanvas CanvasSnapshot { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<string> Changes { get; set; } = new();
    }
}