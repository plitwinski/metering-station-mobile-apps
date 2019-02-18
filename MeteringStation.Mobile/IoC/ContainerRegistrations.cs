using Autofac;
using MeteringStation.Mobile.DataAccess;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Pages;
using MeteringStation.Mobile.Services;
using MeteringStation.Mobile.Services.Communication;
using MeteringStation.Mobile.ViewModels;
using System;
using System.Net.Http;

namespace MeteringStation.Mobile.IoC
{
    internal static class ContainerRegistrations
    {
        public static ContainerBuilder Create()
        {
            var builder = new ContainerBuilder();

            builder.Register(_ => new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(10)
            }).AsSelf().SingleInstance();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<MeteringStationDetector>().As<IMeteringStationDetector>().SingleInstance();
            builder.RegisterType<UdpBroadcaster>().As<IBroadcaster>();

            builder.RegisterModule<ViewModelsModule>();
            builder.RegisterModule<PagesModule>();
            builder.RegisterModule<DataAccessModule>();
            return builder;
        }
    }
}
