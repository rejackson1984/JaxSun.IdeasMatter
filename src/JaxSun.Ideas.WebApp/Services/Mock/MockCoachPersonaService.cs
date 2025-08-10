using JaxSun.Ideas.WebApp.Models;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using CoachingContextChangedEventArgs = JaxSun.Ideas.WebApp.Services.Interfaces.CoachingContextChangedEventArgs;

namespace JaxSun.Ideas.WebApp.Services.Mock
{
    /// <summary>
    /// Mock implementation of coach persona service with predefined coach personalities
    /// </summary>
    public class MockCoachPersonaService : ICoachPersonaService
    {
        private readonly Dictionary<BusinessHub, CoachPersona> _coachPersonas;
        private readonly Dictionary<string, CoachingSession> _activeSessions;
        
        public event EventHandler<CoachingContextChangedEventArgs>? CoachingContextChanged;
        
        public MockCoachPersonaService()
        {
            _coachPersonas = InitializeCoachPersonas();
            _activeSessions = new Dictionary<string, CoachingSession>();
        }
        
        public Task<CoachPersona> GetCoachForHubAsync(BusinessHub hub)
        {
            return Task.FromResult(_coachPersonas.GetValueOrDefault(hub, new CoachPersona()));
        }
        
        public Task<Dictionary<BusinessHub, CoachPersona>> GetAllCoachPersonasAsync()
        {
            return Task.FromResult(_coachPersonas);
        }
        
        public async Task<string> GetCoachMessageAsync(BusinessHub hub, CoachingContext context)
        {
            var persona = await GetCoachForHubAsync(hub);
            
            // Handle null context gracefully
            if (context == null)
            {
                return persona.Greeting;
            }
            
            // Generate contextual message based on hub and progress
            if (context.IsFirstTimeInHub)
            {
                return persona.Greeting;
            }
            
            if (context.UserProgress > 80)
            {
                return GetHighProgressMessage(persona, context.UserProgress);
            }
            
            if (context.UserProgress < 20)
            {
                return GetLowProgressMessage(persona);
            }
            
            return GetMidProgressMessage(persona, context);
        }
        
        public async Task<List<CoachingSuggestion>> GetCoachingSuggestionsAsync(BusinessHub hub, CoachingContext context)
        {
            var persona = await GetCoachForHubAsync(hub);
            var suggestions = new List<CoachingSuggestion>();
            
            switch (hub)
            {
                case BusinessHub.IdeaDevelopment:
                    suggestions.AddRange(GetIdeaDevelopmentSuggestions(context));
                    break;
                case BusinessHub.BusinessPlanning:
                    suggestions.AddRange(GetBusinessPlanningSuggestions(context));
                    break;
                case BusinessHub.BusinessOperations:
                    suggestions.AddRange(GetBusinessOperationsSuggestions(context));
                    break;
            }
            
            return suggestions.OrderByDescending(s => s.Priority).Take(3).ToList();
        }
        
        public Task<CoachingSession> StartCoachingSessionAsync(string userId, BusinessHub hub)
        {
            var session = new CoachingSession
            {
                UserId = userId,
                Hub = hub,
                PersonaName = _coachPersonas[hub].Name,
                StartTime = DateTime.UtcNow,
                IsActive = true
            };
            
            _activeSessions[userId] = session;
            return Task.FromResult(session);
        }
        
        public Task EndCoachingSessionAsync(string sessionId)
        {
            var session = _activeSessions.Values.FirstOrDefault(s => s.Id == sessionId);
            if (session != null)
            {
                session.EndTime = DateTime.UtcNow;
                session.IsActive = false;
                _activeSessions.Remove(session.UserId);
            }
            
            return Task.CompletedTask;
        }
        
        public Task AddCoachingMessageAsync(string sessionId, CoachingMessage message)
        {
            var session = _activeSessions.Values.FirstOrDefault(s => s.Id == sessionId);
            if (session != null)
            {
                session.Messages.Add(message);
                session.InteractionCount++;
            }
            
            return Task.CompletedTask;
        }
        
        public Task<CoachingSession?> GetActiveSessionAsync(string userId)
        {
            _activeSessions.TryGetValue(userId, out var session);
            return Task.FromResult(session);
        }
        
        public async Task<string> GetGreetingMessageAsync(BusinessHub hub, bool isReturningUser = false)
        {
            var persona = await GetCoachForHubAsync(hub);
            
            if (isReturningUser)
            {
                return persona.Style switch
                {
                    CoachingStyle.Enthusiastic => $"Welcome back! {persona.Name} here - ready to spark some amazing progress on your ideas! ⚡",
                    CoachingStyle.Analytical => $"Good to see you again. I'm {persona.Name}, and I'm here to help you strategically build your business foundation.",
                    CoachingStyle.ResultsOriented => $"Welcome back to the operations hub. {persona.Name} reporting - let's focus on scaling your success. 📈",
                    _ => persona.Greeting
                };
            }
            
            return persona.Greeting;
        }
        
        public async Task<string> GetEncouragementMessageAsync(BusinessHub hub, int progressPercentage)
        {
            var persona = await GetCoachForHubAsync(hub);
            
            return persona.Style switch
            {
                CoachingStyle.Enthusiastic => progressPercentage switch
                {
                    >= 80 => "🎉 You're absolutely crushing it! Your idea is really taking shape - I can feel the energy!",
                    >= 50 => "✨ Fantastic progress! You're more than halfway there - keep that momentum going!",
                    >= 25 => "🚀 Great start! Your idea has real potential - let's build on this foundation!",
                    _ => "💪 Every great business starts with a single step - you've got this!"
                },
                CoachingStyle.Analytical => progressPercentage switch
                {
                    >= 80 => "Excellent progress. Your systematic approach is yielding strong results across all metrics.",
                    >= 50 => "Solid advancement. Your methodical planning is building a robust business foundation.",
                    >= 25 => "Good initial progress. Your structured approach will serve you well in the next phases.",
                    _ => "Foundation established. Let's systematically build upon your initial framework."
                },
                CoachingStyle.ResultsOriented => progressPercentage switch
                {
                    >= 80 => "Outstanding execution. Your metrics show strong potential for market success.",
                    >= 50 => "Strong performance indicators. You're on track for operational excellence.",
                    >= 25 => "Solid KPIs emerging. Your business fundamentals are taking shape.",
                    _ => "Initial metrics established. Focus on execution and measurable outcomes."
                },
                _ => "Great progress on your business journey!"
            };
        }
        
        public async Task<CoachingSuggestion> GetNextStepGuidanceAsync(BusinessHub hub, CoachingContext context)
        {
            var persona = await GetCoachForHubAsync(hub);
            
            return hub switch
            {
                BusinessHub.IdeaDevelopment => new CoachingSuggestion
                {
                    Id = "next-step-idea",
                    Title = "Next: Validate Your Market",
                    Message = "Let's dive deep into market research to validate your idea's potential!",
                    Icon = "fas fa-search",
                    Type = CoachingSuggestionType.NextStep,
                    Priority = 10,
                    ActionUrl = "/market-research",
                    ActionText = "Start Market Research"
                },
                BusinessHub.BusinessPlanning => new CoachingSuggestion
                {
                    Id = "next-step-planning",
                    Title = "Next: Build Financial Model",
                    Message = "Time to create detailed financial projections for your business plan.",
                    Icon = "fas fa-chart-bar",
                    Type = CoachingSuggestionType.NextStep,
                    Priority = 10,
                    ActionUrl = "/financial-projections",
                    ActionText = "Create Financial Model"
                },
                BusinessHub.BusinessOperations => new CoachingSuggestion
                {
                    Id = "next-step-operations",
                    Title = "Next: Launch Dashboard Setup",
                    Message = "Configure your KPI dashboard to track business performance metrics.",
                    Icon = "fas fa-tachometer-alt",
                    Type = CoachingSuggestionType.NextStep,
                    Priority = 10,
                    ActionUrl = "/dashboard",
                    ActionText = "Setup KPI Dashboard"
                },
                _ => new CoachingSuggestion()
            };
        }
        
        public async Task<string> GetMilestoneCelebrationAsync(BusinessHub hub, string milestoneId)
        {
            var persona = await GetCoachForHubAsync(hub);
            
            return persona.Style switch
            {
                CoachingStyle.Enthusiastic => $"🎉 Amazing work! You've just completed {milestoneId}! Your energy is contagious - keep shining! ⭐",
                CoachingStyle.Analytical => $"Milestone achieved: {milestoneId}. Your systematic approach continues to yield measurable results.",
                CoachingStyle.ResultsOriented => $"Milestone completed: {milestoneId}. Strong execution - this directly impacts your success metrics. 📊",
                _ => $"Congratulations on completing {milestoneId}!"
            };
        }
        
        private Dictionary<BusinessHub, CoachPersona> InitializeCoachPersonas()
        {
            return new Dictionary<BusinessHub, CoachPersona>
            {
                [BusinessHub.IdeaDevelopment] = new CoachPersona
                {
                    Name = "Spark",
                    Avatar = "fas fa-lightbulb",
                    Personality = "Enthusiastic, encouraging, creative, optimistic",
                    VoicePattern = "Energetic and inspiring with lots of positive reinforcement",
                    Hub = BusinessHub.IdeaDevelopment,
                    Style = CoachingStyle.Enthusiastic,
                    PrimaryColor = "#7b8fef",
                    SecondaryColor = "#22d3ee",
                    Description = "The Innovation Catalyst - transforms ideas into validated opportunities",
                    Specialties = new List<string>
                    {
                        "Idea Development",
                        "Market Validation",
                        "Creative Problem Solving",
                        "Opportunity Recognition",
                        "Innovation Coaching"
                    },
                    CommonPhrases = new List<string>
                    {
                        "That's an exciting idea!",
                        "Let's explore its potential!",
                        "Your creativity is inspiring!",
                        "I can feel the energy!",
                        "This has real spark!"
                    },
                    Greeting = "Hi there! I'm Spark, your Innovation Catalyst! ⚡ I'm here to help transform your ideas into validated business opportunities. Ready to ignite some innovation?",
                    Farewell = "Keep that creative energy flowing! Remember, every great business started with a spark of an idea. You've got this! ✨",
                    ContextualMessages = new Dictionary<string, string>
                    {
                        ["first_visit"] = "Welcome to the Idea Development Hub! This is where magic happens - let's turn your concept into something amazing!",
                        ["market_research"] = "Market research is like treasure hunting - we're looking for golden opportunities in your market! 🏆",
                        ["validation"] = "Validation is exciting! We're proving your idea has real potential - the data is your friend!"
                    }
                },
                
                [BusinessHub.BusinessPlanning] = new CoachPersona
                {
                    Name = "Strategy",
                    Avatar = "fas fa-chess",
                    Personality = "Analytical, detail-oriented, educational, systematic",
                    VoicePattern = "Methodical and educational with step-by-step guidance",
                    Hub = BusinessHub.BusinessPlanning,
                    Style = CoachingStyle.Analytical,
                    PrimaryColor = "#5a6fd8",
                    SecondaryColor = "#3b82f6",
                    Description = "The Business Architect - builds systematic frameworks for success",
                    Specialties = new List<string>
                    {
                        "Strategic Planning",
                        "Business Model Design",
                        "Financial Analysis",
                        "Risk Assessment",
                        "Systematic Framework Development"
                    },
                    CommonPhrases = new List<string>
                    {
                        "Let's break this down systematically",
                        "The data shows us...",
                        "From a strategic perspective...",
                        "This framework will help...",
                        "Consider the implications..."
                    },
                    Greeting = "Greetings! I'm Strategy, your Business Architect. 📋 I specialize in transforming validated ideas into comprehensive business plans. Shall we build your strategic foundation?",
                    Farewell = "Remember, successful businesses are built on solid strategic foundations. Take time to plan thoroughly - it will serve you well in execution.",
                    ContextualMessages = new Dictionary<string, string>
                    {
                        ["first_visit"] = "Welcome to the Business Planning Hub. Here we transform your validated opportunity into a systematic business framework.",
                        ["financial_planning"] = "Financial modeling is the backbone of your business. Let's create projections that investors will respect.",
                        ["strategic_framework"] = "A solid strategic framework guides every business decision. We'll build yours methodically."
                    }
                },
                
                [BusinessHub.BusinessOperations] = new CoachPersona
                {
                    Name = "Execute",
                    Avatar = "fas fa-chart-line",
                    Personality = "Strategic, results-focused, professional, forward-thinking",
                    VoicePattern = "Direct and action-oriented with emphasis on metrics and outcomes",
                    Hub = BusinessHub.BusinessOperations,
                    Style = CoachingStyle.ResultsOriented,
                    PrimaryColor = "#4c5fd7",
                    SecondaryColor = "#059669",
                    Description = "The Business Operations COO - drives execution and scalable growth",
                    Specialties = new List<string>
                    {
                        "Operations Management",
                        "Performance Analytics",
                        "Growth Strategy",
                        "Team Leadership",
                        "Scalable Systems"
                    },
                    CommonPhrases = new List<string>
                    {
                        "Let's focus on execution",
                        "The metrics indicate...",
                        "From an operational standpoint...",
                        "Your KPIs show...",
                        "Scale requires systematic..."
                    },
                    Greeting = "Welcome to Operations. I'm Execute, your Chief Operating Officer. 📈 I'm here to help you launch, scale, and optimize your business for maximum impact. Ready to drive results?",
                    Farewell = "Success is measured in outcomes. Stay focused on your KPIs, execute consistently, and scale systematically. You're building something significant.",
                    ContextualMessages = new Dictionary<string, string>
                    {
                        ["first_visit"] = "Welcome to the Business Operations Hub. This is where strategy meets execution and results drive growth.",
                        ["kpi_dashboard"] = "Your dashboard is mission control. These metrics will guide every operational decision moving forward.",
                        ["growth_planning"] = "Sustainable growth requires systematic operational excellence. Let's build your scaling framework."
                    }
                }
            };
        }
        
        private string GetHighProgressMessage(CoachPersona persona, int progress)
        {
            return persona.Style switch
            {
                CoachingStyle.Enthusiastic => $"🎉 Incredible! You're at {progress}% - you're absolutely crushing this! The finish line is in sight!",
                CoachingStyle.Analytical => $"Excellent progress at {progress}%. Your systematic approach is yielding optimal results.",
                CoachingStyle.ResultsOriented => $"Outstanding performance: {progress}% completion. Your execution metrics are impressive. 📊",
                _ => $"Great progress at {progress}%!"
            };
        }
        
        private string GetLowProgressMessage(CoachPersona persona)
        {
            return persona.Style switch
            {
                CoachingStyle.Enthusiastic => "Every amazing journey begins with a single step! Let's build some momentum together! 🚀",
                CoachingStyle.Analytical => "We're in the foundation phase. Let's establish a systematic approach to build upon.",
                CoachingStyle.ResultsOriented => "Initial stage identified. Focus on establishing core metrics and execution framework.",
                _ => "Let's get started on your journey!"
            };
        }
        
        private string GetMidProgressMessage(CoachPersona persona, CoachingContext context)
        {
            return persona.Style switch
            {
                CoachingStyle.Enthusiastic => "You're building great momentum! I love seeing your progress unfold - keep that energy flowing! ⚡",
                CoachingStyle.Analytical => "Solid progression. Your methodical approach is building a strong foundation for success.",
                CoachingStyle.ResultsOriented => "Good execution metrics. You're on track to meet your operational objectives. 📈",
                _ => "Great progress on your business development!"
            };
        }
        
        private List<CoachingSuggestion> GetIdeaDevelopmentSuggestions(CoachingContext context)
        {
            // Handle null context gracefully
            if (context == null)
            {
                return new List<CoachingSuggestion>();
            }

            var suggestions = new List<CoachingSuggestion>
            {
                new CoachingSuggestion
                {
                    Id = "market-research",
                    Title = "Deep Market Analysis",
                    Message = "Let's dive deeper into your target market to uncover hidden opportunities!",
                    Icon = "fas fa-search",
                    Type = CoachingSuggestionType.NextStep,
                    Priority = 10,
                    ActionUrl = "/market-research",
                    ActionText = "Start Research"
                },
                new CoachingSuggestion
                {
                    Id = "idea-validation",
                    Title = "Validate Your Concept",
                    Message = "Time to test your idea with real potential customers!",
                    Icon = "fas fa-check-circle",
                    Type = CoachingSuggestionType.Tips,
                    Priority = 8,
                    ActionUrl = "/idea-input",
                    ActionText = "Validate Idea"
                }
            };
            
            if (context.CompletedMilestones.Count >= 3)
            {
                suggestions.Add(new CoachingSuggestion
                {
                    Id = "move-to-planning",
                    Title = "Ready for Business Planning?",
                    Message = "You've made great progress! Consider moving to the Business Planning hub.",
                    Icon = "fas fa-arrow-right",
                    Type = CoachingSuggestionType.Achievement,
                    Priority = 9,
                    ActionUrl = "/hub/business-planning",
                    ActionText = "Enter Planning Hub"
                });
            }
            
            return suggestions;
        }
        
        private List<CoachingSuggestion> GetBusinessPlanningSuggestions(CoachingContext context)
        {
            // Handle null context gracefully
            if (context == null)
            {
                return new List<CoachingSuggestion>();
            }

            return new List<CoachingSuggestion>
            {
                new CoachingSuggestion
                {
                    Id = "financial-model",
                    Title = "Build Financial Projections",
                    Message = "Create detailed financial models to validate your business case.",
                    Icon = "fas fa-chart-bar",
                    Type = CoachingSuggestionType.NextStep,
                    Priority = 10,
                    ActionUrl = "/financial-projections",
                    ActionText = "Create Model"
                },
                new CoachingSuggestion
                {
                    Id = "strategic-framework",
                    Title = "Strategic Planning",
                    Message = "Develop your strategic framework for systematic growth.",
                    Icon = "fas fa-sitemap",
                    Type = CoachingSuggestionType.Tips,
                    Priority = 8,
                    ActionUrl = "/scenarios",
                    ActionText = "Plan Strategy"
                }
            };
        }
        
        private List<CoachingSuggestion> GetBusinessOperationsSuggestions(CoachingContext context)
        {
            // Handle null context gracefully
            if (context == null)
            {
                return new List<CoachingSuggestion>();
            }

            return new List<CoachingSuggestion>
            {
                new CoachingSuggestion
                {
                    Id = "kpi-setup",
                    Title = "Configure KPI Dashboard",
                    Message = "Set up your operational dashboard to track key performance indicators.",
                    Icon = "fas fa-tachometer-alt",
                    Type = CoachingSuggestionType.NextStep,
                    Priority = 10,
                    ActionUrl = "/dashboard",
                    ActionText = "Setup Dashboard"
                },
                new CoachingSuggestion
                {
                    Id = "growth-metrics",
                    Title = "Growth Analytics",
                    Message = "Implement analytics to measure and optimize your business growth.",
                    Icon = "fas fa-chart-line",
                    Type = CoachingSuggestionType.Tips,
                    Priority = 8,
                    ActionUrl = "/progress",
                    ActionText = "View Analytics"
                },
                new CoachingSuggestion
                {
                    Id = "operations-warning",
                    Title = "Performance Alert",
                    Message = "Monitor your operational metrics closely to avoid potential bottlenecks.",
                    Icon = "fas fa-exclamation-triangle",
                    Type = CoachingSuggestionType.Warning,
                    Priority = 7,
                    ActionUrl = "/alerts",
                    ActionText = "View Alerts"
                }
            };
        }
    }
}