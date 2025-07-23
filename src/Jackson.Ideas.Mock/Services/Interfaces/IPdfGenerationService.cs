using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces;

public interface IPdfGenerationService
{
    /// <summary>
    /// Generates a comprehensive business analysis PDF report
    /// </summary>
    byte[] GenerateBusinessAnalysisReport(BusinessIdeaScenario scenario, int healthScore);
    
    /// <summary>
    /// Generates a SWOT analysis PDF report
    /// </summary>
    byte[] GenerateSwotAnalysisReport(BusinessIdeaScenario scenario, SwotAnalysis swotAnalysis);
    
    /// <summary>
    /// Generates a market research PDF report
    /// </summary>
    byte[] GenerateMarketResearchReport(BusinessIdeaScenario scenario, MarketResearchData marketData);
    
    /// <summary>
    /// Generates an executive summary PDF
    /// </summary>
    byte[] GenerateExecutiveSummary(BusinessIdeaScenario scenario, int healthScore);
}

public record SwotAnalysis(
    List<string> Strengths,
    List<string> Weaknesses,
    List<string> Opportunities,
    List<string> Threats
);