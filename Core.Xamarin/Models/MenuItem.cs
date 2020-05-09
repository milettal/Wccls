using System;
using System.Threading.Tasks;

namespace Core.Xamarin.Models {
	///<summary>A class that represents a generic menu item. These are generally not related but have unique selected events.</summary>
	public class MenuItem {

		///<summary>The title of the menu item.</summary>
		public string Title { get; set; }

		///<summary>The icon for this menu item. i.e. icon.png.</summary>
		public string Icon { get; set; }

		///<summary>The action that occurs when this item is selected.</summary>
		public Func<Task> OnSelected { get; set; }
	}
}
