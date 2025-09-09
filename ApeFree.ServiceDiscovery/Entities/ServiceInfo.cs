using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApeFree.ServiceDiscovery.Entities
{
    /// <summary>
    /// 服务信息
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务关键字，用于检索
        /// </summary>
        public string[] Keywords { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTimestamp { get; internal set; }

        /// <summary>
        /// 公开的服务详细信息
        /// </summary>
        public Dictionary<string, object> Details { get; set; }
    }
}
