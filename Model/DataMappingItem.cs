/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-05-12 11:49:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 数据库映射项
    /// </summary>
    public class DataMappingItem : Disposable, IDataMappingItem
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataMappingItem() { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="fromName">原来数据库连接串名</param>
        /// <param name="toName">指向数据库连接串名称</param>
        /// <param name="fromIndex">原来数据库连接串索引</param>
        /// <param name="toIndex">指向数据库连接串索引</param>
        public DataMappingItem(string fromName, string toName, uint fromIndex = 0, uint toIndex = 0)
        {
            this.FromName = fromName;
            this.ToName = toName;
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 原来数据库连接串名
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 原来数据库连接串索引
        /// </summary>
        public uint FromIndex { get; set; } = 0;
        /// <summary>
        /// 指向数据库连接串名称
        /// </summary>
        public string ToName { get; set; }
        /// <summary>
        /// 指向数据库连接串索引
        /// </summary>
        public uint ToIndex { get; set; } = 0;
        #endregion

        #region 方法

        #endregion
    }
}