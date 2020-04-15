using Advantech.AppCommon;
using Advantech.BSCPlayer.Schedule;
using Advantech.BSCPlayer.ViewModel;
using Advantech.Repository;
using Advantech.Service;
using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib;
using Advantech.UtilsStandardLib.Http;
using CommonServiceLocator;
using Prism.Unity;
using SqlSugar;
using System;
using System.Configuration;
using Unity;
using Unity.Resolution;

namespace Advantech.BSCPlayer
{
    public class IoC
    {
        public static UnityContainer Container;
        /// <summary>
        /// 配置 IoC 容器，绑定所有需要的信息准备使用
        /// 注意：必须在应用程序启动后立即调用，以确保可以找到所有服务
        /// </summary>
        public static void SetupIoC()
        {
            // Register Unity as the IOC container
            Container = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocatorAdapter(Container));
            BindViewModels();//对象绑定
        }
        /// <summary>
        /// 绑定所有的实例对象
        /// </summary>
        private static void BindViewModels()
        {
            ConnectionConfig cfg = new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.AppSettings["LocalDB_ConnectString"],
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = true
            };
            Container.RegisterInstance(typeof(ISqlSugarClient), new SqlSugarClient(cfg));//数据库信息
            
            //--------------------------------------------------------------------------
            //窗体
            Container.RegisterSingleton<ClientInfoViewModel>();//窗体类模型
            Container.RegisterSingleton<MsgDialogViewModel>();


            //--------------------------------------------------------------------------
            //公共操作
            Container.RegisterType<ILogWrite, Log4NetHelper>();
            Container.RegisterType<IHttpRequest, HttpRequestHelper>();
            Container.RegisterType<IHttpFile, HttpFileHelper>();
            Container.RegisterSingleton<MediaFileTask>();
            Container.RegisterSingleton<RabbitMQClientManager>();
            Container.RegisterSingleton<ProgramScheduleManager>();//排程管理

            //--------------------------------------------------------------------------
            //数据库访问
            Container.RegisterType<IMediaGroupRepository, MediaGroupRepository>();
            Container.RegisterType<IMediaGroupService, MediaGroupService>();

            Container.RegisterType<IMediaInfoRepository, MediaInfoRepository>();
            Container.RegisterType<IMediaInfoService, MediaInfoService>();

            Container.RegisterType<IProgramScheduleRepository, ProgramScheduleRepository>();
            Container.RegisterType<IProgramScheduleService, ProgramScheduleService>();

            Container.RegisterType<IProgramScheduleTimeRepository, ProgramScheduleTimeRepository>();
            Container.RegisterType<IProgramScheduleTimeService, ProgramScheduleTimeService>();

            Container.RegisterType<IScheduleDayRepository, ScheduleDayRepository>();
            Container.RegisterType<IScheduleDayService, ScheduleDayService>();

            Container.RegisterType<IScheduleDayProgramRepository, ScheduleDayProgramRepository>();
            Container.RegisterType<IScheduleDayProgramService, ScheduleDayProgramService>();

            Container.RegisterType<IProgramInfoRepository, ProgramInfoRepository>();
            Container.RegisterType<IProgramInfoService, ProgramInfoService>();

            Container.RegisterType<IProgramRegionRepository, ProgramRegionRepository>();
            Container.RegisterType<IProgramRegionService, ProgramRegionService>();

            Container.RegisterType<IProgramRegionMediaRepository, ProgramRegionMediaRepository>();
            Container.RegisterType<IProgramRegionMediaService, ProgramRegionMediaService>();

            CreateTable();
        }
        public static void CreateTable()
        {
            var service1 = Get<IMediaGroupService>();
            service1.InitTables();

            var service2 = Get<IMediaInfoService>();
            service2.InitTables();

            var service3 = Get<IProgramScheduleService>();
            service3.InitTables();

            var service4 = Get<IProgramScheduleTimeService>();
            service4.InitTables();

            var service5 = Get<IScheduleDayService>();
            service5.InitTables();

            var service6 = Get<IProgramInfoService>();
            service6.InitTables();

            var service7 = Get<IProgramRegionService>();
            service7.InitTables();

            var service8 = Get<IProgramRegionMediaService>();
            service8.InitTables();
            //foreach (IContainerRegistration item in Container.Registrations)
            //{
            //    if (item.RegisteredType.ToString().Contains("Service"))
            //    {
            //        Type type = item.RegisteredType;

            //    }
            //}
        }
        /// <summary>
        /// 获取注入对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService Get<TService>()
        {
           return Container.Resolve<TService>();
        }
        /// <summary>
        /// 专门用于窗体对象获取
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="window">当前需要注入的窗体对象</param>
        /// <returns></returns>
        public static TService GetViewModel<TService>(System.Windows.Window window)
        {
            return Container.Resolve<TService>(new ParameterOverride("window", window));
        }
        /// <summary>
        /// 程序结束退出程序
        /// </summary>
        public static void DisPose()
        {
            Container.Dispose();
        }
    }
}
