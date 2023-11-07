/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-05-12 11:49:14                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 数据库映射项
    /// </summary>
    public interface IDataMappingItem
    {
        #region 属性
        /// <summary>
        /// 原来数据库连接串名
        /// </summary>
        string FromName { get; set; }
        /// <summary>
        /// 原来数据库连接串索引
        /// </summary>
        uint FromIndex { get; set; }
        /// <summary>
        /// 指向数据库连接串名称
        /// </summary>
        string ToName { get; set; }
        /// <summary>
        /// 指向数据库连接串索引
        /// </summary>
        uint ToIndex { get; set; }
        #endregion
    }
}