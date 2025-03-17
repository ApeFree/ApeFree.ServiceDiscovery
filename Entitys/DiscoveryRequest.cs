using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entity
{
    public class DiscoveryRequest
    {

        public string Sign { get; set; }
        public DiscoveryType DiscoveryType { get; set; }

        public string IPAddress { get; set; }
    }
}
