using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thrift;
using Thrift.Protocols;
using Thrift.Server;
using Thrift.Transports;
using Thrift.Transports.Server;

namespace Advantech.SecurityService.Rpc
{
    public class RpcServiceHost : IHostedService
    {
        public IConfiguration Configuration { get; }

        public ITAsyncProcessor Processor { get; }

        public ILoggerFactory LoggerFactory { get; }

        public RpcServiceHost(IConfiguration configuration, ITAsyncProcessor processor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Processor = processor;
            LoggerFactory = loggerFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TServerTransport serverTransport = new TServerSocketTransport(Configuration.GetValue<int>("RpcPort"));

            TBinaryProtocol.Factory factory1 = new TBinaryProtocol.Factory();
            TBinaryProtocol.Factory factory2 = new TBinaryProtocol.Factory();

            TBaseServer server = new AsyncBaseServer(Processor, serverTransport, factory1, factory2, LoggerFactory);

            return server.ServeAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
