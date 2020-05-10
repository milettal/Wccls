using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class ToShortDateConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is DateTime date)) {
				throw new ApplicationException(nameof(value));
			}
			return date.ToShortDateString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
