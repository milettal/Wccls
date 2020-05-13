using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core;
using Core.Extensions;

namespace WcclsCore.Models {

	public static class LibraryItemParser {

		private static Dictionary<string, Library> _dictCodeToLibary { get; set; } = new Dictionary<string, Library>();

		private static Dictionary<Library, string> _dictLibraryToName { get; set; } = new Dictionary<Library, string>();

		private static Dictionary<ItemFormat, string> _dictFormatToDisplay { get; set; } = new Dictionary<ItemFormat, string>();

		private static Dictionary<ItemFormat, FormatCategory> _dictFormatToCategory { get; set; } = new Dictionary<ItemFormat, FormatCategory>();

		private static Dictionary<Library, string> _dictLibraryToCode { get; set; } = new Dictionary<Library, string>();

		public static Library GetLibaryByCode(string code) {
			if(string.IsNullOrWhiteSpace(code)) {
				return Library.Unknown;
			}
			lock(_dictCodeToLibary) {
				if(_dictCodeToLibary.Count == 0) {
					List<FieldInfo> listFields = typeof(Library).GetFields().ToList();
					foreach(Library library in EnumUtils.GetAll<Library>()) {
						if(library == Library.Unknown) {
							continue;
						}
						FieldInfo field = listFields.FirstOrDefault(x => x.Name==library.ToString());
						LibraryAttribute attr = field?.GetCustomAttribute<LibraryAttribute>();
						if(!string.IsNullOrWhiteSpace(attr?.Code)) {
							_dictCodeToLibary[attr.Code]=library;
						}
					}
				}
				_dictCodeToLibary.TryGetValue(code, out Library retVal);
				return retVal;
			}
		}

		public static string GetLibraryName(Library library) {
			lock(_dictLibraryToName) {
				if(_dictLibraryToName.Count == 0) {
					List<FieldInfo> listFields = typeof(Library).GetFields().ToList();
					foreach(Library lib in EnumUtils.GetAll<Library>()) {
						if(lib == Library.Unknown) {
							continue;
						}
						FieldInfo field = listFields.FirstOrDefault(x => x.Name==lib.ToString());
						LibraryAttribute attr = field?.GetCustomAttribute<LibraryAttribute>();
						_dictLibraryToName[lib]=attr?.Name ?? lib.ToString();
					}
				}
				_dictLibraryToName.TryGetValue(library, out string name);
				return name ?? "";
			}
		}

		public static string FormatToDisplay(ItemFormat format) {
			lock(_dictFormatToDisplay) {
				if(_dictFormatToDisplay.Count==0) {
					List<FieldInfo> listFields = typeof(ItemFormat).GetFields().ToList();
					foreach(ItemFormat f in EnumUtils.GetAll<ItemFormat>()) {
						FieldInfo field = listFields.FirstOrDefault(x => x.Name==f.ToString());
						ItemFormatAttribute attr = field?.GetCustomAttribute<ItemFormatAttribute>();
						_dictFormatToDisplay[f] = string.IsNullOrWhiteSpace(attr?.FormatDisplay) ? "Other" : attr.FormatDisplay;
					}
				}
				_dictFormatToDisplay.TryGetValue(format, out string display);
				return display ?? "";
			}
		}

		public static FormatCategory FormatToCategory(ItemFormat format) {
			lock(_dictFormatToCategory) {
				if(_dictFormatToCategory.Count==0) {
					List<FieldInfo> listFields = typeof(ItemFormat).GetFields().ToList();
					foreach(ItemFormat f in EnumUtils.GetAll<ItemFormat>()) {
						FieldInfo field = listFields.FirstOrDefault(x => x.Name == f.ToString());
						ItemFormatAttribute attr = field?.GetCustomAttribute<ItemFormatAttribute>();
						_dictFormatToCategory[f] = attr?.ListCategories?.FirstOrDefault() ?? FormatCategory.Unknown;
					}
				}
				_dictFormatToCategory.TryGetValue(format, out FormatCategory cate);
				return cate;
			}
		}

		///<summary>Returns the code for the given library.</summary>
		public static string GetCodeByLibrary(Library library) {
			lock(_dictLibraryToCode) {
				if(_dictLibraryToCode.Count==0) {
					List<FieldInfo> listFields = typeof(Library).GetFields().ToList();
					foreach(Library lib in EnumUtils.GetAll<Library>()) {
						FieldInfo field = listFields.FirstOrDefault(x => x.Name == lib.ToString());
						LibraryAttribute attr = field?.GetCustomAttribute<LibraryAttribute>();
						_dictLibraryToCode[lib] = attr?.Code ?? "";
					}
				}
				_dictLibraryToCode.TryGetValue(library, out string code);
				return code;
			}
		}
	}

	public enum Library {
		[Library(Name = "Unknown", Code = "-1")]
		Unknown,
		[Library(Name = "Aloha Community Library", Code = "32")]
		AlohaCommunityLibrary,
		[Library(Name = "Banks Public Library", Code = "7")]
		BanksPublicLibrary,
		[Library(Name = "Beaverton City Library", Code = "9")]
		BeavertonCityLibrary,
		[Library(Name = "Beaverton Murray Scholls", Code = "39")]
		BeavertonMurrayScholls,
		[Library(Name = "Bethany Library", Code = "34")]
		BethanyLibrary,
		[Library(Name = "Cedar Mill Library", Code = "11")]
		CedarMillLibrary,
		[Library(Name = "Cooperative Administrative Office", Code = "3")]
		CooperativeAdministrativeOffice,
		[Library(Name = "Cornelius Public Library", Code = "13")]
		CorneliusPublicLibrary,
		[Library(Name = "External Loans (Staff Only)", Code = "4")]
		ExternalLoans,
		[Library(Name = "Forest Grove City Library", Code = "15")]
		ForestGroveCityLibrary,
		[Library(Name = "Garden Home Community Library", Code = "17")]
		GardenHomeCommunityLibrary,
		[Library(Name = "Hillsboro BookOMat Civic Plaza", Code = "40")]
		HillsboroBookOMatCivicPlaza,
		[Library(Name = "Hillsboro Brookwood Library", Code = "20")]
		HillsboroBrookwoordLibrary,
		[Library(Name = "Hillsboro Shute Park Library", Code = "19")]
		HillsboroShuteParkLibrary,
		[Library(Name = "Library Outreach Services", Code = "5")]
		LibraryOutreachServices,
		[Library(Name = "North Plains Public Library", Code = "36")]
		NorthPlainsPublicLibrary,
		[Library(Name = "Sherwood Public Library", Code = "25")]
		SherwoodPublicLibrary,
		[Library(Name = "Tigard Public Library", Code = "29")]
		TigardPublicLibrary,
		[Library(Name = "Tualatin Public Library", Code = "31")]
		TualatinPublicLibrary,
		[Library(Name = "WCCLS Courier", Code = "37")]
		WCCLSCourier,
		[Library(Name = "West Slope Community Library", Code = "33")]
		WestSlopeCommunityLibrary,
	}

	public class LibraryAttribute : Attribute {
		///<summary>The name of the library.</summary>
		public string Name { get; set; }

		///<summary>The unique identifier for this library.</summary>
		public string Code { get; set; }
	}
}
