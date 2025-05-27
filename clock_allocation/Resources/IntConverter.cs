using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace clock_allocation.Resources;

public class IntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() ?? "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value?.ToString();
        return int.TryParse(str, out var result) ? result :
            0;
    }
}