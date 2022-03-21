using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security;
namespace XiaoFeng.Resources
{
    #region 加载资源
    /// <summary>
    /// 加载资源
    /// </summary>
    public class ResourceHelper
    {
        #region 构造器

        #endregion

        #region 属性
        /// <summary>
        /// 所有的资源
        /// </summary>
        static Dictionary<string, Resource> Data = new Dictionary<string, Resource>();
        /// <summary>
        /// 已加载过的dll资源
        /// </summary>
        static List<string> LoadData = new List<string>();
        #endregion

        #region 方法

        #region 加载资源
        /// <summary>
        /// 加载资源[注入到应用入口位置]
        /// </summary>
        [SecuritySafeCritical]
        public static void Load()
        {
            /*获取调用者的程序集*/
            var assembly = new StackTrace(0).GetFrame(1).GetMethod().Module.Assembly;
            /*判断程序集是否已经处理*/
            if (LoadData.Contains(assembly.FullName)) return;
            /*程序集加入已处理集合*/
            LoadData.Add(assembly.FullName);
            /*获取所有资源文件文件名*/
            var res = assembly.GetManifestResourceNames();
            res.Each(r =>
            {
                Resource resource = new Resource()
                {
                    Suffix = r.GetMatch(@"\.(?<a>.*?)$")
                };
                try
                {
                    var s = assembly.GetManifestResourceStream(r);
                    var bts = new byte[s.Length];
                    s.Read(bts, 0, (int)s.Length);
                    resource.Name = r;
                    if (r.IsMatch(@"\.dll$"))
                    {
                        Assembly ass = Assembly.Load(bts);
                        resource.Type = ResourceType.Dll;
                        resource.Data = ass;
                       r = resource.Name = ass.FullName;
                    }
                    else if (r.IsMatch(@"\.(txt|json|xml|config|log|cs|html|htm|jsp|php|cshtml|ashx|aspx|css|js|asax)$"))
                    {
                        resource.Type = ResourceType.File;
                        resource.Data = bts.GetString();
                    }
                    else if (r.IsMatch(@"\.(jpg|jpeg|bmp|png|ico|gif|icon)$"))
                    {
                        resource.Type = ResourceType.Image;
                        resource.Data = bts;
                    }
                    else
                    {
                        resource.Type = ResourceType.Byte;
                        resource.Data = bts;
                    }
                    if (!Data.ContainsKey(r)) Data.Add(r, resource);
                }
                catch (DllNotFoundException ex)
                {
                    LogHelper.Error(ex);
                }
            });
            /*绑定程序集加载失败事件(这里我测试了,就算重复绑也是没关系的)*/
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
            {
                /*获取加载失败的程序集的全名*/
                var assName = new AssemblyName(args.Name).FullName;
                /*判断Dlls集合中是否有已加载的同名程序集*/
                if (Data.TryGetValue(assName, out Resource _data) && _data != null)
                {
                    /*如果有则移除返回*/
                    Data.Remove(assName);
                    return _data.Data as Assembly;
                }
                else
                {
                    throw new DllNotFoundException(assName);
                }
            };
        }
        #endregion

        #region 获取资源
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="Name">名称</param>
        /// <returns></returns>
        public static Resource Get(string Name)
        {
            if (Data.TryGetValue(Name, out Resource data))
                return data;
            else
            {
                Data.Each(kVal =>
                {
                    if (kVal.Key.IsMatch(Name + "$")) { data = kVal.Value; return false; }
                    return true;
                });
            }
            return data;
        }
        #endregion
        
        #endregion
    }
    #endregion

    #region 资源对象
    /// <summary>
    /// 资源对象
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ResourceType Type { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; }
    }
    #endregion

    #region 资源类型
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// 文件流
        /// </summary>
        [Description("文件流")]
        Byte = 0,
        /// <summary>
        /// dll
        /// </summary>
        [Description("组件")]
        Dll = 1,
        /// <summary>
        /// 文本文件
        /// </summary>
        [Description("文本文件")]
        File = 2,
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        Image = 3
    }
    #endregion
}