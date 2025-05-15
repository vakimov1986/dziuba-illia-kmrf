using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CurConvApp.Converters
{
    public class LowercaseToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string currency)
            {
                var code = currency.ToLower();
                var uri = new Uri($"/CurConvApp;component/Assets/Flags/{code}.png", UriKind.Relative);
                return new BitmapImage(uri);
            }

            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
