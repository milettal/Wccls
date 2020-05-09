using System;
using System.Collections;
using System.Globalization;
using Core.Extensions;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {
	public class IsCollectionNullOrEmpty : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return true;
			}
			if(!(value is ICollection collection)) {
				throw new ApplicationException($"Passed a {value.GetType().Name} into {nameof(IsCollectionNullOrEmpty)}.");
			}
			return collection.Count == 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
