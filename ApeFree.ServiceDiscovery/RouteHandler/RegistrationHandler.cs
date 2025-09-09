using ApeFree.ServiceDiscovery.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ApeFree.ServiceDiscovery.RouteHandler
{
    public class RegistrationHandler : IRouteHandler
    {
        public BaseResponse RequestHandler(RequestDispatcher service, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var result = new RegistationResponse();
            result.Success = true;
            try
            {
                //接收客户端传过来的数据并转成字符串类型
                var bodyBytes = RequestHandlerHelper.ReadRequestBody(request);
                var d = Encoding.UTF8.GetString(bodyBytes);
                //处理请求
                var registationRequest = JsonConvert.DeserializeObject<RegistationRequest>(Encoding.UTF8.GetString(bodyBytes));
                if (registationRequest != null)
                {
                    registationRequest.IPAddress = request.Url.ToString();
                    result.Signs = service.RegisterRequestHandler?.Invoke(registationRequest);
                }
                else
                {
                }
                return result;
            }
            catch (System.Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }
    }
}
