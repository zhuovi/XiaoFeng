using System;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-11 13:32:39                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Collections
{
    /// <summary>
    /// 连接池子类
    /// </summary>
    public class PoolItem<T> : Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PoolItem()
        {

        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">对象</param>
        public PoolItem(T value)
        {
            this.Value = value;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 对象
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime LastTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 是否能用
        /// </summary>
        public Boolean IsWork { get; set; } = true;
        #endregion

        #region 方法

        #endregion
    }
}