using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services;

public class BusinessPlanService
{
    public async Task<BusinessPlan> GenerateBusinessPlanAsync(BusinessIdeaScenario scenario)
    {
        // Simulate async operation
        await Task.Delay(500);

        var businessPlan = new BusinessPlan
        {
            Id = Guid.NewGuid().ToString(),
            ScenarioId = scenario.Id,
            ExecutiveSummary = GenerateExecutiveSummary(scenario),
            MarketAnalysis = GenerateMarketAnalysis(scenario),
            FinancialProjections = GenerateFinancialProjections(scenario),
            ImplementationPlan = GenerateImplementationPlan(scenario),
            RiskAnalysis = GenerateRiskAnalysis(scenario),
            SuccessFactors = GenerateSuccessFactors(scenario),
            CreatedAt = DateTime.UtcNow
        };

        return businessPlan;
    }

    private ExecutiveSummary GenerateExecutiveSummary(BusinessIdeaScenario scenario)
    {
        return new ExecutiveSummary
        {
            BusinessConcept = scenario.Description,
            TargetMarket = scenario.TargetMarket,
            RevenueModel = GetRevenueModel(scenario),
            FundingRequired = scenario.EstimatedStartupCost,
            Description = GenerateExecutiveDescription(scenario)
        };
    }

    private string GenerateExecutiveDescription(BusinessIdeaScenario scenario)
    {
        return $"{scenario.Name} addresses a significant market opportunity in the {scenario.Industry} sector. " +
               $"With a viability score of {scenario.ViabilityScore}/100 and targeting {scenario.TargetMarket}, " +
               $"this business concept has strong potential for success. The projected revenue of {scenario.ProjectedRevenue:C0} " +
               $"demonstrates the scalability and market demand for this solution.";
    }

    private string GetRevenueModel(BusinessIdeaScenario scenario)
    {
        return scenario.Industry.ToLower() switch
        {
            "technology" or "software" => "Subscription-based SaaS model",
            "e-commerce" or "retail" => "Transaction-based revenue",
            "healthcare" => "Service-based billing",
            "education" => "Course and certification fees",
            "consulting" => "Hourly and project-based fees",
            "manufacturing" => "Product sales and licensing",
            "real estate" => "Commission and service fees",
            "food & beverage" => "Direct sales and franchising",
            "fitness" => "Membership and personal training fees",
            "financial services" => "Fee-based services",
            _ => "Multi-stream revenue model"
        };
    }

    private MarketAnalysis GenerateMarketAnalysis(BusinessIdeaScenario scenario)
    {
        var insights = new List<string>();
        
        // Generate market insights based on scenario data
        if (scenario.MarketSize > 80)
            insights.Add("Large addressable market with significant growth potential");
        else if (scenario.MarketSize > 60)
            insights.Add("Moderate market size with room for expansion");
        else
            insights.Add("Niche market with specialized customer base");

        if (scenario.CompetitionLevel == "Low")
            insights.Add("Limited competition provides first-mover advantage");
        else if (scenario.CompetitionLevel == "Medium")
            insights.Add("Moderate competition requires differentiation strategy");
        else
            insights.Add("High competition demands innovation and strong positioning");

        // Add industry-specific insights
        insights.AddRange(GetIndustryInsights(scenario.Industry));

        return new MarketAnalysis
        {
            MarketSize = GetMarketSizeDescription(scenario.MarketSize),
            CompetitionLevel = scenario.CompetitionLevel,
            MarketGrowth = GetMarketGrowth(scenario.Industry),
            KeyInsights = insights
        };
    }

    private List<string> GetIndustryInsights(string industry)
    {
        return industry.ToLower() switch
        {
            "technology" => new List<string>
            {
                "Digital transformation driving increased demand",
                "Remote work trends creating new opportunities",
                "AI and automation reshaping the landscape"
            },
            "healthcare" => new List<string>
            {
                "Aging population increasing healthcare needs",
                "Telemedicine adoption accelerating",
                "Focus on preventive care growing"
            },
            "e-commerce" => new List<string>
            {
                "Online shopping continues to grow",
                "Mobile commerce driving engagement",
                "Social commerce emerging as key channel"
            },
            "education" => new List<string>
            {
                "Online learning market expanding rapidly",
                "Skill-based training in high demand",
                "Corporate training budgets increasing"
            },
            _ => new List<string>
            {
                "Market trends favor innovative solutions",
                "Consumer preferences shifting toward quality",
                "Digital channels becoming increasingly important"
            }
        };
    }

    private string GetMarketSizeDescription(int marketSize)
    {
        return marketSize switch
        {
            >= 90 => "Massive market (>$10B)",
            >= 80 => "Large market ($1B-$10B)",
            >= 70 => "Significant market ($100M-$1B)",
            >= 60 => "Moderate market ($10M-$100M)",
            >= 50 => "Niche market ($1M-$10M)",
            _ => "Specialized market (<$1M)"
        };
    }

    private string GetMarketGrowth(string industry)
    {
        return industry.ToLower() switch
        {
            "technology" => "12-15% annually",
            "healthcare" => "8-12% annually",
            "e-commerce" => "10-14% annually",
            "education" => "7-10% annually",
            "fitness" => "8-10% annually",
            "food & beverage" => "5-8% annually",
            _ => "6-9% annually"
        };
    }

    private FinancialProjectionsSummary GenerateFinancialProjections(BusinessIdeaScenario scenario)
    {
        var yearlyProjections = new List<YearlyProjection>();
        
        // Generate 5-year projections
        for (int year = 1; year <= 5; year++)
        {
            var growthRate = GetGrowthRate(year, scenario.Industry);
            var revenue = year == 1 ? scenario.ProjectedRevenue : 
                         yearlyProjections[year - 2].Revenue * (1 + growthRate);
            
            yearlyProjections.Add(new YearlyProjection
            {
                Year = year,
                Revenue = revenue,
                Expenses = revenue * GetExpenseRatio(year),
                NetIncome = revenue * GetNetMarginRatio(year),
                GrowthRate = year == 1 ? 0 : growthRate
            });
        }

        var breakEvenMonth = CalculateBreakEvenMonth(scenario);
        var roiCalculation = CalculateROI(yearlyProjections, scenario.EstimatedStartupCost);

        return new FinancialProjectionsSummary
        {
            YearlyProjections = yearlyProjections,
            BreakEvenMonth = breakEvenMonth,
            InitialInvestment = scenario.EstimatedStartupCost,
            ProjectedROI = roiCalculation,
            GrossMargin = GetGrossMargin(scenario.Industry)
        };
    }

    private decimal GetGrowthRate(int year, string industry)
    {
        var baseRate = industry.ToLower() switch
        {
            "technology" => 0.25m,
            "healthcare" => 0.15m,
            "e-commerce" => 0.20m,
            "education" => 0.12m,
            _ => 0.15m
        };

        // Growth typically slows in later years
        return year switch
        {
            1 => baseRate,
            2 => baseRate * 0.8m,
            3 => baseRate * 0.6m,
            4 => baseRate * 0.4m,
            5 => baseRate * 0.3m,
            _ => baseRate
        };
    }

    private decimal GetExpenseRatio(int year)
    {
        // Expenses as percentage of revenue, typically decreasing over time due to economies of scale
        return year switch
        {
            1 => 0.85m,
            2 => 0.80m,
            3 => 0.75m,
            4 => 0.70m,
            5 => 0.65m,
            _ => 0.75m
        };
    }

    private decimal GetNetMarginRatio(int year)
    {
        return year switch
        {
            1 => 0.05m,
            2 => 0.10m,
            3 => 0.15m,
            4 => 0.20m,
            5 => 0.25m,
            _ => 0.15m
        };
    }

    private decimal CalculateBreakEvenMonth(BusinessIdeaScenario scenario)
    {
        // Simple break-even calculation based on startup cost and monthly revenue
        var monthlyRevenue = scenario.ProjectedRevenue / 12;
        var monthlyExpenses = monthlyRevenue * 0.7m; // Assume 70% expense ratio initially
        var monthlyProfit = monthlyRevenue - monthlyExpenses;
        
        if (monthlyProfit <= 0) return 24; // Default to 24 months if not profitable
        
        return Math.Max(3, Math.Min(24, scenario.EstimatedStartupCost / monthlyProfit));
    }

    private decimal CalculateROI(List<YearlyProjection> projections, decimal initialInvestment)
    {
        var totalNetIncome = projections.Sum(p => p.NetIncome);
        return initialInvestment > 0 ? (totalNetIncome - initialInvestment) / initialInvestment : 0;
    }

    private decimal GetGrossMargin(string industry)
    {
        return industry.ToLower() switch
        {
            "technology" => 0.75m,
            "software" => 0.80m,
            "healthcare" => 0.60m,
            "e-commerce" => 0.40m,
            "education" => 0.70m,
            "consulting" => 0.85m,
            "manufacturing" => 0.35m,
            "retail" => 0.30m,
            _ => 0.50m
        };
    }

    private ImplementationPlan GenerateImplementationPlan(BusinessIdeaScenario scenario)
    {
        var milestones = new List<Milestone>();

        // Phase 1: Foundation (Months 1-3)
        milestones.Add(new Milestone
        {
            Title = "Business Foundation & Setup",
            Timeline = "Months 1-3",
            Description = "Establish legal structure, secure initial funding, and set up core operations",
            Tasks = new List<string>
            {
                "Register business entity and obtain necessary licenses",
                "Secure initial funding and set up business banking",
                "Establish workspace and basic infrastructure",
                "Build core team and define roles"
            }
        });

        // Phase 2: Product/Service Development (Months 2-6)
        milestones.Add(new Milestone
        {
            Title = "Product/Service Development",
            Timeline = "Months 2-6",
            Description = "Develop minimum viable product and conduct initial testing",
            Tasks = GetDevelopmentTasks(scenario.Industry)
        });

        // Phase 3: Market Entry (Months 4-9)
        milestones.Add(new Milestone
        {
            Title = "Market Entry & Customer Acquisition",
            Timeline = "Months 4-9",
            Description = "Launch marketing campaigns and acquire first customers",
            Tasks = new List<string>
            {
                "Launch marketing website and digital presence",
                "Execute customer acquisition strategy",
                "Establish sales processes and customer support",
                "Gather customer feedback and iterate"
            }
        });

        // Phase 4: Scale & Optimize (Months 6-12)
        milestones.Add(new Milestone
        {
            Title = "Scale Operations & Optimize",
            Timeline = "Months 6-12",
            Description = "Scale operations, optimize processes, and expand market reach",
            Tasks = new List<string>
            {
                "Scale team and operations infrastructure",
                "Optimize product based on customer feedback",
                "Expand to additional market segments",
                "Implement performance tracking and analytics"
            }
        });

        return new ImplementationPlan
        {
            Milestones = milestones
        };
    }

    private List<string> GetDevelopmentTasks(string industry)
    {
        return industry.ToLower() switch
        {
            "technology" or "software" => new List<string>
            {
                "Develop minimum viable product (MVP)",
                "Set up development and testing environments",
                "Implement core features and user interface",
                "Conduct user testing and gather feedback"
            },
            "e-commerce" or "retail" => new List<string>
            {
                "Source initial product inventory",
                "Set up e-commerce platform and payment processing",
                "Develop supplier relationships",
                "Create product catalogs and descriptions"
            },
            "healthcare" => new List<string>
            {
                "Develop service protocols and procedures",
                "Obtain necessary certifications and approvals",
                "Set up clinical or service delivery systems",
                "Train staff on compliance and quality standards"
            },
            "consulting" => new List<string>
            {
                "Develop service methodologies and frameworks",
                "Create client engagement processes",
                "Build knowledge base and resource library",
                "Establish pricing and service packages"
            },
            _ => new List<string>
            {
                "Develop core product or service offering",
                "Create operational procedures and standards",
                "Build supplier and vendor relationships",
                "Establish quality control processes"
            }
        };
    }

    private RiskAnalysis GenerateRiskAnalysis(BusinessIdeaScenario scenario)
    {
        var risks = new List<BusinessRisk>();

        // Market risks
        if (scenario.CompetitionLevel == "High")
        {
            risks.Add(new BusinessRisk
            {
                Title = "Intense Competition",
                Level = "High",
                Description = "High competition may limit market share and pricing power",
                MitigationStrategy = "Focus on differentiation, superior customer service, and niche targeting"
            });
        }

        // Financial risks
        if (scenario.EstimatedStartupCost > 100000)
        {
            risks.Add(new BusinessRisk
            {
                Title = "High Capital Requirements",
                Level = "Medium",
                Description = "Significant upfront investment increases financial risk",
                MitigationStrategy = "Secure adequate funding, implement careful cash flow management, consider phased rollout"
            });
        }

        // Industry-specific risks
        risks.AddRange(GetIndustryRisks(scenario.Industry));

        // Add challenges from scenario as risks
        foreach (var challenge in scenario.KeyChallenges.Take(2))
        {
            risks.Add(new BusinessRisk
            {
                Title = "Operational Challenge",
                Level = "Medium",
                Description = challenge,
                MitigationStrategy = "Develop specific action plans and monitor progress regularly"
            });
        }

        return new RiskAnalysis
        {
            Risks = risks
        };
    }

    private List<BusinessRisk> GetIndustryRisks(string industry)
    {
        return industry.ToLower() switch
        {
            "technology" => new List<BusinessRisk>
            {
                new BusinessRisk
                {
                    Title = "Technology Obsolescence",
                    Level = "Medium",
                    Description = "Rapid technology changes may make current solutions outdated",
                    MitigationStrategy = "Continuous innovation, regular technology updates, stay informed on industry trends"
                }
            },
            "healthcare" => new List<BusinessRisk>
            {
                new BusinessRisk
                {
                    Title = "Regulatory Compliance",
                    Level = "High",
                    Description = "Healthcare regulations are complex and frequently changing",
                    MitigationStrategy = "Maintain compliance expertise, regular audits, strong legal counsel"
                }
            },
            "e-commerce" => new List<BusinessRisk>
            {
                new BusinessRisk
                {
                    Title = "Supply Chain Disruption",
                    Level = "Medium",
                    Description = "Dependence on suppliers and logistics partners creates vulnerability",
                    MitigationStrategy = "Diversify suppliers, maintain safety stock, develop contingency plans"
                }
            },
            _ => new List<BusinessRisk>
            {
                new BusinessRisk
                {
                    Title = "Market Demand Fluctuation",
                    Level = "Low",
                    Description = "Economic conditions may affect customer demand",
                    MitigationStrategy = "Monitor market conditions, maintain financial reserves, diversify offerings"
                }
            }
        };
    }

    private List<string> GenerateSuccessFactors(BusinessIdeaScenario scenario)
    {
        var factors = new List<string>(scenario.SuccessFactors);

        // Add industry-specific success factors
        factors.AddRange(GetIndustrySuccessFactors(scenario.Industry));

        // Add viability-based factors
        if (scenario.ViabilityScore > 80)
        {
            factors.Add("Strong market validation and customer demand demonstrated");
        }

        if (scenario.MarketSize > 70)
        {
            factors.Add("Large addressable market provides significant growth opportunity");
        }

        return factors.Take(8).ToList(); // Limit to 8 factors for readability
    }

    private List<string> GetIndustrySuccessFactors(string industry)
    {
        return industry.ToLower() switch
        {
            "technology" => new List<string>
            {
                "Strong technical team with relevant expertise",
                "Scalable technology architecture",
                "Focus on user experience and product design"
            },
            "healthcare" => new List<string>
            {
                "Regulatory compliance and quality standards",
                "Clinical expertise and credibility",
                "Strong patient safety and care protocols"
            },
            "e-commerce" => new List<string>
            {
                "Efficient supply chain and logistics",
                "Strong digital marketing and SEO",
                "Customer acquisition and retention strategies"
            },
            "education" => new List<string>
            {
                "High-quality content and curriculum",
                "Engaging learning experience design",
                "Strong instructor and expert network"
            },
            _ => new List<string>
            {
                "Strong execution and operational excellence",
                "Customer-focused approach and service quality",
                "Effective marketing and brand building"
            }
        };
    }
}

// Business Plan Models
public class BusinessPlan
{
    public string Id { get; set; } = "";
    public string ScenarioId { get; set; } = "";
    public ExecutiveSummary ExecutiveSummary { get; set; } = new();
    public MarketAnalysis MarketAnalysis { get; set; } = new();
    public FinancialProjectionsSummary FinancialProjections { get; set; } = new();
    public ImplementationPlan ImplementationPlan { get; set; } = new();
    public RiskAnalysis RiskAnalysis { get; set; } = new();
    public List<string> SuccessFactors { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ExecutiveSummary
{
    public string BusinessConcept { get; set; } = "";
    public string TargetMarket { get; set; } = "";
    public string RevenueModel { get; set; } = "";
    public decimal FundingRequired { get; set; }
    public string Description { get; set; } = "";
}

public class MarketAnalysis
{
    public string MarketSize { get; set; } = "";
    public string CompetitionLevel { get; set; } = "";
    public string MarketGrowth { get; set; } = "";
    public List<string> KeyInsights { get; set; } = new();
}

public class FinancialProjectionsSummary
{
    public List<YearlyProjection> YearlyProjections { get; set; } = new();
    public decimal BreakEvenMonth { get; set; }
    public decimal InitialInvestment { get; set; }
    public decimal ProjectedROI { get; set; }
    public decimal GrossMargin { get; set; }
}

public class YearlyProjection
{
    public int Year { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetIncome { get; set; }
    public decimal GrowthRate { get; set; }
}

public class ImplementationPlan
{
    public List<Milestone> Milestones { get; set; } = new();
}

public class Milestone
{
    public string Title { get; set; } = "";
    public string Timeline { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Tasks { get; set; } = new();
}

public class RiskAnalysis
{
    public List<BusinessRisk> Risks { get; set; } = new();
}

public class BusinessRisk
{
    public string Title { get; set; } = "";
    public string Level { get; set; } = "";
    public string Description { get; set; } = "";
    public string MitigationStrategy { get; set; } = "";
}