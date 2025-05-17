# ScanUtility

## Overview

ScanUtility is a Windows Forms application designed for scanning documents using TWAIN/WIA compatible scanners. It leverages the NAPS2.Sdk to interact with scanning devices, process images, and export them as PDF files. The application provides a user interface to configure scan settings, preview scanned pages, and manage output files.

## Features

*   Scan documents from scanners supporting TWAIN/WIA drivers.
*   Support for various color modes: Color, Grayscale, Black and White.
*   Configurable paper sources (backend supports Auto, Feeder, Duplex, Flatbed; UI currently defaults to Feeder/Duplex for scans).
*   Adjustable scan resolution (DPI).
*   Automatic image processing options:
    *   Auto Deskew: Straightens skewed images.
    *   Exclude Blank Pages: Detects and removes blank pages.
*   Output scanned documents as:
    *   A single combined PDF file.
    *   Individual PDF files for each scanned page.
*   User-friendly interface to:
    *   Select output folder and base filename.
    *   Configure scan options.
    *   Initiate scanning.
    *   View status updates.
*   Preview scanned pages (as temporary PNGs) within the application with navigation controls.
*   List available scanner devices.
*   Robust input validation for output folder paths and base filenames.
*   Automatically attempts to open the generated PDF file (for combined documents).

## Prerequisites

*   .NET 9 Runtime.
*   A TWAIN or WIA compatible scanner with its drivers installed on the system.
*   Dependencies (managed via NuGet):
    *   `NAPS2.Sdk`
    *   `NAPS2.Images.Gdi`

## How to Use

1.  **Launch the ScanUtility application.**
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
6.  **Options:**
    *   `Auto Deskew`: Check this box to enable automatic straightening of scanned images.
    *   `Exclude Blank Pages`: Check this box to enable automatic detection and removal of blank pages from the scan job.
7.  **Scanner Information:**
    *   Click "Get Scanner List" to populate the list box with names of available scanners detected on your system.
    *   *Note: The current version of the application will use the first available scanner detected for scanning operations. The paper source is defaulted to Feeder/Duplex.*
8.  **Scanning:**
    *   Click the "Scan" button to start the scanning process.
    *   The status label at the bottom will provide feedback on the current operation (e.g., "Building configuration...", "Scanning...", "Scanning completed.").
9.  **Preview:**
    *   As pages are scanned, they are temporarily saved as PNG images and displayed in the preview area.
    *   Use the "Previous" and "Next" buttons to navigate through the preview of scanned images.
10. **Load Images (for preview):**
    *   Click "Load Images" to load and preview existing PNG images from the currently selected output folder. This is useful for reviewing previously scanned temporary images if needed.
11. **Output:**
    *   After scanning is complete and PDF(s) are generated, the temporary PNG files used for preview are automatically deleted.
    *   If the "Combined" document option was selected, the application will attempt to open the resulting PDF file using the system's default PDF viewer.

## Configuration Details

The core scan settings are managed through the `ScanConfiguration` class, which is populated based on UI selections:

*   `OutputFolder`: The directory for saving output files.
*   `OutputBaseFileName`: The user-defined base name for output files.
*   `ColorMode`: `ScannerColorMode.Color`, `ScannerColorMode.Grayscale`, or `ScannerColorMode.BlackAndWhite`.
*   `DocumentOptions`: `DocumentOptions.Combined` or `DocumentOptions.Individual`.
*   `AutoDeskew`: `true` or `false`.
*   `ExcludeBlankPages`: `true` or `false`.
*   `ScanResolutionDPI`: Defaulted to 300 DPI.
*   `ScannerPaperSource`: Defaulted to `ScannerPaperSource.FeederDuplex` in the current UI workflow.

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

*   `ScanUtility.csproj`: The C# project file, defining target framework (.NET 9), dependencies, and build settings.
*   `Program.cs`: The main entry point for the Windows Forms application.
*   `FormScan.cs`: Implements the user interface, event handling, and orchestrates the scanning process based on user input.
*   `DocumentScanner.cs`: Contains the core logic for interacting with scanners via `NAPS2.Sdk`. It handles device discovery, scan execution, image processing, and PDF export.
*   `ScanConfiguration.cs`: A data class that holds all configuration parameters for a scan operation.
*   `PageScannedEventArgs.cs` (Defined within `DocumentScanner.cs` context or as a separate file, used by `PageScanned` event): Carries the file path of a newly scanned page image.
*   `FileNameValidator.cs`: A static utility class providing methods to validate file names against common operating system and file system rules.
*   `FolderValidator.cs`: A static utility class providing methods to validate folder names/paths.
