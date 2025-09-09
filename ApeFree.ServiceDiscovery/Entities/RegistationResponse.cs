using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entities
{
    public class RegistationResponse:BaseResponse
    {
        public Dictionary<string,string> Signs { get; set; }
    }
}
