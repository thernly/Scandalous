using System.ComponentModel.DataAnnotations;
using System.IO;
using NSubstitute;
using Scandalous.Avalonia.ViewModels;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;

namespace Scandalous.Avalonia.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void FormatPaperSize_ReturnsExpectedDisplayText()
    {
        Assert.Equal("Letter", MainWindowViewModel.FormatPaperSize(ScannerPaperSize.Letter));
        Assert.Equal("A4", MainWindowViewModel.FormatPaperSize(ScannerPaperSize.A4));
        Assert.Equal("Legal", MainWindowViewModel.FormatPaperSize(ScannerPaperSize.Legal));
    }

    [Fact]
    public void ScanCommand_IsDisabledWhenNoScannerIsSelected()
    {
        var viewModel = CreateViewModel();

        Assert.False(viewModel.ScanCommand.CanExecute(null));
    }

    [Fact]
    public void ScanCommand_IsEnabledWhenAllRequiredFieldsAreValid()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            Assert.True(viewModel.ScanCommand.CanExecute(null));
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void ScanCommand_IsDisabledWhenScanning()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.IsScanning = true;
            Assert.False(viewModel.ScanCommand.CanExecute(null));
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void AreScanSettingsEnabled_FollowsIsScanningAndRaisesPropertyChanged()
    {
        var viewModel = CreateViewModel();
        var changedProperties = new List<string>();
        viewModel.PropertyChanged += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.PropertyName))
                changedProperties.Add(e.PropertyName);
        };

        Assert.True(viewModel.AreScanSettingsEnabled);

        viewModel.IsScanning = true;

        Assert.False(viewModel.AreScanSettingsEnabled);
        Assert.Contains(nameof(MainWindowViewModel.AreScanSettingsEnabled), changedProperties);

        viewModel.IsScanning = false;

        Assert.True(viewModel.AreScanSettingsEnabled);
    }

    [Fact]
    public void ScanCommand_IsDisabledWhenOutputFolderDoesNotExist()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.OutputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Assert.False(viewModel.ScanCommand.CanExecute(null));
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void ScanCommand_IsDisabledWhenOcrEnabledAndMissingTrainedData()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.SelectedLanguageCode = "fra";
            Assert.False(viewModel.ScanCommand.CanExecute(null));
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void ScanCommand_IsEnabledWhenOcrDisabledEvenWithoutTessdata()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.OcrEnabled = false;
            viewModel.TessdataFolder = string.Empty;
            viewModel.SelectedLanguageCode = string.Empty;
            Assert.True(viewModel.ScanCommand.CanExecute(null));
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void OcrSettingsExpanded_ToggleDoesNotChangeOcrEnabled()
    {
        var viewModel = CreateViewModel();

        Assert.False(viewModel.IsOcrSettingsExpanded);
        Assert.True(viewModel.OcrEnabled);

        viewModel.IsOcrSettingsExpanded = true;
        Assert.True(viewModel.OcrEnabled);
        Assert.True(viewModel.IsOcrSettingsExpanded);

        viewModel.IsOcrSettingsExpanded = false;
        Assert.True(viewModel.OcrEnabled);
        Assert.False(viewModel.IsOcrSettingsExpanded);

        viewModel.OcrEnabled = false;
        viewModel.IsOcrSettingsExpanded = true;
        Assert.False(viewModel.OcrEnabled);
        Assert.True(viewModel.IsOcrSettingsExpanded);

        viewModel.IsOcrSettingsExpanded = false;
        Assert.False(viewModel.OcrEnabled);
        Assert.False(viewModel.IsOcrSettingsExpanded);
    }

    [Fact]
    public void DisablingOcr_DoesNotCollapseSettingsOrClearSelections()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.IsOcrSettingsExpanded = true;

            viewModel.OcrEnabled = false;

            Assert.True(viewModel.IsOcrSettingsExpanded);
            Assert.Equal(tessdataDir, viewModel.TessdataFolder);
            Assert.Equal("eng", viewModel.SelectedLanguageCode);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void ReEnablingOcr_PreservesPreviousTessdataAndLanguageSelection()
    {
        var viewModel = CreateViewModel();
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            var previousFolder = viewModel.TessdataFolder;
            var previousLanguage = viewModel.SelectedLanguageCode;

            viewModel.OcrEnabled = false;
            Assert.Equal(previousFolder, viewModel.TessdataFolder);
            Assert.Equal(previousLanguage, viewModel.SelectedLanguageCode);

            viewModel.OcrEnabled = true;
            Assert.Equal(previousFolder, viewModel.TessdataFolder);
            Assert.Equal(previousLanguage, viewModel.SelectedLanguageCode);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task InitializeAsync_WhenNoLanguageModelsExist_DisablesOcrAndShowsMessage()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var tessdataDir = Directory.CreateTempSubdirectory("scandalous-tessdata-").FullName;
        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns(new List<NAPS2.Scan.ScanDevice>());

        var configManager = Substitute.For<IConfigurationManager>();
        configManager.LoadConfigurationAsync().Returns(new ScanConfiguration
        {
            OutputFolder = outputDir,
            OutputBaseFileName = "scan",
            OcrEnabled = true,
            TessdataFolder = tessdataDir,
            TessdataLanguageCode = "eng"
        });

        var mapper = Substitute.For<IScanConfigurationMapper>();
        mapper.BuildUIStateFromConfiguration(Arg.Any<ScanConfiguration>()).Returns(new UIState
        {
            OutputFolder = outputDir,
            BaseFileName = "scan",
            OcrEnabled = true,
            TessdataFolder = tessdataDir,
            SelectedLanguageCode = "eng"
        });

        var languageService = Substitute.For<ILanguageCodeService>();
        languageService.GetAvailableLanguageCodes(tessdataDir, Arg.Any<string>()).Returns(new List<string>());
        languageService.GetBestLanguageCode(tessdataDir, Arg.Any<string>()).Returns(string.Empty);

        var viewModel = new MainWindowViewModel(
            scanner,
            configManager,
            mapper,
            Substitute.For<IPdfService>(),
            languageService,
            CreateExceptionHandler());

        try
        {
            await viewModel.InitializeAsync();

            Assert.False(viewModel.OcrEnabled);
            Assert.Equal(string.Empty, viewModel.SelectedLanguageCode);
            Assert.Equal("No OCR languages found", viewModel.NoOcrLanguagesText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task InitializeAsync_WhenEnglishLanguageExists_SelectsEnglishByDefault()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var tessdataDir = Directory.CreateTempSubdirectory("scandalous-tessdata-").FullName;
        File.WriteAllText(Path.Combine(tessdataDir, "eng.traineddata"), "");
        File.WriteAllText(Path.Combine(tessdataDir, "deu.traineddata"), "");

        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns(new List<NAPS2.Scan.ScanDevice>());

        var configManager = Substitute.For<IConfigurationManager>();
        configManager.LoadConfigurationAsync().Returns(new ScanConfiguration
        {
            OutputFolder = outputDir,
            OutputBaseFileName = "scan",
            OcrEnabled = true,
            TessdataFolder = tessdataDir,
            TessdataLanguageCode = string.Empty
        });
        configManager.GetInstalledTessdataLanguageCodes(tessdataDir).Returns(new List<string> { "deu", "eng" });

        var mapper = Substitute.For<IScanConfigurationMapper>();
        mapper.BuildUIStateFromConfiguration(Arg.Any<ScanConfiguration>()).Returns(new UIState
        {
            OutputFolder = outputDir,
            BaseFileName = "scan",
            OcrEnabled = true,
            TessdataFolder = tessdataDir,
            SelectedLanguageCode = string.Empty
        });

        var languageService = new LanguageCodeService(configManager);

        var viewModel = new MainWindowViewModel(
            scanner,
            configManager,
            mapper,
            Substitute.For<IPdfService>(),
            languageService,
            CreateExceptionHandler());

        try
        {
            await viewModel.InitializeAsync();

            Assert.Equal(new[] { "deu", "eng" }, viewModel.AvailableLanguageCodes.ToArray());
            Assert.Equal("eng", viewModel.SelectedLanguageCode);
            Assert.True(viewModel.OcrEnabled);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task SaveConfigurationAsync_PersistsSelectedPaperSize()
    {
        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns(new List<NAPS2.Scan.ScanDevice>());

        var mapper = new ScanConfigurationMapper();
        var configManager = Substitute.For<IConfigurationManager>();
        configManager.LoadConfigurationAsync().Returns(new ScanConfiguration());

        var languageService = Substitute.For<ILanguageCodeService>();
        languageService.GetAvailableLanguageCodes(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { "eng" });
        languageService.GetBestLanguageCode(Arg.Any<string>(), Arg.Any<string>()).Returns("eng");

        var viewModel = new MainWindowViewModel(
            scanner,
            configManager,
            mapper,
            Substitute.For<IPdfService>(),
            languageService,
            CreateExceptionHandler());

        viewModel.PaperSize = ScannerPaperSize.A4;

        await viewModel.SaveConfigurationAsync();

        await configManager.Received(1).SaveConfigurationAsync(Arg.Is<ScanConfiguration>(c => c.PaperSize == ScannerPaperSize.A4));
    }

    [Fact]
    public async Task InitializeAsync_RestoresSavedPaperSize()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var tessdataDir = Directory.CreateTempSubdirectory("scandalous-tessdata-").FullName;
        File.WriteAllBytes(Path.Combine(tessdataDir, "eng.traineddata"), [1]);

        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns(new List<NAPS2.Scan.ScanDevice>());

        var configManager = Substitute.For<IConfigurationManager>();
        configManager.LoadConfigurationAsync().Returns(new ScanConfiguration());

        var mapper = Substitute.For<IScanConfigurationMapper>();
        mapper.BuildUIStateFromConfiguration(Arg.Any<ScanConfiguration>()).Returns(new UIState
        {
            OutputFolder = outputDir,
            BaseFileName = "scan",
            OcrEnabled = true,
            TessdataFolder = tessdataDir,
            SelectedLanguageCode = "eng",
            PaperSize = ScannerPaperSize.A4
        });

        var languageService = Substitute.For<ILanguageCodeService>();
        languageService.GetAvailableLanguageCodes(tessdataDir, Arg.Any<string>()).Returns(new List<string> { "eng" });
        languageService.GetBestLanguageCode(tessdataDir, Arg.Any<string>()).Returns("eng");

        var viewModel = new MainWindowViewModel(
            scanner,
            configManager,
            mapper,
            Substitute.For<IPdfService>(),
            languageService,
            CreateExceptionHandler());

        try
        {
            await viewModel.InitializeAsync();

            Assert.Equal(ScannerPaperSize.A4, viewModel.PaperSize);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }


    [Theory]
    [InlineData("invoice", true)]
    [InlineData("invoice.pdf", false)]
    public void ValidateBaseFilename_AcceptsOnlyValidBaseNames(string value, bool isValid)
    {
        var result = MainWindowViewModel.ValidateBaseFilename(value, new ValidationContext(new object()));

        Assert.Equal(isValid, result == ValidationResult.Success);
    }

    [Fact]
    public async Task ScanCommand_WhenConfigurationMappingFails_ShowsErrorAndReturnsToIdle()
    {
        var scanner = new EventTrackingScanner();
        var mapper = Substitute.For<IScanConfigurationMapper>();
        mapper.BuildConfigurationFromUIState(Arg.Any<UIState>()).Returns(_ => throw new InvalidOperationException());
        var viewModel = CreateViewModel(scanner, mapper);
        var (outputDir, tessdataDir) = SetValidState(viewModel);
        string? errorMessage = null;

        try
        {
            viewModel.ShowErrorDialogAsync = (_, message) =>
            {
                errorMessage = message;
                return Task.CompletedTask;
            };

            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.False(viewModel.IsScanning);
            Assert.NotNull(errorMessage);
            Assert.Equal(0, scanner.SubscriberCount);
            Assert.Equal(0, scanner.UnsubscribeCount);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenScannerFails_ShowsErrorReturnsToIdleAndUnsubscribes()
    {
        var scanner = new EventTrackingScanner { ScanException = new InvalidOperationException() };
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);
        string? errorMessage = null;

        try
        {
            viewModel.ShowErrorDialogAsync = (_, message) =>
            {
                errorMessage = message;
                return Task.CompletedTask;
            };

            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.False(viewModel.IsScanning);
            Assert.NotNull(errorMessage);
            Assert.Equal(0, scanner.SubscriberCount);
            Assert.Equal(1, scanner.UnsubscribeCount);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenPdfViewerFails_ShowsWarningReturnsToIdleAndUnsubscribes()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var outputPath = Path.Combine(outputDir, "output.pdf");
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 1,
                OutputFiles = [outputPath]
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        pdfService.PdfFileExists(outputPath).Returns(true);
        pdfService.When(service => service.OpenPdfFile(outputPath, Arg.Any<string>()))
            .Do(_ => throw new InvalidOperationException());
        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (_, tessdataDir) = SetValidState(viewModel);
        viewModel.OutputFolder = outputDir;
        string? errorMessage = null;

        try
        {
            viewModel.ShowErrorDialogAsync = (_, message) =>
            {
                errorMessage = message;
                return Task.CompletedTask;
            };

            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.False(viewModel.IsScanning);
            Assert.Null(errorMessage);
            Assert.Equal("output.pdf was created.", viewModel.CompletionText);
            Assert.Equal("Warning: Could not open the created PDF.", viewModel.CompletionWarningText);
            Assert.Equal(0, scanner.SubscriberCount);
            Assert.Equal(1, scanner.UnsubscribeCount);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task CancelCommand_WhenScanIsRunning_CancelsAndShowsCanceledStatusWithoutError()
    {
        var scanner = new CancelAwareScanner();
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);
        string? errorMessage = null;

        try
        {
            viewModel.ShowErrorDialogAsync = (_, message) =>
            {
                errorMessage = message;
                return Task.CompletedTask;
            };

            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);

            Assert.True(viewModel.CanCancelScan);
            Assert.True(viewModel.CancelCommand.CanExecute(null));

            viewModel.CancelCommand.Execute(null);
            await scanTask;

            Assert.False(viewModel.IsScanning);
            Assert.Equal("Scan canceled.", viewModel.StatusText);
            Assert.Null(errorMessage);
            Assert.Equal(1, scanner.CancelRequestCount);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task CancelCommand_WhenCancellationIsRequested_DoesNotCreateMultipleOperations()
    {
        var scanner = new CancelAwareScanner();
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);

            Assert.True(viewModel.CancelCommand.CanExecute(null));

            viewModel.CancelCommand.Execute(null);
            viewModel.CancelCommand.Execute(null);
            await scanTask;

            Assert.False(viewModel.CancelCommand.CanExecute(null));
            Assert.Equal(1, scanner.CancelRequestCount);
            Assert.False(viewModel.IsScanning);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task CancelCommand_AfterFirstPageCapture_CancelsBeforeNextPageCompletes()
    {
        var scanner = new CancelAfterFirstPageScanner();
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);
            await scanner.WaitForFirstPageAsync();

            Assert.True(viewModel.CancelCommand.CanExecute(null));
            viewModel.CancelCommand.Execute(null);

            await scanTask;

            Assert.Equal("Scan canceled.", viewModel.StatusText);
            Assert.False(viewModel.IsScanning);
            Assert.Equal(1, scanner.CancelRequestCount);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task CancelScanAndWaitAsync_WhenScanIsActive_CancelsAndWaitsForCompletion()
    {
        var scanner = new CancelAwareScanner();
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);

            await viewModel.CancelScanAndWaitAsync();
            await scanTask;

            Assert.False(viewModel.IsScanning);
            Assert.Equal("Scan canceled.", viewModel.StatusText);
            Assert.Equal(1, scanner.CancelRequestCount);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task CancelScanAndWaitAsync_WhenScanIsNotActive_DoesNothing()
    {
        var scanner = new CancelAwareScanner();
        var viewModel = CreateViewModel(scanner);

        await viewModel.CancelScanAndWaitAsync();

        Assert.False(viewModel.IsScanning);
        Assert.Equal(0, scanner.CancelRequestCount);
    }

    [Fact]
    public async Task ScanCommand_UsesCapturedConfigurationForCompletionDecisions()
    {
        var scanner = new DeferredScanner();
        var mapper = Substitute.For<IScanConfigurationMapper>();
        var pdfService = Substitute.For<IPdfService>();
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var tessdataDir = Directory.CreateTempSubdirectory("scandalous-tessdata-").FullName;
        File.WriteAllBytes(Path.Combine(tessdataDir, "eng.traineddata"), [1]);

        mapper.BuildConfigurationFromUIState(Arg.Any<UIState>()).Returns(callInfo =>
        {
            var uiState = callInfo.Arg<UIState>();
            return new ScanConfiguration
            {
                OutputFolder = uiState.OutputFolder,
                DocumentOptions = uiState.DocumentCombined ? DocumentOptions.Combined : DocumentOptions.Individual,
                ScannerPaperSource = uiState.Flatbed ? ScannerPaperSource.Flatbed : ScannerPaperSource.FeederDuplex
            };
        });

        var viewModel = CreateViewModel(scanner, mapper, pdfService);
        var outputPath = Path.Combine(outputDir, "output_2.pdf");
        pdfService.PdfFileExists(outputPath).Returns(true);

        try
        {
            viewModel.OutputFolder = outputDir;
            viewModel.SelectedScanner = "Office scanner";
            viewModel.OcrEnabled = true;
            viewModel.TessdataFolder = tessdataDir;
            viewModel.SelectedLanguageCode = "eng";
            viewModel.DocumentOption = DocumentOptions.Combined;

            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);

            // Simulate a user edit during scanning.
            viewModel.DocumentOption = DocumentOptions.Individual;
            scanner.Complete(new ScanResult
            {
                CapturedPageCount = 1,
                OutputFiles = [outputPath]
            });

            await scanTask;

            pdfService.Received(1).OpenPdfFile(outputPath, outputDir);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenIndividualOutputIsReturned_DoesNotAutoOpenPdf()
    {
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 2,
                OutputFiles = ["first.pdf", "second.pdf"]
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.DocumentOption = DocumentOptions.Individual;

            await viewModel.ScanCommand.ExecuteAsync(null);

            pdfService.DidNotReceive().OpenPdfFile(Arg.Any<string>(), Arg.Any<string>());
            Assert.False(viewModel.IsScanning);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenZeroPagesAreReturned_DoesNotAutoOpenPdf()
    {
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 0,
                OutputFiles = []
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            pdfService.DidNotReceive().OpenPdfFile(Arg.Any<string>(), Arg.Any<string>());
            Assert.False(viewModel.IsScanning);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenZeroPagesAreReturned_UsesNoPagesCompletionMessage()
    {
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 0,
                OutputFiles = []
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal("No pages were captured. No PDF was created.", viewModel.CompletionText);
            Assert.Equal(string.Empty, viewModel.CompletionToolTip);
            Assert.False(viewModel.CanOpenPdf);
            Assert.False(viewModel.CanOpenOutputFolder);
            Assert.Equal("No pages were captured. No PDF was created.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenSingleOutputIsReturned_UsesCreatedMessageAndAutoOpensPdf()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var outputPath = Path.Combine(outputDir, "report.pdf");
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 1,
                OutputFiles = [outputPath]
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        pdfService.PdfFileExists(outputPath).Returns(true);

        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (_, tessdataDir) = SetValidState(viewModel);
        viewModel.OutputFolder = outputDir;

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal("report.pdf was created.", viewModel.CompletionText);
            Assert.Equal(outputPath, viewModel.CompletionToolTip);
            Assert.True(viewModel.CanOpenPdf);
            Assert.True(viewModel.CanOpenOutputFolder);
            Assert.Equal("report.pdf was created.", viewModel.StatusText);
            pdfService.Received(1).OpenPdfFile(outputPath, outputDir);

            viewModel.OpenPdfCommand.Execute(null);
            pdfService.Received(2).OpenPdfFile(outputPath, outputDir);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenMultipleOutputsAreReturned_UsesCountMessageAndFolderAction()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var firstOutputPath = Path.Combine(outputDir, "page-1.pdf");
        var secondOutputPath = Path.Combine(outputDir, "page-2.pdf");
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 2,
                OutputFiles = [firstOutputPath, secondOutputPath]
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (_, tessdataDir) = SetValidState(viewModel);
        viewModel.OutputFolder = outputDir;

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal("2 PDF files were created.", viewModel.CompletionText);
            Assert.Equal($"{firstOutputPath}{Environment.NewLine}{secondOutputPath}", viewModel.CompletionToolTip);
            Assert.False(viewModel.CanOpenPdf);
            Assert.True(viewModel.CanOpenOutputFolder);
            pdfService.DidNotReceive().OpenPdfFile(Arg.Any<string>(), Arg.Any<string>());

            viewModel.OpenOutputFolderCommand.Execute(null);
            pdfService.Received(1).OpenOutputFolder(outputDir, outputDir);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenPdfLaunchFails_ShowsWarningAndPreservesSuccessState()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var outputPath = Path.Combine(outputDir, "report.pdf");
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 1,
                OutputFiles = [outputPath]
            }
        };
        var pdfService = Substitute.For<IPdfService>();
        pdfService.PdfFileExists(outputPath).Returns(true);
        pdfService.When(service => service.OpenPdfFile(outputPath, outputDir)).Do(_ => throw new InvalidOperationException("boom"));

        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (_, tessdataDir) = SetValidState(viewModel);
        viewModel.OutputFolder = outputDir;

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal("report.pdf was created.", viewModel.CompletionText);
            Assert.Equal("Warning: Could not open the created PDF.", viewModel.CompletionWarningText);
            Assert.True(viewModel.CanOpenPdf);
            Assert.Equal("report.pdf was created.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task OpenOutputFolderCommand_WhenLaunchFails_ShowsWarningAndPreservesSuccessState()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var firstOutputPath = Path.Combine(outputDir, "page-1.pdf");
        var secondOutputPath = Path.Combine(outputDir, "page-2.pdf");
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 2,
                OutputFiles = [firstOutputPath, secondOutputPath]
            }
        };

        var pdfService = Substitute.For<IPdfService>();
        pdfService.When(service => service.OpenOutputFolder(outputDir, outputDir)).Do(_ => throw new InvalidOperationException("boom"));

        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
        var (_, tessdataDir) = SetValidState(viewModel);
        viewModel.OutputFolder = outputDir;

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal("2 PDF files were created.", viewModel.CompletionText);
            Assert.True(viewModel.CanOpenOutputFolder);

            viewModel.OpenOutputFolderCommand.Execute(null);

            Assert.Equal("2 PDF files were created.", viewModel.CompletionText);
            Assert.Equal("Warning: Could not open the output folder.", viewModel.CompletionWarningText);
            Assert.Equal("2 PDF files were created.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void StatusKind_StartsAsWorkingWhileSearchingForScanners()
    {
        var viewModel = CreateViewModel();

        Assert.Equal(StatusKind.Working, viewModel.StatusKind);
        Assert.Equal("Searching for scanners...", viewModel.StatusText);
    }

    [Fact]
    public async Task GetScannersCommand_WhenScannersAreFound_SetsSuccessStatus()
    {
        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns(Task.FromResult(new List<NAPS2.Scan.ScanDevice>
        {
            new(NAPS2.Scan.Driver.Escl, "device-1", "Office scanner")
        }));
        var viewModel = CreateViewModel(scanner);

        await viewModel.GetScannersCommand.ExecuteAsync(null);

        Assert.Equal(StatusKind.Success, viewModel.StatusKind);
        Assert.Equal("Found 1 scanner(s).", viewModel.StatusText);
    }

    [Fact]
    public async Task GetScannersCommand_WhenNoScannersAreFound_SetsWarningStatus()
    {
        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns(Task.FromResult(new List<NAPS2.Scan.ScanDevice>()));
        var viewModel = CreateViewModel(scanner);

        await viewModel.GetScannersCommand.ExecuteAsync(null);

        Assert.Equal(StatusKind.Warning, viewModel.StatusKind);
        Assert.StartsWith("No scanners found.", viewModel.StatusText);
    }

    [Fact]
    public async Task GetScannersCommand_WhenDiscoveryFails_SetsErrorStatusWithoutRawExceptionText()
    {
        var scanner = Substitute.For<IDocumentScanner>();
        scanner.GetScanDevicesAsync().Returns<Task<List<NAPS2.Scan.ScanDevice>>>(
            _ => throw new InvalidOperationException("raw native failure 0x8007"));
        var viewModel = CreateViewModel(scanner);

        await viewModel.GetScannersCommand.ExecuteAsync(null);

        Assert.Equal(StatusKind.Error, viewModel.StatusKind);
        Assert.DoesNotContain("0x8007", viewModel.StatusText);
        Assert.Equal("Could not search for scanners. Check that the scanner is connected, then try again.", viewModel.StatusText);
    }

    [Fact]
    public async Task ScanCommand_WhileScanIsRunning_SetsWorkingStatusAndResetsPageCount()
    {
        var scanner = new DeferredScanner();
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);

            Assert.Equal(StatusKind.Working, viewModel.StatusKind);
            Assert.Equal("Connecting to scanner...", viewModel.StatusText);
            Assert.Equal(0, viewModel.PageCount);

            scanner.Complete(new ScanResult { CapturedPageCount = 0, OutputFiles = [] });
            await scanTask;
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenScanSucceeds_SetsSuccessStatus()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var outputPath = Path.Combine(outputDir, "report.pdf");
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult
            {
                CapturedPageCount = 1,
                OutputFiles = [outputPath]
            }
        };
        var viewModel = CreateViewModel(scanner, pdfService: Substitute.For<IPdfService>());
        var (_, tessdataDir) = SetValidState(viewModel);
        viewModel.OutputFolder = outputDir;

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal(StatusKind.Success, viewModel.StatusKind);
            Assert.Equal("report.pdf was created.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenZeroPagesAreReturned_SetsWarningStatus()
    {
        var scanner = new EventTrackingScanner
        {
            ScanResult = new ScanResult { CapturedPageCount = 0, OutputFiles = [] }
        };
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal(StatusKind.Warning, viewModel.StatusKind);
            Assert.Equal("No pages were captured. No PDF was created.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task ScanCommand_WhenScannerFails_SetsErrorStatus()
    {
        var scanner = new EventTrackingScanner { ScanException = new InvalidOperationException("raw driver text") };
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            viewModel.ShowErrorDialogAsync = (_, _) => Task.CompletedTask;

            await viewModel.ScanCommand.ExecuteAsync(null);

            Assert.Equal(StatusKind.Error, viewModel.StatusKind);
            Assert.Equal("Scanning could not be completed.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public async Task CancelCommand_SetsWorkingThenWarningStatus()
    {
        var scanner = new CancelAwareScanner();
        var viewModel = CreateViewModel(scanner);
        var (outputDir, tessdataDir) = SetValidState(viewModel);

        try
        {
            var scanTask = viewModel.ScanCommand.ExecuteAsync(null);
            await WaitUntilAsync(() => viewModel.IsScanning);

            await viewModel.CancelCommand.ExecuteAsync(null);

            Assert.Equal(StatusKind.Working, viewModel.StatusKind);
            Assert.Equal("Canceling scan...", viewModel.StatusText);

            await scanTask;

            Assert.Equal(StatusKind.Warning, viewModel.StatusKind);
            Assert.Equal("Scan canceled.", viewModel.StatusText);
        }
        finally
        {
            Cleanup(outputDir, tessdataDir);
        }
    }

    [Fact]
    public void GetContinuationPrompt_ForFlatbedCombined_UsesScanNextPageAndFinishLabels()
    {
        var prompt = MainWindowViewModel.GetContinuationPrompt(new ScanConfiguration
        {
            ScannerPaperSource = ScannerPaperSource.Flatbed,
            DocumentOptions = DocumentOptions.Combined
        });

        Assert.NotNull(prompt);
        Assert.Equal("Scan Next Page", prompt.PrimaryLabel);
        Assert.Equal("Finish", prompt.SecondaryLabel);
        Assert.DoesNotContain("Yes", prompt.Message);
        Assert.DoesNotContain("No", prompt.Message);
    }

    [Theory]
    [InlineData(ScannerPaperSource.FeederDuplex, DocumentOptions.Combined)]
    [InlineData(ScannerPaperSource.FeederSimplex, DocumentOptions.Combined)]
    [InlineData(ScannerPaperSource.Flatbed, DocumentOptions.Individual)]
    public void GetContinuationPrompt_ForOtherConfigurations_ReturnsNull(
        ScannerPaperSource paperSource,
        DocumentOptions documentOptions)
    {
        Assert.Null(MainWindowViewModel.GetContinuationPrompt(new ScanConfiguration
        {
            ScannerPaperSource = paperSource,
            DocumentOptions = documentOptions
        }));
    }

    private static (string OutputDir, string TessdataDir) SetValidState(MainWindowViewModel viewModel)
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var tessdataDir = Directory.CreateTempSubdirectory("scandalous-tessdata-").FullName;
        File.WriteAllBytes(Path.Combine(tessdataDir, "eng.traineddata"), [1]);

        viewModel.OutputFolder = outputDir;
        viewModel.SelectedScanner = "Office scanner";
        viewModel.OcrEnabled = true;
        viewModel.TessdataFolder = tessdataDir;
        viewModel.SelectedLanguageCode = "eng";

        return (outputDir, tessdataDir);
    }

    private static void Cleanup(string outputDir, string tessdataDir)
    {
        if (Directory.Exists(outputDir))
            Directory.Delete(outputDir, recursive: true);
        if (Directory.Exists(tessdataDir))
            Directory.Delete(tessdataDir, recursive: true);
    }

    private static MainWindowViewModel CreateViewModel(
        IDocumentScanner? scanner = null,
        IScanConfigurationMapper? mapper = null,
        IPdfService? pdfService = null) => new(
        scanner ?? Substitute.For<IDocumentScanner>(),
        Substitute.For<IConfigurationManager>(),
        mapper ?? CreateMapper(),
        pdfService ?? Substitute.For<IPdfService>(),
        Substitute.For<ILanguageCodeService>(),
        CreateExceptionHandler());

    private static IScanConfigurationMapper CreateMapper()
    {
        var mapper = Substitute.For<IScanConfigurationMapper>();
        mapper.BuildConfigurationFromUIState(Arg.Any<UIState>()).Returns(new ScanConfiguration());
        return mapper;
    }

    private static IScanExceptionHandler CreateExceptionHandler()
    {
        var handler = Substitute.For<IScanExceptionHandler>();
        handler.HandleScanException(Arg.Any<Exception>()).Returns(new ScanExceptionResult
        {
            UserMessage = "Scanning could not be completed."
        });
        return handler;
    }

    private sealed class EventTrackingScanner : IDocumentScanner
    {
        private EventHandler<PageScannedEventArgs>? _pageScanned;

        public Exception? ScanException { get; init; }
        public ScanResult ScanResult { get; init; } = new();
        public int SubscriberCount { get; private set; }
        public int UnsubscribeCount { get; private set; }

        public event EventHandler<PageScannedEventArgs>? PageScanned
        {
            add
            {
                _pageScanned += value;
                SubscriberCount++;
            }
            remove
            {
                _pageScanned -= value;
                SubscriberCount--;
                UnsubscribeCount++;
            }
        }

        public Task<ScanResult> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null) =>
            ScanException == null ? Task.FromResult(ScanResult) : Task.FromException<ScanResult>(ScanException);

        public Task<List<NAPS2.Scan.ScanDevice>> GetScanDevicesAsync() => Task.FromResult(new List<NAPS2.Scan.ScanDevice>());

        public void Dispose() { }
    }

    private sealed class CancelAwareScanner : IDocumentScanner
    {
        private int _cancelRequestCount;

        public int CancelRequestCount => _cancelRequestCount;

        public event EventHandler<PageScannedEventArgs>? PageScanned
        {
            add { }
            remove { }
        }

        public async Task<ScanResult> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null)
        {
            cancellationToken.Register(() => Interlocked.Increment(ref _cancelRequestCount));
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            return new ScanResult
            {
                CapturedPageCount = 0,
                OutputFiles = []
            };
        }

        public Task<List<NAPS2.Scan.ScanDevice>> GetScanDevicesAsync() => Task.FromResult(new List<NAPS2.Scan.ScanDevice>());

        public void Dispose() { }
    }

    private sealed class CancelAfterFirstPageScanner : IDocumentScanner
    {
        private int _cancelRequestCount;
        private readonly TaskCompletionSource<bool> _firstPageScanned = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int CancelRequestCount => _cancelRequestCount;

        public event EventHandler<PageScannedEventArgs>? PageScanned;

        public async Task<ScanResult> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null)
        {
            var firstFile = Path.Combine(Path.GetTempPath(), $"scan-{Guid.NewGuid():N}.png");
            await File.WriteAllBytesAsync(firstFile, [1, 2, 3, 4]);
            PageScanned?.Invoke(this, new PageScannedEventArgs(firstFile));
            _firstPageScanned.TrySetResult(true);

            cancellationToken.Register(() => Interlocked.Increment(ref _cancelRequestCount));
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            return new ScanResult
            {
                CapturedPageCount = 0,
                OutputFiles = []
            };
        }

        public Task WaitForFirstPageAsync() => _firstPageScanned.Task;

        public Task<List<NAPS2.Scan.ScanDevice>> GetScanDevicesAsync() => Task.FromResult(new List<NAPS2.Scan.ScanDevice>());

        public void Dispose() { }
    }

    private sealed class DeferredScanner : IDocumentScanner
    {
        private readonly TaskCompletionSource<ScanResult> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public event EventHandler<PageScannedEventArgs>? PageScanned
        {
            add { }
            remove { }
        }

        public Task<ScanResult> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null) =>
            _tcs.Task;

        public Task<List<NAPS2.Scan.ScanDevice>> GetScanDevicesAsync() => Task.FromResult(new List<NAPS2.Scan.ScanDevice>());

        public void Complete(ScanResult scanResult) => _tcs.TrySetResult(scanResult);

        public void Dispose() { }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 50; attempt++)
        {
            if (condition())
                return;

            await Task.Yield();
        }

        throw new TimeoutException("Condition was not met in time.");
    }
}
