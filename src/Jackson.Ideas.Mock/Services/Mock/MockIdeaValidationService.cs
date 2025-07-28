using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Mock implementation of idea validation service with realistic validation logic
    /// </summary>
    public class MockIdeaValidationService : IIdeaValidationService
    {
        private readonly Random _random = new(42); // Fixed seed for consistent results
        
        public Task<IdeaValidationResult> ValidateIdeaAsync(IdeaValidationRequest request)
        {
            var result = new IdeaValidationResult
            {
                Request = request
            };
            
            // Calculate category scores based on idea content
            result.CategoryScores = CalculateCategoryScores(request);
            result.OverallScore = (int)result.CategoryScores.Values.Average();
            
            // Generate SWOT analysis
            result.Strengths = GenerateStrengths(request, result.CategoryScores);
            result.Weaknesses = GenerateWeaknesses(request, result.CategoryScores);
            result.Opportunities = GenerateOpportunities(request);
            result.Threats = GenerateThreats(request);
            
            // Generate next steps
            result.NextSteps = GenerateNextSteps(request, result);
            
            // Determine readiness for next hub (need 70+ score)
            result.ReadyForNextHub = result.OverallScore >= 70;
            
            // Generate market opportunity and competition analysis
            result.MarketOpportunity = GenerateMarketOpportunity(request);
            result.Competition = GenerateCompetitiveAnalysis(request);
            
            return Task.FromResult(result);
        }
        
        public Task<List<ValidationCriteria>> GetValidationCriteriaAsync(string strategy)
        {
            var criteria = strategy switch
            {
                "quick" => GetQuickValidationCriteria(),
                "deep" => GetDeepValidationCriteria(),
                "launch" => GetLaunchValidationCriteria(),
                _ => GetQuickValidationCriteria()
            };
            
            return Task.FromResult(criteria);
        }
        
        public async Task<int> CalculateValidationScoreAsync(IdeaValidationRequest request)
        {
            var result = await ValidateIdeaAsync(request);
            return result.OverallScore;
        }
        
        public Task<List<string>> GetImprovementRecommendationsAsync(IdeaValidationResult result)
        {
            var recommendations = new List<string>();
            
            // Add recommendations based on weak areas
            foreach (var category in result.CategoryScores.Where(kvp => kvp.Value < 70))
            {
                recommendations.AddRange(GetCategoryRecommendations(category.Key, category.Value));
            }
            
            if (!recommendations.Any())
            {
                recommendations.Add("Your idea is looking strong! Focus on market research and customer validation.");
                recommendations.Add("Consider creating a minimal viable product (MVP) to test your assumptions.");
                recommendations.Add("Start building your network and potential customer base early.");
            }
            
            return Task.FromResult(recommendations);
        }
        
        public Task<bool> IsReadyForNextHubAsync(IdeaValidationResult result)
        {
            return Task.FromResult(result.ReadyForNextHub);
        }
        
        public Task<List<string>> GetMarketResearchSuggestionsAsync(IdeaValidationRequest request)
        {
            var suggestions = new List<string>
            {
                $"Research the {ExtractIndustry(request.Description)} industry size and growth trends",
                $"Survey potential customers in the {request.TargetAudience} demographic",
                "Analyze pricing strategies of similar solutions in the market",
                "Study customer reviews and complaints about existing solutions",
                "Investigate regulatory requirements and barriers in your industry"
            };
            
            if (request.Strategy == "deep" || request.Strategy == "launch")
            {
                suggestions.AddRange(new[]
                {
                    "Conduct focus groups with target customers",
                    "Analyze social media conversations about your problem space",
                    "Research distribution channels and partnership opportunities",
                    "Study seasonal trends and market timing factors"
                });
            }
            
            return Task.FromResult(suggestions);
        }
        
        public Task<Services.Interfaces.CompetitiveAnalysis> AnalyzeCompetitionAsync(IdeaValidationRequest request)
        {
            return Task.FromResult(GenerateCompetitiveAnalysis(request));
        }
        
        public Task<MarketOpportunity> EstimateMarketOpportunityAsync(IdeaValidationRequest request)
        {
            return Task.FromResult(GenerateMarketOpportunity(request));
        }
        
        private Dictionary<string, int> CalculateCategoryScores(IdeaValidationRequest request)
        {
            var scores = new Dictionary<string, int>();
            
            // Problem Clarity (based on problem description quality)
            scores["Problem Clarity"] = CalculateProblemScore(request.ProblemSolved);
            
            // Market Potential (based on target audience specificity)
            scores["Market Potential"] = CalculateMarketScore(request.TargetAudience);
            
            // Revenue Viability (based on revenue model clarity)
            scores["Revenue Viability"] = CalculateRevenueScore(request.RevenueModel);
            
            // Innovation Factor (based on idea uniqueness)
            scores["Innovation Factor"] = CalculateInnovationScore(request.Description);
            
            // Execution Feasibility (balanced score based on complexity)
            scores["Execution Feasibility"] = CalculateFeasibilityScore(request.Description);
            
            return scores;
        }
        
        private int CalculateProblemScore(string problemSolved)
        {
            if (string.IsNullOrWhiteSpace(problemSolved)) return 20;
            
            int score = 40; // Base score
            
            // Bonus for specificity
            if (problemSolved.Contains("time") || problemSolved.Contains("money") || problemSolved.Contains("stress"))
                score += 15;
            
            // Bonus for quantification
            if (problemSolved.Any(char.IsDigit) || problemSolved.Contains("%") || problemSolved.Contains("hour"))
                score += 15;
            
            // Bonus for emotional language
            if (problemSolved.Contains("frustrat") || problemSolved.Contains("difficult") || problemSolved.Contains("struggle"))
                score += 10;
            
            // Bonus for clarity
            if (problemSolved.Length > 50 && problemSolved.Length < 200)
                score += 10;
            
            return Math.Min(score, 95);
        }
        
        private int CalculateMarketScore(string targetAudience)
        {
            if (string.IsNullOrWhiteSpace(targetAudience)) return 25;
            
            int score = 45; // Base score
            
            // Bonus for demographic specificity
            if (targetAudience.Contains("age") || targetAudience.Contains("income") || targetAudience.Contains("profession"))
                score += 15;
            
            // Bonus for behavioral descriptors
            if (targetAudience.Contains("busy") || targetAudience.Contains("value") || targetAudience.Contains("want"))
                score += 10;
            
            // Penalty for being too broad
            if (targetAudience.Contains("everyone") || targetAudience.Contains("anyone"))
                score -= 20;
            
            // Bonus for reasonable scope
            if (!targetAudience.Contains("everyone") && targetAudience.Length > 20)
                score += 15;
            
            return Math.Min(score, 90);
        }
        
        private int CalculateRevenueScore(string revenueModel)
        {
            if (string.IsNullOrWhiteSpace(revenueModel)) return 30;
            
            int score = 40; // Base score
            
            // Bonus for specific pricing
            if (revenueModel.Contains("$") || revenueModel.Contains("price"))
                score += 20;
            
            // Bonus for proven models
            if (revenueModel.Contains("subscription") || revenueModel.Contains("commission") || revenueModel.Contains("freemium"))
                score += 15;
            
            // Small penalty for uncertainty (but that's honest!)
            if (revenueModel.Contains("not sure") || revenueModel.Contains("unsure"))
                score += 5; // Actually bonus for honesty
            
            return Math.Min(score, 85);
        }
        
        private int CalculateInnovationScore(string description)
        {
            int score = 50; // Base score
            
            // Bonus for technology mentions
            if (description.Contains("app") || description.Contains("platform") || description.Contains("AI"))
                score += 10;
            
            // Bonus for combination of existing concepts
            if (description.Contains("like") && description.Contains("but"))
                score += 15;
            
            // Random factor for uniqueness
            score += _random.Next(-10, 20);
            
            return Math.Clamp(score, 30, 90);
        }
        
        private int CalculateFeasibilityScore(string description)
        {
            int score = 60; // Base score - assume reasonable feasibility
            
            // Adjust based on complexity indicators
            if (description.Contains("simple") || description.Contains("basic"))
                score += 15;
            
            if (description.Contains("AI") || description.Contains("machine learning"))
                score -= 10; // More complex
            
            if (description.Contains("mobile") || description.Contains("web"))
                score += 5; // Proven platforms
            
            return Math.Clamp(score, 40, 85);
        }
        
        private List<string> GenerateStrengths(IdeaValidationRequest request, Dictionary<string, int> scores)
        {
            var strengths = new List<string>();
            
            if (scores["Problem Clarity"] >= 70)
                strengths.Add("Clear problem definition - you understand the pain point well");
            
            if (scores["Market Potential"] >= 70)
                strengths.Add("Well-defined target audience with specific characteristics");
            
            if (scores["Revenue Viability"] >= 70)
                strengths.Add("Realistic revenue model with clear monetization path");
            
            if (scores["Innovation Factor"] >= 70)
                strengths.Add("Innovative approach with potential for differentiation");
            
            if (scores["Execution Feasibility"] >= 70)
                strengths.Add("Achievable scope with reasonable implementation complexity");
            
            // Always include at least one strength
            if (!strengths.Any())
            {
                strengths.Add("Passionate founder with a clear vision for solving problems");
                strengths.Add("Addresses a real need that people experience regularly");
            }
            
            return strengths;
        }
        
        private List<string> GenerateWeaknesses(IdeaValidationRequest request, Dictionary<string, int> scores)
        {
            var weaknesses = new List<string>();
            
            if (scores["Problem Clarity"] < 60)
                weaknesses.Add("Problem definition could be more specific and quantified");
            
            if (scores["Market Potential"] < 60)
                weaknesses.Add("Target audience needs more specific demographic and behavioral details");
            
            if (scores["Revenue Viability"] < 60)
                weaknesses.Add("Revenue model requires more detailed pricing and monetization strategy");
            
            if (scores["Innovation Factor"] < 50)
                weaknesses.Add("Competitive differentiation could be more clearly defined");
            
            return weaknesses;
        }
        
        private List<string> GenerateOpportunities(IdeaValidationRequest request)
        {
            var opportunities = new List<string>
            {
                "Growing demand for digital solutions in your target market",
                "Potential for subscription-based recurring revenue models",
                "Opportunity to build strong customer relationships and loyalty"
            };
            
            if (request.TargetAudience.Contains("parent"))
                opportunities.Add("Large and engaged parenting community online");
            
            if (request.Description.Contains("mobile") || request.Description.Contains("app"))
                opportunities.Add("Mobile-first market with high engagement rates");
            
            return opportunities;
        }
        
        private List<string> GenerateThreats(IdeaValidationRequest request)
        {
            return new List<string>
            {
                "Established competitors with existing market share",
                "Potential for larger companies to replicate your solution",
                "Customer acquisition costs in digital markets can be high",
                "Technology and market preferences change rapidly"
            };
        }
        
        private List<string> GenerateNextSteps(IdeaValidationRequest request, IdeaValidationResult result)
        {
            var steps = new List<string>();
            
            if (result.OverallScore >= 70)
            {
                steps.Add("🎉 Congratulations! Your idea shows strong potential");
                steps.Add("🔍 Conduct customer interviews to validate your assumptions");
                steps.Add("📊 Research market size and competitive landscape in detail");
                steps.Add("💡 Consider creating a minimal viable product (MVP)");
            }
            else
            {
                steps.Add("💪 Keep refining your idea - you're on the right track!");
                steps.Add("🎯 Focus on clearly defining the specific problem you're solving");
                steps.Add("👥 Get more specific about who your ideal customer really is");
                steps.Add("🔄 Consider iterating on your approach based on this feedback");
            }
            
            return steps;
        }
        
        private Services.Interfaces.CompetitiveAnalysis GenerateCompetitiveAnalysis(IdeaValidationRequest request)
        {
            var industry = ExtractIndustry(request.Description);
            
            return new Services.Interfaces.CompetitiveAnalysis
            {
                DirectCompetitors = GenerateCompetitors(industry, true),
                IndirectCompetitors = GenerateCompetitors(industry, false),
                CompetitiveLandscape = $"The {industry} market shows moderate competition with established players and emerging startups.",
                CompetitiveAdvantages = new List<string>
                {
                    "First-mover advantage in your specific niche",
                    "Focused approach to a specific customer segment",
                    "Potential for superior user experience"
                },
                CompetitiveThreats = new List<string>
                {
                    "Established brands with marketing resources",
                    "Price competition from larger players",
                    "Risk of feature replication"
                },
                CompetitionIntensity = _random.Next(4, 8)
            };
        }
        
        private List<Services.Interfaces.Competitor> GenerateCompetitors(string industry, bool isDirect)
        {
            var competitors = new List<Services.Interfaces.Competitor>();
            
            // Generate 2-3 realistic competitors based on industry
            var competitorCount = isDirect ? 2 : 3;
            
            for (int i = 0; i < competitorCount; i++)
            {
                competitors.Add(new Services.Interfaces.Competitor
                {
                    Name = GenerateCompetitorName(industry, i),
                    Description = $"Established player in the {industry} space",
                    Strengths = isDirect ? "Large user base, proven business model" : "Adjacent market presence, resources",
                    Weaknesses = isDirect ? "Complex interface, high pricing" : "Not focused on your specific problem",
                    MarketShare = isDirect ? $"{_random.Next(10, 30)}%" : $"{_random.Next(5, 15)}%",
                    IsDirect = isDirect
                });
            }
            
            return competitors;
        }
        
        private string GenerateCompetitorName(string industry, int index)
        {
            var prefixes = new[] { "Smart", "Pro", "Quick", "Easy", "Best", "Top" };
            var suffixes = new[] { "Solutions", "Tech", "App", "Pro", "Hub", "Central" };
            
            return $"{prefixes[index % prefixes.Length]}{industry.Replace(" ", "")}{suffixes[index % suffixes.Length]}";
        }
        
        private MarketOpportunity GenerateMarketOpportunity(IdeaValidationRequest request)
        {
            var marketSizes = new[] { "$50M", "$150M", "$500M", "$1.2B", "$3.5B" };
            var growthRates = new[] { "8%", "12%", "18%", "25%", "35%" };
            var sizeCategories = new[] { "Niche", "Medium", "Large" };
            
            var sizeIndex = _random.Next(marketSizes.Length);
            
            return new MarketOpportunity
            {
                MarketSize = marketSizes[sizeIndex],
                GrowthRate = growthRates[_random.Next(growthRates.Length)],
                Trends = "Growing demand for digital solutions, increased focus on efficiency and convenience",
                KeyDrivers = new List<string>
                {
                    "Digital transformation trends",
                    "Changing consumer expectations",
                    "Mobile-first preferences",
                    "Focus on time-saving solutions"
                },
                Barriers = new List<string>
                {
                    "Customer acquisition costs",
                    "Market education requirements",
                    "Competitive market dynamics"
                },
                OpportunityScore = _random.Next(60, 85),
                SizeCategory = sizeCategories[Math.Min(sizeIndex / 2, 2)]
            };
        }
        
        private string ExtractIndustry(string description)
        {
            if (description.Contains("food") || description.Contains("meal") || description.Contains("restaurant"))
                return "Food & Dining";
            if (description.Contains("health") || description.Contains("fitness") || description.Contains("medical"))
                return "Health & Wellness";
            if (description.Contains("education") || description.Contains("learn") || description.Contains("course"))
                return "Education";
            if (description.Contains("finance") || description.Contains("money") || description.Contains("budget"))
                return "Financial Services";
            if (description.Contains("travel") || description.Contains("booking") || description.Contains("hotel"))
                return "Travel & Hospitality";
            if (description.Contains("home") || description.Contains("house") || description.Contains("property"))
                return "Real Estate & Home";
            
            return "Technology";
        }
        
        private List<ValidationCriteria> GetQuickValidationCriteria()
        {
            return new List<ValidationCriteria>
            {
                new() { Name = "Problem Clarity", Description = "How clearly defined is the problem?", Weight = 30, Category = "Problem" },
                new() { Name = "Market Potential", Description = "Is there a clear target market?", Weight = 25, Category = "Market" },
                new() { Name = "Revenue Viability", Description = "Can this make money?", Weight = 25, Category = "Business" },
                new() { Name = "Feasibility", Description = "Can this be built?", Weight = 20, Category = "Execution" }
            };
        }
        
        private List<ValidationCriteria> GetDeepValidationCriteria()
        {
            return new List<ValidationCriteria>
            {
                new() { Name = "Problem Clarity", Description = "How clearly defined and quantified is the problem?", Weight = 20, Category = "Problem" },
                new() { Name = "Market Potential", Description = "Size and accessibility of target market", Weight = 20, Category = "Market" },
                new() { Name = "Revenue Viability", Description = "Realistic revenue model and pricing strategy", Weight = 20, Category = "Business" },
                new() { Name = "Competitive Position", Description = "Differentiation from existing solutions", Weight = 15, Category = "Competition" },
                new() { Name = "Execution Feasibility", Description = "Technical and operational complexity", Weight = 15, Category = "Execution" },
                new() { Name = "Innovation Factor", Description = "Uniqueness and potential for disruption", Weight = 10, Category = "Innovation" }
            };
        }
        
        private List<ValidationCriteria> GetLaunchValidationCriteria()
        {
            return new List<ValidationCriteria>
            {
                new() { Name = "Problem Clarity", Description = "Quantified problem with clear customer pain", Weight = 15, Category = "Problem" },
                new() { Name = "Market Analysis", Description = "Detailed market size, trends, and segmentation", Weight = 15, Category = "Market" },
                new() { Name = "Business Model", Description = "Complete revenue model with unit economics", Weight = 15, Category = "Business" },
                new() { Name = "Competitive Strategy", Description = "Detailed competitive analysis and positioning", Weight = 15, Category = "Competition" },
                new() { Name = "Go-to-Market Plan", Description = "Customer acquisition and distribution strategy", Weight = 15, Category = "Marketing" },
                new() { Name = "Product Strategy", Description = "MVP definition and development roadmap", Weight = 10, Category = "Product" },
                new() { Name = "Financial Projections", Description = "Revenue forecasts and funding requirements", Weight = 10, Category = "Finance" },
                new() { Name = "Risk Assessment", Description = "Key risks and mitigation strategies", Weight = 5, Category = "Risk" }
            };
        }
        
        private List<string> GetCategoryRecommendations(string category, int score)
        {
            return category switch
            {
                "Problem Clarity" => new List<string>
                {
                    "Be more specific about the exact problem you're solving",
                    "Quantify the impact - how much time/money/stress does this problem cause?",
                    "Interview potential customers to validate the problem exists"
                },
                "Market Potential" => new List<string>
                {
                    "Define your target audience more specifically with demographics and behaviors",
                    "Research the size of your potential market",
                    "Avoid targeting 'everyone' - focus on a specific customer segment"
                },
                "Revenue Viability" => new List<string>
                {
                    "Research pricing models used by similar solutions",
                    "Consider multiple revenue streams (subscription, one-time, commission)",
                    "Validate that customers would pay for this solution"
                },
                "Innovation Factor" => new List<string>
                {
                    "Identify what makes your approach unique or better",
                    "Research existing solutions to understand gaps",
                    "Consider combining existing solutions in a new way"
                },
                "Execution Feasibility" => new List<string>
                {
                    "Break down your idea into smaller, manageable components",
                    "Consider starting with a simpler version (MVP)",
                    "Assess what resources and skills you'll need"
                },
                _ => new List<string> { "Continue refining this aspect of your idea" }
            };
        }
    }
}