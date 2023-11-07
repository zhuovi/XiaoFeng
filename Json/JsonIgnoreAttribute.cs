using System;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-25 11:59:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Json
{
    /// <summary>
    /// 表示可序列化类的字段或属性不应进行序列化
    /// </summary>
    public sealed class JsonIgnoreAttribute : Attribute
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonIgnoreAttribute() { }
    }
}