using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.ViewModels.Dtos;
using MeteringStation.Mobile.ViewModels.Models;
using Newtonsoft.Json;
using System;
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
        private List<DeviceDetectedEvent> registeredDevices;

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

        public MetersViewModel(IEventAggregator eventAggregator, HttpClient httpClient)
        {
            registeredDevices = new List<DeviceDetectedEvent>();
            this.httpClient = httpClient;

            eventAggregator.Subscribe<DetectionStartedEvent>(_ => IsLoading = true);
            eventAggregator.Subscribe<DetectionFinishedEvent>(_ => IsLoading = false);
            eventAggregator.Subscribe<DeviceDetectedEvent>(async e => await HandleDevice(e));

            Refresh = new Command(async () => await RefreshDevices(registeredDevices));
        }

        private async Task HandleDevice(DeviceDetectedEvent deviceEvent)
        {
            var result = await DownloadDeviceDataAsync(deviceEvent);
            
            if(Readings.All(x => x.ClientId != deviceEvent.Id))
            {
                result.ToList().ForEach(i => Readings.Add(i));
                registeredDevices.Add(deviceEvent);
            }
        }

        private async Task RefreshDevices(IEnumerable<DeviceDetectedEvent> deviceEvents)
        {
            IsLoading = true;

            var results = deviceEvents.Select(d => DownloadDeviceDataAsync(d)).ToArray();
            await Task.WhenAll(results);
            Readings.Clear();

            results
            .SelectMany(p => p.Result).ToList()
            .ForEach(i => Readings.Add(i));

            IsLoading = false;
        }

        private async Task<IEnumerable<Reading>> DownloadDeviceDataAsync(DeviceDetectedEvent deviceDetected)
        {
            try
            {
                using (var response = await httpClient.GetAsync($"http://{deviceDetected.Ip}:8080/v1/readings"))
                {
                    if (!response.IsSuccessStatusCode)
                        return Enumerable.Empty<Reading>();

                    var content = await response.Content.ReadAsStringAsync();
                    var readings = JsonConvert.DeserializeObject<ReadingDto[]>(content);
                    return readings.Select(p => new Reading()
                    {
                        PM25 = p.PM25,
                        PM10 = p.PM10,
                        DeviceName = p.DeviceName,
                        ClientId = deviceDetected.Id
                    });
                }
            }
            catch(Exception)
            {
                //TODO display error on the UI???
                return Enumerable.Empty<Reading>();
            }
        }
    }
}
