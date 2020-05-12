using System;
using System.Globalization;
using WcclsCore.Models;
using Xamarin.Forms;

namespace WcclsMobile.Converters {
	public class HoldStatusToImageConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is HoldStatus status)) {
				throw new ApplicationException($"Bad input to {nameof(HoldStatusToImageConverter)}");
			}
			switch(status) {
				case HoldStatus.In_Transit:
					return "resource://WcclsMobile.Images.airport_shuttle-black-18dp.svg";
				case HoldStatus.Not_Yet_Available:
					return "resource://WcclsMobile.Images.hourglass_empty-black-18dp.svg";
				case HoldStatus.Ready_For_Pickup:
					return "resource://WcclsMobile.Images.done-black-18dp.svg";
				case HoldStatus.Suspended:
					return "resource://WcclsMobile.Images.pause-24px.svg";
				default:
					return "resource://WcclsMobile.Images.help-24px.svg";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
