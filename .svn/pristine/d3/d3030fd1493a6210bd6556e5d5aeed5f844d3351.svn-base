using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;

namespace XiaoFeng.Model
{
    /// <summary>
    /// 脏数据操作类
    /// </summary>
    public class DirtyCollection : Disposable, IEnumerable<string>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DirtyCollection() { this.Data = new ConcurrentBag<string>(); }
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="collection">集合</param>
        public DirtyCollection(IEnumerable<string> collection)
        {
            this.Data = new ConcurrentBag<string>(collection);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据
        /// </summary>
        private ConcurrentBag<string> Data { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">项</param>
        public void Add(string item)
        {
            if (item.IsNullOrEmpty()) return;
            if (this.Data == null)
                this.Data = new ConcurrentBag<string>();
            if (!this.Contains(item))
                this.Data.Add(item);
        }
        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="item">项</param>
        /// <returns></returns>
        public Boolean Remove(string item)
        {
            if (item.IsNullOrEmpty()) return false;
            if (this.Data == null)
            {
                this.Data = new ConcurrentBag<string>();
                return false;
            }
            else if (this.Data.IsEmpty)
                return false;
            if (this.Contains(item))
            {
                var list = this.Data.ToList();
                list.Remove(item);
                this.Data = new ConcurrentBag<string>();
                list.Each(a =>
                {
                    this.Data.Add(a);
                });
                return true;
            }
            return false;
        }
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            if (this.Data == null)
            {
                this.Data = new ConcurrentBag<string>(); return;
            }
            if (this.Data.IsEmpty) return;
            var val = this.Data.GetEnumerator();
            this.Remove(val.Current);
            while (val.MoveNext())
            {
                this.Remove(val.Current);
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count => this.Data.Count;
        /// <summary>
        /// 是否为空
        /// </summary>
        public Boolean IsEmpty => this.Data.IsEmpty;
        /// <summary>
        /// 是否存在项
        /// </summary>
        /// <param name="item">项</param>
        /// <returns></returns>
        public Boolean Contains(string item) => this.Data.Contains(item);
        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public string[] ToArray() => this.Data.ToArray();
        /// <summary>
        /// 转换为List
        /// </summary>
        /// <returns></returns>
        public List<string> ToList() => this.Data.ToList();
        #endregion

        #region 返回一个循环访问集合的枚举器
        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Data).GetEnumerator();
        }
        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)Data).GetEnumerator();
        }
        #endregion
    }
}