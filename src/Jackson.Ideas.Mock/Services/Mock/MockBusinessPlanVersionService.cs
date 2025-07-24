using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock;

public class MockBusinessPlanVersionService : IBusinessPlanVersionService
{
    private readonly List<BusinessPlanVersion> _versions = new();
    private readonly List<ModificationTemplate> _templates = new();

    public MockBusinessPlanVersionService()
    {
        InitializeTemplates();
        InitializeSampleVersions();
    }

    public async Task<BusinessPlanVersion> CreateVersionAsync(string planId, string versionName, string description = "")
    {
        await Task.Delay(100); // Simulate async operation

        var version = new BusinessPlanVersion
        {
            PlanId = planId,
            VersionNumber = _versions.Count(v => v.PlanId == planId) + 1,
            VersionName = versionName,
            Description = description,
            Status = AnalysisStatus.Draft,
            IsActive = !_versions.Any(v => v.PlanId == planId) // First version is active
        };

        _versions.Add(version);
        return version;
    }

    public async Task<BusinessPlanVersion> CloneVersionAsync(string sourceVersionId, string newVersionName)
    {
        await Task.Delay(100);

        var sourceVersion = _versions.FirstOrDefault(v => v.Id == sourceVersionId);
        if (sourceVersion == null)
            throw new ArgumentException("Source version not found");

        var clonedVersion = new BusinessPlanVersion
        {
            PlanId = sourceVersion.PlanId,
            VersionNumber = _versions.Count(v => v.PlanId == sourceVersion.PlanId) + 1,
            VersionName = newVersionName,
            Description = $"Cloned from {sourceVersion.VersionName}",
            PlanData = ClonePlanData(sourceVersion.PlanData),
            Modifications = CloneModifications(sourceVersion.Modifications),
            ParentVersionId = sourceVersionId,
            Status = AnalysisStatus.Draft
        };

        _versions.Add(clonedVersion);
        return clonedVersion;
    }

    public async Task<BusinessPlanVersion> GetVersionAsync(string versionId)
    {
        await Task.Delay(50);
        return _versions.FirstOrDefault(v => v.Id == versionId) 
               ?? throw new ArgumentException("Version not found");
    }

    public async Task<List<BusinessPlanVersion>> GetVersionsAsync(string planId)
    {
        await Task.Delay(50);
        return _versions.Where(v => v.PlanId == planId)
                       .OrderByDescending(v => v.CreatedAt)
                       .ToList();
    }

    public async Task<BusinessPlanVersion?> GetActiveVersionAsync(string planId)
    {
        await Task.Delay(50);
        return _versions.FirstOrDefault(v => v.PlanId == planId && v.IsActive);
    }

    public async Task<bool> SetActiveVersionAsync(string versionId)
    {
        await Task.Delay(50);

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null) return false;

        // Deactivate other versions in the same plan
        foreach (var v in _versions.Where(v => v.PlanId == version.PlanId))
        {
            v.IsActive = false;
        }

        version.IsActive = true;
        return true;
    }

    public async Task<bool> ArchiveVersionAsync(string versionId)
    {
        await Task.Delay(50);

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null) return false;

        version.Status = AnalysisStatus.Archived;
        version.IsActive = false;
        return true;
    }

    public async Task<BusinessPlanVersion> ApplyModificationsAsync(string versionId, BusinessPlanModifications modifications)
    {
        await Task.Delay(200);

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null)
            throw new ArgumentException("Version not found");

        // Apply modifications
        version.Modifications = modifications;
        version.Status = AnalysisStatus.Draft; // Reset to draft status
        version.LastAnalyzedAt = null;

        // Generate change summary
        version.ChangesSummary = GenerateChangesSummary(modifications);

        // Update plan data based on modifications
        ApplyModificationsToPlanData(version.PlanData, modifications);

        return version;
    }

    public async Task<ModificationSummary> AnalyzeModificationImpactAsync(BusinessPlanModifications currentPlan, BusinessPlanModifications newModifications)
    {
        await Task.Delay(150);

        var impact = new ModificationSummary
        {
            Category = "Timeline & Resources",
            Impact = DetermineModificationScope(currentPlan, newModifications),
            AffectedAnalyses = DetermineAffectedAnalyses(newModifications)
        };

        impact.Description = GenerateImpactDescription(impact.Impact, impact.AffectedAnalyses);
        return impact;
    }

    public async Task<List<string>> GetModificationSuggestionsAsync(string versionId, ModificationScope scope)
    {
        await Task.Delay(100);

        var suggestions = new List<string>();

        switch (scope)
        {
            case ModificationScope.Minor:
                suggestions.AddRange(new[]
                {
                    "Consider adjusting timeline buffer from standard to conservative",
                    "Explore part-time schedule to reduce initial stress",
                    "Add weekend work option for flexibility"
                });
                break;
            case ModificationScope.Moderate:
                suggestions.AddRange(new[]
                {
                    "Switch to bootstrap funding to maintain control",
                    "Consider contractor-based team building for flexibility",
                    "Adjust revenue targets to match new timeline"
                });
                break;
            case ModificationScope.Major:
                suggestions.AddRange(new[]
                {
                    "Complete market strategy overhaul may be needed",
                    "Consider pivoting to different customer segment",
                    "Reassess competitive positioning strategy"
                });
                break;
        }

        return suggestions;
    }

    public async Task<VersionComparison> CompareVersionsAsync(string version1Id, string version2Id)
    {
        await Task.Delay(200);

        var version1 = await GetVersionAsync(version1Id);
        var version2 = await GetVersionAsync(version2Id);

        var comparison = new VersionComparison
        {
            Version1 = version1,
            Version2 = version2,
            Differences = await GetVersionDifferencesAsync(version1Id, version2Id),
            Metrics = GenerateComparisonMetrics(version1, version2),
            Summary = GenerateComparisonSummary(version1, version2)
        };

        return comparison;
    }

    public async Task<List<VersionDifference>> GetVersionDifferencesAsync(string version1Id, string version2Id)
    {
        await Task.Delay(100);

        var version1 = await GetVersionAsync(version1Id);
        var version2 = await GetVersionAsync(version2Id);

        var differences = new List<VersionDifference>();

        // Compare timeline modifications
        if (version1.Modifications.Timeline.Schedule != version2.Modifications.Timeline.Schedule)
        {
            differences.Add(new VersionDifference
            {
                Category = "Timeline",
                Field = "Work Schedule",
                OldValue = version1.Modifications.Timeline.Schedule.ToString(),
                NewValue = version2.Modifications.Timeline.Schedule.ToString(),
                Type = DifferenceType.Modified,
                Description = $"Work schedule changed from {version1.Modifications.Timeline.Schedule} to {version2.Modifications.Timeline.Schedule}",
                Impact = ModificationScope.Moderate
            });
        }

        // Compare resource modifications
        if (version1.Modifications.Resources.FundingStrategy != version2.Modifications.Resources.FundingStrategy)
        {
            differences.Add(new VersionDifference
            {
                Category = "Resources",
                Field = "Funding Strategy",
                OldValue = version1.Modifications.Resources.FundingStrategy.ToString(),
                NewValue = version2.Modifications.Resources.FundingStrategy.ToString(),
                Type = DifferenceType.Modified,
                Description = $"Funding approach changed from {version1.Modifications.Resources.FundingStrategy} to {version2.Modifications.Resources.FundingStrategy}",
                Impact = ModificationScope.Major
            });
        }

        return differences;
    }

    public async Task<List<ModificationTemplate>> GetModificationTemplatesAsync()
    {
        await Task.Delay(50);
        return _templates.ToList();
    }

    public async Task<BusinessPlanModifications> ApplyTemplateAsync(string templateId, BusinessPlanData currentPlan)
    {
        await Task.Delay(100);

        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        if (template == null)
            throw new ArgumentException("Template not found");

        // Create a copy of the template modifications
        var modifications = CloneModifications(template.Modifications);

        // Customize based on current plan characteristics
        CustomizeTemplateForPlan(modifications, currentPlan);

        return modifications;
    }

    public async Task<bool> TriggerReAnalysisAsync(string versionId)
    {
        await Task.Delay(100);

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null) return false;

        version.Status = AnalysisStatus.Analyzing;
        version.LastAnalyzedAt = DateTime.UtcNow;

        // Simulate analysis completion after delay
        _ = Task.Run(async () =>
        {
            await Task.Delay(5000); // Simulate 5 second analysis
            version.Status = AnalysisStatus.Complete;
        });

        return true;
    }

    public async Task<AnalysisProgress> GetAnalysisProgressAsync(string versionId)
    {
        await Task.Delay(50);

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null)
            throw new ArgumentException("Version not found");

        return new AnalysisProgress
        {
            VersionId = versionId,
            Status = version.Status,
            PercentageComplete = version.Status == AnalysisStatus.Complete ? 100 : 
                               version.Status == AnalysisStatus.Analyzing ? 65 : 0,
            Components = GenerateAnalysisComponents(version.Status),
            StartedAt = version.LastAnalyzedAt ?? DateTime.UtcNow,
            EstimatedCompletion = version.LastAnalyzedAt?.AddMinutes(2),
            CurrentlyProcessing = version.Status == AnalysisStatus.Analyzing ? 
                new List<string> { "Financial Projections", "Market Analysis" } : new List<string>()
        };
    }

    public async Task<bool> IsAnalysisRequiredAsync(string versionId)
    {
        await Task.Delay(50);

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null) return false;

        return version.Status == AnalysisStatus.Draft || 
               (version.LastAnalyzedAt.HasValue && 
                version.Modifications.LastModified > version.LastAnalyzedAt.Value);
    }

    // Private helper methods
    private void InitializeTemplates()
    {
        _templates.AddRange(new[]
        {
            new ModificationTemplate
            {
                Name = "Part-Time Entrepreneur",
                Description = "Perfect for those keeping their day job while building their business",
                Category = "Timeline",
                Icon = "⏰",
                IsPopular = true,
                Modifications = new BusinessPlanModifications
                {
                    Timeline = new TimelineModifications
                    {
                        Schedule = WorkSchedule.PartTime,
                        HoursPerWeek = 25,
                        LaunchTimelineMonths = 12,
                        BufferStrategy = TimeBuffer.Conservative,
                        AllowEveningWork = true,
                        AllowWeekendWork = true
                    },
                    Resources = new ResourceModifications
                    {
                        FundingStrategy = FundingApproach.Bootstrap,
                        TeamStrategy = TeamBuildingStrategy.SoloFounder,
                        UseContractors = true
                    }
                }
            },
            new ModificationTemplate
            {
                Name = "Bootstrap Startup",
                Description = "Minimal external funding, maximum control and flexibility",
                Category = "Funding",
                Icon = "💰",
                IsPopular = true,
                Modifications = new BusinessPlanModifications
                {
                    Resources = new ResourceModifications
                    {
                        FundingStrategy = FundingApproach.Bootstrap,
                        TeamStrategy = TeamBuildingStrategy.ContractorBased,
                        UseContractors = true,
                        UseFreelancers = true
                    },
                    Financial = new FinancialModifications
                    {
                        GrowthApproach = GrowthStrategy.Organic,
                        ProfitGoal = ProfitabilityGoal.BreakEven,
                        ReinvestProfits = true
                    }
                }
            },
            new ModificationTemplate
            {
                Name = "Conservative Growth",
                Description = "Steady, sustainable growth with lower risk tolerance",
                Category = "Strategy",
                Icon = "📈",
                Modifications = new BusinessPlanModifications
                {
                    Timeline = new TimelineModifications
                    {
                        BufferStrategy = TimeBuffer.Conservative,
                        LaunchTimelineMonths = 18
                    },
                    Financial = new FinancialModifications
                    {
                        GrowthApproach = GrowthStrategy.Organic,
                        ProfitGoal = ProfitabilityGoal.Moderate
                    },
                    Market = new MarketModifications
                    {
                        MarketingStrategy = GoToMarketStrategy.LocalFirst,
                        TargetScope = MarketScope.Local
                    }
                }
            },
            new ModificationTemplate
            {
                Name = "Digital-First Business",
                Description = "Online-focused with minimal physical presence",
                Category = "Market",
                Icon = "💻",
                Modifications = new BusinessPlanModifications
                {
                    Market = new MarketModifications
                    {
                        MarketingStrategy = GoToMarketStrategy.DigitalFirst,
                        TargetScope = MarketScope.National
                    },
                    Operations = new OperationalModifications
                    {
                        LocationStrategy = WorkLocation.Remote,
                        OperatingModel = BusinessModel.Platform
                    }
                }
            }
        });
    }

    private void InitializeSampleVersions()
    {
        // Sample versions will be created when scenarios are accessed
    }

    private BusinessPlanData ClonePlanData(BusinessPlanData source)
    {
        // Simple cloning - in production, use proper deep cloning
        return new BusinessPlanData
        {
            BusinessName = source.BusinessName,
            Description = source.Description,
            Industry = source.Industry,
            Timeline = source.Timeline,
            Resources = source.Resources,
            MarketStrategy = source.MarketStrategy,
            FinancialPlan = source.FinancialPlan,
            OperationalPlan = source.OperationalPlan,
            RiskAssessment = source.RiskAssessment
        };
    }

    private BusinessPlanModifications CloneModifications(BusinessPlanModifications source)
    {
        return new BusinessPlanModifications
        {
            Timeline = source.Timeline,
            Resources = source.Resources,
            Market = source.Market,
            Financial = source.Financial,
            Operations = source.Operations,
            LastModified = DateTime.UtcNow
        };
    }

    private List<string> GenerateChangesSummary(BusinessPlanModifications modifications)
    {
        var changes = new List<string>();

        if (modifications.Timeline.Schedule != WorkSchedule.FullTime)
            changes.Add($"Work schedule set to {modifications.Timeline.Schedule}");

        if (modifications.Resources.FundingStrategy != FundingApproach.Bootstrap)
            changes.Add($"Funding strategy changed to {modifications.Resources.FundingStrategy}");

        if (modifications.Timeline.LaunchTimelineMonths != 6)
            changes.Add($"Launch timeline adjusted to {modifications.Timeline.LaunchTimelineMonths} months");

        return changes;
    }

    private void ApplyModificationsToPlanData(BusinessPlanData planData, BusinessPlanModifications modifications)
    {
        // Update timeline
        if (modifications.Timeline.PreferredLaunchDate.HasValue)
        {
            planData.Timeline.TargetLaunchDate = modifications.Timeline.PreferredLaunchDate.Value;
        }

        planData.Timeline.TotalDurationMonths = modifications.Timeline.LaunchTimelineMonths;

        // Update resources
        planData.Resources.TotalBudget = modifications.Resources.InitialBudget;
        planData.Resources.TeamSize = modifications.Resources.MaxTeamSize;
    }

    private ModificationScope DetermineModificationScope(BusinessPlanModifications current, BusinessPlanModifications modified)
    {
        int changeCount = 0;

        if (current.Timeline.Schedule != modified.Timeline.Schedule) changeCount += 2;
        if (current.Resources.FundingStrategy != modified.Resources.FundingStrategy) changeCount += 3;
        if (current.Market.MarketingStrategy != modified.Market.MarketingStrategy) changeCount += 2;
        if (Math.Abs(current.Timeline.LaunchTimelineMonths - modified.Timeline.LaunchTimelineMonths) > 3) changeCount += 2;

        return changeCount switch
        {
            <= 2 => ModificationScope.Minor,
            <= 5 => ModificationScope.Moderate,
            _ => ModificationScope.Major
        };
    }

    private List<string> DetermineAffectedAnalyses(BusinessPlanModifications modifications)
    {
        var affected = new List<string>();

        if (modifications.Timeline.LaunchTimelineMonths != 6)
            affected.AddRange(new[] { "Financial Projections", "Milestone Planning" });

        if (modifications.Resources.FundingStrategy != FundingApproach.Bootstrap)
            affected.AddRange(new[] { "Market Sizing", "Competitive Analysis" });

        if (modifications.Market.MarketingStrategy != GoToMarketStrategy.DigitalFirst)
            affected.AddRange(new[] { "Customer Research", "Marketing Strategy" });

        return affected.Distinct().ToList();
    }

    private string GenerateImpactDescription(ModificationScope scope, List<string> affectedAnalyses)
    {
        var description = scope switch
        {
            ModificationScope.Minor => "Minor adjustments detected. Quick re-analysis recommended.",
            ModificationScope.Moderate => "Significant changes detected. Partial re-analysis required.",
            ModificationScope.Major => "Major changes detected. Complete re-analysis strongly recommended.",
            _ => "Changes detected."
        };

        if (affectedAnalyses.Any())
        {
            description += $" Affected areas: {string.Join(", ", affectedAnalyses)}.";
        }

        return description;
    }

    private List<ComparisonMetric> GenerateComparisonMetrics(BusinessPlanVersion version1, BusinessPlanVersion version2)
    {
        return new List<ComparisonMetric>
        {
            new()
            {
                Name = "Launch Timeline",
                Version1Value = version1.Modifications.Timeline.LaunchTimelineMonths,
                Version2Value = version2.Modifications.Timeline.LaunchTimelineMonths,
                Unit = "months",
                Trend = version2.Modifications.Timeline.LaunchTimelineMonths > version1.Modifications.Timeline.LaunchTimelineMonths 
                    ? TrendDirection.Up : TrendDirection.Down
            },
            new()
            {
                Name = "Hours Per Week",
                Version1Value = version1.Modifications.Timeline.HoursPerWeek,
                Version2Value = version2.Modifications.Timeline.HoursPerWeek,
                Unit = "hours",
                Trend = version2.Modifications.Timeline.HoursPerWeek > version1.Modifications.Timeline.HoursPerWeek 
                    ? TrendDirection.Up : TrendDirection.Down
            }
        };
    }

    private ComparisonSummary GenerateComparisonSummary(BusinessPlanVersion version1, BusinessPlanVersion version2)
    {
        return new ComparisonSummary
        {
            Title = $"{version1.VersionName} vs {version2.VersionName}",
            KeyChanges = new List<string>
            {
                "Timeline approach modified for better work-life balance",
                "Resource allocation adjusted for bootstrap funding",
                "Market strategy refined based on constraints"
            },
            ImpactAreas = new List<string> { "Timeline", "Resources", "Market Strategy" },
            OverallScope = ModificationScope.Moderate,
            Recommendation = "Version 2 shows a more sustainable approach for part-time entrepreneurs."
        };
    }

    private List<AnalysisComponent> GenerateAnalysisComponents(AnalysisStatus status)
    {
        var components = new List<AnalysisComponent>
        {
            new() { Name = "Market Research", Status = status == AnalysisStatus.Complete ? AnalysisStatus.Complete : status, PercentageComplete = status == AnalysisStatus.Complete ? 100 : 45 },
            new() { Name = "Financial Projections", Status = status == AnalysisStatus.Complete ? AnalysisStatus.Complete : status, PercentageComplete = status == AnalysisStatus.Complete ? 100 : 30 },
            new() { Name = "Competitive Analysis", Status = status == AnalysisStatus.Complete ? AnalysisStatus.Complete : AnalysisStatus.Draft, PercentageComplete = status == AnalysisStatus.Complete ? 100 : 10 },
            new() { Name = "Risk Assessment", Status = AnalysisStatus.Draft, PercentageComplete = 0 }
        };

        return components;
    }

    private void CustomizeTemplateForPlan(BusinessPlanModifications modifications, BusinessPlanData currentPlan)
    {
        // Customize template based on current plan characteristics
        // This is where AI could make intelligent adjustments
        
        if (currentPlan.Industry.Contains("Technology", StringComparison.OrdinalIgnoreCase))
        {
            modifications.Market.MarketingStrategy = GoToMarketStrategy.DigitalFirst;
            modifications.Operations.LocationStrategy = WorkLocation.Remote;
        }
        
        if (currentPlan.Industry.Contains("Retail", StringComparison.OrdinalIgnoreCase))
        {
            modifications.Market.TargetScope = MarketScope.Local;
            modifications.Operations.LocationStrategy = WorkLocation.Office;
        }
    }
}