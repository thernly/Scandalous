# Scandalous

## Overview

Scandalous is a desktop application (with Windows Forms and WPF interfaces) for scanning documents using TWAIN/WIA compatible scanners. It leverages the NAPS2.Sdk to interact with scanning devices, process images, and export them as PDF files. The application provides a modern, user-friendly interface to configure scan settings, preview scanned pages, and manage output files.

## Features

*   Scan documents from scanners supporting TWAIN/WIA drivers.
*   Support for various color modes: Color, Grayscale, Black and White.
*   Configurable paper sources: Auto, Feeder (Simplex/Duplex), Flatbed.
*   Adjustable scan resolution (DPI).
*   Automatic image processing options:
    *   Auto Deskew: Straightens skewed images.
    *   Exclude Blank Pages: Detects and removes blank pages.
*   **OCR (Optical Character Recognition) support:**
    *   Enable OCR to make scanned PDFs searchable.
    *   Select the desired OCR language from installed Tesseract language data files.
    *   Requires downloading the appropriate `tessdata` language files (see [OCR Requirements](#ocr-requirements)).
*   Output scanned documents as:
    *   A single combined PDF file.
    *   Individual PDF files for each scanned page.
*   User-friendly interface to:
    *   Select output folder and base filename.
    *   Configure scan options (color, DPI, paper source, OCR, etc.).
    *   Initiate scanning.
    *   View status updates.
*   Preview scanned pages (as temporary PNGs) within the application with navigation controls.
*   List available scanner devices.
*   Robust input validation for output folder paths and base filenames.
*   Remembers your last-used scan settings and output location.
*   Automatically attempts to open the generated PDF file (for combined documents).
*   Improved error handling and feedback for common scanner and file issues.

## Prerequisites

*   .NET 10 Runtime.
*   A TWAIN or WIA compatible scanner with its drivers installed on the system.
*   Dependencies (managed via NuGet):
    *   `Microsoft.Extensions.DependencyInjection`
    *   `NAPS2.Sdk`
    *   `NAPS2.Images.Gdi`
    *   `NAPS2.Tesseract.Binaries`

## OCR Requirements

To use the OCR feature, you must download the appropriate Tesseract language data files:

1. Visit [https://github.com/tesseract-ocr/tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast).
2. Download the `.traineddata` files for the languages you wish to use (e.g., `eng.traineddata` for English).
3. Place these files in a folder on your system (e.g., `C:\tessdata`).
4. In the application, set the "Tessdata Folder" to this directory and select the desired language code.

## How to Use

1.  **Launch the Scandalous application.**
2.  **Output Folder:**
    *   The default output folder is displayed.
    *   Click "Browse..." to select a different directory where scanned files (temporary images and final PDFs) will be saved.
3.  **Base Filename:**
    *   Enter a base name for the output PDF files (e.g., "ScannedDocument").
    *   The application will append ".pdf" for combined documents or a unique identifier and ".pdf" for individual documents.
4.  **Color Mode:**
    *   Select the desired color mode for scanning:
        *   `Color`
        *   `Grayscale`
        *   `Black & White`
5.  **Document Options:**
    *   Choose how the scanned pages should be saved:
        *   `Combined`: All scanned pages will be compiled into a single PDF file (e.g., "ScannedDocument.pdf").
        *   `Individual`: Each scanned page will be saved as a separate PDF file (e.g., "ScannedDocument-GUID1.pdf", "ScannedDocument-GUID2.pdf").
6.  **Paper Source:**
    *   Select the paper source:
        *   `Feeder (Duplex)`
        *   `Feeder (Simplex)`
        *   `Flatbed`
        *   `Auto`
7.  **DPI:**
    *   Choose the scan resolution (DPI) from the dropdown.
8.  **Options:**
    *   `Auto Deskew`: Check this box to enable automatic straightening of scanned images.
    *   `Exclude Blank Pages`: Check this box to enable automatic detection and removal of blank pages from the scan job.
    *   **OCR**: Check this box to enable OCR for scanned documents. When enabled:
        *   Set the "Tessdata Folder" to the directory containing your `.traineddata` files.
        *   Select the desired OCR language from the dropdown.
9.  **Scanner Information:**
    *   Click "Get Scanner List" to populate the list box with names of available scanners detected on your system.
    *   The selected scanner will be used for scanning operations.
10. **Scanning:**
    *   Click the "Scan" button to start the scanning process.
    *   The status label at the bottom will provide feedback on the current operation (e.g., "Building configuration...", "Scanning...", "Scanning completed.").
11. **Preview:**
    *   As pages are scanned, they are temporarily saved as PNG images and displayed in the preview area.
    *   Use the "Previous" and "Next" buttons to navigate through the preview of scanned images.
12. **Load Images (for preview):**
    *   Click "Load Images" to load and preview existing PNG images from the currently selected output folder. This is useful for reviewing previously scanned temporary images if needed.
13. **Output:**
    *   After scanning is complete and PDF(s) are generated, the temporary PNG files used for preview are automatically deleted.
    *   If the "Combined" document option was selected, the application will attempt to open the resulting PDF file using the system's default PDF viewer.
14. **Settings Persistence:**
    *   Your last-used scan settings and output location are automatically saved and restored on next launch.

## Configuration Details

The core scan settings are managed through the `ScanConfiguration` class, which is populated based on UI selections:

*   `OutputFolder`: The directory for saving output files.
*   `OutputBaseFileName`: The user-defined base name for output files.
*   `ColorMode`: `ScannerColorMode.Color`, `ScannerColorMode.Grayscale`, or `ScannerColorMode.BlackAndWhite`.
*   `DocumentOptions`: `DocumentOptions.Combined` or `DocumentOptions.Individual`.
*   `AutoDeskew`: `true` or `false`.
*   `ExcludeBlankPages`: `true` or `false`.
*   `ScanResolutionDPI`: User-selectable DPI (default 300).
*   `ScannerPaperSource`: User-selectable (`Auto`, `FeederSimplex`, `FeederDuplex`, `Flatbed`).
*   `OcrEnabled`: `true` or `false`.
*   `TessdataFolder`: Path to the folder containing Tesseract `.traineddata` files.
*   `TessdataLanguageCode`: The selected language code for OCR (e.g., "eng", "deu").

## Input Validation

The application incorporates validators to ensure robust handling of user inputs:

*   **`FolderValidator`**:
    *   Ensures folder names are not null, empty, or whitespace.
    *   Checks for invalid path characters.
    *   Prevents the use of reserved system names (e.g., CON, PRN).
    *   Ensures folder names do not end with a space or a period.
    *   Checks for path traversal sequences (e.g., "..").
    *   Validates folder name length.
*   **`FileNameValidator`**:
    *   Ensures file names (or base file names) are not null, empty, or whitespace.
    *   Checks for invalid file name characters (e.g., `\ / : * ? " < > |`).
    *   Prevents the use of reserved system names.
    *   Ensures file names do not end with a space or a period.
    *   For base file names, ensures they do not contain periods (as extensions are handled separately).
    *   Validates file name length (max 255 characters for the name component).

## Project Structure (Key Components)

*   `Scandalous.Core.csproj`: A class library containing the core logic, services, validators, and configuration for scanning operations.
*   `Scandalous.csproj`: The Windows Forms application project, defining target framework (.NET 10), dependencies, and build settings for the WinForms UI.
*   `Scandalous.WPF.csproj`: The Windows Presentation Foundation application project, providing an alternative modern UI.
*   `Program.cs`: The main entry points for the desktop applications.
*   `FormScan.cs`: Implements the user interface, event handling, and orchestrates the scanning process based on user input.
*   `DocumentScanner.cs`: Contains the core logic for interacting with scanners via `NAPS2.Sdk`. It handles device discovery, scan execution, image processing, PDF export, and OCR integration.
*   `ScanConfiguration.cs`: A data class that holds all configuration parameters for a scan operation.
*   `PageScannedEventArgs.cs` (Defined within `DocumentScanner.cs` context or as a separate file, used by `PageScanned` event): Carries the file path of a newly scanned page image.
*   `FileNameValidator.cs`: A static utility class providing methods to validate file names against common operating system and file system rules.
*   `FolderValidator.cs`: A static utility class providing methods to validate folder names/paths.

## License
© 2025-2026 Thomas Hernly. Released under GNU General Public License v3.0. See LICENSE file for details.
