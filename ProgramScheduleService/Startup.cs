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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //ע��Session������ӿ�
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //ע��sqlsugar
            ServiceExtension.RegisterAssembly(services,"Advantech.Service");   //����ע��
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

            services.AddControllersWithViews(o => { o.UseGeneralRoutePrefix("program"); });//Ĭ��·��ǰ׺
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

            app.UseAuthentication();//����JWT��Ȩ
            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);//���� Cors �����м��

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.Map("/api/ScheduleDay", endpoints.CreateApplicationBuilder()
                //         .UseMiddleware<AuthMiddleware>().Build());
            });
        }
    }
}
