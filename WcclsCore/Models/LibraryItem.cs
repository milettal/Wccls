using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
		///<summary>The parsed format of this item.</summary>
		public ItemFormat Format { get; set; }
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

	///<summary>The format of an item.</summary>
	public enum ItemFormat {
		///<summary>Unable to parse the format out of the known formats.</summary>
		[ItemFormat("Other",FormatCategory.Unknown)]
		Unknown,
		[ItemFormat("Downloadable Audiobook", FormatCategory.Audiobook)]
		Ab,
		[ItemFormat("Atlas", FormatCategory.Book)]
		Atlas,
		[ItemFormat("Oversize Book", FormatCategory.Book)]
		Big_bk,
		[ItemFormat("Book", FormatCategory.Book)]
		Bk,
		[ItemFormat("Blu-ray Disc",FormatCategory.CDDVD)]
		Bluray,
		[ItemFormat("3D Blu-ray Disc", FormatCategory.CDDVD)]
		Bluray_3d,
		[ItemFormat("Board Book", FormatCategory.Book)]
		Board_bk,
		[ItemFormat("Board Game", FormatCategory.Unknown)]
		Board_game,
		[ItemFormat("Audiobook CD", FormatCategory.Audiobook)]
		Book_cd,
		[ItemFormat("Book Club Kit", FormatCategory.Book)]
		Book_club_kit,
		[ItemFormat("Book plus CD", FormatCategory.Book)]
		Book_pcd,
		[ItemFormat("Book plus Cassette", FormatCategory.Book)]
		Book_pcs,
		[ItemFormat("Book plus DVD", FormatCategory.Book)]
		Book_pdvd,
		[ItemFormat("Braille", FormatCategory.Accessible)]
		Br,
		[ItemFormat("CD-ROM or DVD-ROM", FormatCategory.Games)]
		Cdrom,
		[ItemFormat("Comic Book", FormatCategory.Book)]
		Comic_bk,
		[ItemFormat("Audiobook Cassette", FormatCategory.Audiobook)]
		Cs,
		[ItemFormat("Device", FormatCategory.EBook)]
		Device,
		[ItemFormat("DVD", FormatCategory.CDDVD)]
		Dvd,
		[ItemFormat("eBook", FormatCategory.EBook)]
		Ebook,
		[ItemFormat("Equipment", FormatCategory.Unknown)]
		Equipment,
		[ItemFormat("Graphic Novel", FormatCategory.Book)]
		Graphic_novel,
		[ItemFormat("Kit", FormatCategory.Unknown)]
		Kit,
		[ItemFormat("Large Print", FormatCategory.Accessible)]
		Lprint,
		[ItemFormat("Magazine or Journal", FormatCategory.MagazinesJournals)]
		Mag,
		[ItemFormat("Manuscript or Typescript", FormatCategory.Unknown)]
		Manuscript,
		[ItemFormat("Map", FormatCategory.Unknown)]
		Map,
		[ItemFormat("Microform", FormatCategory.Unknown)]
		Mf,
		[ItemFormat("Printed Music", FormatCategory.Music)]
		Mn,
		[ItemFormat("Audiobook on MP3 CD", FormatCategory.Audiobook)]
		Mp3_cd,
		[ItemFormat("Music CD", FormatCategory.Music)]
		Music_cd,
		[ItemFormat("Newspaper", FormatCategory.MagazinesJournals)]
		Newspaper,
		[ItemFormat("Painting or Graphic Art", FormatCategory.Unknown)]
		Painting,
		[ItemFormat("Pass", FormatCategory.Unknown)]
		Pass,
		[ItemFormat("Picture Book", FormatCategory.Book)]
		Picture_book,
		[ItemFormat("Pre-loaded Audiobook",FormatCategory.Audiobook)]
		Playaway_audiobook,
		[ItemFormat("Spoken-word CD", FormatCategory.Audiobook)]
		Spoken_cd,
		[ItemFormat("3-D Object", FormatCategory.Unknown)]
		Three_d_object,
		[ItemFormat("Video Cassette", FormatCategory.Movies)]
		Vc,
		[ItemFormat("Video Game", FormatCategory.Games)]
		Video_game,
		[ItemFormat("Streaming Video", FormatCategory.Movies)]
		Video_online,
		[ItemFormat("Website", FormatCategory.Unknown)]
		Website,
	}

	///<summary>The category of an item.</summary>
	public enum FormatCategory {
		Unknown,
		Accessible,
		Audiobook,
		Book,
		CDDVD,
		EBook,
		Games,
		MagazinesJournals,
		Movies,
		Music,
	}

	public class ItemFormatAttribute : Attribute {

		public List<FormatCategory> ListCategories { get; }

		public string FormatDisplay { get; }

		public ItemFormatAttribute(string formatDisplay,params FormatCategory[] categories) {
			FormatDisplay = formatDisplay;
			ListCategories = categories?.ToList();
		}
	}
}
