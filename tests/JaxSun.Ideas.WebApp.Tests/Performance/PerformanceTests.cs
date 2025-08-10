using FluentAssertions;
using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using System.Diagnostics;
using Xunit;

namespace JaxSun.Ideas.WebApp.Tests.Performance;

public class PerformanceTests
{
    private readonly IPdfGenerationService _pdfService;
    private readonly IDataExportService _exportService;

    public PerformanceTests()
    {
        _pdfService = new PdfGenerationService();
        _exportService = new DataExportService();
    }

    [Fact]
    public void PdfGeneration_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = _pdfService.GenerateBusinessAnalysisReport(scenario, 8);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // PDF generation should complete within 5 seconds for a single report
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, 
            $"PDF generation took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 5 second threshold");
    }

    [Fact]
    public void SwotPdfGeneration_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var swot = CreateLargeSwotAnalysis();
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = _pdfService.GenerateSwotAnalysisReport(scenario, swot);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // SWOT PDF generation should complete within 3 seconds
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000,
            $"SWOT PDF generation took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 3 second threshold");
    }

    [Fact]
    public void CsvExport_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = _exportService.ExportScenarioToCsv(scenario, 7);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // CSV export should be very fast (under 100ms)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100,
            $"CSV export took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 100ms threshold");
    }

    [Fact]
    public void JsonExport_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = _exportService.ExportScenarioToJson(scenario, 7);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // JSON export should be very fast (under 100ms)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100,
            $"JSON export took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 100ms threshold");
    }

    [Fact]
    public void BulkPdfGeneration_ShouldHandleMultipleRequestsEfficiently()
    {
        // Arrange
        var scenarios = CreateMultipleScenarios(5);
        var stopwatch = Stopwatch.StartNew();
        var results = new List<byte[]>();

        // Act
        foreach (var scenario in scenarios)
        {
            results.Add(_pdfService.GenerateBusinessAnalysisReport(scenario, 6));
        }
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(5);
        results.Should().AllSatisfy(r => r.Should().NotBeEmpty());
        
        // 5 PDFs should generate within 20 seconds total (4 seconds per PDF average)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(20000,
            $"Bulk PDF generation took {stopwatch.ElapsedMilliseconds}ms for 5 reports, exceeding 20 second threshold");
    }

    [Fact]
    public void BulkCsvExport_ShouldHandleLargeDataSetsEfficiently()
    {
        // Arrange
        var scenarios = CreateMultipleScenarios(100); // Large dataset
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = _exportService.ExportMarketDataToCsv(scenarios);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // Exporting 100 scenarios to CSV should complete within 1 second
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            $"Bulk CSV export took {stopwatch.ElapsedMilliseconds}ms for 100 scenarios, exceeding 1 second threshold");
    }

    [Fact]
    public void MemoryUsage_PdfGenerationShouldNotLeakMemory()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var initialMemory = GC.GetTotalMemory(true);

        // Act - Generate multiple PDFs to test for memory leaks
        for (int i = 0; i < 10; i++)
        {
            var result = _pdfService.GenerateBusinessAnalysisReport(scenario, 5);
            result.Should().NotBeEmpty();
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(false);

        // Assert
        var memoryIncrease = finalMemory - initialMemory;
        
        // Memory increase should be reasonable (less than 50MB for 10 PDFs)
        memoryIncrease.Should().BeLessThan(50 * 1024 * 1024,
            $"Memory increased by {memoryIncrease / 1024 / 1024}MB, suggesting potential memory leak");
    }

    [Fact]
    public void ConcurrentPdfGeneration_ShouldHandleParallelRequests()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var stopwatch = Stopwatch.StartNew();

        // Act - Generate PDFs concurrently
        var tasks = Enumerable.Range(0, 5).Select(_ => 
            Task.Run(() => _pdfService.GenerateBusinessAnalysisReport(scenario, 7))
        );

        var results = Task.WhenAll(tasks).Result;
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(5);
        results.Should().AllSatisfy(r => r.Should().NotBeEmpty());
        
        // Concurrent generation should be faster than sequential (less than 10 seconds for 5 PDFs)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000,
            $"Concurrent PDF generation took {stopwatch.ElapsedMilliseconds}ms, suggesting poor parallelization");
    }

    [Fact]
    public void LargeSwotAnalysis_ShouldHandleEfficiently()
    {
        // Arrange
        var scenario = CreateTestScenario();
        var largeSwot = CreateVeryLargeSwotAnalysis(50); // 50 items per category
        var stopwatch = Stopwatch.StartNew();

        // Act
        var pdfResult = _pdfService.GenerateSwotAnalysisReport(scenario, largeSwot);
        var csvResult = _exportService.ExportSwotToCsv(largeSwot, "Large Test");
        stopwatch.Stop();

        // Assert
        pdfResult.Should().NotBeNull();
        csvResult.Should().NotBeNull();
        
        // Large SWOT processing should complete within 10 seconds
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000,
            $"Large SWOT processing took {stopwatch.ElapsedMilliseconds}ms, exceeding 10 second threshold");
    }

    [Fact]
    public void PdfSize_ShouldBeReasonableForContent()
    {
        // Arrange
        var scenario = CreateTestScenario();

        // Act
        var businessReport = _pdfService.GenerateBusinessAnalysisReport(scenario, 8);
        var swotReport = _pdfService.GenerateSwotAnalysisReport(scenario, CreateTestSwotAnalysis());
        var marketReport = _pdfService.GenerateMarketResearchReport(scenario, scenario.MarketResearch);
        var executiveSummary = _pdfService.GenerateExecutiveSummary(scenario, 8);

        // Assert
        // Business report should be substantial but not excessive (50KB - 2MB)
        businessReport.Length.Should().BeInRange(50000, 2000000);
        
        // SWOT report should be moderate size (20KB - 1MB)
        swotReport.Length.Should().BeInRange(20000, 1000000);
        
        // Market report should be reasonable (30KB - 1.5MB)
        marketReport.Length.Should().BeInRange(30000, 1500000);
        
        // Executive summary should be compact (10KB - 500KB)
        executiveSummary.Length.Should().BeInRange(10000, 500000);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void CsvExport_PerformanceShouldScaleLinearly(int scenarioCount)
    {
        // Arrange
        var scenarios = CreateMultipleScenarios(scenarioCount);
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = _exportService.ExportMarketDataToCsv(scenarios);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // Performance should scale roughly linearly with data size
        // Allow 10ms per scenario plus 100ms base overhead
        var expectedMaxTime = (scenarioCount * 10) + 100;
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(expectedMaxTime,
            $"CSV export for {scenarioCount} scenarios took {stopwatch.ElapsedMilliseconds}ms, " +
            $"exceeding expected {expectedMaxTime}ms threshold");
    }

    private static BusinessIdeaScenario CreateTestScenario()
    {
        return new BusinessIdeaScenario
        {
            Title = "Performance Test Business",
            Description = "This is a test scenario for performance testing.",
            MarketSize = 500000,
            CompetitionLevel = "Medium",
            StartupCost = 25000,
            MarketResearch = new MarketResearchData
            {
                Industry = "Technology"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 300000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 10 }
            }
        };
    }

    private static List<BusinessIdeaScenario> CreateMultipleScenarios(int count)
    {
        return Enumerable.Range(1, count).Select(i => new BusinessIdeaScenario
        {
            Title = $"Test Business {i}",
            Description = $"Description for test business {i}",
            MarketSize = 100000 + (i * 1000),
            CompetitionLevel = i % 3 == 0 ? "High" : i % 3 == 1 ? "Medium" : "Low",
            StartupCost = 10000 + (i * 500),
            MarketResearch = new MarketResearchData
            {
                Industry = $"Industry {i % 5}"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 150000 + (i * 2000) },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 6 + (i % 12) }
            }
        }).ToList();
    }

    private static SwotAnalysis CreateLargeSwotAnalysis()
    {
        return new SwotAnalysis(
            Strengths: Enumerable.Range(1, 10).Select(i => $"Strength {i}: Detailed description of strength number {i}").ToList(),
            Weaknesses: Enumerable.Range(1, 10).Select(i => $"Weakness {i}: Detailed description of weakness number {i}").ToList(),
            Opportunities: Enumerable.Range(1, 10).Select(i => $"Opportunity {i}: Detailed description of opportunity number {i}").ToList(),
            Threats: Enumerable.Range(1, 10).Select(i => $"Threat {i}: Detailed description of threat number {i}").ToList()
        );
    }

    private static SwotAnalysis CreateVeryLargeSwotAnalysis(int itemsPerCategory)
    {
        return new SwotAnalysis(
            Strengths: Enumerable.Range(1, itemsPerCategory)
                .Select(i => $"Strength {i}: Very detailed description of strength number {i} with lots of additional text to make it substantial")
                .ToList(),
            Weaknesses: Enumerable.Range(1, itemsPerCategory)
                .Select(i => $"Weakness {i}: Very detailed description of weakness number {i} with lots of additional text to make it substantial")
                .ToList(),
            Opportunities: Enumerable.Range(1, itemsPerCategory)
                .Select(i => $"Opportunity {i}: Very detailed description of opportunity number {i} with lots of additional text to make it substantial")
                .ToList(),
            Threats: Enumerable.Range(1, itemsPerCategory)
                .Select(i => $"Threat {i}: Very detailed description of threat number {i} with lots of additional text to make it substantial")
                .ToList()
        );
    }

    private static SwotAnalysis CreateTestSwotAnalysis()
    {
        return new SwotAnalysis(
            Strengths: new List<string> { "Strong team", "Good market position", "Innovative technology" },
            Weaknesses: new List<string> { "Limited funding", "Small team", "New to market" },
            Opportunities: new List<string> { "Growing market", "Partnership potential", "Government support" },
            Threats: new List<string> { "Strong competition", "Economic uncertainty", "Regulatory changes" }
        );
    }
}