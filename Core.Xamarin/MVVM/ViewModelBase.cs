using System.Threading.Tasks;
using Prism.Navigation;

namespace Core.Xamarin.MVVM {
	public class ViewModelBase : BindableObjectBase, INavigationAware, IInitializeAsync, IDestructible {

		protected INavigationService _navigationService { get; }

		public ViewModelBase(INavigationService navigationService) {
			_navigationService = navigationService;
		}

		#region LifeCycle Events

		///<summary>Called when the VM is loaded for the first time.</summary>
		public virtual Task InitializeVMAsync(INavigationParameters parameters) {
			return Task.CompletedTask;
		}

		///<summary>Called when the VM is being navigated away from. Popped.</summary>
		public virtual Task NavigatedFrom(INavigationParameters parameters) {
			return Task.CompletedTask;
		}

		///<summary>Called when the VM is being navigated to. Pushed.</summary>
		public virtual Task NavigationTo(INavigationParameters parameters) {
			return Task.CompletedTask;
		}

		///<summary>Called when the VM is being destroyed. Clean up any resources here.</summary>
		public virtual void OnDestroy() {

		}

		public Task InitializeAsync(INavigationParameters parameters) {
			return InitializeVMAsync(parameters);
		}

		public void OnNavigatedFrom(INavigationParameters parameters) {
			TaskUtils.FireAndForget(NavigatedFrom(parameters));
		}

		public void OnNavigatedTo(INavigationParameters parameters) {
			TaskUtils.FireAndForget(NavigationTo(parameters));
		}

		public void Destroy() {
			OnDestroy();
		}

		#endregion

	}
}
