using System;
using System.Collections.Generic;
using Core.Xamarin.Models;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using Prism.Services;

namespace WcclsMobile.Pages {

	public class MenuPageVM : ViewModelBase {

		private IPageDialogService _pageDialog { get; }

		///<summary>A list of all menu items.</summary>
		public List<MenuItem> ListMenuItems {
			get { return GetBindableProperty(() => ListMenuItems); }
			set { SetBindableProperty(() => ListMenuItems, value); }
		}

		///<summary>The currently selected menu item. Clears selection to not keep the menu items selected.</summary>
		public MenuItem SelectedMenuItem {
			get { return null; }
			set {
				Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(50), () => {
					RaisePropertyChanged(() => SelectedMenuItem);
					return false;
				});
			}
		}

		public MenuPageVM(INavigationService navigationService, IPageDialogService pageDialog) : base(navigationService) {
			_pageDialog = pageDialog;
			ListMenuItems = new List<MenuItem>();
			ListMenuItems.Add(new MenuItem {
				Title = "Accounts",
				Icon = "resource://WcclsMobile.Images.account_circle-black-24px.svg",
				OnSelected = async () => {
					await _navigationService.NavigateAsync(nameof(AccountsPage));
				},
			});
			ListMenuItems.Add(new MenuItem {
				Title = "Settings",
				Icon = "resource://WcclsMobile.Images.settings-24px.svg",
				OnSelected = async () => {
					await _pageDialog.DisplayAlertAsync("Alert", "Fail", "Ok");
				},
			});
			ListMenuItems.Add(new MenuItem {
				Title = "Libraries",
				Icon = "resource://WcclsMobile.Images.local_library-24px.svg",
				OnSelected = async () => {
					await _pageDialog.DisplayAlertAsync("Alert", "Fail", "Ok");
				},
			});
			ListMenuItems.Add(new MenuItem {
				Title = "Feedback",
				Icon = "resource://WcclsMobile.Images.feedback-24px.svg",
				OnSelected = async () => {
					await _pageDialog.DisplayAlertAsync("Alert", "Fail", "Ok");
				},
			});
			ListMenuItems.Add(new MenuItem {
				Title = "About",
				Icon = "resource://WcclsMobile.Images.help-24px.svg",
				OnSelected = async () => {
					await _pageDialog.DisplayAlertAsync("Alert", "Fail", "Ok");
				},
			});
		}

		///<summary>Occurs when a menu item is selected.</summary>
		public IAsyncCommand<MenuItem> MenuItemSelectedCommand => GetCommandAsync(() => MenuItemSelectedCommand, true, async (menuItem) => {
			if(menuItem?.OnSelected != null) {
				await menuItem.OnSelected();
			}
		});

	}

}
