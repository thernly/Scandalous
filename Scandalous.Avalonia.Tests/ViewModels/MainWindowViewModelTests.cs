using System.ComponentModel.DataAnnotations;
using NSubstitute;
using Scandalous.Avalonia.ViewModels;
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
    public void ScanCommand_IsEnabledWhenScannerIsSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedScanner = "Office scanner";

        Assert.True(viewModel.ScanCommand.CanExecute(null));
    }

    [Fact]
    public void ScanCommand_IsDisabledWhenScanning()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedScanner = "Office scanner";
        viewModel.IsScanning = true;

        Assert.False(viewModel.ScanCommand.CanExecute(null));
    }

    [Theory]
    [InlineData("invoice", true)]
    [InlineData("invoice.pdf", false)]
    public void ValidateBaseFilename_AcceptsOnlyValidBaseNames(string value, bool isValid)
    {
        var result = MainWindowViewModel.ValidateBaseFilename(value, new ValidationContext(new object()));

        Assert.Equal(isValid, result == ValidationResult.Success);
    }

    private static MainWindowViewModel CreateViewModel() => new(
        Substitute.For<IDocumentScanner>(),
        Substitute.For<IConfigurationManager>(),
        Substitute.For<IScanConfigurationMapper>(),
        Substitute.For<IPdfService>(),
        Substitute.For<ILanguageCodeService>(),
        Substitute.For<IScanExceptionHandler>());
}
