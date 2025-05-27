using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace clock_allocation.Resources;

public class IsForegroundYearConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && values[0] is int year && values[1] is int selectedYear)
        {
            return year == selectedYear ? Brushes.White : Brush.Parse("#B44A45");
        }

        return Brush.Parse("#B44A45");
    }

    public IEnumerable<object> ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}