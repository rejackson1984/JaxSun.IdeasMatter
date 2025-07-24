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

    public async Task GenerateBusinessPlanPdfAsync(BusinessIdeaScenario scenario, BusinessPlan businessPlan)
    {
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Ideas Matter - Business Plan")
                                .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{scenario.Name}")
                                .SemiBold().FontSize(14).FontColor(Colors.Grey.Darken3);
                        });
                        
                        row.ConstantItem(100).AlignRight()
                            .Text($"Generated: {DateTime.Now:MM/dd/yyyy}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(content =>
                    {
                        content.Spacing(15);

                        // Executive Summary Section
                        content.Item().Column(section =>
                        {
                            section.Item().Text("Executive Summary")
                                .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);
                            
                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Business Concept:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text(businessPlan.ExecutiveSummary.BusinessConcept)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Target Market:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text(businessPlan.ExecutiveSummary.TargetMarket)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                            });

                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Revenue Model:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text(businessPlan.ExecutiveSummary.RevenueModel)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Funding Required:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text($"{businessPlan.ExecutiveSummary.FundingRequired:C0}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                            });

                            section.Item().PaddingTop(10).Text(businessPlan.ExecutiveSummary.Description)
                                .FontSize(10).FontColor(Colors.Grey.Darken1).LineHeight(1.4f);
                        });

                        // Market Analysis Section
                        content.Item().Column(section =>
                        {
                            section.Item().Text("Market Analysis")
                                .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);
                            
                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Market Size:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text(businessPlan.MarketAnalysis.MarketSize)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Competition Level:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text(businessPlan.MarketAnalysis.CompetitionLevel)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Market Growth:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text(businessPlan.MarketAnalysis.MarketGrowth)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                            });

                            section.Item().PaddingTop(10).Text("Key Market Insights:")
                                .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                            
                            foreach (var insight in businessPlan.MarketAnalysis.KeyInsights.Take(4))
                            {
                                section.Item().Row(row =>
                                {
                                    row.ConstantItem(10).Text("•").FontSize(10).FontColor(Colors.Blue.Medium);
                                    row.RelativeItem().Text(insight).FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                            }
                        });

                        // Financial Projections Section
                        content.Item().Column(section =>
                        {
                            section.Item().Text("Financial Projections")
                                .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);
                            
                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Break-even Month:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text($"Month {businessPlan.FinancialProjections.BreakEvenMonth}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Initial Investment:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text($"{businessPlan.FinancialProjections.InitialInvestment:C0}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                            });

                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("5-Year ROI:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text($"{businessPlan.FinancialProjections.ProjectedROI:P1}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                                row.RelativeItem(1).Column(col =>
                                {
                                    col.Item().Text("Gross Margin:")
                                        .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    col.Item().Text($"{businessPlan.FinancialProjections.GrossMargin:P1}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                });
                            });

                            // 5-Year Revenue Projection Table
                            section.Item().PaddingTop(15).Text("Revenue Projections (5-Year)")
                                .FontSize(12).SemiBold().FontColor(Colors.Grey.Darken2);
                            
                            section.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5)
                                        .Text("Year").FontSize(9).SemiBold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5)
                                        .Text("Revenue").FontSize(9).SemiBold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5)
                                        .Text("Expenses").FontSize(9).SemiBold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5)
                                        .Text("Net Income").FontSize(9).SemiBold();
                                });

                                foreach (var projection in businessPlan.FinancialProjections.YearlyProjections.Take(5))
                                {
                                    table.Cell().Border(1).Padding(5)
                                        .Text($"Year {projection.Year}").FontSize(8);
                                    table.Cell().Border(1).Padding(5)
                                        .Text($"{projection.Revenue:C0}").FontSize(8);
                                    table.Cell().Border(1).Padding(5)
                                        .Text($"{projection.Expenses:C0}").FontSize(8);
                                    table.Cell().Border(1).Padding(5)
                                        .Text($"{projection.NetIncome:C0}").FontSize(8);
                                }
                            });
                        });

                        // Implementation Timeline Section
                        content.Item().Column(section =>
                        {
                            section.Item().Text("Implementation Timeline")
                                .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);
                            
                            foreach (var milestone in businessPlan.ImplementationPlan.Milestones.Take(4))
                            {
                                section.Item().PaddingTop(10).Column(milestoneCol =>
                                {
                                    milestoneCol.Item().Row(row =>
                                    {
                                        row.ConstantItem(15).Text("▶").FontSize(10).FontColor(Colors.Blue.Medium);
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text($"{milestone.Title} ({milestone.Timeline})")
                                                .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                            col.Item().Text(milestone.Description)
                                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                                        });
                                    });

                                    if (milestone.Tasks.Any())
                                    {
                                        milestoneCol.Item().PaddingLeft(20).PaddingTop(5).Column(tasksCol =>
                                        {
                                            foreach (var task in milestone.Tasks.Take(3))
                                            {
                                                tasksCol.Item().Row(taskRow =>
                                                {
                                                    taskRow.ConstantItem(10).Text("•").FontSize(8).FontColor(Colors.Grey.Medium);
                                                    taskRow.RelativeItem().Text(task).FontSize(8).FontColor(Colors.Grey.Darken1);
                                                });
                                            }
                                        });
                                    }
                                });
                            }
                        });

                        // Risk Analysis Section
                        content.Item().Column(section =>
                        {
                            section.Item().Text("Risk Analysis & Mitigation")
                                .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);
                            
                            foreach (var risk in businessPlan.RiskAnalysis.Risks.Take(4))
                            {
                                var riskColor = risk.Level.ToLower() switch
                                {
                                    "high" => Colors.Red.Lighten3,
                                    "medium" => Colors.Orange.Lighten3,
                                    "low" => Colors.Green.Lighten3,
                                    _ => Colors.Grey.Lighten3
                                };

                                section.Item().PaddingTop(10).Border(1).BorderColor(riskColor).Padding(8).Column(riskCol =>
                                {
                                    riskCol.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text(risk.Title)
                                            .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                        row.ConstantItem(50).AlignRight().Text(risk.Level.ToUpper())
                                            .FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                                    });
                                    
                                    riskCol.Item().PaddingTop(5).Text(risk.Description)
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                    
                                    riskCol.Item().PaddingTop(5).Row(mitigationRow =>
                                    {
                                        mitigationRow.ConstantItem(70).Text("Mitigation:")
                                            .FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
                                        mitigationRow.RelativeItem().Text(risk.MitigationStrategy)
                                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                                    });
                                });
                            }
                        });

                        // Success Factors Section
                        content.Item().Column(section =>
                        {
                            section.Item().Text("Key Success Factors")
                                .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);
                            
                            section.Item().PaddingTop(10).Column(factorsCol =>
                            {
                                foreach (var factor in businessPlan.SuccessFactors.Take(6))
                                {
                                    factorsCol.Item().Row(row =>
                                    {
                                        row.ConstantItem(15).Text("✓").FontSize(10).FontColor(Colors.Green.Medium);
                                        row.RelativeItem().Text(factor).FontSize(9).FontColor(Colors.Grey.Darken1);
                                    });
                                }
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text($"Ideas Matter - Business Plan | Page 1 | Generated {DateTime.Now:MM/dd/yyyy}")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();

        // Trigger download
        var fileName = $"Business_Plan_{scenario.Name?.Replace(" ", "_") ?? "Plan"}_{DateTime.Now:yyyyMMdd}.pdf";
        await DownloadPdfFile(pdfBytes, fileName);
    }

    private async Task DownloadPdfFile(byte[] pdfBytes, string fileName)
    {
        // This method would need to be implemented to handle file download
        // For now, we'll just save the fact that this was called
        await Task.CompletedTask;
        System.IO.File.WriteAllBytes($"/tmp/{fileName}", pdfBytes);
    }
}