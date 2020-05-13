using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {
	public class IsDateTimeNotMinValConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return false;
			}
			if(!(value is DateTime dateTime)) {
				throw new ApplicationException($"Bad input to {nameof(IsDateTimeNotMinValConverter)}");
			}
			return dateTime != DateTime.MinValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
