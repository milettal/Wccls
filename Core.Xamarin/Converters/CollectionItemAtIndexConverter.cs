using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {
	public class CollectionItemAtIndexConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return null;
			}
			if(parameter == null) {
				throw new ArgumentException(nameof(parameter));
			}
			int index;
			if(parameter is int i) {
				index = i;
			}
			else if(parameter is string s) {
				index = int.Parse(s);
			}
			else {
				throw new ArgumentException(nameof(parameter));
			}
			if(!(value is ICollection collection)) {
				throw new ArgumentException(nameof(value));
			}
			ICollection<object> col = collection.Cast<object>().ToList();
			return col.ElementAt(index);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
