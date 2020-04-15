using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Advantech.CoreExtention;
using Advantech.CoreExtention.Attribute;
using Advantech.CoreExtention.Http;
using Advantech.CoreExtention.Middleware;
using Advantech.Entity;
using Advantech.Entity.Token;
using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;

namespace Advantech.BSCWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            GlobalParameter._AGENTURL = Configuration.GetValue<string>("_agentUrl");
            GlobalParameter._PREFIX = "UserGroup";
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllersWithViews();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSingleton<AuthServerConfig>(ServiceProvider => new AuthServerConfig()
            {
                LoginUrl = "/Home/Index",                   //登录页面
                TokenServerUrl = "http://172.21.168.5:8010"//授权服务地址
            });
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //注入Session工厂类接口
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //注入sqlsugar
            ServiceExtension.RegisterAssembly(services, "Advantech.Service");   //服务注入
            ServiceExtension.RegisterAssembly(services, "Advantech.Repository");//仓库注入

            services.AddTransient<IHttpRequest, HttpRequestHelper>();
            services.AddSession();       //测试使用
            services.AddHttpClientFactoryHelper();//http请求工厂注入
            services.AddInitDataTable();//初始化服务数据库表
            //services.AddSingleton<UserAuthorizeAttribute>();//注入用户权限过滤器

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                //404页面
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
           

            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();         //测试使用
            //app.UseMiddleware<UserAuthorizeMiddleware>();//中间件拦截
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
