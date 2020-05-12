using System;
using System.Globalization;
using WcclsCore.Models;
using Xamarin.Forms;

namespace WcclsMobile.Converters {
	public class FormatToImageConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) {
				return "";
			}
			if(!(value is ItemFormat format)) {
				throw new ApplicationException($"Bad input to {nameof(FormatToImageConverter)}");
			}
			FormatCategory category = LibraryItemParser.FormatToCategory(format);
			switch(category) {
				case FormatCategory.Accessible:
					return "resource://WcclsMobile.Images.text_format-24px.svg";
				case FormatCategory.Audiobook:
					return "resource://WcclsMobile.Images.volume_up-24px.svg";
				case FormatCategory.Book:
					return "resource://WcclsMobile.Images.menu_book-24px.svg";
				case FormatCategory.CDDVD:
					return "resource://WcclsMobile.Images.album-24px.svg";
				case FormatCategory.EBook:
					return "resource://WcclsMobile.Images.devices-24px.svg";
				case FormatCategory.Games:
					return "resource://WcclsMobile.Images.sports_esports-24px.svg";
				case FormatCategory.MagazinesJournals:
					return "resource://WcclsMobile.Images.newspaper.svg";
				case FormatCategory.Movies:
					return "resource://WcclsMobile.Images.movie-24px.svg";
				case FormatCategory.Music:
					return "resource://WcclsMobile.Images.music_note-24px.svg";
				default:
					return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
