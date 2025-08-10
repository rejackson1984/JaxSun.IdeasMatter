using Microsoft.Extensions.Diagnostics.HealthChecks;
using JaxSun.Ideas.Mock.Services.Interfaces;
using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.HealthChecks;

public class PdfServiceHealthCheck : IHealthCheck
{
    private readonly IPdfGenerationService _pdfService;
    private readonly ILogger<PdfServiceHealthCheck> _logger;

    public PdfServiceHealthCheck(
        IPdfGenerationService pdfService,
        ILogger<PdfServiceHealthCheck> logger)
    {
        _pdfService = pdfService;
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
                Description = "Test scenario for PDF service health check",
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

            // Test PDF generation with minimal content
            var testPdf = _pdfService.GenerateExecutiveSummary(testScenario, 1);
            
            if (testPdf?.Length > 0)
            {
                _logger.LogDebug("PDF service health check passed. Generated PDF size: {Size} bytes", testPdf.Length);
                return HealthCheckResult.Healthy("PDF service is operational", 
                    new Dictionary<string, object>
                    {
                        {"test_pdf_size", testPdf.Length},
                        {"last_check", DateTime.UtcNow}
                    });
            }
            else
            {
                _logger.LogWarning("PDF service health check failed: Generated PDF is null or empty");
                return HealthCheckResult.Degraded("PDF service generated empty or null content");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF service health check failed with exception");
            return HealthCheckResult.Unhealthy("PDF service is not working properly", ex);
        }
    }
}