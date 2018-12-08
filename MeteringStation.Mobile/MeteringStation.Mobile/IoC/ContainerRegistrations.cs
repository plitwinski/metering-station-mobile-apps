using MeteringStation.Mobile.Pages;
using MeteringStation.Mobile.Services;
using Xamarin.Forms;

namespace MeteringStation.Mobile.IoC
{
    internal static class ContainerRegistrations
    {
        public static void Register()
        {
            DependencyService.Register<MeteringStationDetector>();
        }
    }
}
