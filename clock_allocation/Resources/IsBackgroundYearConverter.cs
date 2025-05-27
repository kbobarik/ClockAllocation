using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace clock_allocation.Resources;

public class IsBackgroundYearConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && values[0] is int year && values[1] is int selectedYear)
        {
            return year == selectedYear ? Brush.Parse("#B44A45") : Brushes.White;
        }
        return Brushes.White;
    }

    public IEnumerable<object> ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
