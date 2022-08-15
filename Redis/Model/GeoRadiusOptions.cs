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
*  Create Time : 2021-06-19 下午 09:49:10                       *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 位置半径选项
    /// </summary>
    public class GeoRadiusOptions:GeoModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public GeoRadiusOptions()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 半径长度
        /// </summary>
        public double? Radius { get; set; }
        /// <summary>
        /// 单位类型
        /// </summary>
        public UnitType UnitType { get; set; } = UnitType.M;
        /// <summary>
        /// 在返回位置元素的同时， 将位置元素与中心之间的距离也一并返回。
        /// </summary>
        public Boolean IsWithdist { get; set; }
        /// <summary>
        /// 将位置元素的经度和维度也一并返回。
        /// </summary>
        private Boolean IsWithCoord { get; set; } = true;
        /// <summary>
        /// 以 52 位有符号整数的形式， 返回位置元素经过原始 geohash 编码的有序集合分值。 这个选项主要用于底层应用或者调试， 实际中的作用并不大。
        /// </summary>
        public Boolean IsWithHash { get; set; }
        /// <summary>
        /// 返回的记录数。
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// 查找结果根据距离类型
        /// </summary>
        public GeoSortType SortType { get; set; }
        /// <summary>
        /// 存储key
        /// </summary>
        public string StoreKey { get; set; }
        /// <summary>
        /// 存储距离 key
        /// </summary>
        public string StoreDistKey { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 转换成参数
        /// </summary>
        /// <param name="commandType">类型</param>
        /// <returns></returns>
        public virtual object[] ToArgments(CommandType commandType)
        {
            var list = new List<object>();
            if (commandType == CommandType.GEORADIUS)
            {
                list.Add(this.Longitude);
                list.Add(this.Latitude);
            }
            else
                list.Add(this.Address);
            list.Add(this.Radius);
            list.Add(this.UnitType.ToString().ToLower());
            if (this.IsWithCoord) list.Add("WITHCOORD");
            if (this.IsWithdist) list.Add("WITHDIST");
            if (this.IsWithHash) list.Add("WITHHASH");
            if (this.Count.HasValue && this.Count.Value > 0)
            {
                list.Add("COUNT");
                list.Add(this.Count);
            }
            list.Add(this.SortType.ToString());
            if (this.StoreKey.IsNotNullOrEmpty()) list.Add(this.StoreKey);
            if (this.StoreDistKey.IsNotNullOrEmpty()) list.Add(this.StoreDistKey);
            return list.ToArray();
        }
        #endregion
    }
}