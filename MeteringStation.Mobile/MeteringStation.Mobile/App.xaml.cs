using MeteringStation.Mobile.IoC;
using MeteringStation.Mobile.Pages;
using MeteringStation.Mobile.Services;
using MeteringStation.Mobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MeteringStation.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ContainerRegistrations.Register();
            var viewModel = new MetersViewModel(DependencyService.Resolve<MeteringStationDetector>());
            MainPage = new MetersPage(viewModel);
        }

        protected override void OnStart()
        {
            StartMeteringStationsDetection();
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            StartMeteringStationsDetection();
            // Handle when your app resumes
        }

        private void StartMeteringStationsDetection()
        {
            var detector = DependencyService.Resolve<MeteringStationDetector>();
            detector.StartDiscovery();
        }
    }
}
