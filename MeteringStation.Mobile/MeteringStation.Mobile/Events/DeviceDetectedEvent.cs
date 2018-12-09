using System.Net;

namespace MeteringStation.Mobile.Events
{
    public class DeviceDetectedEvent
    {
        public DeviceDetectedEvent(string id, IPAddress ip)
        {
            Id = id;
            Ip = ip;
        }

        public string Id { get; }
        public IPAddress Ip { get; }
    }
}
