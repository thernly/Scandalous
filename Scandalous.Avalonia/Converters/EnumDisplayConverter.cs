using System.Globalization;
using Avalonia.Data.Converters;
using Scandalous.Avalonia.ViewModels;
using Scandalous.Core.Enums;

namespace Scandalous.Avalonia.Converters;

public class EnumDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        ScannerColorMode mode => MainWindowViewModel.FormatColorMode(mode),
        ScannerPaperSource source => MainWindowViewModel.FormatPaperSource(source),
        ScannerPaperSize size => MainWindowViewModel.FormatPaperSize(size),
        DocumentOptions option => MainWindowViewModel.FormatDocumentOption(option),
        _ => value?.ToString() ?? string.Empty
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
