using Autofac;
using MeteringStation.Mobile.DataAccess.Databases;

namespace MeteringStation.Mobile.DataAccess
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<IMetersDatabase>(_ =>
                MetersDatabase.Create())
             .SingleInstance();
        }
    }
}
