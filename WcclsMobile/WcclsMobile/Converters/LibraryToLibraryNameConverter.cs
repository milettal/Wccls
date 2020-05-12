using System;
using System.Globalization;
using WcclsCore.Models;
using Xamarin.Forms;

namespace WcclsMobile.Converters {
	public class LibraryToLibraryNameConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is Library library)) {
				throw new ApplicationException($"Bad input to {nameof(LibraryToLibraryNameConverter)}");
			}
			return LibraryItemParser.GetLibraryName(library);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
