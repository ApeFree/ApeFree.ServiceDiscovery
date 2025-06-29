using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using ApeFree.ServiceDiscovery.Entity;
using ApeFree.ServiceDiscovery.RouteHandler;
using Newtonsoft.Json;

namespace ApeFree.ServiceDiscovery
{
    public class RequestDispatcher
    {
        public string Address { get; private set; }
        public int Port { get; private set; }

        static HttpListener httpListener;

        public Func<RegistationRequest, Dictionary<string,string>> RegisterRequestHandler;
        public Func<DiscoveryRequest, List<ServiceInfo>> DiscoveryRequestHandler;

        /// <summary>
        /// 监听的路由列表和Handler
        /// </summary>
        Dictionary<string, IRouteHandler> routes = new Dictionary<string, IRouteHandler>();

        public RequestDispatcher(string address, int port)
        {
            httpListener = new HttpListener();
            Address = address;
            Port = port;
        }



        public void Start()
        {
            //启动监听器
            httpListener.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpListener.BeginGetContext(Result, null);
            Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
        }


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
        private void Result(IAsyncResult ar)
        {
            httpListener.BeginGetContext(Result, null);
            var guid = Guid.NewGuid().ToString();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"接到新的请求:{guid},时间：{DateTime.Now.ToString()}");
            //获得context对象
            var context = httpListener.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;


            if (request.HttpMethod != "POST")
            {
                Console.WriteLine("不处理除POST外的请求");
                return;
            }

            //获取访问的路径
            var route = request.RawUrl.Replace("/", "");

            IRouteHandler handler = default;
            routes.TryGetValue(route, out handler);
            BaseResponse result = null;
            if (handler != null)
            {
                result = handler.RequestHandler(this, context);
            }
            //构建返回
            response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            response.AddHeader("Content-type", "text/plain");//添加响应头信息
            response.ContentEncoding = Encoding.UTF8;
            var jsonString = JsonConvert.SerializeObject(result);
            var returnByteArr = Encoding.UTF8.GetBytes(jsonString);//设置客户端返回信息的编码

            using (var stream = response.OutputStream)
            {
                //把处理信息返回到客户端
                stream.Write(returnByteArr, 0, returnByteArr.Length);
            }
            Console.WriteLine($"请求处理完成：{guid},时间：{DateTime.Now.ToString()}\r\n");
        }
    }
}
