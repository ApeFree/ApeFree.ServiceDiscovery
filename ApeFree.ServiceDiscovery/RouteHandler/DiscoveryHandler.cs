using ApeFree.ServiceDiscovery.Entity;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace ApeFree.ServiceDiscovery.RouteHandler
{
    public class DiscoveryHandler : IRouteHandler
    {
        public BaseResponse RequestHandler(RequestDispatcher service, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var result = new DiscoveryResponse();
            result.Success = true;
            try
            {
                //接收客户端传过来的数据并转成字符串类型
                var bodyBytes = RequestHandlerHelper.ReadRequestBody(request);
                //处理请求
                var registationRequest = JsonConvert.DeserializeObject<DiscoveryRequest>(Encoding.UTF8.GetString(bodyBytes));
                if (registationRequest != null)
                {
                    registationRequest.IPAddress = request.Url.ToString();
                    result.Services = service.DiscoveryRequestHandler?.Invoke(registationRequest);
                    if (result.Services == null || result.Services.Count <= 0)
                    {
                        throw new ArgumentNullException("未找到服务");
                    }
                }
                else
                {
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }
    }
}
