using System.Threading.Tasks;

namespace MeteringStation.Mobile.Pages.Services
{
    public interface INavigationService
    {
        Task NavigateBackAsync();
        Task NavigateToConfigureMetersPageAsync();
        Task NavigateToMetersPageAsync();
    }
}