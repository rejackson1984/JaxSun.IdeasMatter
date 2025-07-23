using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Services.Mock;
using Jackson.Ideas.Mock.Services;
using Jackson.Ideas.Mock.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

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

// Register Business Translation Service
builder.Services.AddScoped<BusinessTranslationService>();

// Register PDF Generation Service
builder.Services.AddScoped<IPdfGenerationService, PdfGenerationService>();

// Register Data Export Service
builder.Services.AddScoped<IDataExportService, DataExportService>();

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
app.MapFallbackToPage("/_Host");

app.Run();