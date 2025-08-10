using JaxSun.Ideas.WebApp.Models;

namespace JaxSun.Ideas.WebApp.Services.Interfaces;

public interface IMockDataService
{
    Task<List<BusinessIdeaScenario>> GetAllScenariosAsync();
    Task<BusinessIdeaScenario?> GetScenarioByIdAsync(string id);
    Task<List<BusinessIdeaScenario>> GetScenariosByIndustryAsync(string industry);
}