using Microsoft.AspNetCore.Mvc;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfGenerationService _pdfService;

    public PdfController(IPdfGenerationService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost("business-analysis")]
    public IActionResult GenerateBusinessAnalysisReport([FromBody] BusinessAnalysisRequest request)
    {
        try
        {
            var pdfBytes = _pdfService.GenerateBusinessAnalysisReport(request.Scenario, request.HealthScore);
            
            return File(pdfBytes, "application/pdf", 
                $"Business_Analysis_{request.Scenario.Title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating PDF: {ex.Message}");
        }
    }

    [HttpPost("swot-analysis")]
    public IActionResult GenerateSwotAnalysisReport([FromBody] SwotAnalysisRequest request)
    {
        try
        {
            var pdfBytes = _pdfService.GenerateSwotAnalysisReport(request.Scenario, request.SwotAnalysis);
            
            return File(pdfBytes, "application/pdf", 
                $"SWOT_Analysis_{request.Scenario.Title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating PDF: {ex.Message}");
        }
    }

    [HttpPost("market-research")]
    public IActionResult GenerateMarketResearchReport([FromBody] MarketResearchRequest request)
    {
        try
        {
            var pdfBytes = _pdfService.GenerateMarketResearchReport(request.Scenario, request.MarketData);
            
            return File(pdfBytes, "application/pdf", 
                $"Market_Research_{request.Scenario.Title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating PDF: {ex.Message}");
        }
    }

    [HttpPost("executive-summary")]
    public IActionResult GenerateExecutiveSummary([FromBody] ExecutiveSummaryRequest request)
    {
        try
        {
            var pdfBytes = _pdfService.GenerateExecutiveSummary(request.Scenario, request.HealthScore);
            
            return File(pdfBytes, "application/pdf", 
                $"Executive_Summary_{request.Scenario.Title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating PDF: {ex.Message}");
        }
    }
}

// Request DTOs
public record BusinessAnalysisRequest(BusinessIdeaScenario Scenario, int HealthScore);
public record SwotAnalysisRequest(BusinessIdeaScenario Scenario, SwotAnalysis SwotAnalysis);
public record MarketResearchRequest(BusinessIdeaScenario Scenario, MarketResearchData MarketData);
public record ExecutiveSummaryRequest(BusinessIdeaScenario Scenario, int HealthScore);