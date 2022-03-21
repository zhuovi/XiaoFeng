using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-03-16 11:48:25                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 忽略空节点
    /// </summary>
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field| AttributeTargets.Class| AttributeTargets.Struct)]
    public class OmitEmptyNodeAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public OmitEmptyNodeAttribute()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}