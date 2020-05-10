using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class IsCollectionCountEqual : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return false;
			}
			_=parameter??throw new ArgumentNullException(nameof(parameter));
			if(!(value is ICollection collection)) {
				throw new ArgumentException(nameof(value));
			}
			double ParseNumber(object o) {
				double doub = 0;
				if(o is double d) {
					doub=d;
				}
				else if(o is float f) {
					doub=f;
				}
				else if(o is long l) {
					doub=l;
				}
				else if(o is int i) {
					doub=i;
				}
				else if(o is byte b) {
					doub=b;
				}
				else if(o is string str) {
					doub=double.Parse(str);
				}
				else {
					throw new ArgumentException($"Invalid type: {o.GetType().Name} passed into {nameof(IsCollectionCountEqual)}.");
				}
				return doub;
			}
			return collection.Count == ParseNumber(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
