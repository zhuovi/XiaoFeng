using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-10 15:49:22                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel.Model
{
    /// <summary>
    /// 页面边距
    /// </summary>
    public class PageMargin
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PageMargin()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 左边距
        /// </summary>
        public double Left { get; set; } = 0.7;
        /// <summary>
        /// 右边距
        /// </summary>
        public double Right { get; set; } = 0.7;
        /// <summary>
        /// 上边距
        /// </summary>
        public double Top { get; set; } = 0.75;
        /// <summary>
        /// 下边距
        /// </summary>
        public double Bottom { get; set; } = 0.75;
        /// <summary>
        /// 页眉
        /// </summary>
        public double Header { get; set; } = 0.3;
        /// <summary>
        /// 页脚
        /// </summary>
        public double Footer { get; set; } = 0.3;
        #endregion

        #region 方法

        #endregion
    }
}