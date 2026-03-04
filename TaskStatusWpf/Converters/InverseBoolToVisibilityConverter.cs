using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskStatusWpf.Converters;

public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    // false -> Visible, true -> Collapsed
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return Visibility.Collapsed;

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v != Visibility.Visible;
}