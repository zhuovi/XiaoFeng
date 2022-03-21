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
*  Create Time : 2021-06-19 下午 10:06:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// GeoRadiusModel 类说明
    /// </summary>
    public class GeoRadiusModel:GeoModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public GeoRadiusModel()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 距离
        /// </summary>
        public double Dist { get; set; }
        /// <summary>
        /// Hash值
        /// </summary>
        public string Hash { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}