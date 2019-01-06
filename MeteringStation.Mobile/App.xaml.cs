using Autofac;
using MeteringStation.Mobile.IoC;
using MeteringStation.Mobile.Pages;
using MeteringStation.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MeteringStation.Mobile
{
    public partial class App : Application
    {
        private readonly IMeteringStationDetector meteringStationDetector;

        public App()
        {
            InitializeComponent();

            var container = GetContainerBuilder().Build();
            meteringStationDetector = container.Resolve<IMeteringStationDetector>();
            MainPage = container.Resolve<MetersPage>();
        }

        protected virtual ContainerBuilder GetContainerBuilder()
            => ContainerRegistrations.Create();

        protected override async void OnStart()
        {
            await StartMeteringStationsDetection();
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override async void OnResume()
        {
            await StartMeteringStationsDetection();
            // Handle when your app resumes
        }

        private async Task StartMeteringStationsDetection()
        {
            await meteringStationDetector.StartDiscoveryAsync();
        }
    }
}
