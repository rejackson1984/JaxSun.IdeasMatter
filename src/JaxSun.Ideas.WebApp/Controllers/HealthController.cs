using Microsoft.AspNetCore.Mvc;
using JaxSun.Ideas.WebApp.Services.Interfaces;
using System.Reflection;

namespace JaxSun.Ideas.WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly IPdfGenerationService _pdfService;
    private readonly IDataExportService _exportService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        IPdfGenerationService pdfService,
        IDataExportService exportService,
        ILogger<HealthController> logger)
    {
        _pdfService = pdfService;
        _exportService = exportService;
        _logger = logger;
    }

    /// <summary>
    /// Simple health check endpoint for basic liveness probe
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Application = "Ideas Matter Mock",
                Version = GetApplicationVersion()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Detailed health check with service validation
    /// </summary>
    [HttpGet("detailed")]
    public async Task<IActionResult> Detailed()
    {
        var healthChecks = new Dictionary<string, object>();
        var overallHealthy = true;

        try
        {
            // Check basic application health
            healthChecks["Application"] = new
            {
                Status = "Healthy",
                Version = GetApplicationVersion(),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = GC.GetTotalMemory(false)
            };

            // Check PDF service
            try
            {
                var testScenario = CreateTestScenario();
                var testPdf = _pdfService.GenerateExecutiveSummary(testScenario, 5);
                
                healthChecks["PdfService"] = new
                {
                    Status = "Healthy",
                    TestPdfSize = testPdf?.Length ?? 0,
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                overallHealthy = false;
                healthChecks["PdfService"] = new
                {
                    Status = "Unhealthy",
                    Error = ex.Message,
                    LastChecked = DateTime.UtcNow
                };
            }

            // Check Export service
            try
            {
                var testScenario = CreateTestScenario();
                var testCsv = _exportService.ExportScenarioToCsv(testScenario, 5);
                
                healthChecks["ExportService"] = new
                {
                    Status = "Healthy",
                    TestCsvSize = testCsv?.Length ?? 0,
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                overallHealthy = false;
                healthChecks["ExportService"] = new
                {
                    Status = "Unhealthy",
                    Error = ex.Message,
                    LastChecked = DateTime.UtcNow
                };
            }

            // Check disk space
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady)
                    .Select(d => new
                    {
                        Name = d.Name,
                        TotalSize = d.TotalSize,
                        FreeSpace = d.AvailableFreeSpace,
                        FreeSpacePercentage = (double)d.AvailableFreeSpace / d.TotalSize * 100
                    })
                    .ToList();

                var lowDiskSpace = drives.Any(d => d.FreeSpacePercentage < 10);
                if (lowDiskSpace)
                {
                    overallHealthy = false;
                }

                healthChecks["DiskSpace"] = new
                {
                    Status = lowDiskSpace ? "Warning" : "Healthy",
                    Drives = drives,
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                healthChecks["DiskSpace"] = new
                {
                    Status = "Unknown",
                    Error = ex.Message,
                    LastChecked = DateTime.UtcNow
                };
            }

            var statusCode = overallHealthy ? 200 : 503;
            return StatusCode(statusCode, new
            {
                Status = overallHealthy ? "Healthy" : "Degraded",
                Timestamp = DateTime.UtcNow,
                Checks = healthChecks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detailed health check failed");
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message,
                Checks = healthChecks
            });
        }
    }

    /// <summary>
    /// Readiness check - indicates if the application is ready to receive traffic
    /// </summary>
    [HttpGet("ready")]
    public IActionResult Ready()
    {
        try
        {
            // Check if essential services are ready
            var ready = CheckServicesReady();
            
            if (ready)
            {
                return Ok(new
                {
                    Status = "Ready",
                    Timestamp = DateTime.UtcNow,
                    Message = "Application is ready to receive traffic"
                });
            }
            else
            {
                return StatusCode(503, new
                {
                    Status = "Not Ready",
                    Timestamp = DateTime.UtcNow,
                    Message = "Application is not ready to receive traffic"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");
            return StatusCode(503, new
            {
                Status = "Not Ready",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Liveness check - indicates if the application is running
    /// </summary>
    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new
        {
            Status = "Alive",
            Timestamp = DateTime.UtcNow,
            Uptime = GetUptime()
        });
    }

    /// <summary>
    /// Metrics endpoint for monitoring
    /// </summary>
    [HttpGet("metrics")]
    public IActionResult Metrics()
    {
        try
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            
            return Ok(new
            {
                Timestamp = DateTime.UtcNow,
                Memory = new
                {
                    WorkingSet = process.WorkingSet64,
                    PrivateMemory = process.PrivateMemorySize64,
                    VirtualMemory = process.VirtualMemorySize64,
                    GCTotalMemory = GC.GetTotalMemory(false)
                },
                Performance = new
                {
                    ProcessorTime = process.TotalProcessorTime.TotalMilliseconds,
                    UserProcessorTime = process.UserProcessorTime.TotalMilliseconds,
                    ThreadCount = process.Threads.Count,
                    HandleCount = process.HandleCount
                },
                GarbageCollection = new
                {
                    Gen0Collections = GC.CollectionCount(0),
                    Gen1Collections = GC.CollectionCount(1),
                    Gen2Collections = GC.CollectionCount(2)
                },
                System = new
                {
                    MachineName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    OSVersion = Environment.OSVersion.ToString(),
                    CLRVersion = Environment.Version.ToString(),
                    Is64BitProcess = Environment.Is64BitProcess,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to gather metrics");
            return StatusCode(500, new
            {
                Error = "Failed to gather metrics",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    private bool CheckServicesReady()
    {
        try
        {
            // Test PDF service availability
            var testScenario = CreateTestScenario();
            _pdfService.GenerateExecutiveSummary(testScenario, 1);
            
            // Test Export service availability
            _exportService.ExportScenarioToCsv(testScenario, 1);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetApplicationVersion()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private TimeSpan GetUptime()
    {
        try
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            return DateTime.Now - process.StartTime;
        }
        catch
        {
            return TimeSpan.Zero;
        }
    }

    private static JaxSun.Ideas.WebApp.Models.BusinessIdeaScenario CreateTestScenario()
    {
        return new JaxSun.Ideas.WebApp.Models.BusinessIdeaScenario
        {
            Title = "Health Check Test",
            Description = "Test scenario for health checks",
            MarketSize = 1000,
            CompetitionLevel = "Low",
            StartupCost = 1000,
            MarketResearch = new JaxSun.Ideas.WebApp.Models.MarketResearchData
            {
                Industry = "Test"
            },
            FinancialProjections = new JaxSun.Ideas.WebApp.Models.FinancialProjections
            {
                Revenue = new JaxSun.Ideas.WebApp.Models.RevenueProjections { Year1Total = 10000 },
                CashFlow = new JaxSun.Ideas.WebApp.Models.CashFlowProjections { BreakEvenMonth = 6 }
            }
        };
    }
}