namespace JaxSun.Ideas.Mock.Models
{
    /// <summary>
    /// Business model validation result containing viability assessment and recommendations
    /// </summary>
    public class BusinessModelValidation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public BusinessPlanRequest Request { get; set; } = new();
        public int ValidationScore { get; set; }
        public int ViabilityScore { get; set; } // Alternative alias for ValidationScore
        public bool IsViable { get; set; }
        public List<string> Strengths { get; set; } = new();
        public List<string> Weaknesses { get; set; } = new();
        public List<string> Opportunities { get; set; } = new();
        public List<string> Threats { get; set; } = new();
        public List<ValidationCriterion> ValidationCriteria { get; set; } = new();
        public Dictionary<string, decimal> KeyMetrics { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public string OverallAssessment { get; set; } = string.Empty;
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Individual validation criterion with score and details
    /// </summary>
    public class ValidationCriterion
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Score { get; set; }
        public int MaxScore { get; set; } = 100;
        public string Assessment { get; set; } = string.Empty;
        public List<string> ImprovementSuggestions { get; set; } = new();
        public string Category { get; set; } = string.Empty; // Market, Financial, Operational, etc.
    }
}