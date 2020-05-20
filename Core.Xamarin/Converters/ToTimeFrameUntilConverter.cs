using System;
using System.Globalization;
using Core.DependencyInjection;
using Xamarin.Forms;

namespace Core.Xamarin.Converters {

	public class ToTimeFrameUntilConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is DateTime time)) {
				throw new ApplicationException($"Bad inputs to {nameof(ToTimeFrameUntilConverter)}");
			}
			IClock clock = new Clock();
			DateTime today = clock.Today;
			if(today >= time) {
				return "0 days";
			}
			else if(time < today.AddMonths(1)) {
				//Within a month. Let's give it in days.
				int totalDays = (int)Math.Floor((time-today).TotalDays);
				return $"{totalDays} day{(totalDays == 1 ? "" : "s")}";
			}
			else {
				//Within a month. We will say in terms of months
				int months = ((time.Year-today.Year)*12)+time.Month-today.Month;
				return $"{months} month{(months == 1 ? "" : "s")}";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

}
