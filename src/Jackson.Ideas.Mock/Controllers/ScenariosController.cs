using Microsoft.AspNetCore.Mvc;
using Jackson.Ideas.Mock.Services.Interfaces;
using Jackson.Ideas.Mock.Models;

namespace Jackson.Ideas.Mock.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScenariosController : ControllerBase
{
    private readonly IMockDataService _mockDataService;

    public ScenariosController(IMockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllScenarios()
    {
        try
        {
            var scenarios = await _mockDataService.GetAllScenariosAsync();
            return Ok(scenarios);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving scenarios: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScenario(string id)
    {
        try
        {
            var scenarios = await _mockDataService.GetAllScenariosAsync();
            var scenario = scenarios.FirstOrDefault(s => s.Id == id);
            
            if (scenario == null)
            {
                return NotFound($"Scenario with ID '{id}' not found");
            }
            
            return Ok(scenario);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving scenario: {ex.Message}");
        }
    }
}