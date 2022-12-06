using System;
using XiaoFeng.Json;
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
namespace XiaoFeng
{
    /// <summary>
    /// 扩展JSON对象
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 对象转JSON串
        /// <summary>
        /// 对象转JSON串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="formatting">Json格式设置</param>
        /// <returns></returns>
        public static string ToJson(this object o, JsonSerializerSetting formatting = null)
        {
            if (o == null) return string.Empty;
            try
            {
                return JsonParser.SerializeObject(o, formatting);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 对象转JSON串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="indented">是否格式化</param>
        /// <returns></returns>
        public static string ToJson(this object o, bool indented)
        {
            return o.ToJson(new JsonSerializerSetting { Indented = indented });
        }
        #endregion

        #region JSON串转对象
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="_">JSON串</param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string _)
        {
            if (_.IsNullOrEmpty()) return default(T);
            try
            {
                return JsonParser.DeserializeObject<T>(_);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "JSON解析对象出错,JSON字符串为:" + _);
                return default(T);
            }
        }
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <param name="_">JSON串</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object JsonToObject(this string _, Type type)
        {
            if (_.IsNullOrEmpty()) return null;
            try
            {
                return JsonParser.DeserializeObject(_, type);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "JSON解析对象出错,JSON字符串为:" + _);
                return Activator.CreateInstance(type);
            }
        }
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <param name="_">JSON串</param>
        /// <returns></returns>
        public static JsonValue JsonToObject(this string _)
        {
            return _.JsonToObject<JsonValue>();
        }
        #endregion

    }
}