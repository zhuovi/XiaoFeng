using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/2/18 15:14:45          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 表单数据类型
    /// </summary>
    public enum FormType
    {
        /// <summary>
        /// 文本
        /// </summary>
        [Description("文本")]
        Text = 0,
        /// <summary>
        /// 文件
        /// </summary>
        [Description("文件")]
        File = 1
    }
}