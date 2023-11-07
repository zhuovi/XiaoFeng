using System;
using System.Threading.Tasks;
using XiaoFeng.Threading;

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
    /// <summary>
    /// 任务出错事件
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="exception">错误</param>
    public delegate void TaskError(Task task, Exception exception);
    /// <summary>
    /// 任务队列出错事件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="workItem">数据</param>
    /// <param name="exception">错误</param>
    public delegate void TaskQueueError<T>(T workItem, Exception exception);
    /// <summary>
    /// 任务队列成功事件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="workItem">数据</param>
    public delegate void TaskQueueOk<T>(T workItem);
    /// <summary>
    /// 任务队列为空事件
    /// </summary>
    /// <param name="taskService">任务队列</param>
    public delegate void TaskQueueEmpty<T>(ITaskServiceQueue<T> taskService);
}