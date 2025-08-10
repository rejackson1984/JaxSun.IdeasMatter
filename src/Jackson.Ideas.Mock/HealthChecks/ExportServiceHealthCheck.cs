using Microsoft.Extensions.Diagnostics.HealthChecks;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.HealthChecks;

public class ExportServiceHealthCheck : IHealthCheck
{
    private readonly IDataExportService _exportService;
    private readonly ILogger<ExportServiceHealthCheck> _logger;

    public ExportServiceHealthCheck(
        IDataExportService exportService,
        ILogger<ExportServiceHealthCheck> logger)
    {
        _exportService = exportService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create a minimal test scenario
            var testScenario = new BusinessIdeaScenario
            {
                Title = "Health Check Test",
                Description = "Test scenario for export service health check",
                MarketSize = 1000,
                CompetitionLevel = "Low",
                StartupCost = 1000,
                MarketResearch = new MarketResearchData
                {
                    Industry = "Test"
                },
                FinancialProjections = new FinancialProjections
                {
                    Revenue = new RevenueProjections { Year1Total = 10000 },
                    CashFlow = new CashFlowProjections { BreakEvenMonth = 6 }
                }
            };

            // Test CSV export
            var testCsv = _exportService.ExportScenarioToCsv(testScenario, 1);
            
            if (testCsv?.Length > 0)
            {
                // Test JSON export
                var testJson = _exportService.ExportScenarioToJson(testScenario, 1);
                
                if (testJson?.Length > 0)
                {
                    _logger.LogDebug("Export service health check passed. CSV size: {CsvSize}, JSON size: {JsonSize}", 
                        testCsv.Length, testJson.Length);
                    
                    return HealthCheckResult.Healthy("Export service is operational",
                        new Dictionary<string, object>
                        {
                            {"test_csv_size", testCsv.Length},
                            {"test_json_size", testJson.Length},
                            {"last_check", DateTime.UtcNow}
                        });
                }
                else
                {
                    _logger.LogWarning("Export service health check failed: JSON export returned null or empty");
                    return HealthCheckResult.Degraded("Export service JSON export failed");
                }
            }
            else
            {
                _logger.LogWarning("Export service health check failed: CSV export returned null or empty");
                return HealthCheckResult.Degraded("Export service CSV export failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export service health check failed with exception");
            return HealthCheckResult.Unhealthy("Export service is not working properly", ex);
        }
    }
}