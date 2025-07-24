namespace Jackson.Ideas.Mock.Models;

public class ProductDesignMockSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string ScenarioId { get; set; } = "";
    public string SessionName { get; set; } = "";
    public int CurrentStep { get; set; } = 0;
    public Dictionary<string, string> Responses { get; set; } = new();
    public string? GeneratedPRD { get; set; }
    public string? SelectedMockupId { get; set; }
    public string Status { get; set; } = "active"; // active, completed, archived
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public BusinessIdeaScenario? BusinessScenario { get; set; }
    public List<ProductDesignQuestion> Questions { get; set; } = new();
    public MockupTemplate? SelectedMockup { get; set; }
}

public class ProductDesignQuestion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Category { get; set; } = "";
    public string Text { get; set; } = "";
    public string Type { get; set; } = "text"; // text, multiple-choice, boolean, scale
    public List<string> Options { get; set; } = new();
    public string? ConditionalLogic { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
    public string Context { get; set; } = ""; // When to show this question
    public string HelpText { get; set; } = "";
    public string SampleAnswer { get; set; } = ""; // Mock answer for demo purposes
}

public class MockupTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string HtmlContent { get; set; } = "";
    public string CssContent { get; set; } = "";
    public string PreviewImageUrl { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public List<string> RequiredFeatures { get; set; } = new();
    public string IndustryType { get; set; } = "";
    public string AppType { get; set; } = ""; // web, mobile, desktop
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProductRequirement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = "";
    public string Type { get; set; } = ""; // feature, user-story, technical, business-rule
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "medium"; // high, medium, low
    public string Status { get; set; } = "defined"; // defined, approved, implemented
    public List<string> Dependencies { get; set; } = new();
    public string AcceptanceCriteria { get; set; } = "";
}

public class DesignModificationRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = "";
    public string RequestText { get; set; } = "";
    public string ModificationType { get; set; } = ""; // layout, styling, content, feature
    public string AppliedChanges { get; set; } = "";
    public string ResultingMockupId { get; set; } = "";
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public bool IsApplied { get; set; } = false;
}