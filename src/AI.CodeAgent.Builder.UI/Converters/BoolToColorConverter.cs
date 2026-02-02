using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AI.CodeAgent.Builder.UI.Converters;

/// <summary>
/// Converts boolean values to colors for status badges.
/// True (Active) = Green, False (Inactive) = Red
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? Brushes.Green : Brushes.Red;
        }
        return Brushes.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
