using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Jackson.Ideas.Mock.Models;
using Jackson.Ideas.Mock.Services.Interfaces;
using System.Security.Claims;

namespace Jackson.Ideas.Mock.Services;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IMockAuthenticationService _authService;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly IWebHostEnvironment _environment;
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public MockAuthenticationStateProvider(
        IMockAuthenticationService authService, 
        ProtectedSessionStorage sessionStorage,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _sessionStorage = sessionStorage;
        _environment = environment;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Check if we already have a cached authenticated user
        if (_cachedUser.Identity?.IsAuthenticated == true)
        {
            return new AuthenticationState(_cachedUser);
        }

        // In development mode, auto-authenticate with demo user
        // Do this first to avoid JavaScript interop during prerendering
        if (_environment.IsDevelopment())
        {
            var demoUser = await _authService.GetUserByEmailAsync("demo@ideasmatter.com");
            if (demoUser != null)
            {
                _cachedUser = CreateClaimsPrincipal(demoUser);
                return new AuthenticationState(_cachedUser);
            }
        }

        // Try to get user from session storage (only if not prerendering)
        try
        {
            var userResult = await _sessionStorage.GetAsync<string>("userId");
            if (userResult.Success && !string.IsNullOrEmpty(userResult.Value))
            {
                var user = await _authService.GetUserByIdAsync(userResult.Value);
                if (user != null)
                {
                    _cachedUser = CreateClaimsPrincipal(user);
                    return new AuthenticationState(_cachedUser);
                }
            }
        }
        catch (InvalidOperationException)
        {
            // Session storage not available during prerendering - that's ok
            // We already handled dev mode auth above
        }

        // Return anonymous user
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task LoginAsync(string email, string password)
    {
        var user = await _authService.LoginAsync(email, password);
        if (user != null)
        {
            _cachedUser = CreateClaimsPrincipal(user);
            
            // Try to store in session storage, but don't fail if it's not available
            try
            {
                await _sessionStorage.SetAsync("userId", user.Id);
            }
            catch (InvalidOperationException)
            {
                // Session storage not available - that's ok, we'll rely on cached user
            }
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
        }
    }

    public async Task LoginAsync(MockUser user)
    {
        _cachedUser = CreateClaimsPrincipal(user);
        
        // Try to store in session storage, but don't fail if it's not available
        try
        {
            await _sessionStorage.SetAsync("userId", user.Id);
        }
        catch (InvalidOperationException)
        {
            // Session storage not available - that's ok, we'll rely on cached user
        }
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
    }

    public async Task LogoutAsync()
    {
        _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
        
        // Try to clear session storage, but don't fail if it's not available
        try
        {
            await _sessionStorage.DeleteAsync("userId");
        }
        catch (InvalidOperationException)
        {
            // Session storage not available - that's ok
        }
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(MockUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, user.Role)
        };

        if (!string.IsNullOrEmpty(user.Company))
        {
            claims.Add(new Claim("company", user.Company));
        }

        if (!string.IsNullOrEmpty(user.JobTitle))
        {
            claims.Add(new Claim("job_title", user.JobTitle));
        }

        var identity = new ClaimsIdentity(claims, "mock-auth");
        return new ClaimsPrincipal(identity);
    }
}