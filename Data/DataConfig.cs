using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
*  Author : jacky                                              *
*  QQ : 7092734                                                *
*  Email : jacky@fayelf.com                                    *
*  Site : www.fayelf.com                                       *
*  Create Time : 2021/4/13 11:01:38                            *
*  Version : v 1.0.0                                           *
*  CLR Version : 4.0.30319.42000                               *
****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库转换操作类
    /// </summary>
    public class DataConfig
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataConfig() { }
        #endregion

        #region 属性
        /// <summary>
        /// 映射对象
        /// </summary>
        internal static readonly ConcurrentDictionary<string, Item> Map = new ConcurrentDictionary<string, Item>();
        #endregion

        #region 方法
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">项</param>
        /// <returns></returns>
        public static async Task AddOrUpdate(Item item) => await Task.Run(() => Map.AddOrUpdate(item.FromName + "-" + item.FromIndex, item, (k, v) => v));
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">项目名称</param>
        /// <returns></returns>
        public static async Task<Boolean> Remove(string key) => await Task.Run(() => Map.TryRemove(key, out _));
        /// <summary>
        /// 获取数据项目
        /// </summary>
        /// <param name="key">名称</param>
        /// <returns></returns>
        public static async Task<Item> Get(string key) => await Task.Run(() => Map.TryGetValue(key, out var val) ? val : null);
        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public static async Task Clear() => await Task.Run(() => Map.Clear());
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static async Task<Boolean> ContainsKey(string key) => await Task.Run(() => Map.ContainsKey(key));
        #endregion

        #region 子项类
        /// <summary>
        /// 子项类
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 无参构造器
            /// </summary>
            public Item() { }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="fromName">原来数据库连接串名</param>
            /// <param name="toName">指向数据库连接串名称</param>
            /// <param name="fromIndex">原来数据库连接串索引</param>
            /// <param name="toIndex">指向数据库连接串索引</param>
            public Item(string fromName, string toName, uint fromIndex = 0, uint toIndex = 0)
            {
                this.FromName = fromName;
                this.ToName = toName;
                this.FromIndex = fromIndex;
                this.ToIndex = toIndex;
            }

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
        }
        #endregion
    }
}