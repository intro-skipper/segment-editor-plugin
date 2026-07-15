using System.Collections.Frozen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

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
    private const int AssetCacheDurationSeconds = 86400;
    private const string CrossOriginOpenerPolicyValue = "same-origin";
    private const string CrossOriginEmbedderPolicyValue = "credentialless";
    private const string EntryPointAssetPath = "index.js";
    private const string NoCacheValue = "no-cache";
    private const string NoStoreValue = "no-store";

    private static readonly Assembly _assembly = typeof(SegmentEditorController).Assembly;
    private static readonly EntityTagHeaderValue _entityTag =
        new($"\"{_assembly.ManifestModule.ModuleVersionId:N}\"");

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
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None)]
    public ActionResult GetIndex()
    {
        return GetEmbeddedResource("SegmentEditorPlugin.dist.index.html", "text/html");
    }

    /// <summary>
    /// Gets embedded static resources from assets folder.
    /// </summary>
    /// <param name="path">The resource path.</param>
    /// <returns>The action result.</returns>
    [HttpGet("assets/{**path}")]
    [ResponseCache(Duration = AssetCacheDurationSeconds)]
    public ActionResult GetAssets(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (string.Equals(path, EntryPointAssetPath, StringComparison.Ordinal))
        {
            Response.Headers.CacheControl = NoCacheValue;
        }

        var resourcePath = $"SegmentEditorPlugin.dist.assets.{path.Replace('/', '.')}";
        var contentType = _contentTypes.GetValueOrDefault(Path.GetExtension(path), "application/octet-stream");
        return GetEmbeddedResource(resourcePath, contentType);
    }

    /// <summary>
    /// Gets the favicon.
    /// </summary>
    /// <returns>The action result.</returns>
    [HttpGet("favicon.png")]
    [ResponseCache(Duration = AssetCacheDurationSeconds)]
    public ActionResult GetFavicon()
    {
        return GetEmbeddedResource("SegmentEditorPlugin.dist.favicon.png", "image/png");
    }

    /// <inheritdoc />
    [NonAction]
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Response.Headers[CrossOriginOpenerPolicyHeader] = CrossOriginOpenerPolicyValue;
        context.HttpContext.Response.Headers[CrossOriginEmbedderPolicyHeader] = CrossOriginEmbedderPolicyValue;
        return next();
    }

    private ActionResult GetEmbeddedResource(string resourcePath, string contentType)
    {
        var stream = _assembly.GetManifestResourceStream(resourcePath);
        if (stream is null)
        {
            Response.Headers.CacheControl = NoStoreValue;
            return NotFound();
        }

        return File(stream, contentType, lastModified: null, entityTag: _entityTag);
    }
}
