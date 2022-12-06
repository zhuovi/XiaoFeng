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
    /// Json转换类
    /// </summary>
    public class JsonConverter
    {
        #region 构造器
        /// <summary>
        /// 初始化属性
        /// </summary>
        public JsonConverter()
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
