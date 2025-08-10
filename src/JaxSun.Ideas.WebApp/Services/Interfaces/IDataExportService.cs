using JaxSun.Ideas.Mock.Models;
using System.Text.Json;

namespace JaxSun.Ideas.Mock.Services.Interfaces;

public interface IDataExportService
{
    /// <summary>
    /// Exports business scenario data to CSV format
    /// </summary>
    byte[] ExportScenarioToCsv(BusinessIdeaScenario scenario, int healthScore);
    
    /// <summary>
    /// Exports business scenario data to JSON format
    /// </summary>
    byte[] ExportScenarioToJson(BusinessIdeaScenario scenario, int healthScore);
    
    /// <summary>
    /// Exports market research data to CSV format
    /// </summary>
    byte[] ExportMarketDataToCsv(List<BusinessIdeaScenario> scenarios);
    
    /// <summary>
    /// Exports SWOT analysis to CSV format
    /// </summary>
    byte[] ExportSwotToCsv(SwotAnalysis swotAnalysis, string scenarioTitle);
}