using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MeteringStation.Mobile.Services.Communication
{
    public class UdpBroadcaster : IDisposable, IBroadcaster
    {
        private readonly UdpClient udpClient;

        public UdpBroadcaster()
        {
            udpClient = new UdpClient();
        }

        public void Send(string discoverySenderId, int port, Action<IPEndPoint, string> onDetectedAction)
        {
            var requestData = Encoding.ASCII.GetBytes(discoverySenderId);
            var serverEp = new IPEndPoint(IPAddress.Any, 0);
            udpClient.EnableBroadcast = true;
            udpClient.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, port));
            udpClient.BeginReceive(ar => Received(ar, onDetectedAction, port), new object());
        }

        public void Close()
            => udpClient.Close();

        private void Received(IAsyncResult ar, Action<IPEndPoint, string> onDetectedAction, int port)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
                byte[] bytes = udpClient.EndReceive(ar, ref ip);
                var responseMessage = Encoding.ASCII.GetString(bytes);
                onDetectedAction(ip, responseMessage);
                udpClient.BeginReceive(a => Received(a, onDetectedAction, port), new object());
            }
            catch (Exception)
            {
                //ignore if found after the timeout
            }

        }

        public void Dispose()
            => udpClient?.Dispose();
    }
}
