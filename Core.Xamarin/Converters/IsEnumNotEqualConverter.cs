using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class IsEnumNotEqualConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null || parameter == null) {
				return false;
			}
			if(!(value is Enum valEnum)) {
				throw new ApplicationException($"Bad input to {nameof(IsEnumNotEqualConverter)}");
			}
			if(!(parameter is Enum paramEnum)) {
				throw new ApplicationException($"Bad input to {nameof(IsEnumNotEqualConverter)}");
			}
			if(valEnum.GetType() != paramEnum.GetType()) {
				throw new ApplicationException($"Bad input to {nameof(IsEnumNotEqualConverter)}");
			}
			return valEnum.ToString() != paramEnum.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
