﻿using ApeFree.ServiceDiscovery.Entity;
using ApeFree.ServiceDiscovery.RouteHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ApeFree.ServiceDiscovery
{
    public class DiscoveryService
    {

        private RequestDispatcher httpServer;
        private UdpHeartbeatListener udpServer;
        private Dictionary<string, ServiceInfo> ServiceInfoList;

        public string IPAddress { get; private set; }
        public int HttpPort { get; private set; }
        public int UdpPort { get; private set; }

        public int HeartbeatTime = 10000;

        private object writeReadLock = new object();

        public DiscoveryService(string IPAddress, int httpPort = 4555, int udpPort = 4556)
        {
            this.IPAddress = IPAddress;
            HttpPort = httpPort;
            UdpPort = udpPort;
            ServiceInfoList = new Dictionary<string, ServiceInfo>();
            udpServer = new UdpHeartbeatListener(udpPort);
            httpServer = new RequestDispatcher(this.IPAddress, HttpPort);
            udpServer.HeartbeatHandler = OnHeartbeatHandler;
            udpServer.Start();

            httpServer.RegisterRequestHandler = OnRegisterRequestHandler;
            httpServer.DiscoveryRequestHandler = OnDiscoveryRequestHandler;
            httpServer.AddPrefixe("Discovery", new DiscoveryHandler());
            httpServer.AddPrefixe("Registration", new RegistrationHandler());

        }

        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bytes"></param>
        private void OnHeartbeatHandler(object sender, byte[] bytes)
        {
            string jsonString = Encoding.UTF8.GetString(bytes);
            var request = JsonConvert.DeserializeObject<HeartbeatRequest>(jsonString);
            ServiceInfoList = ServiceInfoList.Select(x =>
            {
                if (request.ServiceInfoIds.Contains(x.Key) && x.Value.IPAddress == request.IPAddress)
                {
                    x.Value.LastHeartbeatTime = DateTime.Now;
                }
                return x;
            }).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private Dictionary<string, string> OnRegisterRequestHandler(RegistationRequest request)
        {
            //注册服务列表内部是否有重复
            if (request.ServiceInfoList.GroupBy(p => p.Name).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("注册列表中有重复项，请检查后重新注册");
            }

            lock (writeReadLock)
            {
                if (ServiceInfoList.Any(x => request.ServiceInfoList.Any(s => s.Name == x.Value.Name)))
                {
                    throw new ArgumentException("已存在同样名称的服务");
                }
                else
                {
                    var serviceInfoDic = request.ServiceInfoList.ToDictionary(x => Guid.NewGuid().ToString().Substring(0, 16), x => x);
                    foreach (var item in serviceInfoDic)
                    {
                        item.Value.Id = item.Key;
                        item.Value.LastHeartbeatTime =DateTime.Now;
                        ServiceInfoList.Add(item.Key, item.Value);
                    }
                    return serviceInfoDic.ToDictionary(x => x.Key, x => x.Value.Name);
                }
            }
        }

        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private List<ServiceInfo> OnDiscoveryRequestHandler(DiscoveryRequest request)
        {
            lock (writeReadLock)
            {
                switch (request.DiscoveryType)
                {
                    case DiscoveryType.ById:
                        return ServiceInfoList.Where(x => x.Key == request.Sign && (DateTime.Now - x.Value.LastHeartbeatTime).TotalMilliseconds < HeartbeatTime).Select(x => x.Value).ToList();
                    case DiscoveryType.ByName:
                        return ServiceInfoList.Where(x => x.Value.Name == request.Sign && (DateTime.Now - x.Value.LastHeartbeatTime).TotalMilliseconds < HeartbeatTime).Select(x => x.Value).ToList();
                    case DiscoveryType.ByAlias:
                        return ServiceInfoList.Where(x => (x.Value.Types != null ? x.Value.Types.Contains(request.Sign) : false) && (DateTime.Now - x.Value.LastHeartbeatTime).TotalMilliseconds < HeartbeatTime).Select(x => x.Value).ToList();
                    default:
                        throw new InvalidOperationException("无法以未知的方式筛选服务");
                }
            }
        }

        public void Start()
        {
            httpServer.Start();
            udpServer.Start();
        }

        public void AddPrefixe(string route, IRouteHandler handler)
        {
            httpServer.AddPrefixe(route, handler);
        }

    }
}
