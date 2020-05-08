using System;
using System.Collections.Generic;
using WcclsCore;

namespace WcclsCore.Models.Result {

	///<summary>The result object when querying for an account's fines.</summary>
	public class FinesResult {

		public double TotalAccrued { get; set; }

		public double TotalFines { get; set; }

		public double TotalCredits { get; set; }

		public List<FineItem> ListFines { get; set; }

	}
}
