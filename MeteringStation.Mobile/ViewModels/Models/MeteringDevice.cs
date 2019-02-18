using MeteringStation.Mobile.ViewModels.Models.ValueObjects;
using System.ComponentModel;

namespace MeteringStation.Mobile.ViewModels.Models
{
    public class MeteringDevice : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private string _displayIp;

        public string DisplayIp
        {
            get => _displayIp; set
            {
                if (_displayIp != value)
                {
                    _displayIp = value;
                    OnPropertyChanged(nameof(DisplayIp));
                }
            }
        }

        public string Ip { get; set; }

        private MeteringDeviceStatus _status;

        public MeteringDeviceStatus Status
        {
            get => _status; set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
    }
}
