using System;
using XiaoFeng.Net;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
*  Author : jacky                                              *
*  QQ : 7092734                                                *
*  Email : jacky@fayelf.com                                    *
*  Site : www.fayelf.com                                       *
*  Create Time : 2021-05-28 16:05:45                           *
*  Version : v 1.0.0                                           *
*  CLR Version : 4.0.30319.42000                               *
****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public partial class PrototypeHelper
    {
        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="e">枚举</param>
        /// <param name="valueType">类型</param>
        /// <returns></returns>
        public static object GetValue(this Enum e, EnumValueType valueType = EnumValueType.Name)
        {
            switch (valueType)
            {
                case EnumValueType.Value: return e.ToCast<int>();
                case EnumValueType.Name: return e.ToString();
                case EnumValueType.Description: return e.GetDescription();
                default: return e.ToString();
            }
        }
        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <param name="e">枚举</param>
        /// <returns></returns>
        public static string GetEnumName(this Enum e)
        {
            var f = e.GetType().GetField(e.ToString());
            if (f != null && f.IsDefined(typeof(EnumNameAttribute), false))
                return f.GetEnumName(false);
            return e.ToString();
        }
        /// <summary>
        /// 获取指定属性或事件的端口
        /// </summary>
        /// <param name="e">类型</param>
        /// <returns></returns>
        public static int GetPort(this Enum e)
        {
            var f = e.GetType().GetField(e.ToString());
            if (f != null && f.IsDefined(typeof(PortAttribute), false))
                return f.GetPort(false);
            return (int)f.GetValue(e);
        }
    }
}