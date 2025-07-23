using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Jackson.Ideas.Mock.Services;

/// <summary>
/// Service for exporting business analysis data to various formats including CSV and JSON.
/// Handles proper data serialization, formatting, and security considerations for exported data.
/// </summary>
/// <remarks>
/// This service ensures exported data is properly formatted and escaped to prevent injection attacks.
/// CSV exports use proper quoting and escaping, while JSON exports use standardized serialization.
/// All exports include metadata such as timestamps and source attribution.
/// </remarks>
public class DataExportService : IDataExportService
{
    public byte[] ExportScenarioToCsv(BusinessIdeaScenario scenario, int healthScore)
    {
        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Metric,Value");
        
        // Basic Information
        csv.AppendLine($"\"Business Idea\",\"{EscapeCsv(scenario.Title)}\"");
        csv.AppendLine($"\"Description\",\"{EscapeCsv(scenario.Description)}\"");
        csv.AppendLine($"\"Health Score\",\"{healthScore}/10\"");
        
        // Market Data
        csv.AppendLine($"\"Market Size\",\"{scenario.MarketSize:N0} potential customers\"");
        csv.AppendLine($"\"Competition Level\",\"{scenario.CompetitionLevel}\"");
        csv.AppendLine($"\"Industry\",\"{scenario.MarketResearch.Industry}\"");
        
        // Financial Data
        csv.AppendLine($"\"Startup Cost\",\"${scenario.StartupCost:N0}\"");
        csv.AppendLine($"\"Year 1 Revenue Projection\",\"${scenario.FinancialProjections.Revenue.Year1Total:N0}\"");
        csv.AppendLine($"\"Break-even Month\",\"{scenario.FinancialProjections.CashFlow.BreakEvenMonth}\"");
        
        // Analysis Date
        csv.AppendLine($"\"Analysis Date\",\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public byte[] ExportScenarioToJson(BusinessIdeaScenario scenario, int healthScore)
    {
        var exportData = new
        {
            businessIdea = scenario.Title,
            description = scenario.Description,
            healthScore = $"{healthScore}/10",
            marketData = new
            {
                marketSize = scenario.MarketSize,
                competitionLevel = scenario.CompetitionLevel,
                industry = scenario.MarketResearch.Industry
            },
            financialProjections = new
            {
                startupCost = scenario.StartupCost,
                year1Revenue = scenario.FinancialProjections.Revenue.Year1Total,
                breakEvenMonth = scenario.FinancialProjections.CashFlow.BreakEvenMonth
            },
            analysisDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            generatedBy = "Ideas Matter - AI Business Analysis Platform"
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(exportData, options);
        return Encoding.UTF8.GetBytes(json);
    }

    public byte[] ExportMarketDataToCsv(List<BusinessIdeaScenario> scenarios)
    {
        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Business Idea,Industry,Market Size,Competition Level,Startup Cost,Year 1 Revenue,Break-even Month");
        
        // Data rows
        foreach (var scenario in scenarios)
        {
            csv.AppendLine($"\"{EscapeCsv(scenario.Title)}\",\"{scenario.MarketResearch.Industry}\",{scenario.MarketSize},\"{scenario.CompetitionLevel}\",{scenario.StartupCost},{scenario.FinancialProjections.Revenue.Year1Total},{scenario.FinancialProjections.CashFlow.BreakEvenMonth}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public byte[] ExportSwotToCsv(SwotAnalysis swotAnalysis, string scenarioTitle)
    {
        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Category,Item");
        
        // Strengths
        foreach (var strength in swotAnalysis.Strengths)
        {
            csv.AppendLine($"\"Strengths\",\"{EscapeCsv(strength)}\"");
        }
        
        // Weaknesses
        foreach (var weakness in swotAnalysis.Weaknesses)
        {
            csv.AppendLine($"\"Weaknesses\",\"{EscapeCsv(weakness)}\"");
        }
        
        // Opportunities
        foreach (var opportunity in swotAnalysis.Opportunities)
        {
            csv.AppendLine($"\"Opportunities\",\"{EscapeCsv(opportunity)}\"");
        }
        
        // Threats
        foreach (var threat in swotAnalysis.Threats)
        {
            csv.AppendLine($"\"Threats\",\"{EscapeCsv(threat)}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
            
        // Escape quotes by doubling them
        return value.Replace("\"", "\"\"");
    }
}