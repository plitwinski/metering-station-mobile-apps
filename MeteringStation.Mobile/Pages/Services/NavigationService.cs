using Autofac;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.Pages.Services
{
    public class NavigationService : INavigationService
    {
        private readonly ILifetimeScope _lifetimeScope;

        public NavigationService(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public async Task NavigateToConfigureMetersPageAsync()
        {
            var page = _lifetimeScope.Resolve<ConfigureMetersPage>();
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public Task NavigateToMetersPageAsync()
            => App.Current.MainPage.Navigation.PushAsync(_lifetimeScope.Resolve<MetersPage>());

        public Task NavigateBackAsync()
            => App.Current.MainPage.Navigation.PopAsync();
    }
}
