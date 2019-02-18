using MeteringStation.Mobile.DataAccess.Databases;
using MeteringStation.Mobile.DataAccess.Models;
using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Pages.Services;
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
        private readonly IMetersDatabase metersDatabase;
        private readonly HttpClient httpClient;

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

        public ICommand Refresh { get; }
        public ICommand Configure { get; }

        #endregion

        public MetersViewModel(
            IMetersDatabase metersDatabase,
            HttpClient httpClient, 
            INavigationService navigationService)
        {
            this.metersDatabase = metersDatabase;
            this.httpClient = httpClient;

            Refresh = new Command(async () => await RefreshDevices());
            Configure = new Command(async () => await navigationService.NavigateToConfigureMetersPageAsync());
        }

        private async Task RefreshDevices()
        {
            IsLoading = true;

            var registeredDevices = (await metersDatabase.GetMetersAsync()).ToList();

            var results = registeredDevices.Select(d => DownloadDeviceDataAsync(d)).ToArray();
            await Task.WhenAll(results);
            Readings.Clear();

            results
                .SelectMany(p => p.Result)
                .ToList()
                .ForEach(i => Readings.Add(i));

            IsLoading = false;
        }

        private async Task<IEnumerable<Reading>> DownloadDeviceDataAsync(RegisteredMeter registeredMeter)
        {
            try
            {
                using (var response = await httpClient.GetAsync($"http://{registeredMeter.DeviceIp}:8080/v1/readings"))
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
                        ClientId = registeredMeter.DeviceId
                    });
                }
            }
            catch(Exception)
            {
                // TODO display toast error or something
                return new[]
                {
                    new Reading()
                    {
                        PM25 = "-2",
                        PM10 = "-2",
                        DeviceName = "Error",
                        ClientId = registeredMeter.DeviceId
                    }
                };
            }
        }
    }
}
