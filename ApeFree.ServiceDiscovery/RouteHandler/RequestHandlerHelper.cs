using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ApeFree.ServiceDiscovery.RouteHandler
{
    internal class RequestHandlerHelper
    {
        public static byte[] ReadRequestBody(HttpListenerRequest request)
        {
            var byteList = new List<byte>();
            var byteArr = new byte[1024];
            int readLen = 0;
            int len = 0;
            do
            {
                readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                len += readLen;
                byteList.AddRange(byteArr.Take(readLen));
            } while (readLen != 0);
            return byteList.ToArray();
        }
    }
}
