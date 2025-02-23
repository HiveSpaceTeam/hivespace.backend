using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Interfaces;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/location")]
[ApiController]
[Authorize]
public class LocationController : Controller
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetLocation(int type, string? parentCode)
    {
        var result = await _locationService.GetLocationAsync(type, parentCode);
        return Ok(result);
    }
}
