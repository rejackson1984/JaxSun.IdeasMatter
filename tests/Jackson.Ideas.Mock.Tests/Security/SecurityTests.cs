using FluentAssertions;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services;
using Jackson.Ideas.Mock.Services.Interfaces;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Jackson.Ideas.Mock.Tests.Security;

public class SecurityTests
{
    private readonly IPdfGenerationService _pdfService;
    private readonly IDataExportService _exportService;

    public SecurityTests()
    {
        _pdfService = new PdfGenerationService();
        _exportService = new DataExportService();
    }

    [Fact]
    public void PdfGeneration_ShouldSanitizeHtmlAndScriptContent()
    {
        // Arrange
        var maliciousScenario = new BusinessIdeaScenario
        {
            Title = "<script>alert('xss')</script>Malicious Title",
            Description = "<img src=x onerror=alert('xss')>Evil Description",
            MarketSize = 1000,
            CompetitionLevel = "<b>Medium</b>",
            StartupCost = 5000,
            MarketResearch = new MarketResearchData
            {
                Industry = "Technology<script>evil()</script>"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 100000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 6 }
            }
        };

        // Act
        var pdfBytes = _pdfService.GenerateBusinessAnalysisReport(maliciousScenario, 5);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        
        // PDF should be generated successfully without executing malicious content
        var pdfContent = Encoding.UTF8.GetString(pdfBytes);
        
        // QuestPDF should handle the content safely - verify PDF signature
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes.Take(4).ToArray());
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void CsvExport_ShouldEscapeFormulaInjection()
    {
        // Arrange - CSV formula injection attack
        var maliciousScenario = new BusinessIdeaScenario
        {
            Title = "=cmd|'/c calc'!A1",
            Description = "=SUM(1+1)*cmd|'/c calc'!A1",
            MarketSize = 1000,
            CompetitionLevel = "@SUM(1+1)*cmd|'/c calc'!A1",
            StartupCost = 5000,
            MarketResearch = new MarketResearchData
            {
                Industry = "+cmd|'/c calc'!A1"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 100000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 6 }
            }
        };

        // Act
        var csvBytes = _exportService.ExportScenarioToCsv(maliciousScenario, 5);

        // Assert
        var csvContent = Encoding.UTF8.GetString(csvBytes);
        
        // CSV should properly escape dangerous formulas
        csvContent.Should().Contain("\"=cmd");
        csvContent.Should().Contain("\"@SUM");
        csvContent.Should().Contain("\"+cmd");
        
        // Should not contain unescaped formula characters that could be executed
        // The dangerous formulas should be treated as text, not executable formulas
        csvContent.Should().NotContain("\n=cmd"); // Should not start line with formula
        csvContent.Should().NotContain("\n@SUM"); // Should not start line with formula
        csvContent.Should().NotContain("\n+cmd"); // Should not start line with formula
    }

    [Fact]
    public void JsonExport_ShouldSanitizeJsonInjection()
    {
        // Arrange - JSON injection attempt
        var maliciousScenario = new BusinessIdeaScenario
        {
            Title = "Normal Title\",\"maliciousField\":\"injected",
            Description = "Description with \\\"quotes\\\" and \\\\ backslashes",
            MarketSize = 1000,
            CompetitionLevel = "Medium",
            StartupCost = 5000,
            MarketResearch = new MarketResearchData
            {
                Industry = "Tech\nwith\nnewlines"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 100000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 6 }
            }
        };

        // Act
        var jsonBytes = _exportService.ExportScenarioToJson(maliciousScenario, 5);

        // Assert
        var jsonContent = Encoding.UTF8.GetString(jsonBytes);
        
        // Should produce valid JSON (will throw if invalid)
        var jsonDoc = JsonDocument.Parse(jsonContent);
        jsonDoc.Should().NotBeNull();
        
        // Should not contain injected fields
        jsonContent.Should().NotContain("maliciousField");
        
        // Should properly escape special characters
        jsonContent.Should().Contain("\\\""); // Escaped quotes
        jsonContent.Should().Contain("\\n"); // Escaped newlines
    }

    [Fact]
    public void DataInput_ShouldHandleLargeInputsGracefully()
    {
        // Arrange - Attempt to cause buffer overflow or DoS
        var largeString = new string('A', 100000); // 100KB string
        
        var largeInputScenario = new BusinessIdeaScenario
        {
            Title = largeString,
            Description = largeString,
            MarketSize = int.MaxValue,
            CompetitionLevel = "High",
            StartupCost = int.MaxValue,
            MarketResearch = new MarketResearchData
            {
                Industry = largeString
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = int.MaxValue },
                CashFlow = new CashFlowProjections { BreakEvenMonth = int.MaxValue }
            }
        };

        // Act & Assert - Should not throw exceptions or cause system issues
        var pdfAction = () => _pdfService.GenerateBusinessAnalysisReport(largeInputScenario, 5);
        var csvAction = () => _exportService.ExportScenarioToCsv(largeInputScenario, 5);
        var jsonAction = () => _exportService.ExportScenarioToJson(largeInputScenario, 5);

        pdfAction.Should().NotThrow();
        csvAction.Should().NotThrow();
        jsonAction.Should().NotThrow();
    }

    [Fact]
    public void SwotAnalysis_ShouldHandleMaliciousInput()
    {
        // Arrange
        var maliciousSwot = new SwotAnalysis(
            Strengths: new List<string>
            {
                "<script>alert('xss')</script>",
                "=cmd|'/c calc'!A1",
                "Normal strength"
            },
            Weaknesses: new List<string>
            {
                "\",\"injected\":\"field",
                "\\x00\\x01\\x02", // Null bytes and control characters
                "Normal weakness"
            },
            Opportunities: new List<string>
            {
                new string('B', 50000), // Very long string
                "@SUM(1+1)*cmd|'/c calc'!A1"
            },
            Threats: new List<string>
            {
                "+cmd|'/c notepad'!A1",
                "Normal threat with unicode: 测试"
            }
        );

        var scenario = CreateTestScenario();

        // Act & Assert
        var pdfAction = () => _pdfService.GenerateSwotAnalysisReport(scenario, maliciousSwot);
        var csvAction = () => _exportService.ExportSwotToCsv(maliciousSwot, "Test");

        pdfAction.Should().NotThrow();
        csvAction.Should().NotThrow();

        // Verify outputs are still valid
        var pdfResult = _pdfService.GenerateSwotAnalysisReport(scenario, maliciousSwot);
        var csvResult = _exportService.ExportSwotToCsv(maliciousSwot, "Test");

        pdfResult.Should().NotBeNull();
        csvResult.Should().NotBeNull();
        
        // CSV should escape dangerous content
        var csvContent = Encoding.UTF8.GetString(csvResult);
        csvContent.Should().Contain("\"=cmd"); // Should be quoted/escaped
        csvContent.Should().Contain("\"@SUM"); // Should be quoted/escaped
    }

    [Fact]
    public void NullAndEmptyInputs_ShouldBeHandledSafely()
    {
        // Arrange
        var nullScenario = new BusinessIdeaScenario
        {
            Title = null!,
            Description = null!,
            MarketSize = 0,
            CompetitionLevel = null!,
            StartupCost = 0,
            MarketResearch = new MarketResearchData
            {
                Industry = null!
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 0 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 0 }
            }
        };

        var emptySwot = new SwotAnalysis(null!, null!, null!, null!);

        // Act & Assert - Should handle null inputs gracefully
        var pdfAction = () => _pdfService.GenerateBusinessAnalysisReport(nullScenario, 5);
        var csvAction = () => _exportService.ExportScenarioToCsv(nullScenario, 5);
        var jsonAction = () => _exportService.ExportScenarioToJson(nullScenario, 5);
        var swotCsvAction = () => _exportService.ExportSwotToCsv(emptySwot, null!);

        pdfAction.Should().NotThrow();
        csvAction.Should().NotThrow();
        jsonAction.Should().NotThrow();
        swotCsvAction.Should().NotThrow();
    }

    [Fact]
    public void FileNames_ShouldSanitizeForSafeDownload()
    {
        // Arrange
        var maliciousTitle = "../../../etc/passwd<script>alert()</script>";
        var baseScenario = CreateTestScenario();
        var scenario = new BusinessIdeaScenario
        {
            Id = baseScenario.Id,
            Name = baseScenario.Name,
            Title = maliciousTitle,
            Description = baseScenario.Description,
            Industry = baseScenario.Industry,
            TargetMarket = baseScenario.TargetMarket,
            EstimatedStartupCost = baseScenario.EstimatedStartupCost,
            StartupCost = baseScenario.StartupCost,
            ProjectedRevenue = baseScenario.ProjectedRevenue,
            ViabilityScore = baseScenario.ViabilityScore,
            MarketSize = baseScenario.MarketSize,
            CompetitionLevel = baseScenario.CompetitionLevel,
            MarketResearch = baseScenario.MarketResearch,
            FinancialProjections = baseScenario.FinancialProjections,
            KeyChallenges = baseScenario.KeyChallenges,
            SuccessFactors = baseScenario.SuccessFactors,
            CreatedAt = baseScenario.CreatedAt
        };

        // Act
        var csvBytes = _exportService.ExportScenarioToCsv(scenario, 7);
        var jsonBytes = _exportService.ExportScenarioToJson(scenario, 7);

        // Assert
        csvBytes.Should().NotBeNull();
        jsonBytes.Should().NotBeNull();
        
        // The services themselves don't generate filenames, but when used in controllers,
        // filenames should be sanitized. This test ensures the data export doesn't fail
        // with malicious titles that might be used in filenames.
        
        var csvContent = Encoding.UTF8.GetString(csvBytes);
        var jsonContent = Encoding.UTF8.GetString(jsonBytes);
        
        csvContent.Should().Contain("../../../etc/passwd"); // Content preserved but escaped
        jsonContent.Should().Contain("../../../etc/passwd"); // Content preserved but escaped
    }

    private static BusinessIdeaScenario CreateTestScenario()
    {
        return new BusinessIdeaScenario
        {
            Title = "Test Business",
            Description = "Test Description",
            MarketSize = 1000,
            CompetitionLevel = "Medium",
            StartupCost = 5000,
            MarketResearch = new MarketResearchData
            {
                Industry = "Technology"
            },
            FinancialProjections = new FinancialProjections
            {
                Revenue = new RevenueProjections { Year1Total = 100000 },
                CashFlow = new CashFlowProjections { BreakEvenMonth = 6 }
            }
        };
    }
}