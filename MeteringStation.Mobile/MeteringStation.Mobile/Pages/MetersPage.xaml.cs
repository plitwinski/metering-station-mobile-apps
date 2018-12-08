using MeteringStation.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeteringStation.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MetersPage : ContentPage
	{
		public MetersPage (MetersViewModel viewModel)
		{
			InitializeComponent();
            BindingContext = viewModel;
		}
	}
}