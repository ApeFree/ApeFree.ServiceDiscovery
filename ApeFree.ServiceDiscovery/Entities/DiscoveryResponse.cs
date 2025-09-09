using System.Collections.Generic;

namespace ApeFree.ServiceDiscovery.Entities
{
    public class DiscoveryResponse:BaseResponse
    {
        public List<ServiceInfo> Services { get; set; }
    }
}
