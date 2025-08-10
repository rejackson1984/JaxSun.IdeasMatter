using JaxSun.Ideas.WebApp.Models;

namespace JaxSun.Ideas.WebApp.Models
{
    /// <summary>
    /// Defines the coaching style for different personas
    /// </summary>
    public enum CoachingStyle
    {
        Enthusiastic,      // Spark - energetic and encouraging
        Analytical,        // Strategy - methodical and educational  
        ResultsOriented    // Execute - strategic and data-driven
    }

    /// <summary>
    /// Represents the context for coaching interactions
    /// </summary>
    public class CoachingContext
    {
        public BusinessHub CurrentHub { get; set; }
        public string? CurrentPage { get; set; }
        public int UserProgress { get; set; }
        public List<string> CompletedMilestones { get; set; } = new();
        public string? LastUserAction { get; set; }
        public DateTime SessionStartTime { get; set; }
        public bool IsFirstTimeInHub { get; set; }
        public string? UserIntent { get; set; }
    }

    /// <summary>
    /// Represents a coaching suggestion or tip
    /// </summary>
    public class CoachingSuggestion
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public CoachingSuggestionType Type { get; set; }
        public int Priority { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Types of coaching suggestions
    /// </summary>
    public enum CoachingSuggestionType
    {
        NextStep,          // Guidance on what to do next
        Encouragement,     // Motivational messages
        Tips,              // Helpful tips and tricks
        Warning,           // Important notices or warnings
        Achievement,       // Celebration of milestones
        Educational        // Learning opportunities
    }

    /// <summary>
    /// Represents an AI coach persona with specific characteristics
    /// </summary>
    public class CoachPersona
    {
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Personality { get; set; } = string.Empty;
        public string VoicePattern { get; set; } = string.Empty;
        public BusinessHub Hub { get; set; }
        public CoachingStyle Style { get; set; }
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Specialties { get; set; } = new();
        public List<string> CommonPhrases { get; set; } = new();
        public string Greeting { get; set; } = string.Empty;
        public string Farewell { get; set; } = string.Empty;
        public Dictionary<string, string> ContextualMessages { get; set; } = new();
    }

    /// <summary>
    /// Represents a coaching message with timing and context
    /// </summary>
    public class CoachingMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PersonaName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public CoachingMessageType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public string? RelatedAction { get; set; }
        public string? Icon { get; set; }
        public int Priority { get; set; }
    }

    /// <summary>
    /// Types of coaching messages
    /// </summary>
    public enum CoachingMessageType
    {
        Welcome,
        Progress,
        Encouragement,
        Guidance,
        Achievement,
        Reminder,
        Tip
    }

    /// <summary>
    /// Tracks coaching session data
    /// </summary>
    public class CoachingSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public BusinessHub Hub { get; set; }
        public string PersonaName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public List<CoachingMessage> Messages { get; set; } = new();
        public List<CoachingSuggestion> Suggestions { get; set; } = new();
        public int InteractionCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}