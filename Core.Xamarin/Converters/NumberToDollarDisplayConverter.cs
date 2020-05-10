using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class NumberToDollarDisplayConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
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
					throw new ArgumentException($"Invalid type: {o.GetType().Name} passed into {nameof(IsCollectionCountNotEqual)}.");
				}
				return doub;
			}
			return "$" + ParseNumber(value).ToString("N");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
