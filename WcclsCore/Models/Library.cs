using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WcclsCore.Models {

	public static class LibraryParser {

		private static Dictionary<string, Library> _dictCodeToLibary { get; set; } = new Dictionary<string, Library>();

		public static Library GetLibaryByCode(string code) {
			if(string.IsNullOrWhiteSpace(code)) {
				return Library.Unknown;
			}
			lock(_dictCodeToLibary) {
				if(_dictCodeToLibary.Count == 0) {
					List<FieldInfo> listFields = typeof(Library).GetFields().ToList();
					foreach(Library library in Enum.GetValues(typeof(Library)).Cast<Library>()) {
						if(library == Library.Unknown) {
							continue;
						}
						FieldInfo field = listFields.FirstOrDefault(x => x.Name == library.ToString());
						LibraryAttribute attr = field?.GetCustomAttribute<LibraryAttribute>();
						if(!string.IsNullOrWhiteSpace(attr?.Code)) {
							_dictCodeToLibary[attr.Code] = library;
						}
					}
				}
				_dictCodeToLibary.TryGetValue(code, out Library retVal);
				return retVal;
			}
		}
	}

	public enum Library {
		Unknown,
		[Library(Name = "Garden Home Community Library", Code = "17")]
		GardenHomeCommunityLibrary,
	}

	public class LibraryAttribute : Attribute {
		///<summary>The name of the library.</summary>
		public string Name { get; set; }

		///<summary>The unique identifier for this library.</summary>
		public string Code { get; set; }
	}
}
