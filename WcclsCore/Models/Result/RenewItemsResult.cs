using System;
using System.Collections.Generic;

namespace WcclsCore.Models.Result {

	public class RenewItemsResult {
		///<summary>A dictionary of results for all items that were attempted to be renewed.</summary>
		public Dictionary<string,RenewResult> DictionaryResponses { get; set; }
	}

	public class RenewResult {
		///<summary>Indicates if the given item was renewed successfully.</summary>
		public bool WasRenewed { get; set; }
		///<summary>If the item was not renewed, returns an error message.</summary>
		public string ErrorMessage { get; set; }
	}

}
