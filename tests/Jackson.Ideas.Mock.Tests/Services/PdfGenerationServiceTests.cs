using FluentAssertions;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services;
using Jackson.Ideas.Mock.Services.Interfaces;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Services;

public class PdfGenerationServiceTests
{
    private readonly IPdfGenerationService _pdfService;
    private readonly BusinessIdeaScenario _testScenario;

    public PdfGenerationServiceTests()
    {
        _pdfService = new PdfGenerationService();
        _testScenario = CreateTestScenario();
    }

    [Fact]
    public void GenerateBusinessAnalysisReport_ShouldReturnValidPdfBytes()
    {
        // Arrange
        var healthScore = 8;

        // Act
        var result = _pdfService.GenerateBusinessAnalysisReport(_testScenario, healthScore);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(1000); // PDF should have reasonable size
        
        // Verify PDF signature (PDF files start with %PDF)
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(8)]
    [InlineData(5)]
    [InlineData(2)]
    public void GenerateBusinessAnalysisReport_ShouldHandleDifferentHealthScores(int healthScore)
    {
        // Act
        var result = _pdfService.GenerateBusinessAnalysisReport(_testScenario, healthScore);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // Verify PDF signature
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateSwotAnalysisReport_ShouldReturnValidPdfBytes()
    {
        // Arrange
        var swotAnalysis = CreateTestSwotAnalysis();

        // Act
        var result = _pdfService.GenerateSwotAnalysisReport(_testScenario, swotAnalysis);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(1000);
        
        // Verify PDF signature
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateMarketResearchReport_ShouldReturnValidPdfBytes()
    {
        // Arrange
        var marketData = _testScenario.MarketResearch;

        // Act
        var result = _pdfService.GenerateMarketResearchReport(_testScenario, marketData);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(1000);
        
        // Verify PDF signature
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateExecutiveSummary_ShouldReturnValidPdfBytes()
    {
        // Arrange
        var healthScore = 7;

        // Act
        var result = _pdfService.GenerateExecutiveSummary(_testScenario, healthScore);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(500); // Executive summary should be smaller but still substantial
        
        // Verify PDF signature
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateBusinessAnalysisReport_ShouldHandleEmptyStrings()
    {
        // Arrange
        var scenarioWithEmptyStrings = new BusinessIdeaScenario
        {
            Title = "",
            Description = "",
            MarketSize = 100000,
            CompetitionLevel = "Medium",
            StartupCost = 15000,
            MarketResearch = new MarketResearchData { Industry = "" },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 180000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 8 }
            }
        };

        // Act
        var result = _pdfService.GenerateBusinessAnalysisReport(scenarioWithEmptyStrings, 6);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // Should still generate a valid PDF even with empty strings
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateSwotAnalysisReport_ShouldHandleEmptySwotLists()
    {
        // Arrange
        var emptySwot = new SwotAnalysis(
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<string>()
        );

        // Act
        var result = _pdfService.GenerateSwotAnalysisReport(_testScenario, emptySwot);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // Should still generate a valid PDF even with empty SWOT lists
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000000)]
    public void GenerateBusinessAnalysisReport_ShouldHandleEdgeCaseNumbers(int marketSize)
    {
        // Arrange
        var edgeCaseScenario = _testScenario with { MarketSize = marketSize };

        // Act
        var result = _pdfService.GenerateBusinessAnalysisReport(edgeCaseScenario, 5);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // Should handle edge case numbers gracefully
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    private static BusinessIdeaScenario CreateTestScenario()
    {
        return new BusinessIdeaScenario
        {
            Title = "Test Business Idea",
            Description = "This is a test business idea for unit testing PDF generation functionality.",
            MarketSize = 250000,
            CompetitionLevel = "Medium",
            StartupCost = 15000,
            MarketResearch = new MarketResearchData
            {
                Industry = "Technology"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 180000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 8 }
            }
        };
    }

    private static SwotAnalysis CreateTestSwotAnalysis()
    {
        return new SwotAnalysis(
            Strengths: new List<string>
            {
                "Strong technical team",
                "Innovative product concept",
                "First-mover advantage"
            },
            Weaknesses: new List<string>
            {
                "Limited initial funding",
                "Small team size",
                "No established customer base"
            },
            Opportunities: new List<string>
            {
                "Growing market demand",
                "Partnership opportunities",
                "Government incentives"
            },
            Threats: new List<string>
            {
                "Established competitors",
                "Economic uncertainty",
                "Regulatory changes"
            }
        );
    }
}