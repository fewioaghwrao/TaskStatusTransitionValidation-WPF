using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskStatusWpf.Converters;

public sealed class TaskStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var s = (value?.ToString() ?? "").Trim().ToLowerInvariant();

        return s switch
        {
            "todo" => Brushes.DodgerBlue,
            "doing" => Brushes.Orange,
            "done" => Brushes.SeaGreen,
            "blocked" => Brushes.Tomato,
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}