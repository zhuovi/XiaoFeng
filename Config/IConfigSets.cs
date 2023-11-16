using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-11-14 17:29:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 配置接口
    /// </summary>
    public interface IConfigSets : IConfigSet
    {
    }
    /// <summary>
    /// 配置接口
    /// </summary>
    /// <typeparam name="TConfig">配置</typeparam>
    public interface IConfigSets<TConfig> : IConfigSet<TConfig>, IConfigSets where TConfig : IConfigSets<TConfig>, new()
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        List<TConfig> List { get; set; }
    }
}