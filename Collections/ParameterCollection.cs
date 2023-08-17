using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-08-16 17:31:11                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Collections
{

    /// <summary>
    /// 参数集合
    /// </summary>
    public class ParameterCollection : NameValueCollection
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ParameterCollection() { }
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="collections">集合</param>
        public ParameterCollection(IEnumerable<KeyValuePair<string, string>> collections) => this.AddRange(collections);
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="collections">集合</param>
        public ParameterCollection(ICollection<KeyValuePair<string, string>> collections) => this.AddRange(collections);
        #endregion

        #region 属性
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 中指定索引处的项。
        /// </summary>
        /// <param name="index">要在集合中定位的项的从零开始的索引。</param>
        /// <returns></returns>
        public new string this[int index] => this.Get(index);
        /// <summary>
        /// 获取或设置 XiaoFeng.Collections.ParameterCollection 中具有指定键的项。
        /// </summary>
        /// <param name="name">要定位的项的 System.String 键。</param>
        /// <returns></returns>
        public new string this[string name] => this.Get(name);
        /// <summary>
        /// 排序好的数据
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> _OrderedEnumerable;
        /// <summary>
        /// 排序好的数据
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> OrderedEnumerable
        {
            get
            {
                if (this._OrderedEnumerable == null)
                    this._OrderedEnumerable = this.GetList();
                return this._OrderedEnumerable;
            }
        }
        /// <summary>
        /// 获取包含在 XiaoFeng.Collections.ParameterCollection 实例中的键/值对的数目
        /// </summary>
        public override int Count => base.Count;
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 中的所有键。
        /// </summary>
        public override string[] AllKeys => this.OrderedEnumerable == null ? Array.Empty<string>() : this.OrderedEnumerable?.Select(a => a.Key).ToArray();
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 中的所有值。
        /// </summary>
        public string[] AllValues => this.AllKeys?.Select(k => this.Get(k)).ToArray();
        /// <summary>
        /// 获取包含 System.Collections.Specialized.NameObjectCollectionBase.KeysCollection 实例中所有键的 XiaoFeng.Collections.ParameterCollection 实例。
        /// </summary>
        public override KeysCollection Keys => base.Keys;
        #endregion

        #region 方法

        #region 添加键值
        /// <summary>
        /// 将具有指定名称和值的项添加到 XiaoFeng.Collections.ParameterCollection。
        /// </summary>
        /// <param name="name">要添加的项的 System.String 键。</param>
        /// <param name="value">要添加的项的 System.String 值。</param>
        public override void Add(string name, string value)
        {
            if (name.IsNullOrEmpty()) return;
            base.Add(name, value);
            this.ClearOrderedEnumerable();
        }
        /// <summary>
        /// 将指定 System.Collections.Specialized.NameValueCollection 中的项复制到当前 XiaoFeng.Collections.ParameterCollection。
        /// </summary>
        /// <param name="c">集合</param>
        public new void Add(NameValueCollection c) { base.Add(c); this.ClearOrderedEnumerable(); }
        /// <summary>
        /// 将指定 XiaoFeng.Collections.ParameterCollection 中的项复制到当前 XiaoFeng.Collections.ParameterCollection。
        /// </summary>
        /// <param name="c"></param>
        public void Add(ParameterCollection c) { base.Add(c); this.ClearOrderedEnumerable(); }
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="collections">集合</param>
        /// <returns>ParameterCollection</returns>
        public ParameterCollection AddRange(IEnumerable<KeyValuePair<string, string>> collections)
        {
            if (collections == null) return this;
            collections.Each(a => this.Add(a.Key, a.Value));
            return this;
        }
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="collections">集合</param>
        /// <returns>ParameterCollection</returns>
        public ParameterCollection AddRange(ICollection<KeyValuePair<string, string>> collections)
        {
            if (collections == null) return this;
            collections.Each(a => this.Add(a.Key, a.Value));
            return this;
        }
        /// <summary>
        /// 设置 XiaoFeng.Collections.ParameterCollection 中某个项的值。
        /// </summary>
        /// <param name="name">要向其添加新值的项的 System.String 键。</param>
        /// <param name="value">表示要添加到指定项的新值。</param>
        public override void Set(string name, string value)
        {
            if (name.IsNullOrEmpty()) return;
            base.Set(name, value);
            this.ClearOrderedEnumerable();
        }
        /// <summary>
        /// 设置集合
        /// </summary>
        /// <param name="collections">集合</param>
        /// <returns>ParameterCollection</returns>
        public ParameterCollection SetRange(IEnumerable<KeyValuePair<string, string>> collections)
        {
            if (collections == null) return this;
            collections.Each(a => this.Add(a.Key, a.Value));
            return this;
        }
        /// <summary>
        /// 设置集合
        /// </summary>
        /// <param name="collections">集合</param>
        /// <returns>ParameterCollection</returns>
        public ParameterCollection SetRange(ICollection<KeyValuePair<string, string>> collections)
        {
            if (collections == null) return this;
            collections.Each(a => this.Add(a.Key, a.Value));
            return this;
        }
        #endregion

        #region 获取
        /// <summary>
        /// 是否存在当前键
        /// </summary>
        /// <param name="name">键</param>
        /// <returns>存在则为true 不存在则为false。</returns>
        public Boolean Contains(string name) => this.AllKeys.Contains(name);
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 中指定索引处的值，这些值已合并为一个以逗号分隔的列表。
        /// </summary>
        /// <param name="index">项的从零开始的索引，该项包含要从集合中获取的值。</param>
        /// <returns>如果找到，则为一个 System.String，包含 XiaoFeng.Collections.ParameterCollection 中指定索引处的值的列表（此列表以逗号分隔）；否则为 null。</returns>
        public override string Get(int index) => index >= this.Count ? String.Empty : this.AllValues[index];
        /// <summary>
        /// 获取与 XiaoFeng.Collections.ParameterCollection 中的指定键关联的值，这些值已合并为一个以逗号分隔的列表。
        /// </summary>
        /// <param name="name">项的 System.String 键，该项包含要获取的值。</param>
        /// <returns>如果找到，则为一个 System.String，包含与 XiaoFeng.Collections.ParameterCollection  中的指定键关联的值的列表（此列表以逗号分隔）；否则为 null。</returns>
        public override string Get(string name) => base.Get(name);
        /// <summary>
        /// 返回循环访问 XiaoFeng.Collections.ParameterCollection 的枚举数。
        /// </summary>
        /// <returns>用于 System.Collections.IEnumerator 实例的 XiaoFeng.Collections.ParameterCollection。</returns>
        public override IEnumerator GetEnumerator() => this.OrderedEnumerable.GetEnumerator();
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 中指定索引处的值。
        /// </summary>
        /// <param name="index">项的从零开始的索引，该项包含要从集合中获取的值。</param>
        /// <returns>如果找到，则为一个 System.String 数组，包含 XiaoFeng.Collections.ParameterCollection 中指定索引处的值；否则为 null。</returns>
        public override string[] GetValues(int index) => index >= this.Count ? Array.Empty<string>() : this.AllValues[index].Split(new char[] { ',' });
        /// <summary>
        /// 获取与 XiaoFeng.Collections.ParameterCollection 中的指定键关联的值。
        /// </summary>
        /// <param name="name">项的 System.String 键，该项包含要获取的值。</param>
        /// <returns>如果找到，则为一个 System.String 数组，包含与 XiaoFeng.Collections.ParameterCollection  中的指定键关联的值；否则为 null。</returns>
        public override string[] GetValues(string name) => base.GetValues(name);
        /// <summary>
        /// 移除 XiaoFeng.Collections.ParameterCollection 实例中具有指定键的项。
        /// </summary>
        /// <param name="name">要移除的项的 System.String 键。</param>
        public override void Remove(string name)
        {
            base.Remove(name);
            this.ClearOrderedEnumerable();
        }
        /// <summary>
        /// 确定指定对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns>如果指定的对象等于当前对象，则为 true；否则为 false。</returns>
        public override bool Equals(object obj) => base.Equals(obj);
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 的指定索引处的键。
        /// </summary>
        /// <param name="index">要从集合中获取的从零开始的键索引。</param>
        /// <returns>如果找到，则为一个 System.String，包含 XiaoFeng.Collections.ParameterCollection 中指定索引处的键</returns>
        public override string GetKey(int index) => index >= this.Count ? string.Empty : this.AllKeys[index];
        /// <summary>
        /// 获取 XiaoFeng.Collections.ParameterCollection 的指定索引处的项。
        /// </summary>
        /// <param name="index">要从集合中获取的从零开始的键索引。</param>
        /// <returns>如果找到，则为一个 System.String，包含 XiaoFeng.Collections.ParameterCollection 中指定索引处的项</returns>
        public string GetValue(int index) => index >= this.Count ? string.Empty : this.AllValues[index];
        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override int GetHashCode() => base.GetHashCode();

        #endregion

        #region 清空
        /// <summary>
        /// 使缓存数组无效，并将所有项从 XiaoFeng.Collections.ParameterCollection 中移除。
        /// </summary>
        public override void Clear() { base.Clear(); this.ClearOrderedEnumerable(); }
        #endregion

        #region 排序
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns>ParameterCollection</returns>
        public ParameterCollection OrderBy(Func<KeyValuePair<string, string>, string> func = null)
        {
            var list = this.GetList();
            if (list == null) return this;
            if (func == null) func = a => a.Key;
            this._OrderedEnumerable = list.OrderBy(func);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns>ParameterCollection</returns>
        public ParameterCollection OrderByDescending(Func<KeyValuePair<string, string>, string> func = null)
        {
            var list = this.GetList();
            if (list == null) return this;
            if (func == null) func = a => a.Key;
            this._OrderedEnumerable = list.OrderByDescending(func);
            return this;
        }
        #endregion

        #region 转成字典
        /// <summary>
        /// 转成字典
        /// </summary>
        /// <returns>返回字典</returns>
        public Dictionary<string, string> ToDictionary()
        {
            if (this.OrderedEnumerable == null || !this.OrderedEnumerable.Any()) return null;
            var d = new Dictionary<string, string>();
            this.OrderedEnumerable.Each(a => d.Add(a.Key, a.Value));
            return d;
        }
        /// <summary>
        /// 字典比较器
        /// </summary>
        internal class DictionaryComparer : IEqualityComparer<string>
        {
            ///<inheritdoc/>
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }
            ///<inheritdoc/>
            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
        #endregion

        #region 转换成参数字符串
        /// <summary>
        /// 转换成参数字符串
        /// </summary>
        /// <param name="isEncode">值是否编码</param>
        /// <returns>拼接好的参数字符串</returns>
        public string ToString(bool isEncode) => this.OrderedEnumerable == null ? string.Empty : (from a in this.OrderedEnumerable select $"{a.Key}={(isEncode ? a.Value.UrlEncode() : a.Value)}").Join("&");
        /// <summary>
        /// 转换成参数字符串
        /// </summary>
        /// <returns>拼接好的参数字符串</returns>
        public override string ToString() => this.ToString(false);
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> GetList()
        {
            if (this.Count == 0) return null;
            return from k in base.AllKeys select new KeyValuePair<string, string>(k, this[k]);
        }
        #endregion

        #region 清空排序缓存
        /// <summary>
        /// 清空排序缓存
        /// </summary>
        void ClearOrderedEnumerable()
        {
            if (this._OrderedEnumerable != null) this._OrderedEnumerable = null;
        }
        #endregion

        #region 强制转换
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="p">值</param>
        public static explicit operator Dictionary<string, string>(ParameterCollection p) => p.ToDictionary();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="d">值</param>
        public static implicit operator ParameterCollection(Dictionary<string, string> d) => new ParameterCollection(d);
        #endregion

        #region 释放        
        /// <summary>
        /// 析构器
        /// </summary>
        ~ParameterCollection()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}