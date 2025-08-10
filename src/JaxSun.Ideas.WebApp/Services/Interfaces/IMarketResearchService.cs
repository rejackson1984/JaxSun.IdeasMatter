using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.Services.Interfaces;

public interface IMarketResearchService
{
    Task<MarketResearchData?> GetMarketResearchAsync(string scenarioId);
    Task<List<MarketResearchData>> GetAllMarketResearchAsync();
    Task<MarketResearchData?> GetMarketResearchByIndustryAsync(string industry);
    Task<List<Models.Competitor>> GetCompetitorsAsync(string industry);
    Task<MarketTrends> GetMarketTrendsAsync(string industry);
}