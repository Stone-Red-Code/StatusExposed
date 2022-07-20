using Microsoft.AspNetCore.Mvc;

using StatusExposed.Models;
using StatusExposed.Services;

namespace StatusExposed.Controllers;

[Route("/api/status")]
[ApiController]
public class StatusController : Controller
{
    private readonly IStatusService statusService;

    public StatusController(IStatusService statusService)
    {
        this.statusService = statusService;
    }

    [HttpGet("{*domain}")]
    public async Task<IActionResult> GetStatus(string domain)
    {
        domain = domain.Replace('/', '.');

        ServiceInformation? statusInformation = await statusService.GetStatusAsync(domain);

        if (statusInformation is null)
        {
            return NotFound($"{domain} is not tracked");
        }

        return Ok(statusInformation);
    }
}