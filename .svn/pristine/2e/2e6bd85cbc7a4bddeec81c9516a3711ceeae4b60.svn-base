using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.IO;
/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/25 17:27:58          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// 目录信息
    /// </summary>
    public class Catalog
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Catalog()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 全路径
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension { get => FileHelper.GetExtension(this.Name); }
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public FileAttribute Attribute { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}