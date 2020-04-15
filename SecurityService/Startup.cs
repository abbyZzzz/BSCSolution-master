using Advantech.CoreExtention;
using Advantech.CoreExtention.Web;
using Advantech.SecurityService.Authorize;
using Advantech.SecurityService.Rpc;
using Advantech.SecurityService.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Advantech.SecurityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // 注册Swagger服务
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Security service", Version = "v1" });
            });

            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //注入sqlsugar
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //注入Session工厂类接口
            services.AddAuthentication(Configuration);

            ServiceExtension.RegisterAssembly(services, "Advantech.Service");   //服务注入
            ServiceExtension.RegisterAssembly(services, "Advantech.Repository");//仓库注入

            services.AddScoped<IUserLoginService, UserLoginService>();          //用户登录接口对象注入
            services.AddThriftRpc();    //增加RPC访问
            services.AddInitDataTable();//初始化服务数据库表

            services.AddControllersWithViews(o => { o.UseGeneralRoutePrefix("Security"); });//默认路由前缀
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Security service");
                c.RoutePrefix = "Security";
            });
            #endregion
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();//启用JWT授权
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
