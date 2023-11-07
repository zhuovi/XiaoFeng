using System.ComponentModel;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-20 09:40:00                            *
*  Version : v 2.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region 关联表类型
    /// <summary>
    /// 关联表类型
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// Inner
        /// </summary>
        [Description("数据交集")]
        Inner = 0,
        /// <summary>
        /// Left
        /// </summary>
        [Description("左表数据为主")]
        Left = 1,
        /// <summary>
        /// Right
        /// </summary>
        [Description("右表数据为主")]
        Right = 2,
        /// <summary>
        /// Full
        /// </summary>
        [Description("数据并集")]
        Full = 3,
        /// <summary>
        /// Union All
        /// </summary>
        [Description("数据合并")]
        Union = 4
    }
    #endregion
}
