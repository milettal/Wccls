using System;
using System.Collections.Generic;

namespace WcclsCore.Models.Request {

	public class RenewItemsRequest {

		///<summary>A list of checkout IDs to renew.</summary>
		public List<string> ListCheckoutIds { get; set; }

	}
}
