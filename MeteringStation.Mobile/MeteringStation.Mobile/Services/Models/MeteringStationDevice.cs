using System.Net;

namespace MeteringStation.Mobile.Services.Models
{
    public class MeteringStationDevice
    {
        public MeteringStationDevice(string id, IPAddress ip)
        {
            Id = id;
            Ip = ip;
        }

        public string Id { get; }
        public IPAddress Ip { get; }
    }
}
