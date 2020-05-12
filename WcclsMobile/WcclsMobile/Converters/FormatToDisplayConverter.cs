using System;
using System.Globalization;
using WcclsCore.Models;
using Xamarin.Forms;

namespace WcclsMobile.Converters {
	public class FormatToDisplayConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is ItemFormat format)) {
				throw new ApplicationException($"Bad input to {nameof(FormatToDisplayConverter)}");
			}
			return LibraryItemParser.FormatToDisplay(format);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
