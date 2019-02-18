using MeteringStation.Mobile.DataAccess.Databases;
using MeteringStation.Mobile.DataAccess.Models;
using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Services;
using MeteringStation.Mobile.ViewModels.Models;
using MeteringStation.Mobile.ViewModels.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MeteringStation.Mobile.ViewModels
{
    public class ConfigureMetersViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region public properties

        private ObservableCollection<Models.MeteringDevice> devices = new ObservableCollection<Models.MeteringDevice>();
        public ObservableCollection<Models.MeteringDevice> Devices
        {
            get { return devices; }
            protected set
            {
                if (devices != value)
                {
                    devices = value;
                    OnPropertyChanged(nameof(Devices));
                }
            }
        }

        private bool isLoading = false;

        public bool IsLoading
        {
            get { return isLoading; }
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
        public ICommand Load { get; }
        public ICommand AddDevice { get; }
        public ICommand RemoveDevice { get; }

        #endregion

        private readonly IMeteringStationDetector meteringStationDetector;
        private readonly IMetersDatabase metersDatabase;
        private List<RegisteredMeter> registeredDevices;

        public ConfigureMetersViewModel(IMeteringStationDetector meteringStationDetector, 
            IMetersDatabase metersDatabase,
            IEventAggregator eventAggregator)
        {
            this.meteringStationDetector = meteringStationDetector;
            this.metersDatabase = metersDatabase;

            registeredDevices = Enumerable.Empty<RegisteredMeter>().ToList();

            Refresh = new Command(async () => await meteringStationDetector.StartDiscoveryAsync());
            Load = new Command(() => Device.BeginInvokeOnMainThread(async () => await LoadRegisterdDevicesAsync()));
            AddDevice = new Command<MeteringDevice>(async device => await UpsertDeviceAsync(device));
            RemoveDevice = new Command<MeteringDevice>(async device => await DeleteDeviceAsync(device));

            eventAggregator.Subscribe<DetectionStartedEvent>(_ => IsLoading = true);
            eventAggregator.Subscribe<DetectionFinishedEvent>(_ => IsLoading = false);
            eventAggregator.Subscribe<DeviceDetectedEvent>(e => HandleDevice(e));
        }

        private async Task LoadRegisterdDevicesAsync()
        {
            IsLoading = true;
            registeredDevices = (await metersDatabase.GetMetersAsync()).ToList();

            Devices.Clear();
            Devices = new ObservableCollection<MeteringDevice>(
                registeredDevices.Select(r => new MeteringDevice()
                {
                    Id = r.DeviceId,
                    DisplayIp = r.DeviceIp,
                    Ip = r.DeviceIp,
                    Status = MeteringDeviceStatus.Registered
                }));
            IsLoading = false;

            await meteringStationDetector.StartDiscoveryAsync();
        }

        private void HandleDevice(DeviceDetectedEvent e)
        {
            var existingDevice = Devices.FirstOrDefault(p => p.Id == e.Id);
            var isNewDevice = existingDevice == null;
            if (isNewDevice)
            {
                existingDevice = new MeteringDevice()
                {
                    Id = e.Id
                };
                Devices.Add(existingDevice);
            }
            existingDevice.Status = isNewDevice
                ? MeteringDeviceStatus.New
                : MeteringDeviceStatus.Found;

            existingDevice.DisplayIp = FormatIp(existingDevice, e);
            existingDevice.Ip = e.Ip.ToString();
        }

        private async Task UpsertDeviceAsync(MeteringDevice device)
        {
            var existingDevice = registeredDevices.FirstOrDefault(p => p.DeviceId == device.Id);
            if(existingDevice == null)
            {
                existingDevice = new RegisteredMeter()
                {
                    DeviceId = device.Id
                };
            }
            existingDevice.DeviceIp = device.Ip;
            await metersDatabase.UpsertAsync(existingDevice);
            await LoadRegisterdDevicesAsync();
        }

        private async Task DeleteDeviceAsync(MeteringDevice device)
        {
            var existingDevice = registeredDevices.FirstOrDefault(p => p.DeviceId == device.Id);
            if (existingDevice != null)
            {
                await metersDatabase.RemoveMeterAsync(existingDevice);
                await LoadRegisterdDevicesAsync();
            }
        }

        private string FormatIp(MeteringDevice existingDevice, DeviceDetectedEvent e)
        {
            string prefix = "";
            if(!string.IsNullOrEmpty(existingDevice.DisplayIp))
            {
                prefix = $"{existingDevice.DisplayIp} -> ";
            }
            return $"{prefix}{e.Ip.ToString()}";
        }
    }
}
