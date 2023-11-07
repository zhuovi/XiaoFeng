using System;
using System.Collections.Generic;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-18 02:33:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 排序操作类
    /// </summary>
    public class SortOptions
    {
        /// <summary>
        /// 排序
        /// </summary>
        public SortType Sort { get; set; } = SortType.ASC;
        /// <summary>
        /// 是否按字典排序
        /// </summary>
        public Boolean ALPHA { get; set; } = false;
        /// <summary>
        /// 索引位置
        /// </summary>
        public Limit Limit { get; set; }
        /// <summary>
        /// 外部键
        /// </summary>
        public string OutKey { get; set; }
        /// <summary>
        /// 引用对象
        /// </summary>
        public List<string> GetKeys { get; set; }
        /// <summary>
        /// 存储位置
        /// </summary>
        public string Store { get; set; }
        /// <summary>
        /// 转换成参数集
        /// </summary>
        /// <returns></returns>
        public object[] ToArgments()
        {
            var list = new List<object>();
            if (this.OutKey.IsNotNullOrEmpty())
            {
                list.Add("BY");
                list.Add(OutKey);
            }
            if (this.GetKeys != null && this.GetKeys.Count > 0)
            {
                this.GetKeys.Each(k =>
                {
                    list.Add("GET");
                    list.Add(k);
                });
            }
            if (this.Limit != null)
            {
                list.Add("LIMIT");
                list.Add(this.Limit.Start);
                list.Add(this.Limit.End);
            }
            if (this.ALPHA) list.Add("ALPHA");
            var sort = this.Sort.GetDescription();
            if (sort.IsNotNullOrEmpty()) list.Add(sort);
            if (this.Store.IsNotNullOrEmpty())
            {
                list.Add("STORE");
                list.Add(this.Store);
            }
            return list.ToArray();
        }
    }
}
