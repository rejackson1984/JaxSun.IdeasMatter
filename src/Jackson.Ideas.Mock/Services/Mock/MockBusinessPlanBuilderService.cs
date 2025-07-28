using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Mock implementation of business plan builder service with realistic data
    /// </summary>
    public class MockBusinessPlanBuilderService : IBusinessPlanBuilderService
    {
        private readonly Dictionary<string, Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan> _businessPlans = new();
        private readonly Dictionary<string, Jackson.Ideas.Mock.Services.Interfaces.BusinessModelCanvas> _canvases = new();
        
        public Task<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan> CreateBusinessPlanAsync(BusinessPlanRequest request)
        {
            var businessPlan = new Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan
            {
                BusinessName = request.BusinessName,
                Description = request.Description,
                ExecutiveSummary = CreateExecutiveSummary(request),
                MarketAnalysis = CreateMarketAnalysis(request),
                ProductStrategy = CreateProductStrategy(request),
                MarketingStrategy = CreateMarketingStrategy(request),
                OperationsStrategy = CreateOperationsStrategy(request),
                FinancialStrategy = CreateFinancialStrategy(request),
                RiskAssessment = CreateRiskAssessment(request),
                ImplementationTimeline = CreateImplementationTimeline(request)
            };
            
            businessPlan.CompletenessScore = CalculateCompletenessScore(businessPlan);
            businessPlan.ReadyForOperations = businessPlan.CompletenessScore >= 80;
            
            _businessPlans[businessPlan.Id] = businessPlan;
            
            return Task.FromResult(businessPlan);
        }
        
        public Task<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan?> GetBusinessPlanAsync(string planId)
        {
            // For demo purposes, create a sample plan if none exists
            if (!_businessPlans.ContainsKey(planId))
            {
                var demoRequest = new BusinessPlanRequest
                {
                    BusinessName = "HealthyKids Meal Planner",
                    Description = "An app that helps busy parents find healthy meal ideas their kids will actually eat",
                    ProblemStatement = "Parents struggle to find healthy meals that children will enjoy",
                    Solution = "AI-powered meal planning with kid-friendly healthy recipes",
                    TargetMarket = "Working parents with children ages 3-12",
                    RevenueModel = "Monthly subscription model at $9.99/month"
                };
                
                return CreateBusinessPlanAsync(demoRequest);
            }
            
            return Task.FromResult<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan?>(_businessPlans[planId]);
        }
        
        public Task<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan> UpdateBusinessPlanSectionAsync(string planId, string sectionName, object sectionData)
        {
            if (_businessPlans.TryGetValue(planId, out var plan))
            {
                // Update the specified section (simplified for demo)
                plan.UpdatedAt = DateTime.UtcNow;
                plan.CompletenessScore = CalculateCompletenessScore(plan);
                plan.ReadyForOperations = plan.CompletenessScore >= 80;
                return Task.FromResult(plan);
            }
            
            // If plan not found, throw exception as interface expects non-null return
            throw new ArgumentException($"Business plan with ID '{planId}' not found.", nameof(planId));
        }
        
        public Task<Jackson.Ideas.Mock.Services.Interfaces.BusinessModelCanvas> GetBusinessModelCanvasAsync(string planId)
        {
            if (!_canvases.ContainsKey(planId))
            {
                _canvases[planId] = CreateSampleCanvas(planId);
            }
            
            return Task.FromResult(_canvases[planId]);
        }
        
        public Task<Jackson.Ideas.Mock.Services.Interfaces.BusinessModelCanvas> UpdateBusinessModelCanvasAsync(string planId, Jackson.Ideas.Mock.Services.Interfaces.BusinessModelCanvas canvas)
        {
            canvas.UpdatedAt = DateTime.UtcNow;
            _canvases[planId] = canvas;
            return Task.FromResult(canvas);
        }
        
        public Task<List<Jackson.Ideas.Mock.Services.Interfaces.StrategicRecommendation>> GetStrategicRecommendationsAsync(string planId)
        {
            return Task.FromResult(new List<Jackson.Ideas.Mock.Services.Interfaces.StrategicRecommendation>
            {
                new()
                {
                    Title = "Focus on Customer Acquisition",
                    Description = "Prioritize building a strong customer acquisition funnel through digital marketing channels",
                    Category = "Marketing",
                    Priority = 1,
                    Impact = "High - Direct impact on revenue growth",
                    Implementation = "Implement social media marketing, content marketing, and referral programs"
                },
                new()
                {
                    Title = "Develop MVP First",
                    Description = "Start with a minimum viable product to validate assumptions and gather user feedback",
                    Category = "Product",
                    Priority = 2,
                    Impact = "High - Reduces development risk and costs",
                    Implementation = "Define core features, build basic app, conduct user testing"
                },
                new()
                {
                    Title = "Build Strategic Partnerships",
                    Description = "Partner with complementary businesses to expand reach and credibility",
                    Category = "Business Development",
                    Priority = 3,
                    Impact = "Medium - Accelerates market penetration",
                    Implementation = "Identify potential partners, create partnership proposals, negotiate agreements"
                },
                new()
                {
                    Title = "Establish Financial Controls",
                    Description = "Implement robust financial tracking and reporting systems early",
                    Category = "Finance",
                    Priority = 4,
                    Impact = "Medium - Essential for scaling and investment",
                    Implementation = "Choose accounting software, set up financial dashboards, establish budgeting processes"
                }
            });
        }
        
        public async Task<int> CalculateCompletenessScoreAsync(string planId)
        {
            var plan = await GetBusinessPlanAsync(planId);
            return plan != null ? CalculateCompletenessScore(plan) : 0;
        }
        
        public async Task<bool> IsReadyForOperationsAsync(string planId)
        {
            var score = await CalculateCompletenessScoreAsync(planId);
            return score >= 80;
        }
        
        public Task<List<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlanTemplate>> GetBusinessPlanTemplatesAsync()
        {
            return Task.FromResult(new List<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlanTemplate>
            {
                new()
                {
                    Id = "saas-template",
                    Name = "SaaS Business Plan",
                    Description = "Template for software-as-a-service businesses",
                    Industry = "Technology",
                    BusinessType = "SaaS"
                },
                new()
                {
                    Id = "ecommerce-template",
                    Name = "E-commerce Business Plan",
                    Description = "Template for online retail businesses",
                    Industry = "Retail",
                    BusinessType = "E-commerce"
                },
                new()
                {
                    Id = "service-template",
                    Name = "Service Business Plan",
                    Description = "Template for service-based businesses",
                    Industry = "Services",
                    BusinessType = "Service"
                },
                new()
                {
                    Id = "marketplace-template",
                    Name = "Marketplace Business Plan",
                    Description = "Template for marketplace platforms",
                    Industry = "Technology",
                    BusinessType = "Marketplace"
                }
            });
        }
        
        public Task<Jackson.Ideas.Mock.Services.Interfaces.BusinessPlanExport> ExportBusinessPlanAsync(string planId, Jackson.Ideas.Mock.Services.Interfaces.ExportFormat format)
        {
            var export = new Jackson.Ideas.Mock.Services.Interfaces.BusinessPlanExport
            {
                Format = format,
                FileName = $"business-plan-{planId}.{format.ToString().ToLower()}",
                Data = GenerateMockExportData(format)
            };
            
            return Task.FromResult(export);
        }
        
        private Jackson.Ideas.Mock.Services.Interfaces.ExecutiveSummary CreateExecutiveSummary(BusinessPlanRequest request)
        {
            return new Jackson.Ideas.Mock.Services.Interfaces.ExecutiveSummary
            {
                BusinessConcept = $"{request.BusinessName} addresses the challenge of {request.ProblemStatement} through an innovative solution that {request.Solution}.",
                MissionStatement = $"To empower {request.TargetMarket} with innovative solutions that make their lives easier and more productive.",
                VisionStatement = $"To become the leading platform for {request.TargetMarket}, transforming how they approach daily challenges.",
                ValueProposition = $"We provide {request.TargetMarket} with a unique solution that saves time, reduces stress, and delivers measurable value through {request.Solution}.",
                KeySuccessFactors = "Strong product-market fit, scalable technology platform, effective customer acquisition strategy, and excellent user experience.",
                FundingRequirements = "Seeking $500K in seed funding to develop MVP, build initial team, and launch marketing efforts.",
                ExpectedReturns = "Projected to reach profitability within 18 months with potential for 10x return on investment within 5 years."
            };
        }
        
        private Jackson.Ideas.Mock.Services.Interfaces.MarketAnalysis CreateMarketAnalysis(BusinessPlanRequest request)
        {
            return new Jackson.Ideas.Mock.Services.Interfaces.MarketAnalysis
            {
                IndustryOverview = $"The industry serving {request.TargetMarket} is experiencing significant growth driven by digital transformation and changing consumer expectations.",
                TargetMarket = request.TargetMarket,
                MarketSize = "Total addressable market estimated at $2.5B with a serviceable addressable market of $150M.",
                MarketTrends = "Increasing demand for digital solutions, focus on convenience and efficiency, mobile-first preferences.",
                CompetitiveAnalysis = "Competitive landscape includes both established players and emerging startups, with opportunities for differentiation through superior user experience and targeted features.",
                MarketOpportunity = "Significant opportunity exists due to underserved market segments and evolving customer needs.",
                CustomerSegments = new List<Jackson.Ideas.Mock.Services.Interfaces.CustomerSegment>
                {
                    new() { Name = "Primary Segment", Description = request.TargetMarket, Size = "60% of total market", Characteristics = "High engagement, willing to pay for quality solutions" },
                    new() { Name = "Secondary Segment", Description = "Adjacent customer groups", Size = "25% of total market", Characteristics = "Price-sensitive, moderate engagement" },
                    new() { Name = "Tertiary Segment", Description = "Future expansion opportunities", Size = "15% of total market", Characteristics = "Early adopters, high growth potential" }
                }
            };
        }
        
        private ProductStrategy CreateProductStrategy(BusinessPlanRequest request)
        {
            return new ProductStrategy
            {
                ProductDescription = $"{request.BusinessName} is a comprehensive solution that {request.Description}",
                DevelopmentPlan = "Agile development approach with 2-week sprints, focusing on MVP delivery within 6 months followed by iterative improvements.",
                TechnicalRequirements = "Cloud-based architecture using modern web technologies, mobile-responsive design, scalable infrastructure.",
                IntellectualProperty = "Proprietary algorithms and processes, trademark protection for brand, potential patent applications for unique features.",
                QualityAssurance = "Comprehensive testing strategy including unit tests, integration tests, user acceptance testing, and continuous monitoring.",
                CoreFeatures = new List<ProductFeature>
                {
                    new() { Name = "Core Functionality", Description = "Primary value delivery feature", Priority = "High", Status = "In Development" },
                    new() { Name = "User Management", Description = "Account creation and management", Priority = "High", Status = "Planned" },
                    new() { Name = "Analytics Dashboard", Description = "User insights and metrics", Priority = "Medium", Status = "Planned" },
                    new() { Name = "Integration Capabilities", Description = "Third-party service integrations", Priority = "Medium", Status = "Future" }
                },
                Milestones = new List<DevelopmentMilestone>
                {
                    new() { Name = "MVP Launch", Description = "Core features ready for beta testing", TargetDate = DateTime.Now.AddMonths(6), Status = "On Track" },
                    new() { Name = "Public Launch", Description = "Full public release with marketing campaign", TargetDate = DateTime.Now.AddMonths(9), Status = "Planned" },
                    new() { Name = "Feature Enhancement", Description = "Additional features based on user feedback", TargetDate = DateTime.Now.AddMonths(12), Status = "Planned" }
                }
            };
        }
        
        private MarketingStrategy CreateMarketingStrategy(BusinessPlanRequest request)
        {
            return new MarketingStrategy
            {
                BrandPositioning = "Positioned as the premium, user-friendly solution for discerning customers who value quality and effectiveness.",
                PricingStrategy = $"{request.RevenueModel} with competitive pricing that reflects value delivered.",
                DistributionChannels = "Direct-to-consumer through website and mobile app stores, potential partner channels for expansion.",
                PromotionalStrategy = "Digital marketing focus including content marketing, social media, paid advertising, and influencer partnerships.",
                CustomerAcquisition = "Multi-channel approach targeting customer acquisition cost of under $50 with lifetime value of $300+.",
                CustomerRetention = "Focus on user engagement, regular feature updates, excellent customer support, and loyalty programs.",
                Channels = new List<MarketingChannel>
                {
                    new() { Name = "Social Media Marketing", Strategy = "Organic and paid social media campaigns", Budget = "$2,000/month", ExpectedROI = "300%" },
                    new() { Name = "Content Marketing", Strategy = "Blog, videos, and educational content", Budget = "$1,500/month", ExpectedROI = "400%" },
                    new() { Name = "Search Engine Marketing", Strategy = "SEO and paid search campaigns", Budget = "$3,000/month", ExpectedROI = "250%" },
                    new() { Name = "Email Marketing", Strategy = "Automated email sequences and newsletters", Budget = "$500/month", ExpectedROI = "500%" }
                }
            };
        }
        
        private OperationsStrategy CreateOperationsStrategy(BusinessPlanRequest request)
        {
            return new OperationsStrategy
            {
                OperationalModel = "Lean startup methodology with focus on rapid iteration and customer feedback integration.",
                TechnologyInfrastructure = "Cloud-based infrastructure with automatic scaling, robust security measures, and 99.9% uptime target.",
                QualityControl = "Continuous integration/deployment pipeline with automated testing and monitoring.",
                SupplyChain = "Digital service delivery with minimal physical supply chain requirements.",
                Scalability = "Architecture designed to handle 100x growth without major infrastructure changes.",
                KeyProcesses = new List<KeyProcess>
                {
                    new() { Name = "Customer Onboarding", Description = "Streamlined user registration and setup", Owner = "Product Team", Metrics = "Time to first value, completion rate" },
                    new() { Name = "Customer Support", Description = "Multi-channel customer service", Owner = "Support Team", Metrics = "Response time, satisfaction scores" },
                    new() { Name = "Product Development", Description = "Agile development and release process", Owner = "Engineering Team", Metrics = "Sprint velocity, bug rates" }
                },
                RequiredResources = new List<Resource>
                {
                    new() { Name = "Development Team", Type = "Human", Quantity = "5 developers", Cost = "$50,000/month" },
                    new() { Name = "Cloud Infrastructure", Type = "Technology", Quantity = "Scalable", Cost = "$2,000/month" },
                    new() { Name = "Office Space", Type = "Physical", Quantity = "Co-working space", Cost = "$1,500/month" }
                }
            };
        }
        
        private FinancialStrategy CreateFinancialStrategy(BusinessPlanRequest request)
        {
            return new FinancialStrategy
            {
                RevenueModel = request.RevenueModel,
                CostStructure = "Variable costs scale with usage, fixed costs include team salaries and infrastructure.",
                FundingStrategy = "Bootstrap initially, then seek seed funding for growth acceleration.",
                FinancialProjections = "Break-even expected in month 18, positive cash flow by month 24.",
                BreakEvenAnalysis = "Need 5,000 paying customers at average $9.99/month to break even.",
                CashFlowManagement = "Conservative cash management with 6-month runway buffer maintained."
            };
        }
        
        private Jackson.Ideas.Mock.Services.Interfaces.RiskAssessment CreateRiskAssessment(BusinessPlanRequest request)
        {
            return new Jackson.Ideas.Mock.Services.Interfaces.RiskAssessment
            {
                IdentifiedRisks = new List<Jackson.Ideas.Mock.Services.Interfaces.BusinessRisk>
                {
                    new() { Name = "Market Competition", Description = "Increased competition from established players", Impact = "High", Probability = "Medium", MitigationStrategy = "Focus on differentiation and superior user experience" },
                    new() { Name = "Technology Risk", Description = "Technical challenges or security issues", Impact = "Medium", Probability = "Low", MitigationStrategy = "Robust testing, security audits, and backup systems" },
                    new() { Name = "Customer Acquisition", Description = "Higher than expected acquisition costs", Impact = "Medium", Probability = "Medium", MitigationStrategy = "Diversify marketing channels and optimize conversion rates" },
                    new() { Name = "Regulatory Changes", Description = "Changes in relevant regulations", Impact = "Low", Probability = "Low", MitigationStrategy = "Stay informed on regulatory landscape and maintain compliance" }
                },
                RiskMitigationStrategy = "Comprehensive risk monitoring with proactive mitigation strategies for all identified risks.",
                ContingencyPlans = "Pivot strategies prepared for major market changes or competitive threats.",
                InsuranceRequirements = "Professional liability, cyber security, and general business insurance coverage."
            };
        }
        
        private ImplementationTimeline CreateImplementationTimeline(BusinessPlanRequest request)
        {
            return new ImplementationTimeline
            {
                Phases = new List<Phase>
                {
                    new() { Name = "Phase 1: MVP Development", Description = "Build and test core functionality", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6), Deliverables = new() { "MVP Release", "Beta Testing", "Initial User Feedback" } },
                    new() { Name = "Phase 2: Market Launch", Description = "Public launch and marketing campaign", StartDate = DateTime.Now.AddMonths(6), EndDate = DateTime.Now.AddMonths(12), Deliverables = new() { "Public Release", "Marketing Campaign", "Customer Support System" } },
                    new() { Name = "Phase 3: Growth & Scale", Description = "Feature expansion and market growth", StartDate = DateTime.Now.AddMonths(12), EndDate = DateTime.Now.AddMonths(24), Deliverables = new() { "Feature Enhancements", "Market Expansion", "Team Growth" } }
                },
                KeyMilestones = new List<Jackson.Ideas.Mock.Services.Interfaces.Milestone>
                {
                    new() { Name = "MVP Complete", Description = "Core product ready for testing", TargetDate = DateTime.Now.AddMonths(6), Status = "On Track", Criteria = "All core features functional and tested" },
                    new() { Name = "First 100 Customers", Description = "Achieve initial customer base", TargetDate = DateTime.Now.AddMonths(9), Status = "Planned", Criteria = "100 paying customers acquired" },
                    new() { Name = "Break Even", Description = "Achieve operational break-even", TargetDate = DateTime.Now.AddMonths(18), Status = "Planned", Criteria = "Monthly recurring revenue covers monthly costs" }
                },
                CriticalPath = "Product development → Beta testing → Public launch → Customer acquisition → Revenue growth",
                ResourceAllocation = "60% engineering, 25% marketing, 10% operations, 5% administration"
            };
        }
        
        private Jackson.Ideas.Mock.Services.Interfaces.BusinessModelCanvas CreateSampleCanvas(string planId)
        {
            return new Jackson.Ideas.Mock.Services.Interfaces.BusinessModelCanvas
            {
                BusinessPlanId = planId,
                KeyPartners = new() { "Technology providers", "Marketing agencies", "Strategic advisors", "Complementary service providers" },
                KeyActivities = new() { "Product development", "Customer support", "Marketing and sales", "Data analysis and optimization" },
                KeyResources = new() { "Development team", "Technology platform", "Customer data", "Brand and reputation" },
                ValuePropositions = new() { "Time-saving solution", "Improved outcomes", "User-friendly interface", "Affordable pricing" },
                CustomerRelationships = new() { "Self-service platform", "Community support", "Personal assistance", "Automated services" },
                Channels = new() { "Website", "Mobile app", "Social media", "Partner referrals" },
                CustomerSegments = new() { "Primary target market", "Secondary markets", "Early adopters", "Enterprise customers" },
                CostStructure = new() { "Personnel costs", "Technology infrastructure", "Marketing expenses", "Customer support" },
                RevenueStreams = new() { "Subscription fees", "Premium features", "Partner commissions", "Data insights" }
            };
        }
        
        private int CalculateCompletenessScore(Jackson.Ideas.Mock.Services.Interfaces.BusinessPlan plan)
        {
            int score = 0;
            
            // Executive Summary (20 points)
            if (!string.IsNullOrEmpty(plan.ExecutiveSummary.BusinessConcept)) score += 5;
            if (!string.IsNullOrEmpty(plan.ExecutiveSummary.MissionStatement)) score += 5;
            if (!string.IsNullOrEmpty(plan.ExecutiveSummary.ValueProposition)) score += 5;
            if (!string.IsNullOrEmpty(plan.ExecutiveSummary.FundingRequirements)) score += 5;
            
            // Market Analysis (20 points)
            if (!string.IsNullOrEmpty(plan.MarketAnalysis.TargetMarket)) score += 5;
            if (!string.IsNullOrEmpty(plan.MarketAnalysis.MarketSize)) score += 5;
            if (!string.IsNullOrEmpty(plan.MarketAnalysis.CompetitiveAnalysis)) score += 5;
            if (plan.MarketAnalysis.CustomerSegments.Any()) score += 5;
            
            // Product Strategy (15 points)
            if (!string.IsNullOrEmpty(plan.ProductStrategy.ProductDescription)) score += 5;
            if (!string.IsNullOrEmpty(plan.ProductStrategy.DevelopmentPlan)) score += 5;
            if (plan.ProductStrategy.CoreFeatures.Any()) score += 5;
            
            // Marketing Strategy (15 points)
            if (!string.IsNullOrEmpty(plan.MarketingStrategy.PricingStrategy)) score += 5;
            if (!string.IsNullOrEmpty(plan.MarketingStrategy.CustomerAcquisition)) score += 5;
            if (plan.MarketingStrategy.Channels.Any()) score += 5;
            
            // Financial Strategy (15 points)
            if (!string.IsNullOrEmpty(plan.FinancialStrategy.RevenueModel)) score += 5;
            if (!string.IsNullOrEmpty(plan.FinancialStrategy.FinancialProjections)) score += 5;
            if (!string.IsNullOrEmpty(plan.FinancialStrategy.BreakEvenAnalysis)) score += 5;
            
            // Operations Strategy (10 points)
            if (!string.IsNullOrEmpty(plan.OperationsStrategy.OperationalModel)) score += 5;
            if (plan.OperationsStrategy.KeyProcesses.Any()) score += 5;
            
            // Risk Assessment (5 points)
            if (plan.RiskAssessment.IdentifiedRisks.Any()) score += 5;
            
            return score;
        }
        
        private byte[] GenerateMockExportData(Jackson.Ideas.Mock.Services.Interfaces.ExportFormat format)
        {
            // Return mock data for demo purposes
            string content = $"Mock {format} export data - Business Plan Content";
            return System.Text.Encoding.UTF8.GetBytes(content);
        }
    }
}