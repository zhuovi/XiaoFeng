using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
namespace XiaoFeng
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
       Create Time : 2017/10/25 11:59:42
       Update Time : 2017/10/25 11:59:42
    ===================================================================
    */
    /// <summary>
    /// JSON序列化 反序列化
    /// Verstion : 1.0.0
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2017/10/25 11:59:42
    /// Update Time : 2017/10/25 11:59:42
    /// </summary>
    public class JSONConvert
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        JSONConvert() { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="json"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, string encoding = "UTF-8")
        {
            if (json.IsNullOrEmpty()) return default(T);
            using (var ms = new MemoryStream(Encoding.GetEncoding(encoding).GetBytes(json)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="t">对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string SerializerObject<T>(T t, string encoding = "UTF-8")
        {
            if (t == null) return "{}";
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(t.GetType()).WriteObject(ms, t);
                return Encoding.GetEncoding(encoding).GetString(ms.ToArray());
            }
        }
        #endregion

        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~JSONConvert() { }
        #endregion
    }
}