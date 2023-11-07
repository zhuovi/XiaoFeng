/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/19 19:46:01                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// 转换类
    /// </summary>
    public class XmlConverter
    {
        #region 构造器
        /// <summary>
        /// 初始化属性
        /// </summary>
        public XmlConverter()
        {
            this.CanRead = true;
            this.CanWrite = true;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 能读
        /// </summary>
        public virtual bool CanRead { get; set; }
        /// <summary>
        /// 能写
        /// </summary>
        public virtual bool CanWrite { get; set; }
        #endregion
    }
}