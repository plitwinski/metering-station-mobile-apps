using MeteringStation.Mobile.Services;
using MeteringStation.Mobile.Services.Models;
using MeteringStation.Mobile.ViewModels.Dtos;
using MeteringStation.Mobile.ViewModels.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MeteringStation.Mobile.ViewModels
{
    public class MetersViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient httpClient;
        private IEnumerable<MeteringStationDevice> _registeredDevices;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region public properties

        private ObservableCollection<Reading> readings = new ObservableCollection<Reading>();
        public ObservableCollection<Reading> Readings
        {
            get { return readings; }
            protected set
            {
                if (readings != value)
                {
                    readings = value;
                    OnPropertyChanged(nameof(Readings));
                }
            }
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get  { return isLoading; }
            protected set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public ICommand Refresh { get; protected set; }

        #endregion

        public MetersViewModel(MeteringStationDetector meteringStationDetector)
        {
            IsLoading = true;
            httpClient = new HttpClient();
            meteringStationDetector.SubscribeToFoundDevices(async devices =>
            {
                _registeredDevices = devices;
                await HandleDevices(devices);
            });
            Refresh = new Command(async () =>  await HandleDevices(_registeredDevices));
        }

        private async Task HandleDevices(IEnumerable<MeteringStationDevice> devices)
        {
            IsLoading = true;

            var results = devices.Select(d => DownloadDeviceDataAsync(d)).ToArray();
            await Task.WhenAll(results);
            Readings.Clear();

            results
            .SelectMany(p => p.Result).ToList()
            .ForEach(i => Readings.Add(i));

            IsLoading = false;
        }

        private async Task<IEnumerable<Reading>> DownloadDeviceDataAsync(MeteringStationDevice device)
        {
            var response = await httpClient.GetAsync($"http://{device.Ip}:8080/v1/readings");
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<Reading>();

            var content = await response.Content.ReadAsStringAsync();
            var readings = JsonConvert.DeserializeObject<ReadingDto[]>(content);
            return readings.Select(p => new Reading()
            {
                PM25 = p.PM25,
                PM10 = p.PM10,
                DeviceName = p.DeviceName,
                ClientId = device.Id
            });
        }
    }
}
