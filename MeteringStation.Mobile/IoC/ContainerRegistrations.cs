using Autofac;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Pages;
using MeteringStation.Mobile.Services;
using MeteringStation.Mobile.ViewModels;
using System;
using System.Net.Http;
using Xamarin.Forms;

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
            builder.RegisterType<MetersPage>().AsSelf();
            builder.RegisterType<MetersViewModel>().AsSelf();
            

            return builder;
        }
    }
}
