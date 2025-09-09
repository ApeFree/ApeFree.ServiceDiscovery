using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entities
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
