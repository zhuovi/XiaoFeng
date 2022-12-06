using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021-02-22 下午 10:08:10          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Data
{
    /// <summary>
    /// 指定事务的隔离级别。
    /// Version : 1.0.0
    /// 更新说明
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        /// 空级别
        /// </summary>
        [Description("空级别")]
        DbNull = -1,
        /// <summary>
        /// 可以在事务期间读取可变数据，但是不可以修改，也不可以添加任何新数据。
        /// </summary>
        [Description("串行读")] 
        Serializable = 0,
        /// <summary>
        /// 可以在事务期间读取可变数据，但是不可以修改。 可以在事务期间添加新数据。
        /// </summary>
        [Description("可重复读")]
        RepeatableRead = 1,
        /// <summary>
        /// 不可以在事务期间读取可变数据，但是可以修改。
        /// </summary>
        [Description("提交读")]
        ReadCommitted = 2,
        /// <summary>
        /// 可以在事务期间读取和修改可变数据。
        /// </summary>  
        [Description("未提交读")] 
        ReadUncommitted = 3,
        /// <summary>
        /// 可以读取可变数据。 在事务修改数据之前，它会验证在它最初读取数据之后另一个事务是否更改过这些数据。 如果数据已更新，则会引发错误。 这样，事务可获取先前提交的数据值。
        /// </summary>
        [Description("隔离未提交读")]
        Snapshot = 4,
        ///<summary>
        /// 无法覆盖隔离级别更高的事务中的挂起的更改。
        /// </summary>   
        [Description("混乱读")] 
        Chaos = 5,
        /// <summary>
        /// 正在使用与指定隔离级别不同的隔离级别，但是无法确定该级别。 如果设置了此值，则会引发异常。
        /// </summary>
        [Description("未指定")]
        Unspecified = 6
    }
}