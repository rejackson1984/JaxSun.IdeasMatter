using FluentAssertions;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services;
using Jackson.Ideas.Mock.Services.Interfaces;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Services;

public class DataExportServiceTests
{
    private readonly IDataExportService _exportService;
    private readonly BusinessIdeaScenario _testScenario;

    public DataExportServiceTests()
    {
        _exportService = new DataExportService();
        _testScenario = CreateTestScenario();
    }

    [Fact]
    public void ExportScenarioToCsv_ShouldReturnValidCsvData()
    {
        // Arrange
        var healthScore = 8;

        // Act
        var result = _exportService.ExportScenarioToCsv(_testScenario, healthScore);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("Metric,Value");
        csvContent.Should().Contain("Business Idea");
        csvContent.Should().Contain("Test Business Idea");
        csvContent.Should().Contain("Health Score");
        csvContent.Should().Contain("8/10");
        csvContent.Should().Contain("Market Size");
        csvContent.Should().Contain("250,000");
    }

    [Fact]
    public void ExportScenarioToJson_ShouldReturnValidJsonData()
    {
        // Arrange
        var healthScore = 7;

        // Act
        var result = _exportService.ExportScenarioToJson(_testScenario, healthScore);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var jsonContent = Encoding.UTF8.GetString(result);
        
        // Verify it's valid JSON
        var jsonDocument = JsonDocument.Parse(jsonContent);
        jsonDocument.Should().NotBeNull();

        // Verify key fields are present
        jsonContent.Should().Contain("businessIdea");
        jsonContent.Should().Contain("Test Business Idea");
        jsonContent.Should().Contain("healthScore");
        jsonContent.Should().Contain("7/10");
        jsonContent.Should().Contain("marketData");
        jsonContent.Should().Contain("financialProjections");
    }

    [Fact]
    public void ExportMarketDataToCsv_ShouldHandleMultipleScenarios()
    {
        // Arrange
        var scenarios = new List<BusinessIdeaScenario>
        {
            _testScenario,
            CreateAlternativeScenario()
        };

        // Act
        var result = _exportService.ExportMarketDataToCsv(scenarios);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        
        // Should contain header
        csvContent.Should().Contain("Business Idea,Industry,Market Size");
        
        // Should contain both scenarios
        csvContent.Should().Contain("Test Business Idea");
        csvContent.Should().Contain("Alternative Test Idea");
        
        // Should have proper CSV structure (3 lines: header + 2 data rows)
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Length.Should().Be(3);
    }

    [Fact]
    public void ExportSwotToCsv_ShouldReturnValidSwotCsvData()
    {
        // Arrange
        var swotAnalysis = CreateTestSwotAnalysis();
        var scenarioTitle = "Test Scenario";

        // Act
        var result = _exportService.ExportSwotToCsv(swotAnalysis, scenarioTitle);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        
        // Should contain header
        csvContent.Should().Contain("Category,Item");
        
        // Should contain all SWOT categories
        csvContent.Should().Contain("Strengths");
        csvContent.Should().Contain("Weaknesses");
        csvContent.Should().Contain("Opportunities");
        csvContent.Should().Contain("Threats");
        
        // Should contain actual SWOT items
        csvContent.Should().Contain("Strong technical team");
        csvContent.Should().Contain("Limited funding");
        csvContent.Should().Contain("Market growth");
        csvContent.Should().Contain("Competition risk");
    }

    [Fact]
    public void ExportScenarioToCsv_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var specialScenario = _testScenario with 
        { 
            Title = "Business with \"Quotes\" and, Commas",
            Description = "Description with special chars: @#$%^&*()"
        };

        // Act
        var result = _exportService.ExportScenarioToCsv(specialScenario, 6);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        
        // Should properly escape quotes in CSV
        csvContent.Should().Contain("\"Business with \"\"Quotes\"\" and, Commas\"");
        csvContent.Should().Contain("special chars");
    }

    [Fact]
    public void ExportScenarioToJson_ShouldProduceWellFormattedJson()
    {
        // Act
        var result = _exportService.ExportScenarioToJson(_testScenario, 9);

        // Assert
        var jsonContent = Encoding.UTF8.GetString(result);
        
        // Should be properly indented (well-formatted)
        jsonContent.Should().Contain("{\n");
        jsonContent.Should().Contain("  \"");
        
        // Should use camelCase property names
        jsonContent.Should().Contain("businessIdea");
        jsonContent.Should().Contain("marketData");
        jsonContent.Should().Contain("financialProjections");
        
        // Verify JSON structure
        var jsonDoc = JsonDocument.Parse(jsonContent);
        var root = jsonDoc.RootElement;
        
        root.GetProperty("businessIdea").GetString().Should().Be("Test Business Idea");
        root.GetProperty("healthScore").GetString().Should().Be("9/10");
        root.GetProperty("marketData").GetProperty("marketSize").GetInt32().Should().Be(250000);
    }

    [Fact]
    public void ExportMarketDataToCsv_ShouldHandleEmptyScenariosList()
    {
        // Arrange
        var emptyScenarios = new List<BusinessIdeaScenario>();

        // Act
        var result = _exportService.ExportMarketDataToCsv(emptyScenarios);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        
        // Should still contain header even with no data
        csvContent.Should().Contain("Business Idea,Industry,Market Size");
        
        // Should only have header line
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Length.Should().Be(1);
    }

    [Fact]
    public void ExportSwotToCsv_ShouldHandleEmptySwotAnalysis()
    {
        // Arrange
        var emptySwot = new SwotAnalysis(
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<string>()
        );

        // Act
        var result = _exportService.ExportSwotToCsv(emptySwot, "Empty Test");

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        
        // Should contain header
        csvContent.Should().Contain("Category,Item");
        
        // Should only contain header since no SWOT items exist
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Length.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void ExportScenarioToCsv_ShouldHandleDifferentHealthScores(int healthScore)
    {
        // Act
        var result = _exportService.ExportScenarioToCsv(_testScenario, healthScore);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain($"{healthScore}/10");
    }

    [Fact]
    public void ExportedData_ShouldIncludeTimestamp()
    {
        // Act
        var csvResult = _exportService.ExportScenarioToCsv(_testScenario, 7);
        var jsonResult = _exportService.ExportScenarioToJson(_testScenario, 7);

        // Assert
        var csvContent = Encoding.UTF8.GetString(csvResult);
        var jsonContent = Encoding.UTF8.GetString(jsonResult);

        csvContent.Should().Contain("Analysis Date");
        jsonContent.Should().Contain("analysisDate");
        
        // Should contain current year
        var currentYear = DateTime.Now.Year.ToString();
        csvContent.Should().Contain(currentYear);
        jsonContent.Should().Contain(currentYear);
    }

    private static BusinessIdeaScenario CreateTestScenario()
    {
        return new BusinessIdeaScenario
        {
            Title = "Test Business Idea",
            Description = "This is a test business idea for unit testing export functionality.",
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

    private static BusinessIdeaScenario CreateAlternativeScenario()
    {
        return new BusinessIdeaScenario
        {
            Title = "Alternative Test Idea",
            Description = "Another test business idea.",
            MarketSize = 500000,
            CompetitionLevel = "High",
            StartupCost = 25000,
            MarketResearch = new MarketResearchData
            {
                Industry = "Healthcare"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 300000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 12 }
            }
        };
    }

    private static SwotAnalysis CreateTestSwotAnalysis()
    {
        return new SwotAnalysis(
            Strengths: new List<string>
            {
                "Strong technical team",
                "Innovative approach"
            },
            Weaknesses: new List<string>
            {
                "Limited funding",
                "Small market presence"
            },
            Opportunities: new List<string>
            {
                "Market growth",
                "Partnership potential"
            },
            Threats: new List<string>
            {
                "Competition risk",
                "Economic downturn"
            }
        );
    }
}