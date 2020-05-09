using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class InverseBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			value=value??throw new ArgumentNullException(nameof(value));
			if(value is bool boolValue) {
				return !boolValue;
			}
			throw new ArgumentException($"Invalid type: {value.GetType().FullName} for parameter: {nameof(value)} in {nameof(InverseBoolConverter)}.");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

}
