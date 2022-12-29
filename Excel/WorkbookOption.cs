using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 11:58:20                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 工作文档选项
    /// </summary>
    public class WorkbookOption
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public WorkbookOption()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否包含隐藏表
        /// </summary>
        public Boolean IncludeHiddenSheet { get; set; } = true;
        /// <summary>
        /// 是否包含隐藏行
        /// </summary>
        public Boolean IncludeHiddenRow { get; set; } = true;
        /// <summary>
        /// 是否包含隐藏列
        /// </summary>
        public Boolean IncludeHiddenColumn { get; set; } = true;
        /// <summary>
        /// 是否加载所有表
        /// </summary>
        public Boolean LoadSheets { get; set; } = false;
        #endregion

        #region 方法

        #endregion
    }
}