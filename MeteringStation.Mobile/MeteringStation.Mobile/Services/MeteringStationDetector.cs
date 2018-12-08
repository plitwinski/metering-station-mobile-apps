using MeteringStation.Mobile.Services.Dtos;
using MeteringStation.Mobile.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.Services
{
    public class MeteringStationDetector
    {
        private const string DiscoverySenderId = "bb407f65-e945-49b7-849b-925cee163de5";
        private const string MessageEncryptionKey = "287de7751edd4b3d9897d2f2ebc7e869";
        private const string MessageIV = "a3b334557cfe4613";
        private const int Port = 8999;

        private readonly List<Action<IEnumerable<MeteringStationDevice>>> _listOfCallbacks;

        public MeteringStationDetector()
        {
            _listOfCallbacks = new List<Action<IEnumerable<MeteringStationDevice>>>();
        }

        public void StartDiscovery()
        {
            DiscoverDevicesAsync(TimeSpan.FromSeconds(5))
                .ContinueWith(results => 
                    _listOfCallbacks.ForEach(p => p(results.Result)));
        }

        public void SubscribeToFoundDevices(Action<IEnumerable<MeteringStationDevice>> callback)
        {
            _listOfCallbacks.Add(callback);
        }

        private async Task<IEnumerable<MeteringStationDevice>> DiscoverDevicesAsync(TimeSpan discoveryTimeout)
        {
            var results = new List<MeteringStationDevice>();
            var client = new UdpClient();
            var requestData = Encoding.ASCII.GetBytes(DiscoverySenderId);
            var serverEp = new IPEndPoint(IPAddress.Any, 0);
            client.EnableBroadcast = true;
            client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, Port));
            client.BeginReceive(ar => Received(ar, client, results), new object());
            await Task.Delay(discoveryTimeout);
            client.Close();
            return results;
        }

        private static void Received(IAsyncResult ar, UdpClient client, List<MeteringStationDevice> results)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port);
                byte[] bytes = client.EndReceive(ar, ref ip);
                var responseMessage = Decrypt(Encoding.ASCII.GetString(bytes));
                var response = JsonConvert.DeserializeObject<MeteringStationResponse>(responseMessage);
                results.Add(new MeteringStationDevice(response.ClientId, ip.Address));
                client.BeginReceive(a => Received(a, client, results), new object());
            }
            catch(Exception ex)
            {

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
