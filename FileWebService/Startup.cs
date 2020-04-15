using System;
using System.IO;
using Advantech.CoreExtention;
using Advantech.CoreExtention.Attribute;
using Advantech.CoreExtention.Http;
using Advantech.CoreExtention.Middleware;
using Advantech.CoreExtention.Web;
using Advantech.Entity;
using Advantech.Entity.Token;
using Advantech.FileWebService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileWebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //初始化路径
            GlobalParameter._MEDIA= Configuration.GetValue<string>("_media");
            GlobalParameter._PREVIEW = Configuration.GetValue<string>("_preview");
            GlobalParameter._ICON = Configuration.GetValue<string>("_icon");
            GlobalParameter._TEMPROARY = Configuration.GetValue<string>("_temporary");
            GlobalParameter._FFMPEG= Directory.GetCurrentDirectory()+ Configuration.GetValue<string>("_ffmpeg");
            GlobalParameter._FOLDER= Configuration.GetValue<string>("_folder");
            GlobalParameter._APIURL= Configuration.GetValue<string>("_host");
            GlobalParameter._PROGRAM = Configuration.GetValue<string>("_program");
            GlobalParameter._PROGRAMPREVIEW = Configuration.GetValue<string>("_programPreview");
            //创建默认文件夹
            if (!Directory.Exists(GlobalParameter._FOLDER))
            {
                Directory.CreateDirectory(GlobalParameter._FOLDER);
            }
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // 注册Swagger服务
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "File service", Version = "v1" });
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //注入Session工厂类接口
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //注入sqlsugar
            ServiceExtension.RegisterAssembly(services, "Advantech.Service");   //服务注入
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

            services.AddHttpClientFactoryHelper();//http请求工厂注入
            services.AddSingleton<AuthServerConfig>(ServiceProvider => new AuthServerConfig()
            {
                LoginUrl = "/Home/Index",                   //登录页面
                TokenServerUrl = "http://172.21.168.5:8080"//授权服务地址,不带后缀斜杠
            });
            services.AddScoped<UserAuthorize>();//注入用户权限过滤器
            services.AddSession();//权限需要使用
            services.AddControllersWithViews(o => { o.UseGeneralRoutePrefix("file"); });//默认路由前缀
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //静态资源请求路由拦截，进入中间件处理授权
            //app.Map("/api/MediaInfo", (appBuilder) =>
            //{
            //    appBuilder.UseMiddleware<AuthMiddleware>();
            //});
            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "File service");
                c.RoutePrefix = "file";
            });
            #endregion
            app.UseFileServer();
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(GlobalParameter._FOLDER),
                RequestPath = new PathString("/BSC_Data"),
                EnableDirectoryBrowsing = true
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();//权限需要使用
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);//启用 Cors 跨域中间件

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.Map("/file/MediaInfo", endpoints.CreateApplicationBuilder()
                //         .UseMiddleware<UserAuthorizeMiddleware>().Build());
            });
        }
    }

}
