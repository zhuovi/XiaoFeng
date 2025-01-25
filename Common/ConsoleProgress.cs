using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-01-25 14:31:34                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 控制台进度条类
    /// </summary>
    public class ConsoleProgress
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public ConsoleProgress() : this(true) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="isShowPercent">是否显示百分比</param>
        public ConsoleProgress(Boolean isShowPercent)
        {
            this.IsShowPercent = isShowPercent;
            this.Reset();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 位置
        /// </summary>
        private int Position = 0;
        /// <summary>
        /// 是否显示百分比
        /// </summary>
        public Boolean IsShowPercent { get; set; } = true;
        #endregion

        #region 方法
        /// <summary>
        /// 进度
        /// </summary>
        /// <param name="value">总进度值</param>
        public void Progress(int value)
        {
            if (value > 100) value = 100;
            Console.CursorLeft = value - 1;
            Console.Write(new string('█', value - Position));
            if (this.IsShowPercent)
            {
                Console.CursorLeft = 100;
                Console.Write($" {value:D2}%");
            }
            Position = value;
            if (value == 100)
            {
                Console.ResetColor();
                Console.WriteLine();
                Console.CursorVisible = true;
            }
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Console.WriteLine();
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(new string('█', 100));
            if (this.IsShowPercent)
                Console.Write(" 0%");
            Console.CursorLeft = 0;
            Console.ForegroundColor = ConsoleColor.Green;
            this.Position = 0;
        }
        #endregion
    }
}