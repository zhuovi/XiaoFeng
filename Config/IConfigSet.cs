using System;
using System.Collections.Generic;
using System.ComponentModel;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 系统配置接口
    /// </summary>
    /// <typeparam name="TConfig">结构</typeparam>
    public interface IConfigSet<TConfig> : IConfigSet where TConfig : IConfigSet<TConfig>, new()
    {
        #region 方法
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="reload">是否强制从文件中读取 默认否</param>
        /// <returns></returns>
        TConfig Get(Boolean reload = false);
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        TConfig GetEntity(Func<TConfig, Boolean> func);
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IEnumerable<TConfig> GetEntities(Func<TConfig, Boolean> func);
        /// <summary>
        /// 获取泛路径配置
        /// </summary>
        /// <param name="value">泛值</param>
        /// <returns></returns>
        TConfig GetEntity(object value);
        #endregion
    }
    /// <summary>
    /// 配置接口
    /// </summary>
    public interface IConfigSet : IEntityBase
    {
        #region 属性
        /// <summary>
        /// 配置文件属性
        /// </summary>
        [Description("配置文件属性")]
        [Json.JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        ConfigFileAttribute ConfigFileAttribute { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 读取内容
        /// </summary>
        /// <returns></returns>
        string ReadContent();
        /// <summary>
        /// 保存内容
        /// </summary>
        /// <param name="indented">是否格式化</param>
        /// <param name="comment">是否带注释说明</param>
        bool Save(Boolean indented = true, Boolean comment = true);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        Boolean Delete();
        #endregion
    }
}