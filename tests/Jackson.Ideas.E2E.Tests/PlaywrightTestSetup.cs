using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Jackson.Ideas.E2E.Tests;

/// <summary>
/// Global test setup for Playwright tests.
/// Handles browser configuration and application lifecycle.
/// </summary>
[TestClass]
public class PlaywrightTestSetup
{
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static Process? _applicationProcess;
    private static readonly object _lockObject = new();
    private static bool _isSetupComplete = false;

    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        lock (_lockObject)
        {
            if (_isSetupComplete) return;
            
            try
            {
                // Start the application process
                StartApplicationAsync().Wait();
                
                // Initialize Playwright
                InitializePlaywrightAsync().Wait();
                
                _isSetupComplete = true;
            }
            catch (Exception ex)
            {
                testContext.WriteLine($"Failed to initialize test setup: {ex.Message}");
                throw;
            }
        }
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        try
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser = null;
            }

            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
            }

            if (_applicationProcess != null && !_applicationProcess.HasExited)
            {
                _applicationProcess.Kill(true);
                _applicationProcess.Dispose();
                _applicationProcess = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during cleanup: {ex.Message}");
        }
    }

    private static async Task StartApplicationAsync()
    {
        try
        {
            // Check if application is already running on port 5000
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);
            
            try
            {
                var response = await httpClient.GetAsync("http://localhost:5000");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Application is already running on port 5000");
                    return;
                }
            }
            catch
            {
                // Application not running, we need to start it
            }

            // Start the Mock application for testing
            var projectPath = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "..", "src", "Jackson.Ideas.Mock"));

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --no-build --no-restore",
                WorkingDirectory = projectPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Set environment variables
            startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";
            startInfo.EnvironmentVariables["ASPNETCORE_URLS"] = "http://localhost:5000";

            _applicationProcess = Process.Start(startInfo);

            if (_applicationProcess == null)
            {
                throw new InvalidOperationException("Failed to start application process");
            }

            // Wait for application to start
            var maxAttempts = 30;
            var attempt = 0;
            var isApplicationReady = false;

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(2);

            while (attempt < maxAttempts && !isApplicationReady)
            {
                try
                {
                    await Task.Delay(1000);
                    var response = await client.GetAsync("http://localhost:5000");
                    if (response.IsSuccessStatusCode)
                    {
                        isApplicationReady = true;
                        Console.WriteLine($"Application started successfully after {attempt + 1} attempts");
                    }
                }
                catch
                {
                    attempt++;
                    Console.WriteLine($"Waiting for application to start... Attempt {attempt}/{maxAttempts}");
                }
            }

            if (!isApplicationReady)
            {
                var output = await _applicationProcess.StandardOutput.ReadToEndAsync();
                var error = await _applicationProcess.StandardError.ReadToEndAsync();
                throw new TimeoutException($"Application failed to start within {maxAttempts} seconds.\nOutput: {output}\nError: {error}");
            }

            // Give the application a moment to fully initialize
            await Task.Delay(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting application: {ex.Message}");
            throw;
        }
    }

    private static async Task InitializePlaywrightAsync()
    {
        try
        {
            _playwright = await Playwright.CreateAsync();
            
            // Launch browser with specific options for testing
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = Environment.GetEnvironmentVariable("PLAYWRIGHT_HEADLESS") != "false",
                SlowMo = 0,
                Args = new[] { "--disable-web-security", "--disable-features=VizDisplayCompositor" }
            });

            Console.WriteLine("Playwright initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing Playwright: {ex.Message}");
            throw;
        }
    }

    public static IBrowser? GetBrowser() => _browser;
    public static IPlaywright? GetPlaywright() => _playwright;
}