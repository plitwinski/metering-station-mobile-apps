using System.Threading.Tasks;

namespace MeteringStation.Mobile.Services
{
    public interface IMeteringStationDetector
    {
        Task StartDiscoveryAsync();
    }
}