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
                LoginUrl = "/Home/Index",                   //��¼ҳ��
                TokenServerUrl = "http://172.21.168.5:8010"//��Ȩ�����ַ
            });
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //ע��Session������ӿ�
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //ע��sqlsugar
            ServiceExtension.RegisterAssembly(services, "Advantech.Service");   //����ע��
            ServiceExtension.RegisterAssembly(services, "Advantech.Repository");//�ֿ�ע��

            services.AddTransient<IHttpRequest, HttpRequestHelper>();
            services.AddSession();       //����ʹ��
            services.AddHttpClientFactoryHelper();//http���󹤳�ע��
            services.AddInitDataTable();//��ʼ���������ݿ��
            //services.AddSingleton<UserAuthorizeAttribute>();//ע���û�Ȩ�޹�����

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
                //404ҳ��
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
           

            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();         //����ʹ��
            //app.UseMiddleware<UserAuthorizeMiddleware>();//�м������
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
