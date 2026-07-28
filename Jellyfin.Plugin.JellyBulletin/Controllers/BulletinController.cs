using Jellyfin.Plugin.JellyBulletin.Models;
using Jellyfin.Plugin.JellyBulletin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.JellyBulletin.Controllers;

/// <summary>
/// API used by the home widget and administration page.
/// </summary>
[ApiController]
[Route("Bulletin")]
public sealed class BulletinController : ControllerBase
{
    private readonly BulletinStore _store;

    public BulletinController(BulletinStore store)
    {
        _store = store;
    }

    [HttpGet("News")]
    [Authorize]
    [ProducesResponseType(typeof(BulletinResponse), StatusCodes.Status200OK)]
    public ActionResult<BulletinResponse> GetPublished()
    {
        return Ok(_store.GetPublished());
    }

    [HttpGet("Admin")]
    [Authorize(Policy = "RequiresElevation")]
    [ProducesResponseType(typeof(SaveBulletinsRequest), StatusCodes.Status200OK)]
    public ActionResult<SaveBulletinsRequest> GetAll()
    {
        return Ok(_store.GetAll());
    }

    [HttpPut("Admin")]
    [Authorize(Policy = "RequiresElevation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Save([FromBody] SaveBulletinsRequest request)
    {
        _store.Save(request);
        return NoContent();
    }

    [HttpGet("Client.js")]
    [AllowAnonymous]
    public IActionResult GetClientScript()
    {
        return EmbeddedFile("Web.bulletin.js", "text/javascript; charset=utf-8");
    }

    [HttpGet("Client.css")]
    [AllowAnonymous]
    public IActionResult GetClientStyles()
    {
        return EmbeddedFile("Web.bulletin.css", "text/css; charset=utf-8");
    }

    private FileStreamResult EmbeddedFile(string suffix, string contentType)
    {
        var name = $"{typeof(Plugin).Namespace}.{suffix}";
        var stream = typeof(Plugin).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"Missing resource {name}.");
        return File(stream, contentType);
    }
}
