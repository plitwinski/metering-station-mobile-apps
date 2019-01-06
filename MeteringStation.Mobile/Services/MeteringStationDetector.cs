using Autofac;
using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Services.Communication;
using MeteringStation.Mobile.Services.Dtos;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
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
        private readonly ILifetimeScope lifetimeScope;
        private readonly TimeSpan? discoveryTimeout;

        public MeteringStationDetector(IEventAggregator eventAggregator, 
            ILifetimeScope lifetimeScope,
            TimeSpan? discoveryTimeout = null)
        {
            this.eventAggregator = eventAggregator;
            this.lifetimeScope = lifetimeScope;
            this.discoveryTimeout = discoveryTimeout;
        }

        public async Task StartDiscoveryAsync()
        {
            await DiscoverDevicesAsync(discoveryTimeout ?? TimeSpan.FromSeconds(10));
        }

        private async Task DiscoverDevicesAsync(TimeSpan discoveryTimeout)
        {
            eventAggregator.Publish(new DetectionStartedEvent());
            using (var c = lifetimeScope.BeginLifetimeScope())
            {
                var broadcaster = c.Resolve<IBroadcaster>();
                broadcaster.Send(DiscoverySenderId, Port, OnDeviceDetected);
                await Task.Delay(discoveryTimeout);
                broadcaster.Close();
            }
            eventAggregator.Publish(new DetectionFinishedEvent());
        }

        private void OnDeviceDetected(IPEndPoint ip, string encryptedResponse)
        {
            var responseMessage = Decrypt(encryptedResponse);
            var response = JsonConvert.DeserializeObject<MeteringStationResponse>(responseMessage);
            eventAggregator
                .Publish(new DeviceDetectedEvent(response.ClientId, ip.Address));
        }

        private static string Decrypt(string data)
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
