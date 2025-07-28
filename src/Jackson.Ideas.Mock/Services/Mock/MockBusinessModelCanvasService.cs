using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;

namespace Jackson.Ideas.Mock.Services.Mock
{
    /// <summary>
    /// Mock implementation of business model canvas service with realistic collaborative features
    /// </summary>
    public class MockBusinessModelCanvasService : IBusinessModelCanvasService
    {
        private readonly Dictionary<string, BusinessModelCanvas> _canvases = new();
        private readonly Dictionary<string, CollaborationSession> _sessions = new();
        private readonly Dictionary<string, List<CanvasVersion>> _versions = new();
        
        public Task<BusinessModelCanvas> CreateCanvasAsync(CanvasCreationRequest request)
        {
            var canvas = new BusinessModelCanvas
            {
                BusinessPlanId = request.BusinessPlanId,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Initialize with sample data for HealthyKids Meal Planner
            PopulateCanvasWithSampleData(canvas, request);
            
            _canvases[canvas.Id] = canvas;
            
            // Create initial version
            CreateInitialVersion(canvas);
            
            return Task.FromResult(canvas);
        }
        
        public Task<BusinessModelCanvas?> GetCanvasAsync(string canvasId)
        {
            // For demo purposes, create a sample canvas if none exists
            if (!_canvases.ContainsKey(canvasId))
            {
                var demoRequest = new CanvasCreationRequest
                {
                    BusinessPlanId = "demo-plan",
                    CanvasName = "HealthyKids Meal Planner Canvas",
                    BusinessType = "SaaS",
                    Industry = "HealthTech",
                    UseAIAssistance = true
                };
                
                return CreateCanvasAsync(demoRequest);
            }
            
            return Task.FromResult<BusinessModelCanvas?>(_canvases[canvasId]);
        }
        
        public Task<BusinessModelCanvas> UpdateCanvasSectionAsync(string canvasId, string sectionName, List<string> items)
        {
            if (_canvases.TryGetValue(canvasId, out var canvas))
            {
                // Update the specific section
                switch (sectionName.ToLower())
                {
                    case "keypartners":
                        canvas.KeyPartners = items;
                        break;
                    case "keyactivities":
                        canvas.KeyActivities = items;
                        break;
                    case "keyresources":
                        canvas.KeyResources = items;
                        break;
                    case "valuepropositions":
                        canvas.ValuePropositions = items;
                        break;
                    case "customerrelationships":
                        canvas.CustomerRelationships = items;
                        break;
                    case "channels":
                        canvas.Channels = items;
                        break;
                    case "customersegments":
                        canvas.CustomerSegments = items;
                        break;
                    case "coststructure":
                        canvas.CostStructure = items;
                        break;
                    case "revenuestreams":
                        canvas.RevenueStreams = items;
                        break;
                }
                
                canvas.UpdatedAt = DateTime.UtcNow;
                
                // Create new version for this update
                CreateVersionForUpdate(canvas, sectionName, items);
            }
            
            return Task.FromResult(_canvases[canvasId]);
        }
        
        public Task<List<CanvasTemplate>> GetCanvasTemplatesAsync()
        {
            return Task.FromResult(new List<CanvasTemplate>
            {
                new()
                {
                    Id = "saas-template",
                    Name = "SaaS Business Model",
                    Description = "Template for software-as-a-service businesses with subscription revenue",
                    BusinessType = "SaaS",
                    Industry = "Technology",
                    Tags = new() { "subscription", "software", "scalable", "recurring revenue" },
                    TemplateCanvas = CreateSaaSTemplate()
                },
                new()
                {
                    Id = "marketplace-template",
                    Name = "Marketplace Platform",
                    Description = "Two-sided marketplace connecting buyers and sellers",
                    BusinessType = "Marketplace",
                    Industry = "Platform",
                    Tags = new() { "platform", "network effects", "commission", "multi-sided" },
                    TemplateCanvas = CreateMarketplaceTemplate()
                },
                new()
                {
                    Id = "ecommerce-template",
                    Name = "E-commerce Business",
                    Description = "Online retail business selling products directly to consumers",
                    BusinessType = "E-commerce",
                    Industry = "Retail",
                    Tags = new() { "retail", "inventory", "logistics", "d2c" },
                    TemplateCanvas = CreateEcommerceTemplate()
                },
                new()
                {
                    Id = "freemium-template",
                    Name = "Freemium Model",
                    Description = "Free basic service with premium paid features",
                    BusinessType = "Freemium",
                    Industry = "Technology",
                    Tags = new() { "freemium", "conversion", "viral", "premium" },
                    TemplateCanvas = CreateFreemiumTemplate()
                }
            });
        }
        
        public Task<BusinessModelCanvas> ApplyTemplateAsync(string canvasId, string templateId)
        {
            if (_canvases.TryGetValue(canvasId, out var canvas))
            {
                var templates = GetCanvasTemplatesAsync().Result;
                var template = templates.FirstOrDefault(t => t.Id == templateId);
                
                if (template != null)
                {
                    // Apply template while preserving canvas metadata
                    var originalId = canvas.Id;
                    var originalBusinessPlanId = canvas.BusinessPlanId;
                    var originalCreatedAt = canvas.UpdatedAt;
                    
                    // Copy template structure
                    canvas.KeyPartners = new List<string>(template.TemplateCanvas.KeyPartners);
                    canvas.KeyActivities = new List<string>(template.TemplateCanvas.KeyActivities);
                    canvas.KeyResources = new List<string>(template.TemplateCanvas.KeyResources);
                    canvas.ValuePropositions = new List<string>(template.TemplateCanvas.ValuePropositions);
                    canvas.CustomerRelationships = new List<string>(template.TemplateCanvas.CustomerRelationships);
                    canvas.Channels = new List<string>(template.TemplateCanvas.Channels);
                    canvas.CustomerSegments = new List<string>(template.TemplateCanvas.CustomerSegments);
                    canvas.CostStructure = new List<string>(template.TemplateCanvas.CostStructure);
                    canvas.RevenueStreams = new List<string>(template.TemplateCanvas.RevenueStreams);
                    
                    // Restore metadata
                    canvas.Id = originalId;
                    canvas.BusinessPlanId = originalBusinessPlanId;
                    canvas.UpdatedAt = DateTime.UtcNow;
                    
                    CreateVersionForUpdate(canvas, "Template Applied", new() { template.Name });
                }
            }
            
            return Task.FromResult(_canvases[canvasId]);
        }
        
        public Task<List<CanvasInsight>> GetCanvasInsightsAsync(string canvasId)
        {
            return Task.FromResult(new List<CanvasInsight>
            {
                new()
                {
                    Title = "Strong Value Proposition",
                    Description = "Your value proposition clearly addresses a specific pain point for parents",
                    Section = "Value Propositions",
                    Type = "Strength",
                    Priority = 1,
                    Impact = "High - Clear value proposition increases conversion rates",
                    ActionItem = "Continue emphasizing time-saving and health benefits in marketing"
                },
                new()
                {
                    Title = "Limited Revenue Diversification",
                    Description = "Heavy reliance on subscription revenue may limit growth potential",
                    Section = "Revenue Streams",
                    Type = "Weakness",
                    Priority = 2,
                    Impact = "Medium - Additional revenue streams could increase profitability",
                    ActionItem = "Consider adding premium services, partnerships, or data insights revenue"
                },
                new()
                {
                    Title = "Partnership Opportunity",
                    Description = "Potential partnerships with grocery stores and meal kit services",
                    Section = "Key Partners",
                    Type = "Opportunity",
                    Priority = 2,
                    Impact = "High - Partnerships could significantly expand customer acquisition",
                    ActionItem = "Reach out to regional grocery chains for integration partnerships"
                },
                new()
                {
                    Title = "Competitive Pressure",
                    Description = "Large players like Amazon and Google may enter this space",
                    Section = "Key Activities",
                    Type = "Threat",
                    Priority = 3,
                    Impact = "Medium - Need to build strong moats and customer loyalty",
                    ActionItem = "Focus on specialized features and superior user experience"
                },
                new()
                {
                    Title = "AI Integration Enhancement",
                    Description = "Leverage AI more extensively for personalization",
                    Section = "Key Resources",
                    Type = "Recommendation",
                    Priority = 1,
                    Impact = "High - Advanced AI can significantly improve recommendations",
                    ActionItem = "Invest in machine learning capabilities and data collection"
                }
            });
        }
        
        public Task<CanvasValidation> ValidateCanvasAsync(string canvasId)
        {
            var canvas = _canvases.GetValueOrDefault(canvasId);
            if (canvas == null)
            {
                return Task.FromResult(new CanvasValidation
                {
                    CanvasId = canvasId,
                    IsValid = false,
                    Issues = new() { new() { Section = "General", Severity = "Critical", Message = "Canvas not found" } }
                });
            }
            
            var validation = new CanvasValidation
            {
                CanvasId = canvasId,
                CompletenessScore = CalculateCompletenessScore(canvas),
                Issues = ValidateCanvasSections(canvas),
                Strengths = IdentifyStrengths(canvas),
                Recommendations = GenerateRecommendations(canvas)
            };
            
            validation.IsValid = validation.Issues.All(i => i.Severity != "Critical");
            
            return Task.FromResult(validation);
        }
        
        public Task<CanvasExport> ExportCanvasAsync(string canvasId, ExportFormat format)
        {
            var export = new CanvasExport
            {
                CanvasId = canvasId,
                Format = format,
                FileName = $"business-model-canvas-{canvasId}.{format.ToString().ToLower()}",
                Data = GenerateMockExportData(format)
            };
            
            return Task.FromResult(export);
        }
        
        public Task<CollaborationSession> CreateCollaborationSessionAsync(string canvasId)
        {
            var session = new CollaborationSession
            {
                CanvasId = canvasId,
                SessionName = $"Canvas Collaboration - {DateTime.Now:MMM dd, yyyy}",
                ShareLink = $"https://app.healthykids.com/canvas/{canvasId}/collaborate/{Guid.NewGuid():N}",
                Participants = new()
                {
                    new() { UserId = "user1", Name = "Sarah Johnson", Email = "sarah@healthykids.com", Role = "Owner", Color = "#3b82f6", IsOnline = true },
                    new() { UserId = "user2", Name = "Mike Chen", Email = "mike@healthykids.com", Role = "Editor", Color = "#10b981", IsOnline = true },
                    new() { UserId = "user3", Name = "Lisa Rodriguez", Email = "lisa@healthykids.com", Role = "Viewer", Color = "#f59e0b", IsOnline = false }
                }
            };
            
            _sessions[session.Id] = session;
            
            return Task.FromResult(session);
        }
        
        public Task<List<CanvasVersion>> GetCanvasVersionsAsync(string canvasId)
        {
            return Task.FromResult(_versions.GetValueOrDefault(canvasId, new List<CanvasVersion>()));
        }
        
        private void PopulateCanvasWithSampleData(BusinessModelCanvas canvas, CanvasCreationRequest request)
        {
            // Sample data for HealthyKids Meal Planner
            canvas.KeyPartners = new()
            {
                "Nutrition database providers (USDA)",
                "Recipe content creators",
                "Grocery store chains",
                "Pediatric nutritionists",
                "Meal kit delivery services"
            };
            
            canvas.KeyActivities = new()
            {
                "AI-powered meal recommendation engine",
                "Recipe database curation and management",
                "Mobile app development and maintenance",
                "Customer support and community building",
                "Data analysis and insights generation"
            };
            
            canvas.KeyResources = new()
            {
                "AI/ML recommendation algorithms",
                "Comprehensive recipe and nutrition database",
                "Mobile and web development team",
                "User data and behavioral insights",
                "Brand and customer trust"
            };
            
            canvas.ValuePropositions = new()
            {
                "Save time on meal planning for busy parents",
                "Ensure healthy nutrition for children",
                "Reduce food waste through smart planning",
                "Discover new kid-friendly healthy recipes",
                "Simplify grocery shopping with smart lists"
            };
            
            canvas.CustomerRelationships = new()
            {
                "Self-service mobile app experience",
                "Community forums and recipe sharing",
                "Personalized email recommendations",
                "Customer support chat and help center",
                "Social media engagement and tips"
            };
            
            canvas.Channels = new()
            {
                "iOS and Android mobile apps",
                "Web application platform",
                "Social media marketing (Instagram, Facebook)",
                "Parenting blogs and influencer partnerships",
                "App store optimization and discovery"
            };
            
            canvas.CustomerSegments = new()
            {
                "Working parents with children ages 3-12",
                "Health-conscious families",
                "Parents with picky eaters",
                "Busy professionals managing family meals",
                "Parents with dietary restrictions/allergies"
            };
            
            canvas.CostStructure = new()
            {
                "Software development and engineering (40%)",
                "Cloud infrastructure and data storage (15%)",
                "Customer acquisition and marketing (25%)",
                "Content creation and recipe curation (10%)",
                "Operations and customer support (10%)"
            };
            
            canvas.RevenueStreams = new()
            {
                "Monthly subscription fees ($9.99/month)",
                "Premium family plans ($14.99/month)",
                "Annual subscription discounts",
                "Partner referral commissions",
                "Premium recipe content partnerships"
            };
        }
        
        private BusinessModelCanvas CreateSaaSTemplate()
        {
            return new BusinessModelCanvas
            {
                KeyPartners = new() { "Technology providers", "Integration partners", "Channel partners", "Industry experts" },
                KeyActivities = new() { "Software development", "Customer support", "Sales and marketing", "Product management" },
                KeyResources = new() { "Development team", "Technology platform", "Customer data", "Brand" },
                ValuePropositions = new() { "Scalable solution", "Cost reduction", "Improved efficiency", "24/7 availability" },
                CustomerRelationships = new() { "Self-service", "Dedicated support", "Online community", "Training programs" },
                Channels = new() { "Website", "Direct sales", "Partner channels", "App stores" },
                CustomerSegments = new() { "SMB market", "Enterprise clients", "Specific industry verticals" },
                CostStructure = new() { "Development costs", "Infrastructure", "Sales & marketing", "Customer support" },
                RevenueStreams = new() { "Subscription fees", "Premium features", "Professional services", "Training" }
            };
        }
        
        private BusinessModelCanvas CreateMarketplaceTemplate()
        {
            return new BusinessModelCanvas
            {
                KeyPartners = new() { "Payment processors", "Logistics providers", "Marketing partners", "Technology vendors" },
                KeyActivities = new() { "Platform development", "User acquisition", "Quality control", "Community management" },
                KeyResources = new() { "Platform technology", "User network", "Brand trust", "Data analytics" },
                ValuePropositions = new() { "Easy discovery", "Trust and safety", "Convenient transactions", "Wide selection" },
                CustomerRelationships = new() { "Self-service platform", "Community features", "Support systems", "Rating systems" },
                Channels = new() { "Website platform", "Mobile apps", "Social media", "Word of mouth" },
                CustomerSegments = new() { "Buyers/consumers", "Sellers/providers", "Enterprise clients" },
                CostStructure = new() { "Platform development", "Marketing", "Payment processing", "Customer support" },
                RevenueStreams = new() { "Transaction fees", "Listing fees", "Subscription plans", "Advertising revenue" }
            };
        }
        
        private BusinessModelCanvas CreateEcommerceTemplate()
        {
            return new BusinessModelCanvas
            {
                KeyPartners = new() { "Suppliers", "Logistics partners", "Payment providers", "Marketing agencies" },
                KeyActivities = new() { "Inventory management", "Order fulfillment", "Customer service", "Marketing" },
                KeyResources = new() { "Inventory", "Fulfillment centers", "E-commerce platform", "Customer database" },
                ValuePropositions = new() { "Product quality", "Fast delivery", "Competitive pricing", "Customer service" },
                CustomerRelationships = new() { "Self-service website", "Customer support", "Loyalty programs", "Email marketing" },
                Channels = new() { "Online store", "Mobile app", "Social commerce", "Marketplace presence" },
                CustomerSegments = new() { "Online shoppers", "Mobile users", "Loyalty customers", "Gift buyers" },
                CostStructure = new() { "Cost of goods", "Fulfillment", "Marketing", "Technology", "Operations" },
                RevenueStreams = new() { "Product sales", "Shipping fees", "Premium memberships", "Affiliate commissions" }
            };
        }
        
        private BusinessModelCanvas CreateFreemiumTemplate()
        {
            return new BusinessModelCanvas
            {
                KeyPartners = new() { "Technology partners", "Content providers", "Distribution partners", "Analytics providers" },
                KeyActivities = new() { "Product development", "User acquisition", "Conversion optimization", "Customer success" },
                KeyResources = new() { "Technology platform", "User base", "Data insights", "Product team" },
                ValuePropositions = new() { "Free value delivery", "Premium enhanced features", "No commitment trial", "Scalable solution" },
                CustomerRelationships = new() { "Self-service free tier", "Premium support", "Community", "Educational content" },
                Channels = new() { "Website", "App stores", "Viral/referral", "Content marketing" },
                CustomerSegments = new() { "Free users", "Premium subscribers", "Enterprise clients" },
                CostStructure = new() { "Development", "Infrastructure", "User acquisition", "Support" },
                RevenueStreams = new() { "Premium subscriptions", "Enterprise plans", "Add-on features", "Professional services" }
            };
        }
        
        private void CreateInitialVersion(BusinessModelCanvas canvas)
        {
            var version = new CanvasVersion
            {
                CanvasId = canvas.Id,
                VersionNumber = 1,
                Description = "Initial canvas creation",
                CanvasSnapshot = CloneCanvas(canvas),
                CreatedBy = "System",
                Changes = new() { "Canvas created with initial structure" }
            };
            
            if (!_versions.ContainsKey(canvas.Id))
            {
                _versions[canvas.Id] = new List<CanvasVersion>();
            }
            
            _versions[canvas.Id].Add(version);
        }
        
        private void CreateVersionForUpdate(BusinessModelCanvas canvas, string sectionName, List<string> items)
        {
            var versions = _versions.GetValueOrDefault(canvas.Id, new List<CanvasVersion>());
            var nextVersion = versions.Count + 1;
            
            var version = new CanvasVersion
            {
                CanvasId = canvas.Id,
                VersionNumber = nextVersion,
                Description = $"Updated {sectionName}",
                CanvasSnapshot = CloneCanvas(canvas),
                CreatedBy = "User",
                Changes = new() { $"Modified {sectionName}: {string.Join(", ", items.Take(3))}{(items.Count > 3 ? "..." : "")}" }
            };
            
            versions.Add(version);
            _versions[canvas.Id] = versions;
        }
        
        private BusinessModelCanvas CloneCanvas(BusinessModelCanvas original)
        {
            return new BusinessModelCanvas
            {
                Id = original.Id,
                BusinessPlanId = original.BusinessPlanId,
                KeyPartners = new List<string>(original.KeyPartners),
                KeyActivities = new List<string>(original.KeyActivities),
                KeyResources = new List<string>(original.KeyResources),
                ValuePropositions = new List<string>(original.ValuePropositions),
                CustomerRelationships = new List<string>(original.CustomerRelationships),
                Channels = new List<string>(original.Channels),
                CustomerSegments = new List<string>(original.CustomerSegments),
                CostStructure = new List<string>(original.CostStructure),
                RevenueStreams = new List<string>(original.RevenueStreams),
                UpdatedAt = original.UpdatedAt
            };
        }
        
        private int CalculateCompletenessScore(BusinessModelCanvas canvas)
        {
            int totalSections = 9;
            int completedSections = 0;
            
            if (canvas.KeyPartners.Any()) completedSections++;
            if (canvas.KeyActivities.Any()) completedSections++;
            if (canvas.KeyResources.Any()) completedSections++;
            if (canvas.ValuePropositions.Any()) completedSections++;
            if (canvas.CustomerRelationships.Any()) completedSections++;
            if (canvas.Channels.Any()) completedSections++;
            if (canvas.CustomerSegments.Any()) completedSections++;
            if (canvas.CostStructure.Any()) completedSections++;
            if (canvas.RevenueStreams.Any()) completedSections++;
            
            return (int)Math.Round((double)completedSections / totalSections * 100);
        }
        
        private List<ValidationIssue> ValidateCanvasSections(BusinessModelCanvas canvas)
        {
            var issues = new List<ValidationIssue>();
            
            if (!canvas.ValuePropositions.Any())
            {
                issues.Add(new ValidationIssue
                {
                    Section = "Value Propositions",
                    Severity = "Critical",
                    Message = "Value propositions are missing",
                    Suggestion = "Define at least 2-3 clear value propositions that address customer pain points"
                });
            }
            
            if (!canvas.CustomerSegments.Any())
            {
                issues.Add(new ValidationIssue
                {
                    Section = "Customer Segments",
                    Severity = "Critical",
                    Message = "Customer segments are not defined",
                    Suggestion = "Identify and describe your target customer segments"
                });
            }
            
            if (!canvas.RevenueStreams.Any())
            {
                issues.Add(new ValidationIssue
                {
                    Section = "Revenue Streams",
                    Severity = "Warning",
                    Message = "Revenue streams need clarification",
                    Suggestion = "Define how your business will generate revenue from each customer segment"
                });
            }
            
            if (canvas.KeyPartners.Count < 2)
            {
                issues.Add(new ValidationIssue
                {
                    Section = "Key Partners",
                    Severity = "Info",
                    Message = "Consider expanding key partnerships",
                    Suggestion = "Identify strategic partners that can help scale your business"
                });
            }
            
            return issues;
        }
        
        private List<string> IdentifyStrengths(BusinessModelCanvas canvas)
        {
            var strengths = new List<string>();
            
            if (canvas.ValuePropositions.Count >= 3)
            {
                strengths.Add("Strong value proposition with multiple benefits");
            }
            
            if (canvas.Channels.Count >= 3)
            {
                strengths.Add("Diversified channel strategy");
            }
            
            if (canvas.RevenueStreams.Count >= 2)
            {
                strengths.Add("Multiple revenue streams provide stability");
            }
            
            if (canvas.KeyResources.Any(r => r.ToLower().Contains("data") || r.ToLower().Contains("ai")))
            {
                strengths.Add("Technology and data-driven approach");
            }
            
            return strengths;
        }
        
        private List<string> GenerateRecommendations(BusinessModelCanvas canvas)
        {
            var recommendations = new List<string>();
            
            recommendations.Add("Consider partnerships with complementary businesses to expand reach");
            recommendations.Add("Evaluate additional revenue streams such as premium services or data insights");
            recommendations.Add("Implement customer feedback loops to continuously refine value propositions");
            recommendations.Add("Develop competitive moats through proprietary technology or network effects");
            
            return recommendations;
        }
        
        private byte[] GenerateMockExportData(ExportFormat format)
        {
            string content = $"Mock {format} export data - Business Model Canvas";
            return System.Text.Encoding.UTF8.GetBytes(content);
        }
    }
}