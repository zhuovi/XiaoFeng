using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-10 11:32:11                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库映射操作类
    /// </summary>
    public class DataMapping : EntityBase
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataMapping() {
            if (OS.Platform.IsWebForm)
            {
                var value = Web.HttpCookie.Get(TOKEN_KEY);
                if (value.IsNotNullOrEmpty())
                    this.Items = value.JsonToObject<List<Item>>();
                else
                {
                    if (Web.HttpContext.Current != null)
                    {
                        List<Item> val;
#if NETFRAMEWORK
                        val = Web.HttpContext.Current.Session?[TOKEN_KEY] as List<Item>;
#else
                        try
                        {
                            if (Web.HttpContext.Current.Session.TryGetValue(TOKEN_KEY, out var _val))
                            {
                                val = _val.GetString().JsonToObject<List<Item>>();
                            }
                            else val = null;
                        }
                        catch { val = null; }
#endif
                        if (val != null)
                            this.Items = val;
                    }
                }
            }
        }
#endregion

#region 属性
        /// <summary>
        /// 集合
        /// </summary>
        private List<Item> Items { get; set; } = new List<Item>();
        /// <summary>
        /// Token key
        /// </summary>
        private const string TOKEN_KEY = "_FAYELF_DATA_MAPPING";
        /// <summary>
        /// 是否为空
        /// </summary>
        public Boolean IsEmpty => this.Items == null || this.Items.Count == 0;
#endregion

#region 方法
        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            if (OS.Platform.IsWebForm)
            {
                if (this.Items == null || this.Items.Count == 0)
                {
                    Web.HttpCookie.Remove(TOKEN_KEY);
                    Web.HttpContext.Current.Session.Remove(TOKEN_KEY);
                    return;
                }
                Web.HttpCookie.Set(TOKEN_KEY, this.Items.ToJson(), true);
#if NETFRAMEWORK
                Web.HttpContext.Current.Session.Add(TOKEN_KEY, this.Items);
#else
                Web.HttpContext.Current.Session.Set(TOKEN_KEY, this.Items.ToJson().GetBytes());
#endif
            }
        }
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">子项</param>
        public void Add(Item item)
        {
            if (this.Items == null) this.Items = new List<Item>();
            if (this.Contains(item))
            {
                var index = this.Items.FindIndex(a => a.FromName == item.FromName && a.FromIndex == item.FromIndex);
                this.Items[index] = item;
            }
            else
                this.Items.Add(item);
            this.Save();
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="fromName">原来数据库连接串名</param>
        /// <param name="toName">指向数据库连接串名称</param>
        /// <param name="fromIndex">原来数据库连接串索引</param>
        /// <param name="toIndex">指向数据库连接串索引</param>
        public void Add(string fromName, string toName, uint fromIndex = 0, uint toIndex = 0) => this.Add(new Item
        {
            FromName = fromName,
            ToName = toName,
            FromIndex = fromIndex,
            ToIndex = toIndex
        });
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns>列表</returns>
        public List<Item> Get() => this.Items;
        /// <summary>
        /// 获取子项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public Item Get(int index)
        {
            if (this.Items == null || this.Items.Count == 0 || this.Items.Count <= index) return null;
            return this.Items[index];
        }
        /// <summary>
        /// 查找是否有映射
        /// </summary>
        /// <param name="fromName">原来名称</param>
        /// <param name="fromIndex">原来索引</param>
        /// <returns></returns>
        public Item Get(string fromName, uint fromIndex = 0) => this.Items.Find(a => a.FromName == fromName && a.FromIndex == fromIndex);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="item">子项</param>
        /// <returns></returns>
        public Boolean Contains(Item item)
        {
            if (this.Items == null || this.Items.Count == 0) return false;
            return this.Get(item.FromName, item.FromIndex) != null;
        }
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            if (this.Items != null && this.Items.Count > 0)
            {
                this.Items.Clear(); this.Save();
                Web.HttpContext.Current.Session.Remove(TOKEN_KEY);
            }
        }
        /// <summary>
        /// 移除子项
        /// </summary>
        /// <param name="index">索引</param>
        public void Remove(int index)
        {
            if (this.Items != null && this.Items.Count > index)
            {
                this.Items.RemoveAt(index); this.Save();
            }
        }
        /// <summary>
        /// 清空
        /// </summary>
        public static void Remove() => new DataMapping().Clear();
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