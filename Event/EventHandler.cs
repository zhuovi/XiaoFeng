using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021-01-25 15:33:37               *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Event
{
    /// <summary>
    /// 委托事件
    /// </summary>
    /// <param name="Message">消息</param>
    /// <param name="e">错误信息</param>
    public delegate void MessageEventHandler(string Message, EventArgs e);
    /// <summary>
    /// 委托事件
    /// </summary>
    /// <param name="bytes">字节组</param>
    /// <param name="e">错误信息</param>
    public delegate void MessageByteEventHandler(byte[] bytes, EventArgs e);
}