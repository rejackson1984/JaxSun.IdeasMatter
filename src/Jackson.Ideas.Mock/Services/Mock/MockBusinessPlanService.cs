using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Mock implementation of business plan service with realistic business planning logic
    /// </summary>
    public class MockBusinessPlanService : IBusinessPlanService
    {
        private readonly Random _random = new(42); // Fixed seed for consistent results
        
        public Task<BusinessPlanResult> GenerateBusinessPlanAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var result = new BusinessPlanResult
            {
                Request = request,
                BusinessCategory = DetermineBusinessCategory(request.BusinessIdea),
                ExecutiveSummary = GenerateExecutiveSummary(request),
                MarketAnalysis = GenerateMarketAnalysisInternal(request),
                CompetitiveAnalysis = GenerateCompetitiveAnalysisInternal(request),
                MarketingStrategy = GenerateMarketingStrategyInternal(request),
                OperationalPlan = GenerateOperationalPlanInternal(request),
                FinancialProjections = GenerateFinancialProjectionsInternal(request),
                RiskAssessment = GenerateRiskAssessmentInternal(request),
                FundingRequirements = GenerateFundingRequirements(request),
                ViabilityScore = CalculateViabilityScore(request),
                Recommendations = GenerateRecommendations(request)
            };
            
            return Task.FromResult(result);
        }
        
        public Task<Jackson.Ideas.Mock.Models.FinancialProjections> GenerateFinancialProjectionsAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return Task.FromResult(GenerateFinancialProjectionsInternal(request));
        }
        
        public Task<Jackson.Ideas.Mock.Models.MarketingStrategy> GenerateMarketingStrategyAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return Task.FromResult(GenerateMarketingStrategyInternal(request));
        }
        
        public Task<Jackson.Ideas.Mock.Models.OperationalPlan> GenerateOperationalPlanAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return Task.FromResult(GenerateOperationalPlanInternal(request));
        }
        
        public Task<Jackson.Ideas.Mock.Models.RiskAssessment> GenerateRiskAssessmentAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return Task.FromResult(GenerateRiskAssessmentInternal(request));
        }
        
        public Task<int> CalculateBusinessViabilityScoreAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return Task.FromResult(CalculateViabilityScore(request));
        }
        
        public Task<bool> IsReadyForOperationsHubAsync(BusinessPlanResult businessPlan)
        {
            // Need 80% or higher viability score to advance to operations
            return Task.FromResult(businessPlan.ViabilityScore >= 80);
        }
        
        public Task<List<Jackson.Ideas.Mock.Models.BusinessPlanTemplate>> GetBusinessPlanTemplatesAsync()
        {
            var templates = new List<BusinessPlanTemplate>
            {
                new BusinessPlanTemplate()
                {
                    Name = "Technology Startup",
                    Industry = "Technology",
                    Description = "Comprehensive template for software and tech businesses",
                    Sections = new List<string> { "Executive Summary", "Market Analysis", "Product Development", "Go-to-Market", "Financial Projections", "Team & Operations" },
                    TemplateType = "Comprehensive"
                },
                new()
                {
                    Name = "Service Business",
                    Industry = "Services",
                    Description = "Template for service-based businesses and consultancies",
                    Sections = new List<string> { "Executive Summary", "Service Offering", "Market Analysis", "Operations Plan", "Marketing Strategy", "Financial Projections" },
                    TemplateType = "Standard"
                },
                new()
                {
                    Name = "E-commerce Business",
                    Industry = "Retail",
                    Description = "Template for online retail and e-commerce ventures",
                    Sections = new List<string> { "Executive Summary", "Product Strategy", "Market Analysis", "Supply Chain", "Digital Marketing", "Financial Projections" },
                    TemplateType = "Comprehensive"
                },
                new()
                {
                    Name = "Food & Restaurant",
                    Industry = "Food Service",
                    Description = "Template for restaurants, food trucks, and food service businesses",
                    Sections = new List<string> { "Executive Summary", "Concept & Menu", "Market Analysis", "Location Strategy", "Operations", "Financial Projections" },
                    TemplateType = "Industry-Specific"
                },
                new()
                {
                    Name = "Health & Wellness",
                    Industry = "Healthcare",
                    Description = "Template for health, wellness, and medical service businesses",
                    Sections = new List<string> { "Executive Summary", "Service Overview", "Regulatory Compliance", "Market Analysis", "Operations", "Financial Projections" },
                    TemplateType = "Regulated"
                }
            };
            
            return Task.FromResult(templates);
        }
        
        public Task<List<FundingOption>> GetFundingOptionsAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var fundingOptions = new List<FundingOption>
            {
                new()
                {
                    FundingType = "Bootstrapping",
                    Description = "Self-funding using personal savings and early revenue",
                    TypicalAmountRange = "$0 - $50,000",
                    Requirements = new List<string> { "Personal savings", "Early revenue generation" },
                    Advantages = new List<string> { "Full control", "No debt", "Proven demand" },
                    Disadvantages = new List<string> { "Limited capital", "Slower growth", "Personal financial risk" },
                    SuitabilityScore = CalculateFundingSuitability("bootstrapping", request)
                },
                new()
                {
                    FundingType = "Friends & Family",
                    Description = "Initial funding from personal network",
                    TypicalAmountRange = "$5,000 - $100,000",
                    Requirements = new List<string> { "Personal network", "Basic business plan", "Trust and relationships" },
                    Advantages = new List<string> { "Flexible terms", "Quick access", "Supportive investors" },
                    Disadvantages = new List<string> { "Limited amounts", "Personal relationships at risk", "Less expertise" },
                    SuitabilityScore = CalculateFundingSuitability("friends_family", request)
                },
                new()
                {
                    FundingType = "Angel Investors",
                    Description = "Individual investors providing capital and mentorship",
                    TypicalAmountRange = "$25,000 - $500,000",
                    Requirements = new List<string> { "Solid business plan", "Prototype or MVP", "Clear market opportunity" },
                    Advantages = new List<string> { "Expertise and mentorship", "Network access", "Strategic guidance" },
                    Disadvantages = new List<string> { "Equity dilution", "Investor involvement", "Due diligence required" },
                    SuitabilityScore = CalculateFundingSuitability("angel", request)
                },
                new()
                {
                    FundingType = "Small Business Loan",
                    Description = "Traditional bank or SBA loans for established businesses",
                    TypicalAmountRange = "$50,000 - $2,000,000",
                    Requirements = new List<string> { "Good credit score", "Business plan", "Collateral", "Revenue history" },
                    Advantages = new List<string> { "No equity dilution", "Tax-deductible interest", "Build credit history" },
                    Disadvantages = new List<string> { "Personal guarantees", "Regular payments", "Strict requirements" },
                    SuitabilityScore = CalculateFundingSuitability("loan", request)
                },
                new()
                {
                    FundingType = "Venture Capital",
                    Description = "Professional investors for high-growth potential businesses",
                    TypicalAmountRange = "$1,000,000+",
                    Requirements = new List<string> { "Scalable business model", "Large market opportunity", "Experienced team", "Traction metrics" },
                    Advantages = new List<string> { "Large funding amounts", "Professional expertise", "Strategic partnerships" },
                    Disadvantages = new List<string> { "Significant equity dilution", "Loss of control", "High growth pressure" },
                    SuitabilityScore = CalculateFundingSuitability("vc", request)
                }
            };
            
            return Task.FromResult(fundingOptions.OrderByDescending(f => f.SuitabilityScore).ToList());
        }
        
        public Task<BusinessModelValidation> ValidateBusinessModelAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var viabilityScore = CalculateViabilityScore(request);
            
            var validation = new BusinessModelValidation
            {
                IsViable = viabilityScore >= 60,
                ViabilityScore = viabilityScore,
                Strengths = GenerateModelStrengths(request, viabilityScore),
                Weaknesses = GenerateModelWeaknesses(request, viabilityScore),
                Recommendations = GenerateModelRecommendations(request, viabilityScore),
                KeyMetrics = GenerateKeyMetrics(request)
            };
            
            return Task.FromResult(validation);
        }
        
        public Task<Jackson.Ideas.Mock.Models.CompetitiveAnalysis> GenerateCompetitiveAnalysisAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return Task.FromResult(GenerateCompetitiveAnalysisInternal(request));
        }
        
        public Task<Jackson.Ideas.Mock.Models.ImplementationTimeline> GenerateTimelineAsync(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var phases = GenerateImplementationPhases(request);
            
            var timeline = new ImplementationTimeline
            {
                Phases = phases,
                TotalDurationWeeks = phases.Sum(p => p.DurationWeeks),
                KeyMilestones = phases.SelectMany(p => p.Deliverables).Take(5).ToList(),
                CriticalPath = string.Join(" → ", phases.Select(p => p.Name)),
                ResourceAllocation = "Standard resource allocation across phases",
                RiskFactors = "Timeline and resource risks identified"
            };
            
            return Task.FromResult(timeline);
        }
        
        private string DetermineBusinessCategory(string businessIdea)
        {
            var idea = businessIdea.ToLower();
            
            if (idea.Contains("app") || idea.Contains("software") || idea.Contains("platform") || idea.Contains("ai"))
                return "Technology";
            if (idea.Contains("food") || idea.Contains("restaurant") || idea.Contains("meal") || idea.Contains("dining"))
                return "Food Service";
            if (idea.Contains("health") || idea.Contains("fitness") || idea.Contains("medical") || idea.Contains("wellness"))
                return "Healthcare";
            if (idea.Contains("education") || idea.Contains("learning") || idea.Contains("course") || idea.Contains("training"))
                return "Education";
            if (idea.Contains("retail") || idea.Contains("store") || idea.Contains("shop") || idea.Contains("product"))
                return "Retail";
            if (idea.Contains("service") || idea.Contains("consulting") || idea.Contains("support"))
                return "Professional Services";
            
            return "General Business";
        }
        
        private string GenerateExecutiveSummary(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return $"This business plan outlines the development and launch of {request.BusinessIdea}. " +
                   $"The venture addresses {request.ProblemSolved} for {request.TargetMarket}. " +
                   $"Operating under a {request.RevenueModel} model, the business leverages {request.CompetitiveAdvantages} " +
                   $"to capture market share in the {DetermineBusinessCategory(request.BusinessIdea).ToLower()} sector. " +
                   $"With an initial investment requirement of ${request.InitialInvestment:N0}, the business is projected to " +
                   $"achieve profitability within 18-24 months and scale to significant market presence.";
        }
        
        private MarketAnalysis GenerateMarketAnalysisInternal(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var marketSizes = new[] { "$50M", "$150M", "$500M", "$1.2B", "$3.5B", "$8.7B" };
            var growthRates = new[] { "8%", "12%", "18%", "25%", "35%" };
            
            return new MarketAnalysis
            {
                MarketSize = marketSizes[_random.Next(marketSizes.Length)],
                GrowthRate = growthRates[_random.Next(growthRates.Length)],
                TargetSegments = GenerateTargetSegments(request),
                MarketTrends = GenerateMarketTrends(request),
                KeyDrivers = GenerateMarketDrivers(request),
                MarketBarriers = GenerateMarketBarriers(request)
            };
        }
        
        private Jackson.Ideas.Mock.Models.CompetitiveAnalysis GenerateCompetitiveAnalysisInternal(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new Jackson.Ideas.Mock.Models.CompetitiveAnalysis
            {
                DirectCompetitors = GenerateDirectCompetitors(request),
                IndirectCompetitors = GenerateIndirectCompetitors(request),
                MarketPositioning = GenerateMarketPositioning(request),
                CompetitiveAdvantages = request.CompetitiveAdvantages.Split(',').Select(a => a.Trim()).ToList(),
                CompetitiveThreats = GenerateCompetitiveThreats(request),
                MarketGaps = GenerateMarketGaps(request),
                DifferentiationStrategy = GenerateCompetitiveStrategy(request),
                CompetitiveIntensity = "Medium"
            };
        }
        
        private Jackson.Ideas.Mock.Models.MarketingStrategy GenerateMarketingStrategyInternal(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new Jackson.Ideas.Mock.Models.MarketingStrategy
            {
                TargetMarketSegments = GenerateTargetSegments(request),
                ValueProposition = GenerateValueProposition(request),
                MarketingChannels = GenerateMarketingChannels(request),
                CustomerAcquisitionStrategy = GenerateAcquisitionStrategy(request),
                BrandingStrategy = GenerateBrandingStrategy(request),
                PricingStrategy = GeneratePricingStrategyString(request),
                CustomerRetentionStrategy = GenerateRetentionStrategy(request),
                MarketingBudget = CalculateMarketingBudget(request)
            };
        }
        
        private Jackson.Ideas.Mock.Models.OperationalPlan GenerateOperationalPlanInternal(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new Jackson.Ideas.Mock.Models.OperationalPlan
            {
                BusinessModel = GenerateBusinessModel(request),
                OperationalStructure = GenerateOperationalStructure(request),
                TechnologyRequirements = GenerateTechnologyRequirements(request),
                StaffingPlan = GenerateStaffingPlan(request),
                SupplyChainManagement = string.Join("; ", GenerateSupplyChainStrategy(request)),
                QualityControl = string.Join("; ", GenerateQualityProcesses(request)),
                ScalingStrategy = GenerateScalingStrategy(request)
            };
        }
        
        private Jackson.Ideas.Mock.Models.FinancialProjections GenerateFinancialProjectionsInternal(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var hasRecurring = request.RevenueModel.ToLower().Contains("subscription") || 
                              request.RevenueModel.ToLower().Contains("saas") ||
                              request.RevenueModel.ToLower().Contains("recurring");
            
            var revenueProjections = GenerateRevenueProjections(request, hasRecurring);
            var expenseProjections = GenerateExpenseProjections(request);
            
            return new Jackson.Ideas.Mock.Models.FinancialProjections
            {
                Revenue = new Jackson.Ideas.Mock.Models.RevenueProjections(),
                Expenses = new Jackson.Ideas.Mock.Models.ExpenseProjections(),
                CashFlow = new Jackson.Ideas.Mock.Models.CashFlowProjections(),
                YearlyBreakdown = new List<Jackson.Ideas.Mock.Models.YearlyFinancials>(),
                Metrics = new Jackson.Ideas.Mock.Models.FinancialMetrics(),
                Funding = new FundingRequirements()
            };
        }
        
        private Jackson.Ideas.Mock.Models.RiskAssessment GenerateRiskAssessmentInternal(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var riskLevels = new[] { "Low", "Medium", "High" };
            
            return new Jackson.Ideas.Mock.Models.RiskAssessment
            {
                MarketRisks = GenerateMarketRisks(request),
                CompetitiveRisks = GenerateCompetitiveRisks(request),
                FinancialRisks = GenerateFinancialRisks(request),
                OperationalRisks = GenerateOperationalRisks(request),
                TechnicalRisks = GenerateTechnologyRisks(request),
                RegulatoryRisks = GenerateRegulatoryRisks(request),
                MitigationStrategies = GenerateMitigationStrategies(request),
                OverallRiskLevel = riskLevels[_random.Next(riskLevels.Length)]
            };
        }
        
        private FundingRequirements GenerateFundingRequirements(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var totalFunding = request.InitialInvestment * (decimal)(1.2 + (_random.NextDouble() * 0.8)); // 20-100% buffer
            
            return new FundingRequirements
            {
                TotalFundingNeeded = totalFunding,
                StartupCosts = totalFunding * 0.4m,
                WorkingCapital = totalFunding * 0.3m,
                GrowthCapital = totalFunding * 0.3m,
                RecommendedOptions = new List<FundingOption>(),
                FundingStrategy = GenerateFundingStrategy(request),
                UseOfFunds = GenerateUseOfFunds(request),
                FundingMilestones = GenerateFundingMilestones(request),
                InvestorTargeting = "Angel investors and VCs",
                EquityDilution = 0.2m,
                ExitStrategy = "Strategic acquisition or IPO"
            };
        }
        
        private int CalculateViabilityScore(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            int score = 50; // Base score
            
            // Market opportunity score
            if (!string.IsNullOrWhiteSpace(request.TargetMarket) && request.TargetMarket.Length > 20)
                score += 15;
            
            // Problem clarity score
            if (!string.IsNullOrWhiteSpace(request.ProblemSolved) && request.ProblemSolved.Length > 30)
                score += 15;
            
            // Revenue model score
            if (!string.IsNullOrWhiteSpace(request.RevenueModel))
            {
                if (request.RevenueModel.ToLower().Contains("subscription") || request.RevenueModel.ToLower().Contains("recurring"))
                    score += 20;
                else if (request.RevenueModel.Contains("$"))
                    score += 15;
                else
                    score += 10;
            }
            
            // Investment appropriateness
            if (request.InitialInvestment > 0 && request.InitialInvestment < 1000000)
                score += 10;
            else if (request.InitialInvestment >= 1000000)
                score += 5;
            
            // Competitive advantages
            if (!string.IsNullOrWhiteSpace(request.CompetitiveAdvantages))
                score += 10;
            
            // Business category bonus
            var category = DetermineBusinessCategory(request.BusinessIdea);
            if (category == "Technology")
                score += 5;
            
            // Random market factor
            score += _random.Next(-5, 10);
            
            return Math.Clamp(score, 30, 95);
        }
        
        private List<string> GenerateRecommendations(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var recommendations = new List<string>();
            var viabilityScore = CalculateViabilityScore(request);
            
            if (viabilityScore >= 80)
            {
                recommendations.Add("Your business plan shows strong potential - consider advancing to operational planning");
                recommendations.Add("Focus on customer acquisition and market validation strategies");
                recommendations.Add("Explore funding options to accelerate growth");
            }
            else if (viabilityScore >= 60)
            {
                recommendations.Add("Solid foundation - refine your financial projections and market analysis");
                recommendations.Add("Conduct additional competitive research to strengthen positioning");
                recommendations.Add("Consider pilot testing your business model");
            }
            else
            {
                recommendations.Add("Focus on strengthening your value proposition and target market definition");
                recommendations.Add("Reassess your revenue model and pricing strategy");
                recommendations.Add("Consider pivoting or refining your core business concept");
            }
            
            return recommendations;
        }
        
        // Helper methods for generating specific plan sections
        private List<string> GenerateTargetSegments(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                $"Primary: {request.TargetMarket}",
                "Secondary: Adjacent market segment with similar needs",
                "Tertiary: Expanded market opportunity for future growth"
            };
        }
        
        private string GenerateMarketTrends(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            return category switch
            {
                "Technology" => "Digital transformation acceleration, AI adoption, remote work normalization",
                "Food Service" => "Health-conscious dining, delivery optimization, sustainable sourcing",
                "Healthcare" => "Telehealth expansion, preventive care focus, personalized medicine",
                "Education" => "Online learning growth, skill-based education, lifelong learning",
                _ => "Market digitization, customer experience focus, sustainability emphasis"
            };
        }
        
        private List<string> GenerateMarketDrivers(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Increasing customer demand for efficient solutions",
                "Technology advancement enabling new capabilities",
                "Market gap in current solution offerings",
                "Economic factors supporting business growth"
            };
        }
        
        private List<string> GenerateMarketBarriers(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Initial customer acquisition costs",
                "Market education requirements",
                "Competitive response from established players",
                "Regulatory or compliance considerations"
            };
        }
        
        private List<Jackson.Ideas.Mock.Models.Competitor> GenerateDirectCompetitors(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            var competitors = new List<Jackson.Ideas.Mock.Models.Competitor>();
            
            for (int i = 0; i < 3; i++)
            {
                competitors.Add(new Jackson.Ideas.Mock.Models.Competitor
                {
                    Name = $"{category} Leader {i + 1}",
                    Description = $"Established player in {category.ToLower()} market",
                    Strengths = new List<string> { "Market presence", "Brand recognition", "Resources" },
                    Weaknesses = new List<string> { "Legacy systems", "Slower innovation", "High costs" },
                    MarketShare = $"{_random.Next(15, 35)}%",
                    IsDirect = true
                });
            }
            
            return competitors;
        }
        
        private List<Jackson.Ideas.Mock.Models.Competitor> GenerateIndirectCompetitors(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var competitors = new List<Jackson.Ideas.Mock.Models.Competitor>();
            
            for (int i = 0; i < 2; i++)
            {
                competitors.Add(new Jackson.Ideas.Mock.Models.Competitor
                {
                    Name = $"Alternative Solution {i + 1}",
                    Description = "Alternative approach to solving similar customer problems",
                    Strengths = new List<string> { "Different market focus", "Established user base" },
                    Weaknesses = new List<string> { "Not directly focused on target problem" },
                    MarketShare = $"{_random.Next(5, 20)}%",
                    IsDirect = false
                });
            }
            
            return competitors;
        }
        
        private string GenerateMarketPositioning(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return $"Position as the premium solution for {request.TargetMarket} seeking {request.ProblemSolved.ToLower()}. " +
                   $"Differentiate through {request.CompetitiveAdvantages} while maintaining competitive pricing.";
        }
        
        private List<string> GenerateCompetitiveThreats(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "New entrants with significant funding",
                "Existing competitors expanding into our market",
                "Technology shifts making current approach obsolete",
                "Customer switching to free or lower-cost alternatives"
            };
        }
        
        private List<string> GenerateMarketGaps(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Underserved customer segments in current market",
                "Feature gaps in existing competitive solutions",
                "Price points not addressed by current offerings",
                "Geographic or demographic markets with limited coverage"
            };
        }
        
        private string GenerateCompetitiveStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Focus on rapid customer acquisition through superior user experience and targeted marketing. " +
                   "Build defensible competitive advantages through technology, partnerships, and customer loyalty. " +
                   "Monitor competitive responses and adapt strategy to maintain market position.";
        }
        
        private string GenerateValueProposition(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return $"We help {request.TargetMarket} {request.ProblemSolved.ToLower()} through our innovative approach. " +
                   $"Unlike alternatives, we provide {request.CompetitiveAdvantages} at an accessible price point.";
        }
        
        private List<string> GenerateMarketingChannels(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            
            var channels = new List<string> { "Digital marketing (SEO, PPC, social media)", "Content marketing and thought leadership" };
            
            if (category == "Technology")
            {
                channels.AddRange(new[] { "Product hunt launches", "Developer community engagement", "Partner integrations" });
            }
            else if (category == "Food Service")
            {
                channels.AddRange(new[] { "Local community events", "Food blogger partnerships", "Delivery platform presence" });
            }
            else
            {
                channels.AddRange(new[] { "Industry trade shows", "Professional networking", "Referral programs" });
            }
            
            return channels;
        }
        
        private string GenerateAcquisitionStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Multi-channel approach focusing on digital marketing for cost-effective customer acquisition. " +
                   "Implement referral programs to leverage satisfied customers. " +
                   "Strategic partnerships to access complementary customer bases.";
        }
        
        private BrandingStrategy GenerateBrandingStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new BrandingStrategy
            {
                BrandPosition = "Innovative, reliable solution provider",
                BrandPersonality = "Professional, approachable, results-driven",
                BrandValues = new List<string> { "Customer success", "Innovation", "Transparency", "Quality" },
                VisualIdentity = "Modern, clean design with strong color palette reflecting trustworthiness and innovation"
            };
        }
        
        private string GeneratePricingStrategyString(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var isSubscription = request.RevenueModel.ToLower().Contains("subscription");
            
            if (isSubscription)
            {
                return "Subscription-based pricing: Basic ($29.99/month), Professional ($79.99/month), Enterprise (Custom). Pricing reflects value delivered and competitive positioning.";
            }
            else
            {
                return "Value-based pricing: Standard ($199.99), Premium ($399.99), Enterprise ($999.99). Pricing reflects value delivered and competitive positioning.";
            }
        }
        
        private decimal CalculateMarketingBudget(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return request.InitialInvestment * 0.3m; // 30% of initial investment for marketing
        }
        
        private string GenerateBusinessModel(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            return category switch
            {
                "Technology" => $"Software-as-a-Service (SaaS) model with {request.RevenueModel}",
                "Food Service" => $"Direct-to-consumer model with {request.RevenueModel}",
                "Healthcare" => $"Service-based model with {request.RevenueModel}",
                _ => $"Business-to-business model with {request.RevenueModel}"
            };
        }
        
        private string GenerateOperationalStructure(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Lean startup structure with core team handling key functions. " +
                   "Outsource non-core activities initially, bring in-house as scale increases. " +
                   "Remote-first approach for talent acquisition flexibility.";
        }
        
        private List<string> GenerateTechnologyRequirements(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            
            if (category == "Technology")
            {
                return new List<string>
                {
                    "Cloud infrastructure (AWS/Azure/GCP)",
                    "Development frameworks and databases",
                    "Security and compliance tools",
                    "Analytics and monitoring systems",
                    "Customer support and communication tools"
                };
            }
            
            return new List<string>
            {
                "Customer relationship management (CRM) system",
                "Financial management and accounting software",
                "Communication and collaboration tools",
                "Basic website and e-commerce platform",
                "Security and backup solutions"
            };
        }
        
        private StaffingPlan GenerateStaffingPlan(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var estimatedSalary = request.InitialInvestment * 0.4m; // 40% for salaries
            
            return new StaffingPlan
            {
                KeyRoles = new List<string> { "Founder/CEO", "Operations Manager", "Marketing Specialist", "Customer Success" },
                HiringTimeline = string.Join("; ", new List<string> 
                { 
                    "Months 1-3: Core founding team",
                    "Months 4-6: First employee (operations/marketing)",
                    "Months 7-12: Scale based on revenue growth",
                    "Year 2+: Specialized roles and management"
                }),
                StaffingBudget = estimatedSalary,
                OrganizationalStructure = "Flat structure initially, evolving to functional teams as company grows"
            };
        }
        
        private string GenerateSupplyChainStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            
            return category switch
            {
                "Technology" => "Digital-first approach with cloud service providers. Partner with established SaaS tools for non-core functions.",
                "Food Service" => "Local supplier relationships for freshness and community support. Backup suppliers for reliability.",
                "Retail" => "Diversified supplier base to ensure inventory availability. Direct manufacturer relationships where possible.",
                _ => "Lean supply chain focused on essential services and strategic partnerships."
            };
        }
        
        private List<string> GenerateQualityProcesses(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Regular customer feedback collection and analysis",
                "Quality assurance testing and review processes",
                "Continuous improvement methodology implementation",
                "Performance metrics tracking and optimization",
                "Customer satisfaction monitoring and response"
            };
        }
        
        private string GenerateScalingStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Scale operations based on validated customer demand and market feedback. " +
                   "Prioritize systems and processes that can handle 10x growth. " +
                   "Geographic expansion after establishing strong local presence.";
        }
        
        private List<YearlyProjection> GenerateRevenueProjections(Jackson.Ideas.Mock.Models.BusinessPlanRequest request, bool hasRecurring)
        {
            var baseRevenue = request.InitialInvestment * 0.5m; // Conservative start
            var growthMultiplier = hasRecurring ? 2.5 : 1.8; // Recurring revenue grows faster
            
            return new List<YearlyProjection>
            {
                new() { Year = 1, Revenue = baseRevenue, Expenses = baseRevenue * 0.8m, NetIncome = baseRevenue * 0.2m, GrowthRate = 0.0m },
                new() { Year = 2, Revenue = baseRevenue * (decimal)growthMultiplier, Expenses = baseRevenue * (decimal)growthMultiplier * 0.75m, NetIncome = baseRevenue * (decimal)growthMultiplier * 0.25m, GrowthRate = (decimal)((growthMultiplier - 1) * 100) },
                new() { Year = 3, Revenue = baseRevenue * (decimal)(growthMultiplier * 1.6), Expenses = baseRevenue * (decimal)(growthMultiplier * 1.6) * 0.70m, NetIncome = baseRevenue * (decimal)(growthMultiplier * 1.6) * 0.30m, GrowthRate = 60.0m }
            };
        }
        
        private List<YearlyProjection> GenerateExpenseProjections(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var baseExpenses = request.InitialInvestment * 0.8m; // Higher initial costs
            
            return new List<YearlyProjection>
            {
                new() { Year = 1, Revenue = 0, Expenses = baseExpenses, NetIncome = -baseExpenses, GrowthRate = 0.0m },
                new() { Year = 2, Revenue = 0, Expenses = baseExpenses * 0.9m, NetIncome = -baseExpenses * 0.9m, GrowthRate = -10.0m },
                new() { Year = 3, Revenue = 0, Expenses = baseExpenses * 1.2m, NetIncome = -baseExpenses * 1.2m, GrowthRate = 33.3m }
            };
        }
        
        private ProfitabilityAnalysis GenerateProfitabilityAnalysis(List<YearlyProjection> revenue, List<YearlyProjection> expenses)
        {
            var year3Revenue = revenue.First(r => r.Year == 3).Revenue;
            var year3Expenses = expenses.First(e => e.Year == 3).Expenses;
            
            return new ProfitabilityAnalysis
            {
                GrossMarginPercentage = 65m, // Typical for service/software businesses
                NetMarginPercentage = ((year3Revenue - year3Expenses) / year3Revenue) * 100m,
                OperatingMarginPercentage = 45m,
                RevenueDrivers = new List<string> { "Customer acquisition efficiency", "Premium pricing power", "Market expansion" },
                CostDrivers = new List<string> { "Technology infrastructure", "Customer support", "Marketing spend" },
                ProfitabilityTimeline = "Break-even expected in month 14-18, sustained profitability by year 2",
                ScenarioAnalysis = "Best case: 15% higher margins, Base case: projected margins, Worst case: 20% lower margins"
            };
        }
        
        private BreakEvenAnalysis GenerateBreakEvenAnalysis(List<YearlyProjection> revenue, List<YearlyProjection> expenses)
        {
            var monthlyRevenue = revenue.First().Revenue / 12m;
            var monthlyExpenses = expenses.First().Expenses / 12m;
            var breakEvenMonth = (int)Math.Ceiling(12m * (monthlyExpenses / monthlyRevenue));
            
            return new BreakEvenAnalysis
            {
                BreakEvenPointMonths = Math.Min(breakEvenMonth, 24), // Cap at 24 months
                BreakEvenRevenue = monthlyExpenses * breakEvenMonth,
                BreakEvenCustomerCount = 100, // Simplified assumption
                FixedCosts = monthlyExpenses * 12,
                VariableCostPerUnit = 50m,
                ContributionMargin = 200m,
                SensitivityAnalysis = "Break-even sensitive to customer acquisition costs and pricing changes"
            };
        }
        
        private List<string> GenerateCashFlowProjections(List<YearlyProjection> revenue, List<YearlyProjection> expenses)
        {
            return new List<string>
            {
                "Year 1: Negative cash flow due to initial investments and market entry costs",
                "Year 2: Improving cash flow as revenue scales and operational efficiency increases",
                "Year 3: Positive cash flow with reinvestment in growth opportunities"
            };
        }
        
        private List<string> GenerateFinancialAssumptions(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Customer acquisition cost decreases over time through referrals and brand recognition",
                "Market demand remains stable with projected growth rates",
                "Operational costs scale efficiently with revenue growth",
                "No major economic disruptions or regulatory changes",
                "Competitive landscape remains relatively stable"
            };
        }
        
        private List<string> GenerateMarketRisks(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Market demand lower than projected",
                "Economic downturns affecting customer spending",
                "Market saturation from new competitors",
                "Changes in customer preferences or behavior"
            };
        }
        
        private List<string> GenerateCompetitiveRisks(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Large competitors entering market with superior resources",
                "Price wars reducing profit margins",
                "Intellectual property disputes or patent issues",
                "Loss of key competitive advantages"
            };
        }
        
        private List<string> GenerateFinancialRisks(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Higher than expected customer acquisition costs",
                "Difficulty securing additional funding rounds",
                "Cash flow management challenges",
                "Currency fluctuations (if applicable)"
            };
        }
        
        private List<string> GenerateOperationalRisks(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Key personnel departure or unavailability",
                "Supply chain disruptions",
                "Quality control issues affecting reputation",
                "Scaling challenges with rapid growth"
            };
        }
        
        private List<string> GenerateTechnologyRisks(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            
            if (category == "Technology")
            {
                return new List<string>
                {
                    "Technology platform failures or security breaches",
                    "Rapid technology changes making platform obsolete",
                    "Data privacy and compliance issues",
                    "Integration challenges with third-party services"
                };
            }
            
            return new List<string>
            {
                "System reliability and data security concerns",
                "Technology vendor dependencies",
                "Digital transformation challenges",
                "Cybersecurity threats"
            };
        }
        
        private List<string> GenerateRegulatoryRisks(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var category = DetermineBusinessCategory(request.BusinessIdea);
            
            return category switch
            {
                "Healthcare" => new List<string> { "HIPAA compliance requirements", "FDA regulatory changes", "State licensing requirements", "Insurance and liability regulations" },
                "Food Service" => new List<string> { "Food safety regulations", "Health department compliance", "Labor law requirements", "Environmental regulations" },
                "Technology" => new List<string> { "Data privacy regulations (GDPR, CCPA)", "Content moderation requirements", "International compliance issues", "Platform regulation changes" },
                _ => new List<string> { "Industry-specific compliance requirements", "Tax and business license regulations", "Consumer protection laws", "Employment and labor regulations" }
            };
        }
        
        private List<string> GenerateMitigationStrategies(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Diversify customer base to reduce concentration risk",
                "Maintain adequate cash reserves for unexpected challenges",
                "Build strong vendor and partner relationships",
                "Invest in continuous market research and competitive intelligence",
                "Develop contingency plans for major risk scenarios",
                "Secure appropriate insurance coverage",
                "Implement robust financial controls and monitoring systems"
            };
        }
        
        private List<string> GenerateFundingBreakdown(decimal totalFunding)
        {
            return new List<string>
            {
                $"Operations and salaries: ${totalFunding * 0.4m:N0} (40%)",
                $"Marketing and customer acquisition: ${totalFunding * 0.3m:N0} (30%)",
                $"Technology and infrastructure: ${totalFunding * 0.15m:N0} (15%)",
                $"Working capital and reserves: ${totalFunding * 0.15m:N0} (15%)"
            };
        }
        
        private string GenerateFundingTimeline(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Initial funding needed within 3 months to begin operations. " +
                   "Additional funding rounds anticipated at 12-month intervals based on growth milestones. " +
                   "Series A funding targeted for month 18-24 to accelerate scaling.";
        }
        
        private List<string> GenerateRecommendedFundingSources(Jackson.Ideas.Mock.Models.BusinessPlanRequest request, decimal totalFunding)
        {
            var sources = new List<string>();
            
            if (totalFunding <= 50000)
            {
                sources.AddRange(new[] { "Bootstrapping", "Friends & Family", "Small business grants" });
            }
            else if (totalFunding <= 500000)
            {
                sources.AddRange(new[] { "Angel Investors", "Small business loans", "Crowdfunding" });
            }
            else
            {
                sources.AddRange(new[] { "Venture Capital", "Strategic investors", "Bank loans" });
            }
            
            return sources;
        }
        
        private int CalculateFundingSuitability(string fundingType, Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            var baseScore = 50;
            var category = DetermineBusinessCategory(request.BusinessIdea);
            
            switch (fundingType)
            {
                case "bootstrapping":
                    if (request.InitialInvestment <= 25000) baseScore += 30;
                    if (category == "Professional Services") baseScore += 15;
                    break;
                    
                case "friends_family":
                    if (request.InitialInvestment <= 100000) baseScore += 25;
                    baseScore += 10; // Always somewhat suitable
                    break;
                    
                case "angel":
                    if (category == "Technology") baseScore += 25;
                    if (request.InitialInvestment >= 50000 && request.InitialInvestment <= 500000) baseScore += 20;
                    break;
                    
                case "loan":
                    if (category != "Technology") baseScore += 15; // Traditional businesses
                    if (request.InitialInvestment >= 100000) baseScore += 10;
                    break;
                    
                case "vc":
                    if (category == "Technology") baseScore += 30;
                    if (request.InitialInvestment >= 500000) baseScore += 20;
                    else baseScore -= 20; // Not suitable for small ventures
                    break;
            }
            
            return Math.Clamp(baseScore, 10, 95);
        }
        
        private List<string> GenerateModelStrengths(Jackson.Ideas.Mock.Models.BusinessPlanRequest request, int viabilityScore)
        {
            var strengths = new List<string>();
            
            if (viabilityScore >= 80)
            {
                strengths.AddRange(new[]
                {
                    "Strong market opportunity with clear customer need",
                    "Scalable business model with growth potential",
                    "Competitive advantages provide market differentiation"
                });
            }
            else if (viabilityScore >= 60)
            {
                strengths.AddRange(new[]
                {
                    "Solid foundation with identified target market",
                    "Clear value proposition addresses customer pain points",
                    "Reasonable financial projections and assumptions"
                });
            }
            else
            {
                strengths.Add("Entrepreneur passion and commitment to solving the problem");
            }
            
            if (request.RevenueModel.ToLower().Contains("subscription"))
            {
                strengths.Add("Recurring revenue model provides predictable income");
            }
            
            return strengths;
        }
        
        private List<string> GenerateModelWeaknesses(Jackson.Ideas.Mock.Models.BusinessPlanRequest request, int viabilityScore)
        {
            var weaknesses = new List<string>();
            
            if (viabilityScore < 60)
            {
                weaknesses.AddRange(new[]
                {
                    "Market opportunity may be too narrow or undefined",
                    "Revenue model requires further validation",
                    "Competitive positioning needs strengthening"
                });
            }
            else if (viabilityScore < 80)
            {
                weaknesses.AddRange(new[]
                {
                    "Customer acquisition strategy needs more detail",
                    "Financial projections may be optimistic",
                    "Operational scaling challenges not fully addressed"
                });
            }
            
            if (string.IsNullOrWhiteSpace(request.CompetitiveAdvantages))
            {
                weaknesses.Add("Competitive differentiation not clearly articulated");
            }
            
            return weaknesses;
        }
        
        private List<string> GenerateModelRecommendations(Jackson.Ideas.Mock.Models.BusinessPlanRequest request, int viabilityScore)
        {
            var recommendations = new List<string>();
            
            if (viabilityScore >= 80)
            {
                recommendations.AddRange(new[]
                {
                    "Proceed with operational planning and market launch preparation",
                    "Begin fundraising process if external capital is needed",
                    "Develop detailed go-to-market and customer acquisition plans"
                });
            }
            else if (viabilityScore >= 60)
            {
                recommendations.AddRange(new[]
                {
                    "Conduct additional market validation through customer interviews",
                    "Refine financial projections based on market feedback",
                    "Strengthen competitive analysis and positioning strategy"
                });
            }
            else
            {
                recommendations.AddRange(new[]
                {
                    "Reassess core business concept and target market definition",
                    "Consider pivot opportunities or alternative approaches",
                    "Conduct thorough market research before proceeding"
                });
            }
            
            return recommendations;
        }
        
        private Dictionary<string, decimal> GenerateKeyMetrics(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new Dictionary<string, decimal>
            {
                ["Customer Acquisition Cost"] = 50m,
                ["Customer Lifetime Value"] = 500m,
                ["Monthly Recurring Revenue Growth"] = 15m,
                ["Gross Margin %"] = 65m,
                ["Burn Rate (Monthly)"] = request.InitialInvestment / 18m // 18 months runway
            };
        }
        
        private List<ImplementationPhase> GenerateImplementationPhases(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<ImplementationPhase>
            {
                new()
                {
                    Name = "Foundation Phase",
                    Description = "Business setup, team building, and initial development",
                    DurationWeeks = 12,
                    KeyTasks = new List<string> { "Legal entity formation", "Core team hiring", "MVP development", "Initial market research" },
                    Deliverables = new List<string> { "Legal structure", "Core team", "MVP product", "Market analysis" },
                    ResourceRequirements = new List<string> { "Legal counsel", "Development team", "Office space" },
                    Dependencies = new List<string> { "Funding secured", "Market research completed" },
                    SuccessCriteria = "MVP launched and initial market validation achieved"
                },
                new()
                {
                    Name = "Launch Phase",
                    Description = "Market entry, customer acquisition, and product refinement",
                    DurationWeeks = 24,
                    KeyTasks = new List<string> { "Product launch", "Marketing campaigns", "Customer onboarding", "Feedback integration" },
                    Deliverables = new List<string> { "Live product", "Initial customers", "Marketing presence", "Product improvements" },
                    ResourceRequirements = new List<string> { "Marketing team", "Customer support", "Sales infrastructure" },
                    Dependencies = new List<string> { "MVP completed", "Initial funding raised" },
                    SuccessCriteria = "Product-market fit achieved and sustainable customer growth"
                },
                new()
                {
                    Name = "Growth Phase",
                    Description = "Scaling operations, expanding market presence",
                    DurationWeeks = 36,
                    KeyTasks = new List<string> { "Scale operations", "Expand team", "Market expansion", "Strategic partnerships" },
                    Deliverables = new List<string> { "Scaled operations", "Expanded team", "Market presence", "Partnership agreements" },
                    ResourceRequirements = new List<string> { "Senior leadership", "Engineering team", "Operations infrastructure" },
                    Dependencies = new List<string> { "Series A funding", "Market validation" },
                    SuccessCriteria = "Sustainable growth and market leadership position established"
                }
            };
        }
        
        private List<string> GenerateMilestones(List<ImplementationPhase> phases)
        {
            return new List<string>
            {
                "MVP completion and initial testing",
                "First paying customer acquisition",
                "Break-even point achievement",
                "Market validation and product-market fit",
                "Series A funding completion (if applicable)",
                "Team scaling to 10+ employees",
                "Market expansion to second geographic region"
            };
        }
        
        private List<string> GenerateCriticalPath(List<ImplementationPhase> phases)
        {
            return new List<string>
            {
                "MVP development → Customer testing → Product refinement",
                "Team hiring → Operations setup → Market launch",
                "Customer acquisition → Revenue generation → Growth funding",
                "Market validation → Scaling preparation → Expansion execution"
            };
        }
        
        private List<string> GenerateDependencies(List<ImplementationPhase> phases)
        {
            return new List<string>
            {
                "Funding availability for each phase progression",
                "Key team member hiring and retention",
                "Market conditions and customer receptivity",
                "Technology platform stability and scalability",
                "Regulatory approval processes (if applicable)"
            };
        }

        private string GenerateRetentionStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Multi-faceted retention approach including excellent customer support, regular product updates, loyalty programs, and community building initiatives.";
        }

        private string GenerateFundingStrategy(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "Phased funding approach starting with bootstrap/angel funding, followed by seed round and potential Series A based on growth milestones.";
        }

        private string GenerateUseOfFunds(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return "40% product development, 30% marketing and customer acquisition, 20% operations and infrastructure, 10% working capital and contingency.";
        }

        private List<string> GenerateFundingMilestones(Jackson.Ideas.Mock.Models.BusinessPlanRequest request)
        {
            return new List<string>
            {
                "Months 1-6: Bootstrap/angel funding for MVP development",
                "Months 6-12: Seed round upon MVP completion and initial traction",
                "Months 12-24: Series A funding based on growth metrics and market validation",
                "Year 2+: Additional rounds based on expansion needs and performance"
            };
        }
    }
}