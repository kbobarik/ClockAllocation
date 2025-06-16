using System;
using System.Globalization;
using Avalonia.Data.Converters;
using clock_allocation.Models;

namespace clock_allocation.Resources;

public class GroupToStringConverter:IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Group group) return null;
        return group.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}