using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-12-18 09:55:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// XiaoFeng 配置
    /// </summary>
    public class ConfigOptions : IOptions
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public ConfigOptions()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否加密配置
        /// </summary>
        [Description("是否加密配置")]
        public Boolean? IsEncryptConfig { get; set; }
        /// <summary>
        /// 加密KEY
        /// </summary>
        [Description("加密KEY")]
        public string EncryptKey { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}