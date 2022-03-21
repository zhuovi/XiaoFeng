using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Sockets
{
    #region Socket状态
    /// <summary>
    /// Socket状态
    /// </summary>
    public enum SocketState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 0,
        /// <summary>
        /// 运行中
        /// </summary>
        Runing = 1,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 2
    }
    #endregion
}