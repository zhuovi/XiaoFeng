using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-01-03 11:25:06                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql.NewBack.Interface
{
    /// <summary>
    /// ISqlBuilder说明
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 数据仓
        /// </summary>
        IDataBuilder DataBuilder { get; set; }
    }
    public interface ISqlBuilder<T> : ISqlBuilder
    {

    }
    public interface ISqlBuilder<T1, T2> : ISqlBuilder<T1>
    {

    }
    public interface ISqlBuilder<T1, T2, T3> : ISqlBuilder<T1, T2>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4> : ISqlBuilder<T1, T2, T3>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5> : ISqlBuilder<T1, T2, T3, T4>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6> : ISqlBuilder<T1, T2, T3, T4, T5>
    {

    }
}