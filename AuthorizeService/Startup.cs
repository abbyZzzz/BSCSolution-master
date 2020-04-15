using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advantech.AuthenticationService.Authorize;
using Advantech.AuthenticationService.Rpc;
using Advantech.AuthenticationService.Service;
using Advantech.CoreExtention;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Thrift;
using static Advantech.AuthenticationService.Rpc.ThriftRpcTokenService;

namespace Advantech.AuthenticationService
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
            services.AddSqlSugar(Configuration, ServiceLifetime.Scoped);        //ע��sqlsugar
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //ע��Session������ӿ�
            services.AddAuthentication(Configuration);
           
            ServiceExtension.RegisterAssembly(services, "Advantech.Service");   //����ע��
            ServiceExtension.RegisterAssembly(services, "Advantech.Repository");//�ֿ�ע��

            services.AddThriftRpc();    //����RPC����
            services.AddInitDataTable();//��ʼ���������ݿ��
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
