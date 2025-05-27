using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using clock_allocation.Models;

namespace clock_allocation.Resources;

public class TupleConverter : IMultiValueConverter
{
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        
        if (values.Count < 2 || values[0] == null) 
            return null;

        var teacher = values[0] as Teacher; // Явное приведение к Teacher
        if (teacher == null) 
            return null;

        var isFalse = values[1]?.ToString() == "False";
        return (teacher, isFalse); // Возвращаем ValueTuple

    }
}