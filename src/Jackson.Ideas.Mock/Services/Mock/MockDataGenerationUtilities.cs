using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Utility class providing common mock data generation functionality
    /// Reduces code duplication across mock services and ensures consistency
    /// </summary>
    public static class MockDataGenerationUtilities
    {
        private static readonly Random _random = new();

        #region Business Types and Industries

        public static readonly Dictionary<string, string[]> BusinessTypeKeywords = new()
        {
            ["Technology"] = new[] { "platform", "app", "software", "AI", "cloud", "digital", "tech", "system" },
            ["E-commerce"] = new[] { "marketplace", "store", "retail", "shopping", "sales", "commerce" },
            ["Healthcare"] = new[] { "health", "medical", "wellness", "therapy", "care", "clinical" },
            ["Education"] = new[] { "learning", "education", "training", "course", "teaching", "academic" },
            ["Finance"] = new[] { "finance", "fintech", "banking", "payment", "investment", "financial" },
            ["Food & Beverage"] = new[] { "food", "restaurant", "meal", "dining", "beverage", "catering" },
            ["Professional Services"] = new[] { "consulting", "service", "professional", "advisory", "management" }
        };

        public static readonly string[] CommonBusinessChallenges = new[]
        {
            "Market competition and differentiation",
            "Customer acquisition and retention",
            "Scaling operations efficiently",
            "Technology integration and upgrades",
            "Regulatory compliance requirements",
            "Cash flow and financial management",
            "Talent acquisition and retention",
            "Supply chain optimization"
        };

        public static readonly string[] CommonSuccessFactors = new[]
        {
            "Strong product-market fit",
            "Effective marketing and branding",
            "Operational excellence",
            "Customer service quality",
            "Financial discipline",
            "Innovation and adaptability",
            "Strategic partnerships",
            "Team expertise and culture"
        };

        #endregion

        #region Score Generation

        /// <summary>
        /// Generates a realistic score based on business idea complexity and market factors
        /// </summary>
        public static int GenerateRealisticScore(string businessIdea, int baseScore = 75, int variance = 15)
        {
            var complexityBonus = CalculateComplexityBonus(businessIdea);
            var marketFactorBonus = CalculateMarketFactorBonus(businessIdea);
            
            var score = baseScore + complexityBonus + marketFactorBonus + _random.Next(-variance, variance);
            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Calculates bonus points based on business idea complexity and innovation
        /// </summary>
        private static int CalculateComplexityBonus(string businessIdea)
        {
            var idea = businessIdea.ToLowerInvariant();
            var bonus = 0;

            // AI/ML bonus
            if (idea.Contains("ai") || idea.Contains("artificial intelligence") || idea.Contains("machine learning"))
                bonus += 10;

            // Innovation keywords
            if (idea.Contains("revolutionary") || idea.Contains("innovative") || idea.Contains("disruptive"))
                bonus += 5;

            // Blockchain/crypto
            if (idea.Contains("blockchain") || idea.Contains("crypto"))
                bonus += 8;

            // Social impact
            if (idea.Contains("sustainable") || idea.Contains("environment") || idea.Contains("social"))
                bonus += 7;

            return Math.Min(bonus, 20); // Cap at 20 points
        }

        /// <summary>
        /// Calculates bonus/penalty based on market factors
        /// </summary>
        private static int CalculateMarketFactorBonus(string businessIdea)
        {
            var idea = businessIdea.ToLowerInvariant();
            var bonus = 0;

            // High-demand markets
            if (idea.Contains("health") || idea.Contains("education") || idea.Contains("finance"))
                bonus += 8;

            // Saturated markets penalty
            if (idea.Contains("social media") && !idea.Contains("innovative"))
                bonus -= 10;

            // B2B vs B2C considerations
            if (idea.Contains("enterprise") || idea.Contains("business"))
                bonus += 5;

            return Math.Max(-15, Math.Min(15, bonus)); // Cap between -15 and +15
        }

        #endregion

        #region Timeline Generation

        /// <summary>
        /// Generates realistic timeline based on business type and complexity
        /// </summary>
        public static int GenerateRealisticTimelineWeeks(string businessIdea, string businessType)
        {
            var baseWeeks = businessType?.ToLowerInvariant() switch
            {
                "technology" => 20,
                "e-commerce" => 16,
                "healthcare" => 32,
                "finance" => 28,
                "professional services" => 12,
                _ => 18
            };

            var complexityMultiplier = CalculateComplexityMultiplier(businessIdea);
            var finalWeeks = (int)(baseWeeks * complexityMultiplier);

            return Math.Max(8, Math.Min(52, finalWeeks)); // Between 8 and 52 weeks
        }

        private static double CalculateComplexityMultiplier(string businessIdea)
        {
            var idea = businessIdea.ToLowerInvariant();
            var multiplier = 1.0;

            if (idea.Contains("ai") || idea.Contains("machine learning")) multiplier += 0.5;
            if (idea.Contains("blockchain")) multiplier += 0.4;
            if (idea.Contains("platform") || idea.Contains("marketplace")) multiplier += 0.3;
            if (idea.Contains("mobile") && idea.Contains("web")) multiplier += 0.2;

            return Math.Max(0.5, Math.Min(2.0, multiplier));
        }

        #endregion

        #region Financial Projections

        /// <summary>
        /// Generates realistic financial projections based on business parameters
        /// </summary>
        public static (decimal Revenue, decimal Costs, decimal Profit) GenerateFinancialProjections(
            decimal initialInvestment, string revenueModel, int targetCustomers)
        {
            var monthlyRevenuePerCustomer = revenueModel?.ToLowerInvariant() switch
            {
                var model when model.Contains("subscription") => _random.Next(10, 100),
                var model when model.Contains("freemium") => _random.Next(5, 50),
                var model when model.Contains("enterprise") => _random.Next(100, 1000),
                var model when model.Contains("transaction") => _random.Next(1, 20),
                _ => _random.Next(20, 80)
            };

            var projectedRevenue = targetCustomers * monthlyRevenuePerCustomer * 12 * 0.7m; // 70% conversion assumption
            var projectedCosts = initialInvestment * 0.8m; // 80% of investment as operational costs
            var projectedProfit = projectedRevenue - projectedCosts;

            return (projectedRevenue, projectedCosts, projectedProfit);
        }

        #endregion

        #region Market Analysis

        /// <summary>
        /// Generates market size estimates based on business type and target audience
        /// </summary>
        public static MarketSizeEstimate GenerateMarketSizeEstimate(string businessType, string targetAudience)
        {
            var baseMarketSize = businessType?.ToLowerInvariant() switch
            {
                "technology" => _random.Next(1000000, 10000000),
                "healthcare" => _random.Next(500000, 5000000),
                "finance" => _random.Next(2000000, 20000000),
                "education" => _random.Next(800000, 8000000),
                _ => _random.Next(500000, 5000000)
            };

            var targetMarketSize = (int)(baseMarketSize * _random.NextDouble() * 0.3 + 0.1); // 10-40% of total market
            var servicableMarketSize = (int)(targetMarketSize * _random.NextDouble() * 0.5 + 0.2); // 20-70% of target market

            return new MarketSizeEstimate
            {
                TotalAddressableMarket = baseMarketSize,
                ServiceableAddressableMarket = targetMarketSize,
                ServiceableObtainableMarket = servicableMarketSize
            };
        }

        #endregion

        #region Resource Planning

        /// <summary>
        /// Generates realistic resource requirements based on business parameters
        /// </summary>
        public static ResourceRequirements GenerateResourceRequirements(decimal budget, string businessType, int teamSize = 0)
        {
            var suggestedTeamSize = teamSize > 0 ? teamSize : CalculateOptimalTeamSize(budget, businessType);
            
            var humanResourcesBudget = budget * 0.4m;
            var technologyBudget = budget * 0.25m;
            var marketingBudget = budget * 0.2m;
            var operationalBudget = budget * 0.15m;

            return new ResourceRequirements
            {
                RecommendedTeamSize = suggestedTeamSize,
                HumanResourcesBudget = humanResourcesBudget,
                TechnologyBudget = technologyBudget,
                MarketingBudget = marketingBudget,
                OperationalBudget = operationalBudget,
                TotalBudget = budget
            };
        }

        private static int CalculateOptimalTeamSize(decimal budget, string businessType)
        {
            var baseTeamSize = businessType?.ToLowerInvariant() switch
            {
                "technology" => 5,
                "e-commerce" => 4,
                "professional services" => 3,
                "healthcare" => 6,
                "finance" => 5,
                _ => 4
            };

            // Adjust based on budget (assuming $60k average salary)
            var budgetFactor = Math.Max(0.5, Math.Min(2.0, (double)(budget / 300000m)));
            return (int)(baseTeamSize * budgetFactor);
        }

        #endregion

        #region Quality Metrics

        /// <summary>
        /// Generates realistic quality and performance metrics
        /// </summary>
        public static QualityMetrics GenerateQualityMetrics(string businessType)
        {
            return new QualityMetrics
            {
                CustomerSatisfactionTarget = _random.Next(85, 95),
                DefectRateTarget = Math.Round(_random.NextDouble() * 2.0 + 0.5, 2), // 0.5-2.5%
                ResponseTimeTarget = businessType?.ToLowerInvariant() switch
                {
                    "technology" => $"{_random.Next(100, 500)}ms",
                    "e-commerce" => $"{_random.Next(1, 4)} seconds",
                    "professional services" => $"{_random.Next(2, 24)} hours",
                    _ => $"{_random.Next(1, 8)} hours"
                },
                UptimeTarget = Math.Round(99.0 + _random.NextDouble() * 0.9, 2), // 99.0-99.9%
                ProcessEfficiencyTarget = _random.Next(90, 98)
            };
        }

        #endregion

        #region Helper Classes

        public class MarketSizeEstimate
        {
            public int TotalAddressableMarket { get; set; }
            public int ServiceableAddressableMarket { get; set; }
            public int ServiceableObtainableMarket { get; set; }
        }

        public class ResourceRequirements
        {
            public int RecommendedTeamSize { get; set; }
            public decimal HumanResourcesBudget { get; set; }
            public decimal TechnologyBudget { get; set; }
            public decimal MarketingBudget { get; set; }
            public decimal OperationalBudget { get; set; }
            public decimal TotalBudget { get; set; }
        }

        public class QualityMetrics
        {
            public int CustomerSatisfactionTarget { get; set; }
            public double DefectRateTarget { get; set; }
            public string ResponseTimeTarget { get; set; } = string.Empty;
            public double UptimeTarget { get; set; }
            public int ProcessEfficiencyTarget { get; set; }
        }

        #endregion

        #region Text Generation Helpers

        /// <summary>
        /// Generates contextual recommendations based on business idea analysis
        /// </summary>
        public static List<string> GenerateContextualRecommendations(string businessIdea, string targetMarket, int count = 5)
        {
            var recommendations = new List<string>();
            var idea = businessIdea.ToLowerInvariant();
            var market = targetMarket.ToLowerInvariant();

            // Technology-specific recommendations
            if (idea.Contains("app") || idea.Contains("software") || idea.Contains("platform"))
            {
                recommendations.Add("Focus on user experience and intuitive design");
                recommendations.Add("Implement robust security and data protection measures");
                recommendations.Add("Plan for scalable architecture from the beginning");
            }

            // Market-specific recommendations
            if (market.Contains("small business") || market.Contains("sme"))
            {
                recommendations.Add("Emphasize ease of implementation and quick ROI");
                recommendations.Add("Provide comprehensive support and training resources");
                recommendations.Add("Consider flexible pricing tiers for different business sizes");
            }

            // Add general business recommendations to reach target count
            var generalRecommendations = new[]
            {
                "Conduct thorough market validation before full development",
                "Build strong partnerships with key industry players",
                "Establish clear metrics for measuring success",
                "Develop a comprehensive go-to-market strategy",
                "Invest in building a strong brand presence",
                "Plan for regulatory compliance and legal requirements",
                "Focus on customer retention and lifetime value",
                "Implement data-driven decision making processes"
            };

            recommendations.AddRange(generalRecommendations.Take(Math.Max(0, count - recommendations.Count)));
            
            return recommendations.Take(count).ToList();
        }

        /// <summary>
        /// Determines business type from business idea text
        /// </summary>
        public static string DetermineBusinessType(string businessIdea)
        {
            var idea = businessIdea.ToLowerInvariant();
            
            foreach (var (businessType, keywords) in BusinessTypeKeywords)
            {
                if (keywords.Any(keyword => idea.Contains(keyword)))
                {
                    return businessType;
                }
            }
            
            return "Technology"; // Default fallback
        }

        #endregion
    }
}