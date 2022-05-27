using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace BluetoothSample.Converters
{
    public class NativeDeviceToAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var propInfo = value?.GetType()?.GetProperty("Address");
            string address = (string)propInfo?.GetValue(value, null);
            return (string)address;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
