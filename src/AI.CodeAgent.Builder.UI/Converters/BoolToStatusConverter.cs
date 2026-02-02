using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AI.CodeAgent.Builder.UI.Converters;

/// <summary>
/// Converts boolean values to status text.
/// True = "Active", False = "Inactive"
/// </summary>
public class BoolToStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? "Active" : "Inactive";
        }
        return "Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
