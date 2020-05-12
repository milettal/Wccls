using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class PrependStringIfNotEmptyConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is string strVal)) {
				throw new ApplicationException($"Bad inputs to {nameof(PrependStringIfNotEmptyConverter)}");
			}
			if(parameter == null) {
				return strVal;
			}
			if(!(parameter is string strParam)) {
				throw new ApplicationException($"Bad inputs to {nameof(PrependStringIfNotEmptyConverter)}");
			}
			return strParam + strVal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
