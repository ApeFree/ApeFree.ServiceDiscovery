using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entity
{
    public class RegistationResponse:BaseResponse
    {
        public Dictionary<string,string> Signs { get; set; }
    }
}
