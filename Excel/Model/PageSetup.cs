using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-13 18:51:39                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel.Model
{
    /// <summary>
    /// 页面设置
    /// </summary>
    public class PageSetup
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PageSetup()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 纸张大小
        /// </summary>
        public int PaperSize { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public string Orientation { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}