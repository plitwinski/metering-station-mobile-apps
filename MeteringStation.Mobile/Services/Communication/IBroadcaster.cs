using System;
using System.Net;

namespace MeteringStation.Mobile.Services.Communication
{
    public interface IBroadcaster
    {
        void Close();
        void Dispose();
        void Send(string discoverySenderId, int port, Action<IPEndPoint, string> onDetectedAction);
    }
}