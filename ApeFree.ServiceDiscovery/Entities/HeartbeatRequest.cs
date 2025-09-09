using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entities
{
    public class HeartbeatRequest
    {
        //包格式
        //固定头（1byte）+版本号（1byte）+包长度（4byte）+数据（x byte）
        public string[] ServiceInfoIds { get; set; }
        public ClientInfo ClientInfo { get; set; }

        [JsonIgnore]
        public string IPAddress { get; set; }

        public HeartbeatRequest()
        {

        }

        public HeartbeatRequest(byte[] bytes)
        {
            var jsonString = Encoding.UTF8.GetString(bytes);
            var request = JsonConvert.DeserializeObject<HeartbeatRequest>(jsonString);

            this.ServiceInfoIds = request.ServiceInfoIds;
            this.ClientInfo = request.ClientInfo;
        }

        public HeartbeatRequest(ClientInfo info, string[] nodeIds)
        {
            ServiceInfoIds = nodeIds;
            ClientInfo = info;
        }

        public byte[] GetBytes()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var byteArr = Encoding.UTF8.GetBytes(jsonString);

            var bytes = new List<byte>();
            //    bytes.AddRange(BitConverter.GetBytes(byteArr.Length + 4));
            bytes.AddRange(byteArr);
            return bytes.ToArray();
        }
    }
}
