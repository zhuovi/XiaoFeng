using System;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/2/22 17:11:26                *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 忽略字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FieldIgnoreAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FieldIgnoreAttribute()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}