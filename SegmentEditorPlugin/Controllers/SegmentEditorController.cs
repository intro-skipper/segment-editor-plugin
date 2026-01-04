using System.Collections.Frozen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SegmentEditorPlugin.Controllers;

/// <summary>
/// Controller for segment editor operations.
/// </summary>
[Route("SegmentEditor")]
[ApiController]
public class SegmentEditorController : ControllerBase
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    private static readonly FrozenDictionary<string, string> _contentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".js"] = "application/javascript",
            [".css"] = "text/css",
            [".woff2"] = "font/woff2",
            [".woff"] = "font/woff",
            [".ttf"] = "font/ttf",
            [".svg"] = "image/svg+xml",
            [".png"] = "image/png",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".json"] = "application/json"
        }.ToFrozenDictionary();

    /// <summary>
    /// Gets main index page.
    /// </summary>
    /// <response code="200">Main index page retrieved.</response>
    /// <returns>The action result.</returns>
    [HttpGet]
    [HttpGet("index.html")]
    [ResponseCache(Duration = 86400)]
    public ActionResult GetIndex()
    {
        var stream = _assembly.GetManifestResourceStream("SegmentEditorPlugin.dist.index.html");
        return stream == null ? NotFound() : new FileStreamResult(stream, "text/html");
    }

    /// <summary>
    /// Gets embedded static resources from assets folder.
    /// </summary>
    /// <param name="path">The resource path.</param>
    /// <returns>The action result.</returns>
    [HttpGet("assets/{**path}")]
    [ResponseCache(Duration = 86400)]
    public ActionResult GetAssets(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var resourcePath = $"SegmentEditorPlugin.dist.assets.{path.Replace('/', '.')}";
        var stream = _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            return NotFound();
        }

        var contentType = _contentTypes.GetValueOrDefault(Path.GetExtension(path), "application/octet-stream");
        return new FileStreamResult(stream, contentType);
    }

    /// <summary>
    /// Gets the favicon.
    /// </summary>
    /// <returns>The action result.</returns>
    [HttpGet("favicon.png")]
    [ResponseCache(Duration = 86400)]
    public ActionResult GetFavicon()
    {
        var stream = _assembly.GetManifestResourceStream("SegmentEditorPlugin.dist.favicon.png");
        return stream == null ? NotFound() : new FileStreamResult(stream, "image/png");
    }
}
