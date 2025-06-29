using System.Collections.Generic;

namespace ApeFree.ServiceDiscovery.Entity
{
    public class DiscoveryResponse:BaseResponse
    {
        public List<ServiceInfo> Services { get; set; }
    }
}
