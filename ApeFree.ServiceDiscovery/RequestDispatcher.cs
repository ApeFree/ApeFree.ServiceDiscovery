using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using ApeFree.ServiceDiscovery.Entities;
using ApeFree.ServiceDiscovery.RouteHandler;
using Newtonsoft.Json;

namespace ApeFree.ServiceDiscovery
{
    /// <summary>
    /// 请求分发器
    /// </summary>
    public class RequestDispatcher
    {
        // HTTP监听器
        private HttpListener httpListener;

        /// <summary>
        /// 监听的地址
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 注册请求处理事件
        /// </summary>
        public Func<RegistationRequest, Dictionary<string, string>> RegisterRequestHandler;

        /// <summary>
        ///  发现请求处理事件
        /// </summary>
        public Func<DiscoveryRequest, List<ServiceInfo>> DiscoveryRequestHandler;

        /// <summary>
        /// 监听的路由列表和Handler
        /// </summary>
        private Dictionary<string, IRouteHandler> routes = new Dictionary<string, IRouteHandler>();

        public RequestDispatcher(string address, int port)
        {
            httpListener = new HttpListener();
            Address = address;
            Port = port;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            //启动监听器
            httpListener.Start();

            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpListener.BeginGetContext(OnRequestReceived, null);

            Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
        }

        /// <summary>
        /// 添加监听的路由和处理器
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void AddPrefixe(string route, IRouteHandler handler)
        {
            var url = $"http://{Address}:{Port}/{route}/";
            httpListener.Prefixes.Add(url);  //监听的是以item.Key + "/"+XXX接口
            routes.Add(route, handler);
            Console.WriteLine(url);
        }

        /// <summary>
        /// 接受到请求的处理事件
        /// </summary>· 
        /// <param name="ar"></param>
        private void OnRequestReceived(IAsyncResult ar)
        {
            // 继续监听其他请求
            httpListener.BeginGetContext(OnRequestReceived, null);

            //获得请求上下文
            var context = httpListener.EndGetContext(ar);

            // 获取请求和响应对象
            var req = context.Request;
            var resp = context.Response;

            if (req.HttpMethod != "POST")
            {
                Console.WriteLine("不处理除POST外的请求");
                return;
            }

            //获取访问的路径
            var route = req.RawUrl.Replace("/", "");

            // 根据路由找到对应的处理器
            routes.TryGetValue(route, out var handler);
            BaseResponse result = null;
            if (handler != null)
            {
                result = handler.RequestHandler(this, context);
            }
            //构建返回
            resp.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            resp.AddHeader("Content-type", "text/plain");//添加响应头信息
            resp.ContentEncoding = Encoding.UTF8;
            var jsonString = JsonConvert.SerializeObject(result);
            var returnByteArr = Encoding.UTF8.GetBytes(jsonString);//设置客户端返回信息的编码

            using (var stream = resp.OutputStream)
            {
                //把处理信息返回到客户端
                stream.Write(returnByteArr, 0, returnByteArr.Length);
            }
        }
    }
}
