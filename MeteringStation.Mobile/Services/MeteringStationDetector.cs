using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Services.Dtos;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.Services
{
    public class MeteringStationDetector : IMeteringStationDetector
    {
        private const string DiscoverySenderId = "bb407f65-e945-49b7-849b-925cee163de5";
        private const string MessageEncryptionKey = "287de7751edd4b3d9897d2f2ebc7e869";
        private const string MessageIV = "a3b334557cfe4613";
        private const int Port = 8999;
        private readonly IEventAggregator eventAggregator;

        public MeteringStationDetector(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public async Task StartDiscoveryAsync()
        {
            await DiscoverDevicesAsync(TimeSpan.FromSeconds(10));
        }

        private async Task DiscoverDevicesAsync(TimeSpan discoveryTimeout)
        {
            eventAggregator.Publish(new DetectionStartedEvent());
            var client = new UdpClient();
            var requestData = Encoding.ASCII.GetBytes(DiscoverySenderId);
            var serverEp = new IPEndPoint(IPAddress.Any, 0);
            client.EnableBroadcast = true;
            client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, Port));
            client.BeginReceive(ar => Received(ar, client), new object());
            await Task.Delay(discoveryTimeout);
            client.Close();
            eventAggregator.Publish(new DetectionFinishedEvent());
        }

        private void Received(IAsyncResult ar, UdpClient client)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port);
                byte[] bytes = client.EndReceive(ar, ref ip);
                var responseMessage = Decrypt(Encoding.ASCII.GetString(bytes));
                var response = JsonConvert.DeserializeObject<MeteringStationResponse>(responseMessage);
                eventAggregator
                    .Publish(new DeviceDetectedEvent(response.ClientId, ip.Address));

                client.BeginReceive(a => Received(a, client), new object());
            }
            catch(Exception)
            {
                //ignore if found after the timeout
            }
            
        }

        public static string Decrypt(string data)
        {
            byte[] key = Encoding.ASCII.GetBytes(MessageEncryptionKey);
            byte[] iv = Encoding.ASCII.GetBytes(MessageIV);

            using (var rijndaelManaged =
                    new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
            {
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.KeySize = 256;
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(data)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
        }
    }
}
