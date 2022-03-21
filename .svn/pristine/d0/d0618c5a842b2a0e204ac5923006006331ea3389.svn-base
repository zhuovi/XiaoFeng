using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Json;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-08-27 09:28:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存数据模型
    /// </summary>
    public class CacheModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CacheModel()
        {
            
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否为空
        /// </summary>
        [Json.JsonIgnore]
        public Boolean IsEmpty { get; set; } = false;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 缓存时长
        /// </summary>
        public double? ExpiresIn { get; set; }
        /// <summary>
        /// 是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）
        /// </summary>
        public Boolean IsSliding { get; set; } = false;
        /// <summary>
        /// 关联文件
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 对象类型
        /// </summary>
        public Type ObjectType { get; set; }
        /// <summary>
        /// 缓存值
        /// </summary>
        public object Value { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            this.ObjectType = this.Value.GetType();
            if (this.ObjectType.Name.IsMatch(@"AnonymousType")) this.ObjectType = typeof(Dictionary<string, object>);
            return this.ToJson();
        }
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static CacheModel Parse(string value)
        {
            var model = new CacheModel();
            if (value.IsNullOrEmpty()) model.IsEmpty = true;
            model = value.JsonToObject<CacheModel>();
            if (model == null) return model;
            if (model.ObjectType.Name.IsMatch(@"Dictionary"))
                model.Value = (Dictionary<string, JsonValue>)model.Value;
            else 
                model.Value = model.Value.GetValue(model.ObjectType);
            return model;
        }
        #endregion
    }
}