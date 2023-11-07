/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/20 14:11:52                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// XML节点类型
    /// </summary>
    public enum XmlType
    {
        #region 属性
        /// <summary>
        /// Null
        /// </summary>
        None = 0,
        /// <summary>
        /// 元素 (例如， item )
        /// </summary>
        Element = 1,
        /// <summary>
        /// 属性 (例如， id='123' )
        /// </summary>
        Attribute = 2,
        /// <summary>
        /// 节点的文本内容
        /// </summary>
        Text = 3,
        /// <summary>
        ///  CDATA 节 (例如， &lt;![CDATA[my escaped text]]&gt; )
        /// </summary>
        CDATA = 4,
        /// <summary>
        /// 对实体的引用
        /// </summary>
        EntityReference = 5,
        /// <summary>
        /// 实体声明 (例如， &lt;!ENTITY...&gt; )
        /// </summary>
        Entity = 6,
        /// <summary>
        /// 处理指令 (例如， &lt;?pi test?&gt; )
        /// </summary>
        ProcessingInstruction = 7,
        /// <summary>
        /// 注释 (例如， &lt;!-- my comment --&gt; )
        /// </summary>
        Comment = 8,
        /// <summary>
        /// 文档提供的对象，作为文档树的根访问整个 XML 文档
        /// </summary>
        Document = 9,
        /// <summary>
        /// 文档类型声明中，由以下标记 (例如， &lt;!DOCTYPE...&gt; )
        /// </summary>
        DocumentType = 10,
        /// <summary>
        /// 将文档片段
        /// </summary>
        DocumentFragment = 11,
        /// <summary>
        /// 在文档类型声明中的表示法 (例如， &lt;!NOTATION...&gt; )
        /// </summary>
        Notation = 12,
        /// <summary>
        /// 标记之间的空白区域
        /// </summary>
        Whitespace = 13,
        /// <summary>
        /// 在混合内容模型或内的空格中标记之间空白区域 xml:space="preserve" 作用域
        /// </summary>
        SignificantWhitespace = 14,
        /// <summary>
        /// 结束元素标记 (例如， &lt;/item&gt; )
        /// </summary>
        EndElement = 15,
        /// <summary>
        /// 返回当 XmlReader 到达实体替换为调用的结果末尾 System.Xml.XmlReader.ResolveEntity
        /// </summary>
        EndEntity = 16,
        /// <summary>
        /// XML 声明 (例如，&lt;?xml version='1.0'?&gt; )
        /// </summary>
        XmlDeclaration = 17,
        /// <summary>
        /// 数组
        /// </summary>
        Array = 18,
        /// <summary>
        /// 对象
        /// </summary>
        Object = 19
        #endregion
    }
}