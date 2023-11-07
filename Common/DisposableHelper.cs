using System;
using System.Collections;
using System.Collections.Generic;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 销毁助手扩展方法
    /// </summary>
    public static class DisposableHelper
    {
        /// <summary>尝试销毁对象，如果有<see cref="IDisposable"/>则调用</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Object TryDispose(this Object obj)
        {
            if (obj == null) return obj;

            // 列表元素销毁
            if (obj is IEnumerable)
            {
                /*对于枚举成员，先考虑添加到列表，再逐个销毁，避免销毁过程中集合改变*/
                if (!(obj is IList list))
                {
                    list = new List<Object>();
                    foreach (var item in (obj as IEnumerable))
                        if (item is IDisposable) list.Add(item);
                }
                foreach (var item in list)
                {
                    if (item is IDisposable)
                    {
                        try
                        {
                            // 只需要释放一层，不需要递归
                            // 因为一般每一个对象负责自己内部成员的释放
                            (item as IDisposable).Dispose();
                        }
                        catch { }
                    }
                }
            }
            // 对象销毁
            if (obj is IDisposable)
            {
                try
                {
                    (obj as IDisposable).Dispose();
                }
                catch { }
            }
            return obj;
        }
    }
}