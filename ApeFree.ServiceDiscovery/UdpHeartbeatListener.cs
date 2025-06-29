using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ApeFree.ServiceDiscovery
{
    public class UdpHeartbeatListener
    {
        public int Port { get; private set; }

        private UdpClient client;


        public EventHandler<byte[]> HeartbeatHandler;
        public UdpHeartbeatListener(int port)
        {
            Port = port;
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(RemoteIpEndPoint);
        }

        public void Start()
        {
            client.BeginReceive(EndReceive, client);
        }

        private void EndReceive(IAsyncResult ar)
        {
            var udpclient = ar.AsyncState as UdpClient;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedBytes = udpclient.EndReceive(ar, ref remoteEP); // 获取接收到的数据

            HeartbeatHandler?.Invoke(this, receivedBytes);
            udpclient.BeginReceive(EndReceive, udpclient);
        }
    }
}
