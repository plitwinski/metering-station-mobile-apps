using Autofac;
using MeteringStation.Mobile.Pages.Services;

namespace MeteringStation.Mobile.Pages
{
    public class PagesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NavigationService>()
                .As<INavigationService>()
                .SingleInstance();

            builder.RegisterType<MetersPage>().AsSelf();
            builder.RegisterType<ConfigureMetersPage>().AsSelf();
        }
    }
}
