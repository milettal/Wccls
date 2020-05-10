using System;
using Newtonsoft.Json;

namespace WcclsCore.Models {

	///<summary>Represents a single line item for a fine.</summary>
	public class FineItem {

		///<summary>The unique identifier for this Fine item.</summary>
		public string Id { get; set; }
		///<summary>The amount the fine was for for this Fine item.</summary>
		public double Amount { get; set; }
		///<summary>A message related to the fine of this fine item.</summary>
		public string Message { get; set; }
		///<summary>The time the custom message was added.</summary>
		public DateTime? MessageDate { get; set; }
		///<summary>The date that the item related to this fine was orignally due.</summary>
		public DateTime? DueDate { get; set; }
		///<summary>The date that item related to this fine was renewed.</summary>
		public DateTime? RenewedDate { get; set; }
		///<summary>The date that this item was returned.</summary>
		public DateTime? ReturnedDate { get; set; }
		///<summary>The date that this item was checked out.</summary>
		public DateTime? CheckedOutDate { get; set; }
		///<summary>A unique identifier for the billing of this Fine.</summary>
		public string BillId { get; set; }
		///<summary>A description of why this fine occurred.</summary>
		public string Description { get; set; }
		///<summary>The status of this fine.</summary>
		public string Status { get; set; }
		///<summary>Indicates if this fine can be payed.</summary>
		public bool? Payable { get; set; }
		///<summary>The ID for the item that this fine was about.</summary>
		public string MetadataId { get; set; }
		///<summary>The title for the item that this fine was about.</summary>
		public string Bibtitle { get; set; }
		///<summary>The subtitle for the item that this fine was about. May be blank.</summary>
		public string Bibsubtitle { get; set; }

	}

}
