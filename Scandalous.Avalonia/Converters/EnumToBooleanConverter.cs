using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Scandalous.Avalonia.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.Equals(parameter) == true;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? parameter! : AvaloniaProperty.UnsetValue;
}
