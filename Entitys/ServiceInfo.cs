using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entity
{
    public class ServiceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Types { get; set; }
        public string IPAddress { get; set; }
        public DateTime LastHeartbeatTime { get; internal set; }
        public Dictionary<string, object> ExtendedInfo { get; set; }

        public ServiceInfo()
        {

        }

        public ServiceInfo(Type type)
        {
        }
    }
}
