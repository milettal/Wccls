using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {
	public class IsNumberNotEqualConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null || parameter == null) {
				return false;
			}
			double Parse(object o) {
				double doub;
				if(o is int i) {
					doub = i;
				}
				else if(o is double d) {
					doub = d;
				}
				else if(o is float f) {
					doub = f;
				}
				else if(o is long l) {
					doub = l;
				}
				else if(o is string s) {
					doub = double.Parse(s);
				}
				else {
					throw new ArgumentException("Bad argument - " + o.ToString());
				}
				return doub;
			}
			return Parse(value) != Parse(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
