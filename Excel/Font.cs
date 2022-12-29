using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-16 09:27:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 字体样式
    /// </summary>
    public class Font
    {
        #region 构造器
        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="name">字体名称</param>
        /// <param name="size">字体大小</param>
        /// <param name="color">字体颜色</param>
        public Font(string name, int size, string color)
        {
            this.Name = name;
            this.Size = size;
            this.Color = color;
        }
        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="name">字体名称</param>
        /// <param name="size">字体大小</param>
        /// <param name="color">字体颜色</param>
        /// <param name="charset">字符集</param>
        /// <param name="scheme">方案</param>
        /// <param name="bold">是否加粗</param>
        /// <param name="italic">是否斜体</param>
        /// <param name="underline">Underline</param>
        public Font(string name, int size, string color, string charset, string scheme, bool bold, bool italic, bool underline) : this(name, size, charset)
        {
            Scheme = scheme;
            Color = color;
            Bold = bold;
            Italic = italic;
            Underline = underline;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 字体名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 字符集
        /// </summary>
        public string Charset { get; set; }
        /// <summary>
        /// 方案
        /// </summary>
        public string Scheme { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 是否加粗
        /// </summary>
        public Boolean Bold { get; set; }
        /// <summary>
        /// 是否斜体
        /// </summary>
        public Boolean Italic { get; set; }
        /// <summary>
        /// 是否有下划线
        /// </summary>
        public Boolean Underline { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}