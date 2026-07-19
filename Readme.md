# Scandalous

Scandalous is a cross-platform Avalonia desktop application for scanning documents and exporting them as PDF files. It uses the NAPS2 SDK for scanner access, image processing, OCR, and PDF generation.

## Features

- Discovers and scans with platform-appropriate scanner backends:
  - Windows: WIA
  - macOS: eSCL-compatible network scanners discovered through Bonjour (`dns-sd`)
  - Linux: SANE
- Supports Color, Grayscale, and Black & White scanning.
- Supports feeder duplex, feeder simplex, and flatbed paper sources.
- Offers 150, 300, 600, and 1200 DPI scan resolutions.
- Can automatically deskew pages and exclude blank pages.
- Exports pages as either one combined PDF or separate PDFs.
- Prompts for additional pages when using a flatbed with combined-PDF output.
- Shows the most recently scanned page in the preview area and reports scan progress.
- Can apply Tesseract OCR to create searchable PDFs.
- Remembers scan settings, output location, selected scanner, and window state.
- Opens a completed combined PDF in the system's default PDF viewer.
- Provides validation and user-friendly errors for common scanner and file problems.

## Requirements

To build and run from source:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A compatible scanner and any drivers or backend required by your operating system
- macOS: an eSCL-compatible scanner available on the local network
- Windows: a scanner available through WIA
- Linux: a scanner available through SANE

The macOS publishing script creates a self-contained application bundle, so the resulting app does not require a separate .NET runtime installation.

Key NuGet dependencies include:

- `Avalonia` 12.1.0
- `CommunityToolkit.Mvvm`
- `Microsoft.Extensions.DependencyInjection`
- `NAPS2.Sdk`
- `NAPS2.Images.ImageSharp`
- `NAPS2.Tesseract.Binaries`

## Build and Run

Restore dependencies:

```bash
dotnet restore
```

Build the Avalonia application:

```bash
dotnet build Scandalous.Avalonia/Scandalous.Avalonia.csproj
```

Or build the entire solution, including the core library and tests:

```bash
dotnet build Scandalous.sln
```

Run the application in development:

```bash
dotnet run --project Scandalous.Avalonia/Scandalous.Avalonia.csproj
```

Run the tests:

```bash
dotnet test Scandalous.Core.Tests/Scandalous.Core.Tests.csproj
```

## Publish for macOS

The included script publishes a self-contained macOS `.app` bundle.

Apple Silicon (`osx-arm64`, the default):

```bash
./publish-mac.sh
```

Intel Mac (`osx-x64`):

```bash
./publish-mac.sh osx-x64
```

The finished bundle is written to `publish/Scandalous.app`. You can run it directly or drag it into `/Applications`.

If the script is not executable, run this once:

```bash
chmod +x publish-mac.sh
```

To create a self-contained publish directory without assembling an `.app` bundle, use the .NET CLI directly:

```bash
# Apple Silicon
dotnet publish Scandalous.Avalonia/Scandalous.Avalonia.csproj \
  -c Release -r osx-arm64 --self-contained true -o publish/out

# Intel Mac
dotnet publish Scandalous.Avalonia/Scandalous.Avalonia.csproj \
  -c Release -r osx-x64 --self-contained true -o publish/out
```

## OCR Setup

OCR requires Tesseract language data files:

1. Download the required `.traineddata` files from [tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast). For example, download `eng.traineddata` for English.
2. Place the files in a directory such as `C:\tessdata` on Windows or `~/tessdata` on macOS and Linux.
3. Expand **OCR Settings** in Scandalous, enable OCR, and choose that directory.
4. Select a language found in the directory.

> **Current limitation:** PDF export currently invokes OCR with the `eng` language code even if another language is selected. Keep `eng.traineddata` in the configured directory. Other language selections are persisted but are not yet passed to the exporter.

## Usage

1. Launch Scandalous. The application automatically searches for scanners; use **Refresh** to search again.
2. Select an output folder and enter a base filename without an extension.
3. Select a scanner.
4. Configure the color mode, paper source, document output, DPI, deskew, blank-page exclusion, and OCR settings.
5. Select **Scan**.
6. For a flatbed scan in Combined PDF mode, select **Yes** after each page to scan another page or **No** to finish.

During scanning, Scandalous saves temporary PNG files in the operating system's temporary directory and displays the most recently scanned page. These temporary files are deleted when the scan finishes.

### Output filenames

- Combined output starts with `<base-name>.pdf`.
- Individual output creates one PDF per page.
- If a filename already exists, Scandalous adds a numeric suffix: `<base-name>_2.pdf`, `<base-name>_3.pdf`, and so on.

After combined output is created, Scandalous attempts to open it in the system's default PDF viewer. Individual PDFs are not opened automatically.

## Configuration

`Scandalous.Core.Models.ScanConfiguration` contains the persisted scan settings:

- `OutputFolder`
- `OutputBaseFileName`
- `ColorMode`
- `DocumentOptions`
- `AutoDeskew`
- `ExcludeBlankPages`
- `ScanResolutionDPI`
- `ScannerPaperSource`
- `OcrEnabled`
- `TessdataFolder`
- `TessdataLanguageCode`
- `SelectedScannerName`
- `LastKnownScannerUrl`

Settings are saved when the main window closes. Window size, position, and state are saved separately.

## Input Validation

`FolderValidator` checks that folder paths are non-empty and rejects invalid path segments, navigation segments (`.` and `..`), trailing spaces or periods, and segments longer than 255 characters. Windows reserved device names such as `CON`, `PRN`, and `NUL` are rejected on Windows.

`FileNameValidator` checks that base filenames are non-empty, contain no extension separator or platform-invalid filename characters, do not end in a space or period, and are no longer than 255 characters. Windows reserved device names are rejected on Windows. The UI currently limits base filename input to 200 characters.

## Project Structure

- `Scandalous.Avalonia/` — Avalonia desktop UI, view model, dialogs, and application entry point.
- `Scandalous.Core/` — scanner integration, PDF and OCR services, configuration, models, and validation.
- `Scandalous.Core.Tests/` — xUnit tests for the core library.
- `Scandalous.sln` — solution containing the Avalonia app, core library, and tests.
- `publish-mac.sh` — creates a self-contained macOS application bundle.
- `ThirdPartyNotices.txt` — notices for third-party software distributed with the application.

## License

Copyright 2025-2026 Thomas Hernly. Released under the [GNU General Public License v3.0](LICENSE).
