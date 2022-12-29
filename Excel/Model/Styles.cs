using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.Xml.Linq;
using System.Xml.XPath;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-14 10:34:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel.Model
{
    /// <summary>
    /// 样式类
    /// </summary>
    public class Styles
    {
        #region 构造器
        /// <summary>
        /// 设置实体
        /// </summary>
        public Styles(ZipArchiveEntry entry)
        {
            Entry = entry;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 实体
        /// </summary>
        private ZipArchiveEntry Entry { get; set; }
        /// <summary>
        /// 值格式
        /// </summary>
        public Dictionary<int, string> NumFmts { get; set; }
        /// <summary>
        /// 字体集
        /// </summary>
        public List<Font> Fonts { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {

        }
        #endregion
    }
}