using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces;

public interface IBusinessPlanVersionService
{
    // Version Management
    Task<BusinessPlanVersion> CreateVersionAsync(string planId, string versionName, string description = "");
    Task<BusinessPlanVersion> CloneVersionAsync(string sourceVersionId, string newVersionName);
    Task<BusinessPlanVersion> GetVersionAsync(string versionId);
    Task<List<BusinessPlanVersion>> GetVersionsAsync(string planId);
    Task<BusinessPlanVersion?> GetActiveVersionAsync(string planId);
    Task<bool> SetActiveVersionAsync(string versionId);
    Task<bool> ArchiveVersionAsync(string versionId);
    
    // Modification Management
    Task<BusinessPlanVersion> ApplyModificationsAsync(string versionId, BusinessPlanModifications modifications);
    Task<ModificationSummary> AnalyzeModificationImpactAsync(BusinessPlanModifications currentPlan, BusinessPlanModifications newModifications);
    Task<List<string>> GetModificationSuggestionsAsync(string versionId, ModificationScope scope);
    
    // Version Comparison
    Task<VersionComparison> CompareVersionsAsync(string version1Id, string version2Id);
    Task<List<VersionDifference>> GetVersionDifferencesAsync(string version1Id, string version2Id);
    
    // Template Management
    Task<List<ModificationTemplate>> GetModificationTemplatesAsync();
    Task<BusinessPlanModifications> ApplyTemplateAsync(string templateId, BusinessPlanData currentPlan);
    
    // Analysis Integration
    Task<bool> TriggerReAnalysisAsync(string versionId);
    Task<AnalysisProgress> GetAnalysisProgressAsync(string versionId);
    Task<bool> IsAnalysisRequiredAsync(string versionId);
}

public class VersionComparison
{
    public BusinessPlanVersion Version1 { get; set; } = new();
    public BusinessPlanVersion Version2 { get; set; } = new();
    public List<ComparisonMetric> Metrics { get; set; } = new();
    public List<VersionDifference> Differences { get; set; } = new();
    public ComparisonSummary Summary { get; set; } = new();
}

public class VersionDifference
{
    public string Category { get; set; } = "";
    public string Field { get; set; } = "";
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
    public DifferenceType Type { get; set; }
    public string Description { get; set; } = "";
    public ModificationScope Impact { get; set; }
}

public class ComparisonMetric
{
    public string Name { get; set; } = "";
    public decimal Version1Value { get; set; }
    public decimal Version2Value { get; set; }
    public decimal PercentageChange { get; set; }
    public TrendDirection Trend { get; set; }
    public string Unit { get; set; } = "";
}

public class ComparisonSummary
{
    public string Title { get; set; } = "";
    public List<string> KeyChanges { get; set; } = new();
    public List<string> ImpactAreas { get; set; } = new();
    public ModificationScope OverallScope { get; set; }
    public string Recommendation { get; set; } = "";
}

public class ModificationTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public BusinessPlanModifications Modifications { get; set; } = new();
    public List<string> ApplicableIndustries { get; set; } = new();
    public string Icon { get; set; } = "";
    public bool IsPopular { get; set; } = false;
}

public class AnalysisProgress
{
    public string VersionId { get; set; } = "";
    public AnalysisStatus Status { get; set; }
    public int PercentageComplete { get; set; }
    public List<AnalysisComponent> Components { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public List<string> CurrentlyProcessing { get; set; } = new();
}

public class AnalysisComponent
{
    public string Name { get; set; } = "";
    public AnalysisStatus Status { get; set; }
    public int PercentageComplete { get; set; }
    public string Description { get; set; } = "";
    public bool IsRequired { get; set; } = true;
}

public enum DifferenceType
{
    Added,
    Removed,
    Modified,
    Unchanged
}

public enum TrendDirection
{
    Up,
    Down,
    Stable,
    Unknown
}