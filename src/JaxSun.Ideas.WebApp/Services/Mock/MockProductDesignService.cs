using JaxSun.Ideas.Mock.Models;
using JaxSun.Ideas.Mock.Services.Interfaces;
using System.Text.Json;

namespace JaxSun.Ideas.Mock.Services.Mock;

public class MockProductDesignService : IProductDesignService
{
    private readonly List<ProductDesignMockSession> _sessions = new();
    private readonly List<ProductDesignQuestion> _questionBank = new();
    private readonly List<MockupTemplate> _mockupTemplates = new();
    private readonly List<ProductRequirement> _requirements = new();
    private readonly List<DesignModificationRequest> _modifications = new();

    public MockProductDesignService()
    {
        InitializeQuestionBank();
        InitializeMockupTemplates();
    }

    #region Session Management

    public async Task<ProductDesignMockSession> CreateSessionAsync(string userId, string scenarioId, string sessionName)
    {
        var session = new ProductDesignMockSession
        {
            UserId = userId,
            ScenarioId = scenarioId,
            SessionName = sessionName,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _sessions.Add(session);
        return await Task.FromResult(session);
    }

    public async Task<ProductDesignMockSession?> GetSessionAsync(string sessionId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        return await Task.FromResult(session);
    }

    public async Task<List<ProductDesignMockSession>> GetUserSessionsAsync(string userId)
    {
        var sessions = _sessions.Where(s => s.UserId == userId).ToList();
        return await Task.FromResult(sessions);
    }

    public async Task<ProductDesignMockSession> UpdateSessionAsync(ProductDesignMockSession session)
    {
        var existingSession = _sessions.FirstOrDefault(s => s.Id == session.Id);
        if (existingSession != null)
        {
            existingSession.SessionName = session.SessionName;
            existingSession.CurrentStep = session.CurrentStep;
            existingSession.Responses = session.Responses;
            existingSession.GeneratedPRD = session.GeneratedPRD;
            existingSession.SelectedMockupId = session.SelectedMockupId;
            existingSession.Status = session.Status;
            existingSession.UpdatedAt = DateTime.UtcNow;
        }
        return await Task.FromResult(session);
    }

    public async Task<bool> DeleteSessionAsync(string sessionId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            _sessions.Remove(session);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    #endregion

    #region Question Flow

    public async Task<List<ProductDesignQuestion>> GetQuestionBankAsync()
    {
        return await Task.FromResult(_questionBank);
    }

    public async Task<ProductDesignQuestion?> GetNextQuestionAsync(string sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return null;

        var answeredQuestions = session.Responses.Keys.ToList();
        var nextQuestion = _questionBank
            .Where(q => !answeredQuestions.Contains(q.Id))
            .OrderBy(q => q.Order)
            .FirstOrDefault();

        return await Task.FromResult(nextQuestion);
    }

    public async Task<ProductDesignMockSession> SubmitAnswerAsync(string sessionId, string questionId, string answer)
    {
        var session = await GetSessionAsync(sessionId);
        if (session != null)
        {
            session.Responses[questionId] = answer;
            session.CurrentStep++;
            session.UpdatedAt = DateTime.UtcNow;
            await UpdateSessionAsync(session);
        }
        return session!;
    }

    public async Task<bool> IsQuestionFlowCompleteAsync(string sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return false;

        var totalRequiredQuestions = _questionBank.Count(q => q.IsRequired);
        var answeredRequiredQuestions = session.Responses.Count;

        return await Task.FromResult(answeredRequiredQuestions >= totalRequiredQuestions);
    }

    #endregion

    #region PRD Generation

    public async Task<string> GeneratePRDAsync(string sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return "";

        // Generate PRD based on responses
        var prdContent = GeneratePRDFromResponses(session.Responses);
        
        session.GeneratedPRD = prdContent;
        await UpdateSessionAsync(session);

        return await Task.FromResult(prdContent);
    }

    public async Task<ProductDesignMockSession> UpdatePRDAsync(string sessionId, string updatedPRD)
    {
        var session = await GetSessionAsync(sessionId);
        if (session != null)
        {
            session.GeneratedPRD = updatedPRD;
            session.UpdatedAt = DateTime.UtcNow;
            await UpdateSessionAsync(session);
        }
        return session!;
    }

    public async Task<byte[]> ExportPRDAsPDFAsync(string sessionId)
    {
        // Mock PDF generation - in real implementation, use a PDF library
        var session = await GetSessionAsync(sessionId);
        if (session?.GeneratedPRD == null) return Array.Empty<byte>();

        var pdfContent = System.Text.Encoding.UTF8.GetBytes(session.GeneratedPRD);
        return await Task.FromResult(pdfContent);
    }

    #endregion

    #region Mockup Management

    public async Task<List<MockupTemplate>> GetMockupTemplatesAsync()
    {
        return await Task.FromResult(_mockupTemplates);
    }

    public async Task<List<MockupTemplate>> GetRecommendedMockupsAsync(string sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return new List<MockupTemplate>();

        // Simple recommendation logic based on responses
        var recommendedTemplates = _mockupTemplates.Take(5).ToList();
        return await Task.FromResult(recommendedTemplates);
    }

    public async Task<MockupTemplate?> GetMockupTemplateAsync(string templateId)
    {
        var template = _mockupTemplates.FirstOrDefault(t => t.Id == templateId);
        return await Task.FromResult(template);
    }

    public async Task<ProductDesignMockSession> SelectMockupAsync(string sessionId, string templateId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session != null)
        {
            session.SelectedMockupId = templateId;
            session.UpdatedAt = DateTime.UtcNow;
            await UpdateSessionAsync(session);
        }
        return session!;
    }

    #endregion

    #region Design Modifications

    public async Task<string> ProcessModificationRequestAsync(string sessionId, string requestText)
    {
        // Mock modification processing
        var modification = new DesignModificationRequest
        {
            SessionId = sessionId,
            RequestText = requestText,
            ModificationType = DetermineModificationType(requestText),
            AppliedChanges = $"Applied modification: {requestText}",
            IsApplied = true,
            RequestedAt = DateTime.UtcNow
        };

        _modifications.Add(modification);

        var responseText = GenerateModificationResponse(requestText);
        return await Task.FromResult(responseText);
    }

    public async Task<List<DesignModificationRequest>> GetModificationHistoryAsync(string sessionId)
    {
        var history = _modifications.Where(m => m.SessionId == sessionId).ToList();
        return await Task.FromResult(history);
    }

    public async Task<bool> UndoLastModificationAsync(string sessionId)
    {
        var lastModification = _modifications
            .Where(m => m.SessionId == sessionId && m.IsApplied)
            .OrderByDescending(m => m.RequestedAt)
            .FirstOrDefault();

        if (lastModification != null)
        {
            lastModification.IsApplied = false;
            return await Task.FromResult(true);
        }

        return await Task.FromResult(false);
    }

    #endregion

    #region Requirements Management

    public async Task<List<ProductRequirement>> GetSessionRequirementsAsync(string sessionId)
    {
        var requirements = _requirements.Where(r => r.SessionId == sessionId).ToList();
        return await Task.FromResult(requirements);
    }

    public async Task<ProductRequirement> AddRequirementAsync(string sessionId, ProductRequirement requirement)
    {
        requirement.SessionId = sessionId;
        _requirements.Add(requirement);
        return await Task.FromResult(requirement);
    }

    public async Task<ProductRequirement> UpdateRequirementAsync(ProductRequirement requirement)
    {
        var existingRequirement = _requirements.FirstOrDefault(r => r.Id == requirement.Id);
        if (existingRequirement != null)
        {
            existingRequirement.Title = requirement.Title;
            existingRequirement.Description = requirement.Description;
            existingRequirement.Priority = requirement.Priority;
            existingRequirement.Status = requirement.Status;
            existingRequirement.AcceptanceCriteria = requirement.AcceptanceCriteria;
        }
        return await Task.FromResult(requirement);
    }

    public async Task<bool> DeleteRequirementAsync(string requirementId)
    {
        var requirement = _requirements.FirstOrDefault(r => r.Id == requirementId);
        if (requirement != null)
        {
            _requirements.Remove(requirement);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    #endregion

    #region Private Helper Methods

    private void InitializeQuestionBank()
    {
        _questionBank.AddRange(new[]
        {
            // User Experience Questions
            new ProductDesignQuestion
            {
                Category = "User Experience",
                Text = "Who is the primary user of your software?",
                Type = "text",
                Order = 1,
                Context = "Understanding target audience",
                HelpText = "Describe your main user type (e.g., small business owners, students, professionals)",
                SampleAnswer = "Small business owners and entrepreneurs who need to manage their projects, track customer relationships, and organize their daily operations more efficiently."
            },
            new ProductDesignQuestion
            {
                Category = "User Experience", 
                Text = "What is the main action users will take in your app?",
                Type = "text",
                Order = 2,
                Context = "Core user journey",
                HelpText = "What's the primary task or goal users want to accomplish?",
                SampleAnswer = "Users will primarily create and manage projects, assign tasks to team members, track progress with visual dashboards, and collaborate through shared workspaces."
            },
            new ProductDesignQuestion
            {
                Category = "User Experience",
                Text = "Should users create accounts or use it anonymously?", 
                Type = "multiple-choice",
                Options = new List<string> { "Require account creation", "Allow anonymous use", "Both options available" },
                Order = 3,
                Context = "Authentication requirements",
                SampleAnswer = "Require account creation"
            },

            // Core Features Questions
            new ProductDesignQuestion
            {
                Category = "Core Features",
                Text = "What are the 3 most important features of your software?",
                Type = "text",
                Order = 4,
                Context = "Feature prioritization",
                HelpText = "List the core features that are essential for your MVP",
                SampleAnswer = "1. Project management dashboard with task assignments and progress tracking\n2. Real-time team collaboration tools with chat and file sharing\n3. Automated reporting and analytics to track productivity and project outcomes"
            },
            new ProductDesignQuestion
            {
                Category = "Core Features",
                Text = "Do you need user profiles and personal settings?",
                Type = "boolean",
                Options = new List<string> { "Yes", "No" },
                Order = 5,
                Context = "User management features",
                SampleAnswer = "Yes"
            },
            new ProductDesignQuestion
            {
                Category = "Core Features",
                Text = "Will you have a dashboard or homepage for users?",
                Type = "multiple-choice",
                Options = new List<string> { "Dashboard with data/analytics", "Simple homepage", "Landing page only", "Direct to main feature" },
                Order = 6,
                Context = "Main interface design",
                SampleAnswer = "Dashboard with data/analytics"
            },

            // Technical Preferences
            new ProductDesignQuestion
            {
                Category = "Technical Preferences",
                Text = "Do you prefer a web app, mobile app, or both?",
                Type = "multiple-choice", 
                Options = new List<string> { "Web application", "Mobile app", "Both web and mobile", "Desktop application" },
                Order = 7,
                Context = "Platform selection",
                SampleAnswer = "Both web and mobile"
            },
            new ProductDesignQuestion
            {
                Category = "Technical Preferences",
                Text = "Do you need real-time updates or notifications?",
                Type = "boolean",
                Options = new List<string> { "Yes", "No" },
                Order = 8,
                Context = "Real-time functionality requirements",
                SampleAnswer = "Yes"
            },
            new ProductDesignQuestion
            {
                Category = "Technical Preferences",
                Text = "Should your app work offline or require internet?",
                Type = "multiple-choice",
                Options = new List<string> { "Requires internet connection", "Basic offline functionality", "Full offline capability" },
                Order = 9,
                Context = "Connectivity requirements",
                SampleAnswer = "Basic offline functionality"
            },

            // Business Logic
            new ProductDesignQuestion
            {
                Category = "Business Logic",
                Text = "How will users pay for your service (if applicable)?",
                Type = "multiple-choice",
                Options = new List<string> { "One-time purchase", "Monthly subscription", "Freemium model", "Free with ads", "Transaction fees", "Not applicable" },
                Order = 10,
                Context = "Monetization strategy",
                SampleAnswer = "Monthly subscription"
            }
        });
    }

    private void InitializeMockupTemplates()
    {
        _mockupTemplates.AddRange(new[]
        {
            new MockupTemplate
            {
                Name = "SaaS Dashboard",
                Category = "Business Tools",
                Description = "Clean dashboard with analytics and data visualization",
                HtmlContent = GenerateSaaSDashboardHTML(),
                CssContent = GenerateModernCSS(),
                PreviewImageUrl = "/images/mockups/saas-dashboard.png",
                Tags = new List<string> { "dashboard", "analytics", "business", "charts" },
                RequiredFeatures = new List<string> { "user accounts", "data visualization", "navigation" },
                IndustryType = "SaaS",
                AppType = "web"
            },
            new MockupTemplate
            {
                Name = "E-commerce Store",
                Category = "E-commerce",
                Description = "Modern online store with product catalog and shopping cart",
                HtmlContent = GenerateEcommerceHTML(),
                CssContent = GenerateModernCSS(),
                PreviewImageUrl = "/images/mockups/ecommerce-store.png",
                Tags = new List<string> { "ecommerce", "shopping", "products", "cart" },
                RequiredFeatures = new List<string> { "product catalog", "shopping cart", "checkout" },
                IndustryType = "Retail",
                AppType = "web"
            },
            new MockupTemplate
            {
                Name = "Mobile App Interface",
                Category = "Mobile",
                Description = "Native-style mobile interface with bottom navigation",
                HtmlContent = GenerateMobileAppHTML(),
                CssContent = GenerateMobileCSS(),
                PreviewImageUrl = "/images/mockups/mobile-app.png",
                Tags = new List<string> { "mobile", "app", "navigation", "touch" },
                RequiredFeatures = new List<string> { "mobile navigation", "touch interface", "responsive" },
                IndustryType = "General",
                AppType = "mobile"
            }
        });
    }

    private string GeneratePRDFromResponses(Dictionary<string, string> responses)
    {
        var prd = $@"# Product Requirements Document

## 1. Product Overview
**Generated:** {DateTime.Now:yyyy-MM-dd}

## 2. User Personas
**Primary User:** {responses.GetValueOrDefault("user-type", "Not specified")}

## 3. Core Features
**Main Features:** {responses.GetValueOrDefault("core-features", "Not specified")}

## 4. Technical Requirements
**Platform:** {responses.GetValueOrDefault("platform", "Web application")}
**Authentication:** {responses.GetValueOrDefault("authentication", "Required")}
**Connectivity:** {responses.GetValueOrDefault("connectivity", "Internet required")}

## 5. User Stories
- As a user, I want to {responses.GetValueOrDefault("main-action", "accomplish my primary goal")}
- As a user, I want to access my {responses.GetValueOrDefault("user-profiles", "personal settings")}

## 6. Business Requirements
**Monetization:** {responses.GetValueOrDefault("monetization", "To be determined")}

## 7. Success Metrics
- User engagement and retention
- Feature adoption rates  
- Customer satisfaction scores

## 8. Technical Specifications
- Responsive design for multiple devices
- Modern web technologies
- Secure user authentication
- Scalable architecture

*This PRD was generated based on your responses and can be edited as needed.*";

        return prd;
    }

    private string DetermineModificationType(string requestText)
    {
        var lowerRequest = requestText.ToLower();
        if (lowerRequest.Contains("color") || lowerRequest.Contains("style") || lowerRequest.Contains("font"))
            return "styling";
        if (lowerRequest.Contains("move") || lowerRequest.Contains("layout") || lowerRequest.Contains("position"))
            return "layout";
        if (lowerRequest.Contains("add") || lowerRequest.Contains("remove") || lowerRequest.Contains("feature"))
            return "feature";
        return "content";
    }

    private string GenerateModificationResponse(string requestText)
    {
        var responses = new[]
        {
            $"I've applied your request: '{requestText}'. The mockup has been updated to reflect this change.",
            $"Great suggestion! I've modified the design based on your request: '{requestText}'.",
            $"Your change has been implemented: '{requestText}'. You can see the updated version in the preview.",
            $"Perfect! I've made the adjustment you requested: '{requestText}'. How does it look now?"
        };

        var random = new Random();
        return responses[random.Next(responses.Length)];
    }

    private string GenerateSaaSDashboardHTML()
    {
        return @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>SaaS Dashboard</title>
    <link href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css' rel='stylesheet'>
</head>
<body>
    <div class='dashboard-container'>
        <nav class='sidebar'>
            <div class='brand'>
                <h2><i class='fas fa-chart-line'></i> YourApp</h2>
            </div>
            <ul class='nav-menu'>
                <li><a href='#' class='active'><i class='fas fa-home'></i> Dashboard</a></li>
                <li><a href='#'><i class='fas fa-users'></i> Users</a></li>
                <li><a href='#'><i class='fas fa-chart-bar'></i> Analytics</a></li>
                <li><a href='#'><i class='fas fa-cog'></i> Settings</a></li>
            </ul>
        </nav>
        <main class='main-content'>
            <header class='page-header'>
                <h1>Dashboard Overview</h1>
                <div class='user-profile'>
                    <img src='https://via.placeholder.com/40' alt='Profile' class='profile-img'>
                    <span>John Doe</span>
                </div>
            </header>
            <div class='stats-grid'>
                <div class='stat-card'>
                    <h3>Total Users</h3>
                    <div class='stat-number'>12,345</div>
                    <div class='stat-change positive'>+5.2%</div>
                </div>
                <div class='stat-card'>
                    <h3>Revenue</h3>
                    <div class='stat-number'>$89,234</div>
                    <div class='stat-change positive'>+12.8%</div>
                </div>
                <div class='stat-card'>
                    <h3>Active Sessions</h3>
                    <div class='stat-number'>1,892</div>
                    <div class='stat-change negative'>-2.1%</div>
                </div>
            </div>
            <div class='charts-section'>
                <div class='chart-container'>
                    <h3>Growth Over Time</h3>
                    <div class='mock-chart'>[Interactive Chart Placeholder]</div>
                </div>
            </div>
        </main>
    </div>
</body>
</html>";
    }

    private string GenerateEcommerceHTML()
    {
        return @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Your Store</title>
</head>
<body>
    <header class='store-header'>
        <div class='container'>
            <div class='brand'>
                <h1>Your Store</h1>
            </div>
            <nav class='main-nav'>
                <a href='#'>Home</a>
                <a href='#'>Products</a>
                <a href='#'>Categories</a>
                <a href='#'>About</a>
                <a href='#'>Contact</a>
            </nav>
            <div class='header-actions'>
                <a href='#' class='search-btn'><i class='fas fa-search'></i></a>
                <a href='#' class='cart-btn'><i class='fas fa-shopping-cart'></i> <span class='cart-count'>3</span></a>
            </div>
        </div>
    </header>
    <main class='store-main'>
        <section class='hero-section'>
            <div class='hero-content'>
                <h2>Welcome to Your Store</h2>
                <p>Discover amazing products at great prices</p>
                <button class='cta-button'>Shop Now</button>
            </div>
        </section>
        <section class='products-grid'>
            <div class='container'>
                <h2>Featured Products</h2>
                <div class='grid'>
                    <div class='product-card'>
                        <img src='https://via.placeholder.com/300x200' alt='Product'>
                        <h3>Product Name</h3>
                        <p class='price'>$99.99</p>
                        <button class='add-to-cart'>Add to Cart</button>
                    </div>
                    <div class='product-card'>
                        <img src='https://via.placeholder.com/300x200' alt='Product'>
                        <h3>Product Name</h3>
                        <p class='price'>$149.99</p>
                        <button class='add-to-cart'>Add to Cart</button>
                    </div>
                    <div class='product-card'>
                        <img src='https://via.placeholder.com/300x200' alt='Product'>
                        <h3>Product Name</h3>
                        <p class='price'>$79.99</p>
                        <button class='add-to-cart'>Add to Cart</button>
                    </div>
                </div>
            </div>
        </section>
    </main>
</body>
</html>";
    }

    private string GenerateMobileAppHTML()
    {
        return @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Mobile App</title>
</head>
<body class='mobile-app'>
    <div class='app-container'>
        <header class='app-header'>
            <button class='menu-btn'><i class='fas fa-bars'></i></button>
            <h1>Your App</h1>
            <button class='profile-btn'><i class='fas fa-user'></i></button>
        </header>
        <main class='app-content'>
            <div class='welcome-section'>
                <h2>Welcome back!</h2>
                <p>Here's what's happening today</p>
            </div>
            <div class='quick-actions'>
                <button class='action-btn'>
                    <i class='fas fa-plus'></i>
                    <span>New Item</span>
                </button>
                <button class='action-btn'>
                    <i class='fas fa-search'></i>
                    <span>Search</span>
                </button>
                <button class='action-btn'>
                    <i class='fas fa-star'></i>
                    <span>Favorites</span>
                </button>
            </div>
            <div class='content-list'>
                <div class='list-item'>
                    <div class='item-icon'><i class='fas fa-file'></i></div>
                    <div class='item-content'>
                        <h3>Item Title</h3>
                        <p>Item description goes here</p>
                    </div>
                    <button class='item-action'><i class='fas fa-chevron-right'></i></button>
                </div>
                <div class='list-item'>
                    <div class='item-icon'><i class='fas fa-calendar'></i></div>
                    <div class='item-content'>
                        <h3>Another Item</h3>
                        <p>More description text</p>
                    </div>
                    <button class='item-action'><i class='fas fa-chevron-right'></i></button>
                </div>
            </div>
        </main>
        <nav class='bottom-nav'>
            <a href='#' class='nav-item active'>
                <i class='fas fa-home'></i>
                <span>Home</span>
            </a>
            <a href='#' class='nav-item'>
                <i class='fas fa-search'></i>
                <span>Search</span>
            </a>
            <a href='#' class='nav-item'>
                <i class='fas fa-bell'></i>
                <span>Notifications</span>
            </a>
            <a href='#' class='nav-item'>
                <i class='fas fa-user'></i>
                <span>Profile</span>
            </a>
        </nav>
    </div>
</body>
</html>";
    }

    private string GenerateModernCSS()
    {
        return @"
/* Modern CSS for mockup templates */
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

body {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    line-height: 1.6;
    color: #333;
}

.dashboard-container {
    display: flex;
    min-height: 100vh;
}

.sidebar {
    width: 250px;
    background: #2c3e50;
    color: white;
    padding: 1rem;
}

.main-content {
    flex: 1;
    padding: 2rem;
    background: #f8f9fa;
}

.stats-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
    margin: 2rem 0;
}

.stat-card {
    background: white;
    padding: 1.5rem;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

/* Mobile App Styles */
.mobile-app {
    max-width: 375px;
    margin: 0 auto;
    background: #f5f5f5;
}

.app-container {
    display: flex;
    flex-direction: column;
    height: 100vh;
}

.bottom-nav {
    display: flex;
    background: white;
    border-top: 1px solid #e0e0e0;
    padding: 0.5rem 0;
}

.nav-item {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-decoration: none;
    color: #666;
    padding: 0.5rem;
}

/* Responsive Design */
@media (max-width: 768px) {
    .dashboard-container {
        flex-direction: column;
    }
    
    .sidebar {
        width: 100%;
        order: 2;
    }
    
    .main-content {
        order: 1;
    }
}";
    }

    private string GenerateMobileCSS()
    {
        return GenerateModernCSS(); // Using the same CSS for simplicity in mock
    }

    #endregion
}