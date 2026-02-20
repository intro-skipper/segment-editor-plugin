using System.Collections.Frozen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SegmentEditorPlugin.Controllers;

/// <summary>
/// Controller for segment editor operations.
/// </summary>
[Route("SegmentEditor")]
[ApiController]
public class SegmentEditorController : ControllerBase, IAsyncActionFilter
{
    private const string CrossOriginOpenerPolicyHeader = "Cross-Origin-Opener-Policy";
    private const string CrossOriginEmbedderPolicyHeader = "Cross-Origin-Embedder-Policy";
    private const string CrossOriginOpenerPolicyValue = "same-origin";
    private const string CrossOriginEmbedderPolicyValue = "credentialless";

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
            [".json"] = "application/json",
            [".wasm"] = "application/wasm"
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

    /// <inheritdoc />
    [NonAction]
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Response.Headers[CrossOriginOpenerPolicyHeader] = CrossOriginOpenerPolicyValue;
        context.HttpContext.Response.Headers[CrossOriginEmbedderPolicyHeader] = CrossOriginEmbedderPolicyValue;
        await next().ConfigureAwait(false);
    }
}
