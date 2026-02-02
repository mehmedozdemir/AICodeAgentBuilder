using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.UI.Converters;

/// <summary>
/// Converts AIResponseStatus enum values to colors for status badges.
/// </summary>
public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AIResponseStatus status)
        {
            return status switch
            {
                AIResponseStatus.Pending => Brushes.Orange,
                AIResponseStatus.Validated => Brushes.Green,
                AIResponseStatus.Rejected => Brushes.Red,
                _ => Brushes.Gray
            };
        }
        return Brushes.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
