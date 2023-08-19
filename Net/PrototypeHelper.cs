using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020/6/29 17:20:50                *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络扩展
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 方法
        /// <summary>
        /// EndPoint转IPEndPoint
        /// </summary>
        /// <param name="endPoint">EndPoint</param>
        /// <returns></returns>
        public static IPEndPoint ToIPEndPoint(this EndPoint endPoint)
        {
            if (endPoint == null) return null;
            if (endPoint is IPEndPoint) return (IPEndPoint)endPoint;
            var IPPorts = endPoint.ToString().Split(':');
            return new IPEndPoint(IPAddress.Parse(IPPorts[0]), IPPorts[1].ToCast<int>());
        }
        #endregion
    }
}