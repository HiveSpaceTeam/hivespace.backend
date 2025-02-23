using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Interfaces;
using HiveSpace.Domain.Enums;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/files")]
[ApiController]
public class FileController(IStorageService fileService) : Controller
{
    private readonly IStorageService _fileService = fileService;

    [HttpGet("preview/{type}/{fileId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetFileUrl(Guid fileId, StorageType type)
    {
        var result = await _fileService.GetFileUrl(fileId, type);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("upload")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] StorageType type)
    {
        var result = await _fileService.UploadFromFileAsync(file, type);
        return Ok(result);
    }
}
