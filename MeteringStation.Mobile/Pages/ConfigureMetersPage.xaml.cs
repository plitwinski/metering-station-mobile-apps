using MeteringStation.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeteringStation.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigureMetersPage : ContentPage
    {
        public ConfigureMetersPage(ConfigureMetersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}