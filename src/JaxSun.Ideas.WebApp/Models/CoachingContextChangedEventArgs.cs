namespace JaxSun.Ideas.WebApp.Models
{
    /// <summary>
    /// Event arguments for coaching context changes
    /// Provides context information when the coaching persona or strategy changes
    /// </summary>
    public class CoachingContextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The previous coaching context before the change
        /// </summary>
        public CoachingContext? PreviousContext { get; set; }
        
        /// <summary>
        /// The new coaching context after the change
        /// </summary>
        public CoachingContext NewContext { get; set; } = new();
        
        /// <summary>
        /// The reason for the context change
        /// </summary>
        public string ChangeReason { get; set; } = string.Empty;
        
        /// <summary>
        /// Timestamp when the context change occurred
        /// </summary>
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// User ID associated with the context change
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional metadata about the context change
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
    
}