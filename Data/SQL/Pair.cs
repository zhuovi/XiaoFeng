using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-01-14 20:46:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 成对操作类
    /// </summary>
    public class Pair
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Pair() { }
        /// <summary>
        /// 设置开始标识符
        /// </summary>
        /// <param name="startMark">开始标识符</param>
        public Pair(char startMark)
        {
            this.StartMark = startMark;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始标识符
        /// </summary>
        private char _StartMark;
        /// <summary>
        /// 开始标识符
        /// </summary>
        public char StartMark
        {
            get { return this._StartMark; }
            set
            {
                this._StartMark = value;
                if (RegexString.Mark.TryGetValue(value, out var end))
                    this.EndMark = end;
            }
        }
        /// <summary>
        /// 结束标识符
        /// </summary>
        public char EndMark { get; set; }
        /// <summary>
        /// 是否成对
        /// </summary>
        public Boolean IsPair { get; set; } = false;
        /// <summary>
        /// 子结点
        /// </summary>
        public List<Pair> ChildPair { get; set; }
        /// <summary>
        /// 父结点
        /// </summary>
        public Pair ParentPair { get; set; }
        #endregion
    }
}