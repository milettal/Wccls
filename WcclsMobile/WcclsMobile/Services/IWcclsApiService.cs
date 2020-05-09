using System;
using System.Threading.Tasks;

namespace WcclsMobile.Services {
	///<summary>This interface defines all web methods available for the WCCLS backend.</summary>
	public interface IWcclsApiService {

		///<summary>Attempts to log the user in with the given username and password. If there is an error,
		///the error string will be populated. Otherwise, the sessionGuid will be used to make web calls in the future.</summary>
		Task<(string error, string sessionGuid)> Login(string username, string password);
	}
}
