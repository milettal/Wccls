using System;
using Core.Xamarin.MVVM;

namespace WcclsMobile.Models {
	///<summary>Represents a single logged in user account.</summary>
	public class User : BindableObjectBase {

		///<summary>The username for this account.</summary>
		public string Username {
			get { return GetBindableProperty(() => Username); }
			set { SetBindableProperty(() => Username, value); }
		}

		///<summary>A nickname for this account. Can be changed.</summary>
		public string Nickname {
			get { return GetBindableProperty(() => Nickname); }
			set { SetBindableProperty(() => Nickname, value); }
		}

		///<summary>The password for the user. Stored securely in keychain or whatever secure storage the platform supplies.</summary>
		public string Password {
			get { return GetBindableProperty(() => Password); }
			set { SetBindableProperty(() => Password, value); }
		}

		///<summary>The current session id for this user. May be blank if freshly logged in. May be stale.</summary>
		public string SessionId {
			get { return GetBindableProperty(() => SessionId); }
			set { SetBindableProperty(() => SessionId, value); }
		}
	}
}
