namespace JaxSun.Ideas.WebApp.Models;

public class BusinessIdeaScenario
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Industry { get; set; } = "";
    public string TargetMarket { get; set; } = "";
    public decimal EstimatedStartupCost { get; set; }
    public decimal StartupCost { get; set; }
    public decimal ProjectedRevenue { get; set; }
    public int ViabilityScore { get; set; }
    public int MarketSize { get; set; }
    public string CompetitionLevel { get; set; } = "";
    public MarketResearchData MarketResearch { get; set; } = new();
    public FinancialProjections FinancialProjections { get; set; } = new();
    public List<string> KeyChallenges { get; set; } = new();
    public List<string> SuccessFactors { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Phase tracking properties
    public IdeaPhase CurrentPhase { get; set; } = IdeaPhase.Concept;
    public bool IsBusinessPlanGenerated { get; set; } = false;
    public bool IsPRDGenerated { get; set; } = false;
    public bool IsMockupCreated { get; set; } = false;
    public bool IsActionPlanGenerated { get; set; } = false;
    public DateTime? LastActivityDate { get; set; }
    public string NextSuggestedAction { get; set; } = "Generate Business Plan";
}

public enum IdeaPhase
{
    Concept = 1,
    BusinessPlan = 2,
    ProductDesign = 3,
    Mockup = 4,
    ActionPlan = 5,
    Implementation = 6
}