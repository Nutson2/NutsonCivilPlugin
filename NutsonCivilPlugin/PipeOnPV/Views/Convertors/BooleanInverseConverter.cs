using System.Globalization;
using System.Windows.Data;

namespace NutsonCivilPlugin.PipeOnPV.Views.Convertors;

public class BooleanInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is bool flag ? !flag : value;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => value is bool flag ? !flag : value;
}
