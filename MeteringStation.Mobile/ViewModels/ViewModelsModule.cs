using Autofac;

namespace MeteringStation.Mobile.ViewModels
{
    public class ViewModelsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MetersViewModel>().AsSelf();
            builder.RegisterType<ConfigureMetersViewModel>().AsSelf();
        }
    }
}
