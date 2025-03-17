using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entity
{
    public class RegistationRequest
    {
        public List<ServiceInfo> ServiceInfoList { get; set; }

        public ClientInfo ClientInfo { get; set; }
        public string IPAddress { get; set; }
    }
}
