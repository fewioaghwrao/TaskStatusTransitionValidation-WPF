// ============================
// Converters/BoolToVisibilityConverter.cs
// ============================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskStatusWpf.Converters;

public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// true -> Visible, false -> Collapsed
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return Visibility.Visible;

        return Visibility.Collapsed;
    }

    /// <summary>
    /// Visible -> true, otherwise -> false
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility v && v == Visibility.Visible;
    }
}