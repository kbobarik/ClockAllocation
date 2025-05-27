using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace clock_allocation.Resources;

public class VisibleDeleteButtonConverter:IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null or not int  => false,
            _ => true
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}