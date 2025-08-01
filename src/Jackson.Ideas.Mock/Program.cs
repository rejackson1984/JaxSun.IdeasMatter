using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Services.Mock;
using Jackson.Ideas.Mock.Services;
using Jackson.Ideas.Mock.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    builder.Logging.AddEventLog();
}

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

// Add authentication services
builder.Services.AddScoped<MockAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<MockAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = !builder.Environment.IsProduction();
});

// Add health checks with specific services
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "ready", "live" })
    .AddCheck<Jackson.Ideas.Mock.HealthChecks.PdfServiceHealthCheck>("pdf_service", tags: new[] { "ready" })
    .AddCheck<Jackson.Ideas.Mock.HealthChecks.ExportServiceHealthCheck>("export_service", tags: new[] { "ready" });

// Configure Mock services
builder.Services.Configure<MockConfiguration>(builder.Configuration.GetSection("MockConfiguration"));

// Register Mock services
builder.Services.AddScoped<IMockDataService, MockDataService>();
builder.Services.AddScoped<IMockAuthenticationService, MockAuthenticationService>();
builder.Services.AddScoped<IMarketResearchService, MockMarketResearchService>();
builder.Services.AddScoped<IFinancialProjectionService, MockFinancialProjectionService>();
builder.Services.AddScoped<IUserProfileService, MockUserProfileService>();
builder.Services.AddScoped<ILaunchedBusinessService, MockLaunchedBusinessService>();

// Register Business Translation Service
builder.Services.AddScoped<BusinessTranslationService>();

// Register Business Plan Service
builder.Services.AddScoped<BusinessPlanService>();

// Register Business Plan Service with Interface
builder.Services.AddScoped<IBusinessPlanService, MockBusinessPlanService>();

// Register PDF Generation Service
builder.Services.AddScoped<IPdfGenerationService, PdfGenerationService>();

// Register Data Export Service
builder.Services.AddScoped<IDataExportService, DataExportService>();

// Register Business Plan Version Service
builder.Services.AddScoped<IBusinessPlanVersionService, MockBusinessPlanVersionService>();

// Register Product Design Service
builder.Services.AddScoped<IProductDesignService, MockProductDesignService>();

// Register Hub Management Services
builder.Services.AddScoped<IHubConfigurationService, MockHubConfigurationService>();
builder.Services.AddScoped<IHubContextService, MockHubContextService>();
builder.Services.AddScoped<ICoachPersonaService, MockCoachPersonaService>();

// Register Idea Validation Service
builder.Services.AddScoped<IIdeaValidationService, MockIdeaValidationService>();

// Register Business Plan Builder Service
builder.Services.AddScoped<IBusinessPlanBuilderService, MockBusinessPlanBuilderService>();

// Register Solution Design Service
builder.Services.AddScoped<ISolutionDesignService, MockSolutionDesignService>();

// Register Business Model Canvas Service
builder.Services.AddScoped<IBusinessModelCanvasService, MockBusinessModelCanvasService>();

// Register Business Operations Service
builder.Services.AddScoped<IBusinessOperationsService, MockBusinessOperationsService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add health check middleware
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapRazorPages();
app.MapBlazorHub();
app.MapControllers();

// Handle scenario ID routes that might be causing JSON parsing errors
app.MapGet("/{scenarioId:regex(^[a-z-]+-[0-9]+$)}", async (string scenarioId, HttpContext context) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    // Check if request expects JSON/JavaScript - return appropriate error
    var acceptHeader = context.Request.Headers.Accept.ToString();
    if (acceptHeader.Contains("application/json") || 
        acceptHeader.Contains("text/javascript") || 
        acceptHeader.Contains("application/javascript"))
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"Not found\"}");
        return;
    }
    
    // For browser requests, redirect to proper scenario page
    context.Response.Redirect($"/scenario/{scenarioId}");
});


// Map the hub system to /app route
app.MapFallbackToPage("/app", "/_Host");

// Map all other Blazor component routes (like /market-research, /dashboard, etc.)
app.MapFallbackToPage("/_Host");

app.Run();