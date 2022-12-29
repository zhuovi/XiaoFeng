using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XiaoFeng.Excel.Model;
using System.Xml.XPath;
using System.Xml;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 11:56:49                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 数据表
    /// </summary>
    public class Sheet
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Sheet()
        {

        }
        /// <summary>
        /// 读取数据表
        /// </summary>
        /// <param name="name">表名</param>
        /// <param name="id">id</param>
        /// <param name="hidden">是否隐藏</param>
        public Sheet(string name, string id, Boolean hidden)
        {
            this.Name=name;
            this.Id = id;
            this.Hidden=hidden;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据本
        /// </summary>
        public Workbook Workbook { get; set; }
        /// <summary>
        /// 数据表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public Boolean Hidden { get; set; }
        /// <summary>
        /// 数据表ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 数据表路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 所有行
        /// </summary>
        private List<Row> CurrentRows { get; set; } = new List<Row>();
        /// <summary>
        /// 所有列
        /// </summary>
        private List<Column> CurrentColumns { get; set; } = new List<Column>();
        /// <summary>
        /// 页面边距
        /// </summary>
        public PageMargin PageMargins { get; set; }
        /// <summary>
        /// 合并单元格
        /// </summary>
        public List<MergeCell> MergeCells { get; set; } = new List<MergeCell>();
        /// <summary>
        /// 激活单元
        /// </summary>
        public CellLocation ActiveCell { get; set; }
        /// <summary>
        /// 开始单元
        /// </summary>
        public CellLocation BeginCell { get; set; }
        /// <summary>
        /// 结束单元
        /// </summary>
        public CellLocation EndCell { get; set; }
        /// <summary>
        /// 默认高
        /// </summary>
        public double DefaultRowHeight { get; set; }
        /// <summary>
        /// 默认宽
        /// </summary>
        public double DefaultColumnWidth { get; set; }
        /// <summary>
        /// 大纲行数
        /// </summary>
        public int OutlineLevelRow { get; set; }
        /// <summary>
        /// 大纲列数
        /// </summary>
        public int OutlineLevelColumn { get; set; }
        /// <summary>
        /// 是否加载
        /// </summary>
        private Boolean IsLoad { get; set; }
        /// <summary>
        /// 页面设置
        /// </summary>
        public PageSetup PageSetup { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 打开数据表
        /// </summary>
        public void Open()
        {
            if (this.Path.IsNullOrEmpty()) return;

            using (var archive = ZipFile.OpenRead(this.Workbook.FilePath))
            {
                var entry = archive.Entries.FirstOrDefault(e => e.FullName.EqualsIgnoreCase(this.Path));
                XDocument document = XDocument.Load(entry.Open());
                XElement root = document.Root;
                if (root.IsEmpty) return;
                var hiddenColumns = GetHiddenColumns(root);

                var sheetData = root.GetXElement("sheetData");
                var BeginEndNode = root.GetXElement("dimension");
                if(BeginEndNode!=null && BeginEndNode.HasAttributes)
                {
                    var BeginEndValue = BeginEndNode.GetXAttribute<string>("ref");
                    if (BeginEndValue.IsNotNullOrEmpty())
                    {
                        var vals = BeginEndValue.Split(':');
                        this.BeginCell = new CellLocation(vals[0]);
                        this.EndCell = new CellLocation(vals[1]);
                    }
                }
                var Outline = root.GetXElement("sheetFormatPr");
                if (Outline != null)
                {
                    this.DefaultColumnWidth = Outline.GetXAttribute<double>("defaultColWidth");
                    this.DefaultRowHeight = Outline.GetXAttribute<double>("defaultRowHeight");
                    this.OutlineLevelRow = Outline.GetXAttribute<int>("outlineLevelRow");
                    this.OutlineLevelColumn = Outline.GetXAttribute<int>("outlineLevelCol");
                }
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace("ns", root.GetDefaultNamespace().NamespaceName);
                var ActiveCellNode = root.XPathSelectElement("ns:sheetViews/ns:sheetView[@workbookViewId='0']/ns:selection", nsmgr);
                if(ActiveCellNode != null)
                {
                    var v = ActiveCellNode.GetXAttribute<string>("activeCell");
                    if (v.IsNotNullOrEmpty()) this.ActiveCell = new CellLocation(v);
                }
                var rows = sheetData.GetXElements("row");
                if (rows!=null)
                {
                    rows.Each(r =>
                    {
                        var colIndexs = r.GetXAttribute<string>("spans");
                        int minIndex = 0, maxIndex = 0;
                        if (colIndexs.IsNotNullOrEmpty())
                        {
                            var spans = colIndexs.Split(':');
                            minIndex = spans[0].ToCast<int>();
                            maxIndex = spans[1].ToCast<int>();
                        }
                        var row = new Row(r.GetXAttribute<int>("r"), r.GetXAttribute<double>("ht"), minIndex, maxIndex, r.GetXAttribute<int>("hidden") > 0);
                        var cells = r.GetXElements("c");
                        if (cells!=null)
                        {
                            cells.Each(c =>
                            {
                                var CellIndexs = c.GetXAttribute<string>("r");
                                if (CellIndexs.IsNullOrEmpty()) return;
                                var val = c.GetXElement("v");
                                var cell = new Cell
                                {
                                    Row = row,
                                    Location = new CellLocation(CellIndexs)
                                };
                                var cellStyleIndex = c.GetXAttribute("s", -1);
                                if (cellStyleIndex > -1)
                                {
                                    cell.Format = this.Workbook.NumberFormats[cellStyleIndex];
                                }
                                var t = c.GetXAttribute<string>("t");
                                cell.Value = new CellValue(t == "s" ? val == null ? "" : this.Workbook.SharedStrings[val.Value.ToCast<int>()] : val?.Value, cell.Format);
                                var column = this.GetColumn(cell.Location.ColumnIndex);
                                column.Hidden = hiddenColumns.Contains(column.Index);
                                column.Sheet = this;
                                cell.Column = column;
                                cell.Row = row;
                                row.Sheet = this;
                                row.AddCell(cell);
                                column.AddCell(cell);

                                this.AddColumn(column);
                            });
                        }
                        this.AddRow(row);
                    });
                }
                var margeCells = root.GetXElement("mergeCells");
                if (margeCells != null)
                {
                    var marges = margeCells.GetXElements("mergeCell");
                    if (marges != null)
                        marges.Each(m =>
                        {
                            var v = m.GetXAttribute<string>("ref");
                            if (v.IsNotNullOrEmpty())
                            {
                                var vs = v.Split(':');
                                this.MergeCells.Add(new MergeCell(new CellLocation(vs[0]), new CellLocation(vs[1])));
                            }
                        });
                }

                var margin = root.GetXElement("pageMargins");
                if (margin != null)
                {
                    this.PageMargins = new PageMargin()
                    {
                         Top = margin.GetXAttribute<double>("top"),
                         Left = margin.GetXAttribute<double>("left"),
                         Bottom = margin.GetXAttribute<double>("bottom"),
                         Right = margin.GetXAttribute<double>("right"),
                         Header = margin.GetXAttribute<double>("header"),
                         Footer = margin.GetXAttribute<double>("footer"),
                    };
                }

                var pageSet = root.GetXElement("pageSetup");
                if (pageSet != null)
                {
                    this.PageSetup = new PageSetup
                    {
                        PaperSize = pageSet.GetXAttribute<int>("paperSize"),
                        Orientation = pageSet.GetXAttribute<string>("orientation")
                    };
                }
            }
            this.IsLoad = true;
        }
        /// <summary>
        /// 获取隐藏列
        /// </summary>
        /// <param name="root">根节点</param>
        /// <returns>隐藏列集合</returns>
        private List<int> GetHiddenColumns(XElement root)
        {
            var list = new List<int>();
            var cols = root.GetXElement("cols");
            if (cols == null) return list;
            cols.GetXElements("col").Each(e =>
            {
                var hide = e.GetXAttribute<int>("hidden")>0;
                if (!hide) return;
                var min = e.GetXAttribute<int>("min");
                var max = e.GetXAttribute<int>("max");
                for (var i = min; i < max; i++) list.Add(i);
            });
            return list;
        }
        /// <summary>
        /// 行
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <returns></returns>
        public Row Row(int rowIndex) => this.CurrentRows.SingleOrDefault(r => r.Index == rowIndex && (this.Workbook.Option.IncludeHiddenRow||r.Hidden == false));
        /// <summary>
        /// 列
        /// </summary>
        /// <param name="columnIndex">列索引</param>
        /// <returns></returns>
        public Column Column(int columnIndex) => this.CurrentColumns.SingleOrDefault(c => c.Index == columnIndex && (this.Workbook.Option.IncludeHiddenColumn||c.Hidden == false));
        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="name">名称 如 A1 B2</param>
        /// <returns></returns>
        public Cell Cell(string name)
        {
            if (name.IsNotMatch(@"^[a-z]+\d+$")) return null;
            var values = name.GetMatchs(@"^(?<a>[a-z]+)(?<b>\d+)$");
            if(values == null ||!values.ContainsKey("a") || !values.ContainsKey("b")) return null;
            return Cell(values["b"].ToCast<int>(), Common.GetColumnIndex(values["a"]));
        }
        /// <summary>
        /// 单元格
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <returns></returns>
        public Cell Cell(int rowIndex, int columnIndex)
        {
            var cells = this.CurrentRows.SelectMany(r => r.Cells);
            var cell = (from c in cells where c.Row.Index == rowIndex && c.Row.Hidden == false && c.Column.Index == columnIndex && (this.Workbook.Option.IncludeHiddenColumn||c.Column.Hidden == false) select c).SingleOrDefault();
            return cell;
        }
        /// <summary>
        /// 所有单元格
        /// </summary>
        public IEnumerable<Cell> Cells
        {
            get
            {
                return this.Workbook.Option.IncludeHiddenRow ? this.CurrentRows.Where(a => a.Cells != null).SelectMany(r => r.Cells) : this.CurrentRows.Where(r => r.Cells != null && r.Hidden == false).SelectMany(r => r.Cells);
            }
        }

        /// <summary>
        /// 所有行
        /// </summary>
        public IEnumerable<Row> Rows
        {
            get
            {
                if (this.CurrentRows == null || this.CurrentRows.Count == 0) return null;
                return this.Workbook.Option.IncludeHiddenRow ? this.CurrentRows.OrderBy(r => r.Index) : this.CurrentRows.Where(r => r.Hidden == false).OrderBy(r => r.Index);
            }
        }
        /// <summary>
        /// 所有列
        /// </summary>
        public IEnumerable<Column> Columns
        {
            get
            {
                if (this.CurrentColumns == null || this.CurrentColumns.Count == 0) return null;
                return this.Workbook.Option.IncludeHiddenColumn?this.CurrentColumns.OrderBy(c => c.Index): this.CurrentColumns.Where(c => c.Hidden == false).OrderBy(c => c.Index);
            }
        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="row">行</param>
        public void AddRow(Row row)
        {
            var Row = (from r in this.CurrentRows where r.Index == row.Index select r).SingleOrDefault();
            if (Row == null)
            {
                this.CurrentRows.Add(row);
            }
        }
        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="column">列</param>
        public void AddColumn(Column column)
        {
            var Column = (from c in this.CurrentColumns where c.Index == column.Index select c).SingleOrDefault();
            if (Column == null)
            {
                this.CurrentColumns.Add(column);
            }
        }
        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="columnIndex">列索引</param>
        /// <returns></returns>
        private Column GetColumn(int columnIndex)
        {
            return this.CurrentColumns.SingleOrDefault(c => c.Index == columnIndex) ?? new Column(columnIndex);
        }
        #endregion
    }
}