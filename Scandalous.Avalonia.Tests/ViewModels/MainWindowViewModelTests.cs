using System.ComponentModel.DataAnnotations;
using System.IO;
using NSubstitute;
using Scandalous.Avalonia.ViewModels;
using Scandalous.Core.Models;
using Scandalous.Core.Services;

namespace Scandalous.Avalonia.Tests.ViewModels;

public class MainWindowViewModelTests
{
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
    public async Task ScanCommand_WhenPdfViewerFails_ShowsErrorReturnsToIdleAndUnsubscribes()
    {
        var scanner = new EventTrackingScanner { ScanResult = "output.pdf" };
        var pdfService = Substitute.For<IPdfService>();
        pdfService.PdfFileExists("output.pdf").Returns(true);
        pdfService.When(service => service.OpenPdfFile("output.pdf", Arg.Any<string>()))
            .Do(_ => throw new InvalidOperationException());
        var viewModel = CreateViewModel(scanner, pdfService: pdfService);
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
        public string ScanResult { get; init; } = string.Empty;
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

        public Task<string> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null) =>
            ScanException == null ? Task.FromResult(ScanResult) : Task.FromException<string>(ScanException);

        public Task<List<NAPS2.Scan.ScanDevice>> GetScanDevicesAsync() => Task.FromResult(new List<NAPS2.Scan.ScanDevice>());

        public void Dispose() { }
    }
}
