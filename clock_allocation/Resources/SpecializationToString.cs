using System;
using System.Globalization;
using Avalonia.Data.Converters;
using clock_allocation.Models;

namespace clock_allocation.Resources;

public class SpecializationToString : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not Specialization spec ? "" : $"{spec.Qualification}, {spec.FormOfEducation}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}