using System;
using System.Text;
using System.Xml;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/19 11:40:51                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// 序列化设置
    /// </summary>
    public class XmlSerializerSetting
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XmlSerializerSetting()
        {

        }
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
        public bool Indented { get; set; } = true;
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
        public bool OmitComment { get; set; } = true;
        /// <summary>
        /// 忽略大小写 key值统一变为小写
        /// </summary>
        public bool IgnoreCase { get; set; } = false;
        /// <summary>
        /// 默认根目录节点名称
        /// </summary>
        public string DefaultRootName { get; set; }
        /// <summary>
        /// 默认编码
        /// </summary>
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 获取或设置一个值，该值指示是否 System.Xml.XmlWriter 编写 XML 内容时应移除重复的命名空间声明。 写入器的默认行为是输出写入器的命名空间解析程序中存在的所有命名空间声明。
        /// </summary>
        public NamespaceHandling NamespaceHandling { get; set; }
        /// <summary>
        /// 是否忽略输出XML声明
        /// </summary>
        public Boolean OmitXmlDeclaration { get; set; } = false;
        /// <summary>
        /// 获取或设置要用于换行符的字符串。要用于换行符的字符串。 它可以设置为任何字符串值。 但是，为了确保 XML 有效，应该只指定有效的空格字符，例如空格、制表符、回车符或换行符。 默认值是\r\n （回车符、 换行符）。
        /// </summary>
        public string NewLineChars { get; set; } = Environment.NewLine;
        /// <summary>
        /// 是否忽略数组项未指定KEY的项用节点名称代替
        /// </summary>
        public Boolean OmitArrayItemName { get; set; } = true;
        /// <summary>
        /// 是否忽略空节点
        /// </summary>
        public Boolean OmitEmptyNode { get; set; } = true;
        /// <summary>
        /// 是否忽略命名空间
        /// </summary>
        public Boolean OmitNamespace { get; set; } = true;
        /// <summary>
        /// 获取或设置一个值，该值指示是否可对特定的 <see cref="System.Xml.XmlWriter"/> 实例使用异步 <see cref="System.Xml.XmlWriter"/> 方法。
        /// </summary>
        public bool Async { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示调用 <see cref="System.Xml.XmlWriter.Close"/> 方法时 <see cref="System.Xml.XmlWriter"/> 是否也应关闭基础流或 <see cref="System.IO.TextWriter"/>。
        /// </summary>
        public bool CloseOutput { get; set; } = false;
        /// <summary>
        /// 获取或设置缩进时要使用的字符串。 在 <see cref="Indented"/> 属性设置为 true 时使用此设置。
        /// </summary>
        public string IndentChars { get; set; } = "  ";
        /// <summary>
        /// 获取或设置一个值，该值指示在调用 <see cref="System.Xml.XmlWriter.Close"/> 方法时 <see cref="System.Xml.XmlWriter"/> 是否会向所有未关闭的元素标记添加结束标记。
        /// </summary>
        public bool WriteEndDocumentOnClose { get; set; } = true;
        #endregion
    }
}