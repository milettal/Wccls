using System;
using System.Collections.Generic;

namespace WcclsCore.Models {

	///<summary>Represents an item that can be checked out at the library. Contains descriptions
	///about the format, URLs for images, and location information.</summary>
	public class LibraryItem {
		///<summary>The unique identifier for this item.</summary>
		public string Id { get; set; }
		///<summary>The library that owns this item.</summary>
		public Library SourceLibrary { get; set; }
		///<summary>The title of this item.</summary>
		public string Title { get; set; }
		///<summary>The subtitle of this item.</summary>
		public string Subtitle { get; set; }
		///<summary>The format of this item, i.e. LP, DVD, CD.</summary>
		public string Format { get; set; }
		///<summary>What is on the binding of the item JF, CD, LP, DVD, etc.</summary>
		public string CallNumber { get; set; }
		///<summary>A description of the item.</summary>
		public string Description { get; set; }
		///<summary>A list of authors.</summary>
		public List<string> ListAuthors { get; set; }
		///<summary>The date (maybe only year) that this item was published.</summary>
		public string PublicationDate { get; set; }
		///<summary>The resources and images for this item.</summary>
		public ItemResources ImageResources { get; set; }
		///<summary>A list of ISBNs for this item.</summary>
		public List<string> ListISBNs { get; set; }
		///<summary>The edition of this item, i.e. first, second.</summary>
		public string Edition { get; set; }
		///<summary>The type of content this is. Similar to genre i.e. fiction.</summary>
		public string ContentType { get; set; }
		///<summary>The average rating out of 100 for this item.</summary>
		public int AverageRating { get; set; }
		///<summary>The total number of people who have rated this item.</summary>
		public int TotalCount { get; set; }
	}

	public class ItemResources {
		///<summary>A link to the small image for this item.</summary>
		public string SmallUrl { get; set; }
		///<summary>A link to the medium image for this item.</summary>
		public string MediumUrl { get; set; }
		///<summary>A link to the large image for this item.</summary>
		public string LargeUrl { get; set; }
	}
}
