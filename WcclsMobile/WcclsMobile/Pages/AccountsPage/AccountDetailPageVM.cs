using System;
using System.Threading.Tasks;
using Core.Async;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {
	public class AccountDetailPageVM : ViewModelBase {

		public const string USER_KEY = "USER_KEY";

		private IPageDialogService _pageDialogService { get; }

		private IEventAggregator _eventAggregator { get; }

		private IUserAuthenticationService _userService { get; }

		private AsyncLock _nicknameLock { get; } = new AsyncLock();

		///<summary>The user in question.</summary>
		public User User {
			get { return GetBindableProperty(() => User); }
			set { SetBindableProperty(() => User, value); }
		}

		///<summary>An editable nickname for this user. Defaults to the cardnumber/username.</summary>
		public string Nickname {
			get { return GetBindableProperty(() => Nickname); }
			set { SetBindableProperty(() => Nickname, value); }
		}

		public AccountDetailPageVM(INavigationService navService, IPageDialogService pageDialogService, IEventAggregator eventAggregator, IUserAuthenticationService userService)
			: base(navService)
		{
			_pageDialogService = pageDialogService;
			_eventAggregator = eventAggregator;
			_userService = userService;
		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(!parameters.ContainsKey(USER_KEY)) {
				throw new ApplicationException($"Did not pass in a User to {nameof(AccountDetailPageVM)}");
			}
			User = parameters.GetValue<User>(USER_KEY);
			Nickname = User.Nickname;
			return Task.CompletedTask;
		}

		///<summary>Removes the account from the stored accounts.</summary>
		public IAsyncCommand RemoveAccountCommand => GetCommandAsync(() => RemoveAccountCommand, true, async () => {
			if(!await _pageDialogService.DisplayAlertAsync("Confirm", "Are you sure you want to remove this account?", "Continue", "Cancel")) {
				return;
			}
			//This will handle eventing to alert other pages of this removal.
			await _userService.RemoveUser(User);
			await _navigationService.GoBackAsync();
		});

		///<summary>Occurs when the nickname is changed. This can fire multiple times so async code should account for this.</summary>
		public IAsyncCommand NicknameChangedCommand => GetCommandAsync(() => NicknameChangedCommand, false, async () => {
			string nickname = Nickname;
			//If they get rid of the whole nickname, change it back to their username.
			if(string.IsNullOrWhiteSpace(nickname)) {
				nickname = User.Username;
			}
			//Only let one command at a time in. They will queue up in order to make sure the last nickname in wins.
			await _nicknameLock.LockAsync(async () => {
				User.Nickname = Nickname;
				//Handles eventing.
				await _userService.UpdateNickname(nickname, User);
			});
		});

	}
}
