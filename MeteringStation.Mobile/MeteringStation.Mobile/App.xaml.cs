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
        private readonly MeteringStationDetector meteringStationDetector;

        public App()
        {
            InitializeComponent();
            var container = ContainerRegistrations.Create();
            meteringStationDetector = container.Resolve<MeteringStationDetector>();
            MainPage = container.Resolve<MetersPage>();
        }

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
