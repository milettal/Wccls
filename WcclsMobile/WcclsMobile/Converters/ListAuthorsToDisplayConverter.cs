using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace WcclsMobile.Converters {
	public class ListAuthorsToDisplayConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is IEnumerable<string> authors)) {
				throw new ApplicationException("Bad Input.");
			}
			string s = "";
			foreach(string author in authors) {
				if(s == "") {
					s += author;
				}
				else {
					s+=", et al";
					break;
				}
			}
			return s;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
