using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advantech.CoreExtention;
using Advantech.CoreExtention.Web;
using Advantech.Entity;
using Advantech.ProgramScheduleService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProgramScheduleService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            GlobalParameter._APIURL = Configuration.GetValue<string>("_host");
            GlobalParameter._AGENTURL = Configuration.GetValue<string>("_agentUrl");
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //注入Session工厂类接口
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //注入sqlsugar
            ServiceExtension.RegisterAssembly(services,"Advantech.Service");   //服务注入
            ServiceExtension.RegisterAssembly(services, "Advantech.Repository");//仓库注入
         
            services.AddInitDataTable();//初始化服务数据库表
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("*");
                    builder.AllowAnyHeader();//允许任意头
                    builder.AllowAnyMethod();//允许任意方法
                });
            });

            services.AddControllersWithViews(o => { o.UseGeneralRoutePrefix("program"); });//默认路由前缀
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();//启用JWT授权
            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);//启用 Cors 跨域中间件

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.Map("/api/ScheduleDay", endpoints.CreateApplicationBuilder()
                //         .UseMiddleware<AuthMiddleware>().Build());
            });
        }
    }
}
