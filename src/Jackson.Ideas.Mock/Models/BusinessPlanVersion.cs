namespace Jackson.Ideas.Mock.Models;

public class BusinessPlanVersion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PlanId { get; set; } = "";
    public int VersionNumber { get; set; }
    public string VersionName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "";
    public BusinessPlanModifications Modifications { get; set; } = new();
    public BusinessPlanData PlanData { get; set; } = new();
    public AnalysisStatus Status { get; set; } = AnalysisStatus.Draft;
    public List<string> ChangesSummary { get; set; } = new();
    public string? ParentVersionId { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime? LastAnalyzedAt { get; set; }
}

public class BusinessPlanData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BusinessName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Industry { get; set; } = "";
    public BusinessTimeline Timeline { get; set; } = new();
    public BusinessResources Resources { get; set; } = new();
    public MarketStrategy MarketStrategy { get; set; } = new();
    public FinancialPlan FinancialPlan { get; set; } = new();
    public OperationalPlan OperationalPlan { get; set; } = new();
    public RiskAssessment RiskAssessment { get; set; } = new();
    public MarketResearchData MarketResearch { get; set; } = new();
    public FinancialProjections FinancialProjections { get; set; } = new();
}

public class BusinessPlanModifications
{
    public TimelineModifications Timeline { get; set; } = new();
    public ResourceModifications Resources { get; set; } = new();
    public MarketModifications Market { get; set; } = new();
    public FinancialModifications Financial { get; set; } = new();
    public OperationalModifications Operations { get; set; } = new();
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}

public enum AnalysisStatus
{
    Draft,
    Analyzing,
    Complete,
    Failed,
    Archived
}

public enum ModificationScope
{
    Minor,      // Small adjustments, quick re-analysis
    Moderate,   // Significant changes, partial re-analysis
    Major       // Fundamental changes, full re-analysis
}

public class ModificationSummary
{
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public ModificationScope Impact { get; set; }
    public List<string> AffectedAnalyses { get; set; } = new();
}