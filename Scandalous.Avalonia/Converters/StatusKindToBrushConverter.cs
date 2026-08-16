using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Scandalous.Core.Enums;

namespace Scandalous.Avalonia.Converters;

/// <summary>
/// Maps a <see cref="StatusKind"/> to a theme brush so the status area distinguishes
/// idle, working, success, warning, and error states in both light and dark themes.
/// </summary>
public class StatusKindToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var (resourceKey, fallback) = (value as StatusKind?) switch
        {
            StatusKind.Working => ("SystemFillColorAttentionBrush", Colors.SteelBlue),
            StatusKind.Success => ("SystemFillColorSuccessBrush", Colors.Green),
            StatusKind.Warning => ("SystemFillColorCautionBrush", Colors.DarkOrange),
            StatusKind.Error => ("SystemFillColorCriticalBrush", Colors.Firebrick),
            _ => ("TextFillColorSecondaryBrush", Colors.Gray)
        };

        if (Application.Current != null
            && Application.Current.TryGetResource(resourceKey, Application.Current.ActualThemeVariant, out var resource)
            && resource is IBrush brush)
        {
            return brush;
        }

        return new SolidColorBrush(fallback);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
