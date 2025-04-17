using System.Globalization;
using System.Windows.Data;

namespace FrontPage.Converter;

public class Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString().Contains(parameter?.ToString() ?? "") ?? false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}