using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using XiaoFeng.Xml;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 11:48:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 数据工作本
    /// </summary>
    public class Workbook
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Workbook() { }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="option">配置</param>
        public Workbook(WorkbookOption option)
        {
            this.Option = option;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 工作本配置
        /// </summary>
        public WorkbookOption Option { get; set; } = new WorkbookOption();
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 基础年
        /// </summary>
        public int BaseYear { get; set; } = 1900;
        /// <summary>
        /// 数据表集
        /// </summary>
        private List<Sheet> CurrentSheets { get; set; } = new List<Sheet>();
        /// <summary>
        /// 数据表集
        /// </summary>
        public IEnumerable<Sheet> Sheets => this.CurrentSheets.Where(a => this.Option.IncludeHiddenSheet || a.Hidden == false).OrderBy(a => a.Id);
        /// <summary>
        /// 值
        /// </summary>
        public Dictionary<int, string> SharedStrings = new Dictionary<int, string>();
        /// <summary>
        /// 数据值类型
        /// </summary>
        public Dictionary<int, NumberFormat> NumberFormats = new Dictionary<int, NumberFormat>();
        #endregion

        #region 方法

        #region 打开文件
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="option">配置</param>
        public void Open(string path, WorkbookOption option = null)
        {
            if (option != null)
                this.Option = option;
            this.FilePath = path;
            this.Open();
        }
        /// <summary>
        /// 打开
        /// </summary>
        private void Open()
        {
            using (ZipArchive archive = ZipFile.OpenRead(this.FilePath))
            {
                var sheetsEntry = archive.Entries.FirstOrDefault(e => e.FullName == Common.XL_WORKBOOK_XML);
                LoadWorkbookXml(sheetsEntry);

                var sheetPathsEntry = archive.Entries.FirstOrDefault(e => e.FullName == Common.XL_RELS_WORKBOOK_XML_RELS);
                LoadWorkbookXmlRels(sheetPathsEntry);

                var sharedStringsEntry = archive.Entries.FirstOrDefault(e => e.FullName == Common.XL_SHAREDSTRINGS_XML);
                LoadSharedStrings(sharedStringsEntry);

                var stylesEntry = archive.Entries.FirstOrDefault(e => e.FullName == Common.XL_STYLES_XML);
                LoadStyles(stylesEntry);

                if (Option.LoadSheets)
                {
                    this.CurrentSheets.Each(sheet => sheet.Open());
                }
            }
        }
        #endregion

        #region 加载WorkbookXml数据
        /// <summary>
        /// 加载WorkbookXml数据
        /// </summary>
        /// <param name="entry">压缩包实体</param>
        private void LoadWorkbookXml(ZipArchiveEntry entry)
        {
            if (entry == null) return;
            var doc = XDocument.Load(entry.Open());
            var root = doc.Root;

            var pr = root.GetXElement("workbookPr");
            var date1904 = pr.Attribute("date1904");
            if (date1904 != null && date1904.Value == "1")
                this.BaseYear = 1904;
            root.GetXElement("sheets").Elements().Each(e =>
            {
                var state = e.GetXAttribute("state");
                this.CurrentSheets.Add(new Sheet(e.GetXAttribute("name").Value, e.GetXAttribute("r:id").Value, state.IsNotNullOrEmpty() && state.Value == "hidden")
                {
                    Workbook = this
                });
            });
        }
        #endregion

        #region 加载workbook资源
        /// <summary>
        /// 加载workbook资源
        /// </summary>
        /// <param name="entry">实体</param>
        private void LoadWorkbookXmlRels(ZipArchiveEntry entry)
        {
            if (entry == null) return;
            var doc = XDocument.Load(entry.Open());
            var root = doc.Root;

            root.GetXElements("Relationship").Each(e =>
            {
                var target = e.GetXAttribute("Target");
                if (!target.Value.StartsWith("worksheets")) return;
                var id = e.GetXAttribute("Id");
                var sheet = this.CurrentSheets.Find(a => a.Id == id.Value);
                if (sheet != null)
                {
                    sheet.Path = $"xl/{target.Value}";
                }
            });
        }
        #endregion

        #region 加载SharedStrings资源
        /// <summary>
        /// 加载SharedStrings资源
        /// </summary>
        /// <param name="entry">实体</param>
        private void LoadSharedStrings(ZipArchiveEntry entry)
        {
            if (entry == null) return;
            var doc = XDocument.Load(entry.Open());
            var root = doc.Root;
            int index = 0;
            root.GetXElements("si").Each(e =>
            {
                var vals = e.GetXDescendants("t");
                if (vals == null || !vals.Any()) return;
                var sbr = new StringBuilder();
                vals.Each(v => sbr.Append(v.Value));
                this.SharedStrings.Add(index++, sbr.ToString());
            });
        }
        #endregion

        #region 加载样式
        /// <summary>
        /// 加载样式
        /// </summary>
        /// <param name="entry">实体</param>
        private void LoadStyles(ZipArchiveEntry entry)
        {
            if (entry == null) return;
            var doc = XDocument.Load(entry.Open());
            var cellXfs = doc.Root.GetXElement("cellXfs");
            int index = 0;
            doc.Root.GetXElement("cellXfs").Elements().Each(e =>
            {
                var fmtId = e.GetXAttribute("numFmtId");
                if (fmtId.IsNotNullOrEmpty())
                {
                    var fmt = fmtId.Value.ToCast<int>().ToEnum<NumberFormat>();
                    this.NumberFormats.Add(index++, fmt);
                }
            });
        }
        #endregion

        #region 获取数据表
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sheetName">表名</param>
        /// <returns></returns>
        public Sheet GetSheet(string sheetName) => this.CurrentSheets.SingleOrDefault(a => a.Name.EqualsIgnoreCase(sheetName));
        #endregion

        #endregion
    }
}