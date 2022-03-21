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
*  Create Time : 2021-06-20 上午 12:08:20                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// GeoRadiusSearchOptions 类说明
    /// </summary>
    public class GeoRadiusSearchOptions:GeoRadiusOptions
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public GeoRadiusSearchOptions()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 搜索区域
        /// </summary>
        public GeoRadiusSearchBox Box { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 转换参数
        /// </summary>
        /// <returns></returns>
        public object[] ToArgments()
        {
            var list = new List<object>();
            if (this.Address.IsNotNullOrEmpty())
            {
                list.Add("FROMMEMBER");
                list.Add(this.Address);
            }
            if(this.Latitude.HasValue && this.Longitude.HasValue)
            {
                list.Add("FROMLONLAT");
                list.Add(this.Longitude);
                list.Add(this.Latitude);
            }
            if (this.Radius.HasValue)
            {
                list.Add("BYRADIUS");
                list.Add(this.Radius);
                list.Add(this.UnitType.ToString().ToLower());
            }
            if (this.Box != null)
            {
                list.Add("BYBOX");
                list.Add(this.Box.Width);
                list.Add(this.Box.Height);
                list.Add(this.Box.UnitType.ToString().ToLower());
            }
            list.Add(this.SortType.ToString());
            if (this.Count.HasValue && this.Count.Value > 0)
            {
                list.Add("COUNT");
                list.Add(this.Count);
            }
            list.Add("WITHCOORD");
            if (this.IsWithdist) list.Add("WITHDIST");
            if (this.IsWithHash) list.Add("WITHHASH");
            if (this.StoreKey.IsNotNullOrEmpty()) list.Add(this.StoreKey);
            if (this.StoreDistKey.IsNotNullOrEmpty()) list.Add(this.StoreDistKey);
            return list.ToArray();
        }
        #endregion
    }
    /// <summary>
    /// 搜索区域
    /// </summary>
    public class GeoRadiusSearchBox
    {
        /// <summary>
        /// 宽
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public UnitType UnitType { get; set; } = UnitType.M;
    }
}