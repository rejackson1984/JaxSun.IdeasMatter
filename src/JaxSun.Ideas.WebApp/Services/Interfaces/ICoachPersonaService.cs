using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.Services.Interfaces
{
    /// <summary>
    /// Service for managing AI coach personas and coaching interactions
    /// </summary>
    public interface ICoachPersonaService
    {
        /// <summary>
        /// Gets the coach persona for a specific hub
        /// </summary>
        Task<CoachPersona> GetCoachForHubAsync(BusinessHub hub);
        
        /// <summary>
        /// Gets all available coach personas
        /// </summary>
        Task<Dictionary<BusinessHub, CoachPersona>> GetAllCoachPersonasAsync();
        
        /// <summary>
        /// Generates a contextual coaching message based on current state
        /// </summary>
        Task<string> GetCoachMessageAsync(BusinessHub hub, CoachingContext context);
        
        /// <summary>
        /// Gets coaching suggestions for the current context
        /// </summary>
        Task<List<CoachingSuggestion>> GetCoachingSuggestionsAsync(BusinessHub hub, CoachingContext context);
        
        /// <summary>
        /// Starts a new coaching session
        /// </summary>
        Task<CoachingSession> StartCoachingSessionAsync(string userId, BusinessHub hub);
        
        /// <summary>
        /// Ends an active coaching session
        /// </summary>
        Task EndCoachingSessionAsync(string sessionId);
        
        /// <summary>
        /// Adds a message to the current coaching session
        /// </summary>
        Task AddCoachingMessageAsync(string sessionId, CoachingMessage message);
        
        /// <summary>
        /// Gets the active coaching session for a user
        /// </summary>
        Task<CoachingSession?> GetActiveSessionAsync(string userId);
        
        /// <summary>
        /// Generates persona-specific greeting message
        /// </summary>
        Task<string> GetGreetingMessageAsync(BusinessHub hub, bool isReturningUser = false);
        
        /// <summary>
        /// Generates encouragement message based on progress
        /// </summary>
        Task<string> GetEncouragementMessageAsync(BusinessHub hub, int progressPercentage);
        
        /// <summary>
        /// Gets next step guidance for the current context
        /// </summary>
        Task<CoachingSuggestion> GetNextStepGuidanceAsync(BusinessHub hub, CoachingContext context);
        
        /// <summary>
        /// Celebrates milestone completion with persona-appropriate message
        /// </summary>
        Task<string> GetMilestoneCelebrationAsync(BusinessHub hub, string milestoneId);
        
        /// <summary>
        /// Event triggered when coaching context changes
        /// </summary>
        event EventHandler<CoachingContextChangedEventArgs>? CoachingContextChanged;
    }
    
    /// <summary>
    /// Event arguments for coaching context changes
    /// </summary>
    public class CoachingContextChangedEventArgs : EventArgs
    {
        public BusinessHub Hub { get; set; }
        public CoachPersona Persona { get; set; } = new();
        public CoachingContext Context { get; set; } = new();
        public CoachingMessage? NewMessage { get; set; }
    }
}