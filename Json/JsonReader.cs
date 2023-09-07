using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
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
    /// Json读取器
    /// </summary>
    public class JsonReader : Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonReader() { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region 反序列化Json成对象
        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public T Read<T>(string json)
        {
            return (T)this.Read(json, typeof(T));
        }
        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public object Read(string json, Type type)
        {
            var jsonValue = JsonParser.ParseValue(new JsonData(json));
            if (jsonValue.IsNullOrEmpty()) return null;
            return jsonValue.ToObject(type);
        }
        #endregion
        #endregion
    }
}