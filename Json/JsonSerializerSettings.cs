using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Json格式设置
    /// </summary>
    public class JsonSerializerSetting
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonSerializerSetting() { }
        #endregion

        #region 属性
        /// <summary>
        /// Guid格式
        /// </summary>
        public string GuidFormat { get; set; } = "D";
        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
        /// <summary>
        /// 是否格式化
        /// </summary>
        public bool Indented { get; set; } = false;
        /// <summary>
        /// 枚举值
        /// </summary>
        public EnumValueType EnumValueType { get; set; } = 0;
        /// <summary>
        /// 解析最大深度
        /// </summary>
        public int MaxDepth { get; set; } = 28;
        /// <summary>
        /// 是否写注释
        /// </summary>
        public bool IsComment { get; set; } = false;
        /// <summary>
        /// 忽略大小写 key值统一变为小写
        /// </summary>
        public bool IgnoreCase { get; set; } = false;
        /// <summary>
        /// 忽略空节点
        /// </summary>
        public bool OmitEmptyNode { get; set; } = false;
        /// <summary>
        /// 忽略自定义节点
        /// </summary>
        public bool IgnoreJsonElement { get; set; } = false;
        /// <summary>
        /// 长整型数字序列化成字符串（超过9007199254740992(2的53次方)的数字则前端会把后边数字给变成0）
        /// </summary>
        public bool LongSerializeString { get; set; } = false;
        #endregion
    }
}