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
            //��ʼ��·��
            GlobalParameter._MEDIA= Configuration.GetValue<string>("_media");
            GlobalParameter._PREVIEW = Configuration.GetValue<string>("_preview");
            GlobalParameter._ICON = Configuration.GetValue<string>("_icon");
            GlobalParameter._TEMPROARY = Configuration.GetValue<string>("_temporary");
            GlobalParameter._FFMPEG= Directory.GetCurrentDirectory()+ Configuration.GetValue<string>("_ffmpeg");
            GlobalParameter._FOLDER= Configuration.GetValue<string>("_folder");
            GlobalParameter._APIURL= Configuration.GetValue<string>("_host");
            GlobalParameter._PROGRAM = Configuration.GetValue<string>("_program");
            GlobalParameter._PROGRAMPREVIEW = Configuration.GetValue<string>("_programPreview");
            //����Ĭ���ļ���
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
            // ע��Swagger����
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "File service", Version = "v1" });
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //ע��Session������ӿ�
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //ע��sqlsugar
            ServiceExtension.RegisterAssembly(services, "Advantech.Service");   //����ע��
            ServiceExtension.RegisterAssembly(services, "Advantech.Repository");//�ֿ�ע��
            services.AddInitDataTable();//��ʼ���������ݿ��
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("*");
                    builder.AllowAnyHeader();//��������ͷ
                    builder.AllowAnyMethod();//�������ⷽ��
                });
            });

            services.AddHttpClientFactoryHelper();//http���󹤳�ע��
            services.AddSingleton<AuthServerConfig>(ServiceProvider => new AuthServerConfig()
            {
                LoginUrl = "/Home/Index",                   //��¼ҳ��
                TokenServerUrl = "http://172.21.168.5:8080"//��Ȩ�����ַ,������׺б��
            });
            services.AddScoped<UserAuthorize>();//ע���û�Ȩ�޹�����
            services.AddSession();//Ȩ����Ҫʹ��
            services.AddControllersWithViews(o => { o.UseGeneralRoutePrefix("file"); });//Ĭ��·��ǰ׺
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //��̬��Դ����·�����أ������м��������Ȩ
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
            app.UseSession();//Ȩ����Ҫʹ��
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);//���� Cors �����м��

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.Map("/file/MediaInfo", endpoints.CreateApplicationBuilder()
                //         .UseMiddleware<UserAuthorizeMiddleware>().Build());
            });
        }
    }

}
