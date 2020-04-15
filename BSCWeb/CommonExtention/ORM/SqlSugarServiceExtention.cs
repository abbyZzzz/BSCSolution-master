using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.CoreExtention
{
    public static class SqlSugarServiceExtention
    {
        public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration, ServiceLifetime injection = ServiceLifetime.Scoped)
        {
            //ServiceCollectionDescriptorExtensions
            switch (injection)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceProvider =>
                    {
                        ISqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                        {
                            ConnectionString = configuration["connectionString"],
                            DbType = DbType.PostgreSQL,
                            IsAutoCloseConnection = true,
                            InitKeyType = InitKeyType.Attribute,
                            IsShardSameThread = true
                        });
                        return db;
                    });
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceProvider =>
                    {
                        ISqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                        {
                            ConnectionString = configuration["connectionString"],
                            DbType = DbType.PostgreSQL,
                            IsAutoCloseConnection = true,
                            InitKeyType = InitKeyType.Attribute,
                            IsShardSameThread = true
                        });
                        return db;
                    });
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient(serviceProvider =>
                    {
                        ISqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                        {
                            ConnectionString = configuration["connectionString"],
                            DbType = DbType.PostgreSQL,
                            IsAutoCloseConnection = true,
                            InitKeyType = InitKeyType.Attribute,
                            IsShardSameThread = true
                        });
                        return db;
                    });
                    break;
            }
            return services;
        }
    }
}
