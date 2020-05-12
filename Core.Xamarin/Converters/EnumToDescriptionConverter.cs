using System;
using System.Globalization;
using Core.Extensions;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class EnumToDescriptionConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is Enum en)) {
				throw new ApplicationException($"Bad input to {nameof(EnumToDescriptionConverter)}");
			}
			return en.GetDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
