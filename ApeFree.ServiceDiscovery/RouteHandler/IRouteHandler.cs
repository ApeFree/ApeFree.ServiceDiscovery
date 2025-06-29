using ApeFree.ServiceDiscovery.Entity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ApeFree.ServiceDiscovery.RouteHandler
{
    public interface IRouteHandler
    {
        BaseResponse RequestHandler(RequestDispatcher service,HttpListenerContext context);
    }
}
