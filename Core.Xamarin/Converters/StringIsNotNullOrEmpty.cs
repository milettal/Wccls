using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {
	public class StringIsNotNullOrEmpty : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return false;
			}
			if(!(value is string str)) {
				throw new ApplicationException($"Bad input to {nameof(StringIsNotNullOrEmpty)}");
			}
			return !string.IsNullOrEmpty(str);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
