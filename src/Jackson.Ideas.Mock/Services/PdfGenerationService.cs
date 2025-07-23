using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Jackson.Ideas.Mock.Services;

/// <summary>
/// Service for generating PDF reports using QuestPDF library.
/// Provides comprehensive business analysis reports, SWOT analysis, market research reports, and executive summaries.
/// </summary>
/// <remarks>
/// This service generates professional PDF documents with proper formatting, color-coding, and layout.
/// All generated PDFs include proper headers, footers, and branding consistent with Ideas Matter platform.
/// </remarks>
public class PdfGenerationService : IPdfGenerationService
{
    static PdfGenerationService()
    {
        // Set license for QuestPDF (Community license)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateBusinessAnalysisReport(BusinessIdeaScenario scenario, int healthScore)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                    .Text("Ideas Matter - Business Analysis Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        // Title Section
                        x.Spacing(20);
                        x.Item().Text($"Business Idea: {scenario.Title}")
                            .FontSize(18).SemiBold().FontColor(Colors.Grey.Darken3);
                        
                        x.Item().Text($"Generated on: {DateTime.Now:MMMM dd, yyyy}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);

                        // Health Score Section
                        x.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Business Health Score").FontSize(16).SemiBold();
                                col.Item().PaddingTop(10).Row(r =>
                                {
                                    r.ConstantItem(60).Container()
                                        .Border(3).BorderColor(GetScoreColor(healthScore))
                                        .AlignCenter()
                                        .AlignMiddle()
                                        .Height(60)
                                        .Text($"{healthScore}/10")
                                        .FontSize(20).SemiBold().FontColor(GetScoreColor(healthScore));
                                    
                                    r.RelativeItem().PaddingLeft(20).Column(c =>
                                    {
                                        c.Item().Text(GetScoreDescription(healthScore))
                                            .FontSize(14).FontColor(Colors.Grey.Darken2);
                                        c.Item().Text(GetScoreText(healthScore))
                                            .FontSize(12).FontColor(GetScoreColor(healthScore)).SemiBold();
                                    });
                                });
                            });
                        });

                        // Description
                        x.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Idea Description").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Text(scenario.Description)
                                .FontSize(12).LineHeight(1.4f);
                        });

                        // Key Metrics
                        x.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Key Business Metrics").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Cell().Border(1).Background(Colors.Grey.Lighten3)
                                    .Padding(10).Text("Metric").SemiBold();
                                table.Cell().Border(1).Background(Colors.Grey.Lighten3)
                                    .Padding(10).Text("Value").SemiBold();

                                table.Cell().Border(1).Padding(10).Text("Market Size");
                                table.Cell().Border(1).Padding(10).Text($"{scenario.MarketSize:N0} potential customers");

                                table.Cell().Border(1).Padding(10).Text("Competition Level");
                                table.Cell().Border(1).Padding(10).Text(scenario.CompetitionLevel);

                                table.Cell().Border(1).Padding(10).Text("Startup Investment");
                                table.Cell().Border(1).Padding(10).Text($"${scenario.StartupCost:N0}");

                                table.Cell().Border(1).Padding(10).Text("Year 1 Revenue Potential");
                                table.Cell().Border(1).Padding(10).Text($"${scenario.FinancialProjections.Revenue.Year1Total:N0}");

                                table.Cell().Border(1).Padding(10).Text("Break-even Timeline");
                                table.Cell().Border(1).Padding(10).Text($"{scenario.FinancialProjections.CashFlow.BreakEvenMonth} months");
                            });
                        });

                        // Market Analysis Summary
                        x.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Market Analysis Summary").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Text(GetMarketAnalysisSummary(scenario))
                                .FontSize(12).LineHeight(1.4f);
                        });

                        // Next Steps
                        x.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Recommended Next Steps").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Column(steps =>
                            {
                                var nextSteps = GetNextSteps(scenario, healthScore);
                                foreach (var step in nextSteps)
                                {
                                    steps.Item().Row(row =>
                                    {
                                        row.ConstantItem(20).Text("•").FontSize(12);
                                        row.RelativeItem().Text(step).FontSize(12).LineHeight(1.3f);
                                    });
                                }
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by ");
                        x.Span("Ideas Matter").SemiBold();
                        x.Span(" - Your AI Business Partner");
                    });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateSwotAnalysisReport(BusinessIdeaScenario scenario, SwotAnalysis swotAnalysis)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                    .Text("SWOT Analysis Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                        
                        // Title
                        x.Item().Text($"SWOT Analysis: {scenario.Title}")
                            .FontSize(18).SemiBold().FontColor(Colors.Grey.Darken3);

                        // SWOT Matrix
                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Strengths
                            table.Cell().Border(2).Background(Colors.Green.Lighten4)
                                .Padding(15).Column(col =>
                                {
                                    col.Item().Text("STRENGTHS").SemiBold().FontSize(14)
                                        .FontColor(Colors.Green.Darken2);
                                    col.Item().PaddingTop(10).Column(items =>
                                    {
                                        foreach (var strength in swotAnalysis.Strengths)
                                        {
                                            items.Item().Row(row =>
                                            {
                                                row.ConstantItem(15).Text("•").FontColor(Colors.Green.Darken1);
                                                row.RelativeItem().Text(strength).FontSize(11).LineHeight(1.3f);
                                            });
                                        }
                                    });
                                });

                            // Weaknesses
                            table.Cell().Border(2).Background(Colors.Red.Lighten4)
                                .Padding(15).Column(col =>
                                {
                                    col.Item().Text("WEAKNESSES").SemiBold().FontSize(14)
                                        .FontColor(Colors.Red.Darken2);
                                    col.Item().PaddingTop(10).Column(items =>
                                    {
                                        foreach (var weakness in swotAnalysis.Weaknesses)
                                        {
                                            items.Item().Row(row =>
                                            {
                                                row.ConstantItem(15).Text("•").FontColor(Colors.Red.Darken1);
                                                row.RelativeItem().Text(weakness).FontSize(11).LineHeight(1.3f);
                                            });
                                        }
                                    });
                                });

                            // Opportunities
                            table.Cell().Border(2).Background(Colors.Blue.Lighten4)
                                .Padding(15).Column(col =>
                                {
                                    col.Item().Text("OPPORTUNITIES").SemiBold().FontSize(14)
                                        .FontColor(Colors.Blue.Darken2);
                                    col.Item().PaddingTop(10).Column(items =>
                                    {
                                        foreach (var opportunity in swotAnalysis.Opportunities)
                                        {
                                            items.Item().Row(row =>
                                            {
                                                row.ConstantItem(15).Text("•").FontColor(Colors.Blue.Darken1);
                                                row.RelativeItem().Text(opportunity).FontSize(11).LineHeight(1.3f);
                                            });
                                        }
                                    });
                                });

                            // Threats
                            table.Cell().Border(2).Background(Colors.Orange.Lighten4)
                                .Padding(15).Column(col =>
                                {
                                    col.Item().Text("THREATS").SemiBold().FontSize(14)
                                        .FontColor(Colors.Orange.Darken2);
                                    col.Item().PaddingTop(10).Column(items =>
                                    {
                                        foreach (var threat in swotAnalysis.Threats)
                                        {
                                            items.Item().Row(row =>
                                            {
                                                row.ConstantItem(15).Text("•").FontColor(Colors.Orange.Darken1);
                                                row.RelativeItem().Text(threat).FontSize(11).LineHeight(1.3f);
                                            });
                                        }
                                    });
                                });
                        });

                        // Strategic Implications
                        x.Item().PaddingTop(30).Column(col =>
                        {
                            col.Item().Text("Strategic Implications").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Text(GetStrategicImplications(swotAnalysis))
                                .FontSize(12).LineHeight(1.4f);
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by ");
                        x.Span("Ideas Matter").SemiBold();
                        x.Span($" on {DateTime.Now:MMMM dd, yyyy}");
                    });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateMarketResearchReport(BusinessIdeaScenario scenario, MarketResearchData marketData)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                    .Text("Market Research Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                        
                        x.Item().Text($"Market Research: {scenario.Title}")
                            .FontSize(18).SemiBold().FontColor(Colors.Grey.Darken3);

                        x.Item().Text($"Industry: {marketData.Industry}")
                            .FontSize(14).FontColor(Colors.Grey.Medium);

                        // Market Overview
                        x.Item().Column(col =>
                        {
                            col.Item().Text("Market Overview").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Text($"The {marketData.Industry} industry represents a significant opportunity with a potential market of {scenario.MarketSize:N0} customers. Competition levels are currently {scenario.CompetitionLevel.ToLower()}, providing {("good".Equals(scenario.CompetitionLevel.ToLower()) ? "balanced" : scenario.CompetitionLevel.ToLower())} entry conditions for new market participants.")
                                .FontSize(12).LineHeight(1.4f);
                        });

                        // Financial Projections
                        x.Item().Column(col =>
                        {
                            col.Item().Text("Financial Projections").FontSize(16).SemiBold();
                            col.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Cell().Border(1).Background(Colors.Grey.Lighten3)
                                    .Padding(10).Text("Financial Metric").SemiBold();
                                table.Cell().Border(1).Background(Colors.Grey.Lighten3)
                                    .Padding(10).Text("Projection").SemiBold();

                                table.Cell().Border(1).Padding(10).Text("Required Investment");
                                table.Cell().Border(1).Padding(10).Text($"${scenario.StartupCost:N0}");

                                table.Cell().Border(1).Padding(10).Text("Year 1 Revenue");
                                table.Cell().Border(1).Padding(10).Text($"${scenario.FinancialProjections.Revenue.Year1Total:N0}");

                                table.Cell().Border(1).Padding(10).Text("Break-even Point");
                                table.Cell().Border(1).Padding(10).Text($"Month {scenario.FinancialProjections.CashFlow.BreakEvenMonth}");

                                table.Cell().Border(1).Padding(10).Text("ROI Potential");
                                table.Cell().Border(1).Padding(10).Text($"{((double)scenario.FinancialProjections.Revenue.Year1Total / (double)scenario.StartupCost * 100):F1}%");
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by ");
                        x.Span("Ideas Matter").SemiBold();
                        x.Span($" on {DateTime.Now:MMMM dd, yyyy}");
                    });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateExecutiveSummary(BusinessIdeaScenario scenario, int healthScore)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                    .Text("Executive Summary")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(15);
                        
                        x.Item().Text($"Business Opportunity: {scenario.Title}")
                            .FontSize(16).SemiBold().FontColor(Colors.Grey.Darken3);

                        x.Item().Text("Executive Summary")
                            .FontSize(14).SemiBold();

                        x.Item().Text(GetExecutiveSummaryText(scenario, healthScore))
                            .FontSize(12).LineHeight(1.5f);

                        x.Item().Text("Key Highlights")
                            .FontSize(14).SemiBold();

                        x.Item().Column(highlights =>
                        {
                            var keyHighlights = GetKeyHighlights(scenario, healthScore);
                            foreach (var highlight in keyHighlights)
                            {
                                highlights.Item().Row(row =>
                                {
                                    row.ConstantItem(20).Text("•").FontSize(12);
                                    row.RelativeItem().Text(highlight).FontSize(12).LineHeight(1.3f);
                                });
                            }
                        });

                        x.Item().Text("Investment Requirements")
                            .FontSize(14).SemiBold();

                        x.Item().Text($"Initial investment of ${scenario.StartupCost:N0} is required to launch this business opportunity, with projected first-year revenues of ${scenario.FinancialProjections.Revenue.Year1Total:N0} and break-even expected by month {scenario.FinancialProjections.CashFlow.BreakEvenMonth}.")
                            .FontSize(12).LineHeight(1.4f);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by ");
                        x.Span("Ideas Matter").SemiBold();
                        x.Span($" on {DateTime.Now:MMMM dd, yyyy}");
                    });
            });
        }).GeneratePdf();
    }

    // Helper methods
    private static string GetScoreColor(int score) => score switch
    {
        >= 8 => Colors.Green.Medium,
        >= 6 => Colors.Orange.Medium,
        >= 4 => Colors.Yellow.Medium,
        _ => Colors.Red.Medium
    };

    private static string GetScoreDescription(int score) => score switch
    {
        >= 8 => "Excellent business potential with strong fundamentals",
        >= 6 => "Good opportunity with manageable challenges",
        >= 4 => "Moderate potential requiring strategic improvements",
        _ => "High-risk opportunity needing significant development"
    };

    private static string GetScoreText(int score) => score switch
    {
        >= 8 => "Strong Recommendation",
        >= 6 => "Promising Opportunity",
        >= 4 => "Proceed with Caution",
        _ => "Requires Major Improvements"
    };

    private static string GetMarketAnalysisSummary(BusinessIdeaScenario scenario)
    {
        return $"Market analysis indicates a {scenario.CompetitionLevel.ToLower()} competitive landscape with {scenario.MarketSize:N0} potential customers in the target market. The business model shows strong revenue potential with projected first-year earnings of ${scenario.FinancialProjections.Revenue.Year1Total:N0}. Initial investment requirements of ${scenario.StartupCost:N0} are reasonable for this market segment, with break-even projected for month {scenario.FinancialProjections.CashFlow.BreakEvenMonth}.";
    }

    private static List<string> GetNextSteps(BusinessIdeaScenario scenario, int healthScore)
    {
        var steps = new List<string>();
        
        if (healthScore >= 7)
        {
            steps.Add("Develop a detailed business plan and financial model");
            steps.Add("Conduct customer interviews to validate assumptions");
            steps.Add("Create a minimum viable product (MVP) or prototype");
            steps.Add("Establish legal business structure and intellectual property protection");
            steps.Add("Develop go-to-market strategy and launch timeline");
        }
        else if (healthScore >= 5)
        {
            steps.Add("Refine the business concept based on market feedback");
            steps.Add("Address identified weaknesses in the business model");
            steps.Add("Conduct additional market research to validate assumptions");
            steps.Add("Develop risk mitigation strategies for identified threats");
            steps.Add("Consider strategic partnerships to strengthen market position");
        }
        else
        {
            steps.Add("Significantly revise the business concept or consider pivot");
            steps.Add("Conduct extensive market research to identify better opportunities");
            steps.Add("Address fundamental weaknesses before proceeding");
            steps.Add("Consider alternative business models or market segments");
            steps.Add("Seek mentorship or expert guidance before major investments");
        }
        
        return steps;
    }

    private static string GetStrategicImplications(SwotAnalysis swot)
    {
        return "Based on the SWOT analysis, the business should leverage its key strengths to capitalize on identified opportunities while developing strategies to address weaknesses and mitigate potential threats. The strategic focus should be on building competitive advantages through the strength areas while systematically addressing vulnerability points identified in the weaknesses and threats sections.";
    }

    private static string GetExecutiveSummaryText(BusinessIdeaScenario scenario, int healthScore)
    {
        var potential = healthScore >= 7 ? "strong" : healthScore >= 5 ? "moderate" : "limited";
        return $"The {scenario.Title} business opportunity presents {potential} market potential in the {scenario.MarketResearch.Industry} sector. With a target market of {scenario.MarketSize:N0} potential customers and {scenario.CompetitionLevel.ToLower()} competition levels, this venture requires an initial investment of ${scenario.StartupCost:N0} and projects first-year revenues of ${scenario.FinancialProjections.Revenue.Year1Total:N0}. The business model demonstrates {(healthScore >= 6 ? "favorable" : "challenging")} financial fundamentals with break-even projected for month {scenario.FinancialProjections.CashFlow.BreakEvenMonth}.";
    }

    private static List<string> GetKeyHighlights(BusinessIdeaScenario scenario, int healthScore)
    {
        var highlights = new List<string>
        {
            $"Market size of {scenario.MarketSize:N0} potential customers",
            $"${scenario.FinancialProjections.Revenue.Year1Total:N0} projected first-year revenue",
            $"Break-even expected by month {scenario.FinancialProjections.CashFlow.BreakEvenMonth}",
            $"{scenario.CompetitionLevel} competition level in target market"
        };

        if (healthScore >= 7)
        {
            highlights.Add("Strong overall business health score indicating high success probability");
        }
        else if (healthScore >= 5)
        {
            highlights.Add("Moderate business health score with identified improvement opportunities");
        }

        return highlights;
    }
}