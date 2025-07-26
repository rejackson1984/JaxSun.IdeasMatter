using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Interfaces;

public interface ILaunchedBusinessService
{
    Task<List<LaunchedBusiness>> GetAllLaunchedBusinessesAsync();
    Task<LaunchedBusiness?> GetLaunchedBusinessByIdAsync(string id);
    Task<Dictionary<string, EducationalTooltip>> GetEducationalTooltipsAsync();
    Task<List<BusinessTip>> GetPersonalizedTipsAsync(string businessId);
    Task<Dictionary<string, BenchmarkData>> GetBenchmarkDataAsync(string businessId);
    Task<List<Achievement>> GetRecentAchievementsAsync(string businessId);
}