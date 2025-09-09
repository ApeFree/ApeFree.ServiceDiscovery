using ApeFree.ServiceDiscovery.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ApeFree.ServiceDiscovery
{
    public class ServiceDiscoveryClient
    {
        private ClientInfo Info;
        private UdpClient udpClient;
        private Dictionary<string, string> ServicesInfoList;
        private Timer heartbeatTimer;
        private HttpClient httpClient;
        private IPEndPoint serviceIPAddress;

        public ServiceDiscoveryClient(string ipAddress, int httpPort = 4555, int udpPort = 4556)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://{ipAddress}:{httpPort}");

            Info = new ClientInfo()
            {
                ClientId = Guid.NewGuid().ToString(),
            };

            ServicesInfoList = new Dictionary<string, string>();

            serviceIPAddress = new IPEndPoint(IPAddress.Parse(ipAddress), udpPort);

            udpClient = new UdpClient();
            heartbeatTimer = new Timer();
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();
        }

        private void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ServicesInfoList.Count <= 0)
            {
                return;
            }

            var bytes = new HeartbeatRequest(Info, ServicesInfoList.Keys.ToArray()).GetBytes();
            udpClient.Send(bytes, bytes.Length, serviceIPAddress);
        }

        public async Task<RegistationResponse> Register(List<ServiceInfo> servicesInfoList)
        {
            var reuqest = new RegistationRequest()
            {
                ServiceInfoList = servicesInfoList,
                ClientInfo = Info
            };

            var response = await httpClient.PostAsync<RegistationResponse>("Registration", reuqest);

            if (response.Success)
            {
                response.Signs.ForEach(x => ServicesInfoList.Add(x.Key, x.Value));
            }
            return response;
        }

        public void UnRegister(string nodeId)
        {
            ServicesInfoList.Remove(nodeId);
        }




        public async Task<DiscoveryResponse> Discovery(string sign, DiscoveryType type = DiscoveryType.Name)
        {
            var request = new DiscoveryRequest()
            {
                Sign = sign,
                DiscoveryType = type,
            };

            return await httpClient.PostAsync<DiscoveryResponse>("Discovery", request);
        }
    }
}
