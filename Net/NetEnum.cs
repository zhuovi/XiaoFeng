/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
using System.ComponentModel;

namespace XiaoFeng.Net
{
    #region WebSocket类型
    /// <summary>
    /// WebSocket类型
    /// </summary>
    public enum WebSocketType
    {
        /// <summary>
        /// 无
        /// </summary>
        Null = 0,
        /// <summary>
        /// WS类型
        /// </summary>
        WS = 1,
        /// <summary>
        /// WSS类型
        /// </summary>
        WSS = 2
    }
    #endregion

    #region Socket 数据类型
    /// <summary>
    /// Socket 数据类型
    /// </summary>
    public enum SocketDataType
    {
        /// <summary>
        /// 字符串类型
        /// </summary>
        String = 0,
        /// <summary>
        /// 16进制类型 一般用于串口传输
        /// </summary>
        HexString = 1
    }
    #endregion

    #region Socket类型
    /// <summary>
    /// Socket类型
    /// </summary>
    public enum SocketTypes
    {
        /// <summary>
        /// Socket
        /// </summary>
        Socket = 0,
        /// WebSocket
        WebSocket = 1
    }
    #endregion

    #region Socket状态
    /// <summary>
    /// Socket状态
    /// </summary>
    public enum SocketState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [Description("空闲")]
        Idle = 0,
        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Runing = 1,
        /// <summary>
        /// 停止
        /// </summary>
        [Description("停止")]
        Stop = 2
    }
    #endregion

    #region 连接类型
    /// <summary>
    /// 连接类型
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Socket
        /// </summary>
        Socket = 0,
        /// <summary>
        /// WebSocket
        /// </summary>
        WebSocket = 1,
        /// <summary>
        /// Http
        /// </summary>
        Http = 2
    }
    #endregion
}