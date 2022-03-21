#if NETCORE
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
#endif
namespace XiaoFeng.Web
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public static class HttpContext
    {
#if NETFRAMEWORK
        /// <summary>
        /// 当前请求对象
        /// </summary>
        public static System.Web.HttpContext Current => System.Web.HttpContext.Current;
#else
        /// <summary>
        /// 应用程式建立器
        /// </summary>
        public static IApplicationBuilder App { get; set; }
        /// <summary>
        /// 访问器
        /// </summary>
        private static IHttpContextAccessor _contextAccessor;// => App.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        /// <summary>
        /// 当前上下文
        /// </summary>
        public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor?.HttpContext;
        /// <summary>
        /// 配置当前HttpContext
        /// </summary>
        /// <param name="contextAccessor">访问器</param>
        public static void Configure(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        /// <summary>
        /// 配置当前应用程式建立器
        /// </summary>
        /// <param name="app">应用程式建立器</param>
        public static void Configure(IApplicationBuilder app)
        {
            App = app;
        }
#endif
    }
#if !NETFRAMEWORK
    /// <summary>
    /// 请求对象
    /// </summary>
    public class HttpContextX
    {
        #region 构造器
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="accessor">访问器</param>
        public HttpContextX(IHttpContextAccessor accessor)
        {
            _contextAccessor = accessor;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 访问器
        /// </summary>
        private IHttpContextAccessor _contextAccessor;
        /// <summary>
        /// 访问器
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor => _contextAccessor;
        #endregion

        #region 方法
        #endregion
    }
#endif
}