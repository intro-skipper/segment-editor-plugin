# Segment Editor (Jellyfin Plugin)

<div align="center">
  <p>
    <img alt="Segment Editor" src="images/logo.png" />
  </p>
  <p>
    Segment editing UI embedded into Jellyfin as a plugin page.
  </p>
</div>

This repository packages the Segment Editor web app (built from `intro-skipper/segment-editor`) as a Jellyfin server plugin.

## What you get

- A new main menu entry: "Segment Editor"
- A plugin page that hosts the Segment Editor web UI
- Embedded static assets served by the Jellyfin server at `/SegmentEditor/*` (no separate web hosting)

## Requirements

- Jellyfin Server compatible with the referenced Jellyfin packages (see `SegmentEditorPlugin/SegmentEditorPlugin.csproj`)
- For building from source:
  - .NET SDK 9.0 (see `.github/workflows/build.yml`)
  - Node.js (LTS) + `pnpm` 10 (only if rebuilding frontend assets)

> [!NOTE]
> The plugin project currently targets `net9.0` (see `SegmentEditorPlugin/SegmentEditorPlugin.csproj`).
> Your Jellyfin server must be able to load `net9.0` plugins.

## Installation

## Manifest URL (Recommended)

Add the Intro Skipper repository to Jellyfin, then install "Segment Editor" from the Catalog.

```
https://intro-skipper.org/manifest.json
```

> [!NOTE]
> This URL returns a manifest based on the Jellyfin version used to access it.
> It will not return a manifest when viewed in a browser (no Jellyfin version is provided).

### Option A: Install from a release

1. Download the latest release zip.
2. Extract `SegmentEditorPlugin.dll`.
3. Copy it into your Jellyfin plugins directory:
   - Windows: `%LocalAppData%\jellyfin\plugins\SegmentEditorPlugin\SegmentEditorPlugin.dll`
   - Linux: `~/.local/share/jellyfin/plugins/SegmentEditorPlugin/SegmentEditorPlugin.dll`
   - Docker: mount into the container's `/config/plugins/SegmentEditorPlugin/`
4. Restart Jellyfin.

### Option B: Build and install locally

```bash
dotnet restore SegmentEditorPlugin.sln
dotnet build SegmentEditorPlugin.sln -c Release
```

Then copy `SegmentEditorPlugin/bin/Release/net9.0/SegmentEditorPlugin.dll` to your Jellyfin plugins directory and restart Jellyfin.

## Usage

- Open the Jellyfin web UI and navigate to "Segment Editor" from the main menu.
- You can also reach it from the Dashboard plugin page (Dashboard -> Plugins).
- Direct URL: `http://myserver:8096/SegmentEditor`

## Development

### Update/sync frontend assets

The plugin embeds static files from `SegmentEditorPlugin/dist/` as assembly resources. Those assets are typically produced by the upstream Segment Editor web app.

The CI workflow does the following (see `.github/workflows/build.yml`):

```bash
rm -rf ./SegmentEditorPlugin/dist/assets
mkdir -p ./SegmentEditorPlugin/dist/assets
cp ./segment-editor/dist-plugin/index.html ./SegmentEditorPlugin/dist/index.html
cp ./segment-editor/dist-plugin/favicon.png ./SegmentEditorPlugin/dist/favicon.png
cp -R ./segment-editor/dist-plugin/assets/. ./SegmentEditorPlugin/dist/assets/
```

### Debug in VS Code

This repo includes a VS Code launch/task setup that will publish the plugin and copy it into your Jellyfin data directory.

- Configure paths in `.vscode/settings.json`
- Run the `Launch` debug configuration (uses `.vscode/tasks.json` + `.vscode/launch.json`)

## Releases

- `build.yml` builds the plugin and uploads the compiled DLL as a workflow artifact.
- `release.yml` can auto-bump `Directory.Build.props`, rebuild assets from upstream, and publish a GitHub Release zip.

## License

GPL-3.0. See `LICENSE`.
