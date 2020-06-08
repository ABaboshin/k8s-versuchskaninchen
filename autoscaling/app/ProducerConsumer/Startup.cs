using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SampleApp.MessageBus;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;
using System;
using System.Linq;

namespace SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            var loggerFactory = new SerilogLoggerFactory(CreateLogger(), false);
            serviceCollection.AddSingleton((ILoggerFactory)loggerFactory);

            serviceCollection
                .AddMvc(options => { options.EnableEndpointRouting = false; });
            ConfigureMessageBus(serviceCollection);
        }

        private Logger CreateLogger()
        {
            var logLevel = ParseLoggingLevel(Environment.GetEnvironmentVariable("LOG_LEVEL"));

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .WriteTo.Console(new JsonFormatter(renderMessage: true), logLevel)
                .CreateLogger();
            return logger;
        }

        private LogEventLevel ParseLoggingLevel(string logLevelRaw)
        {
            Enum.TryParse(logLevelRaw, out LogEventLevel level);
            return level as LogEventLevel? ?? LogEventLevel.Verbose;
        }

        private void ConfigureMessageBus(IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<RabbitMQConfiguration>(Configuration.GetSection(RabbitMQConfiguration.SectionKey));

            serviceCollection.AddMassTransit(x =>
            {
                x.AddBus(context =>
                {
                    return Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var loggerFactory = context.GetRequiredService<ILoggerFactory>();
                        cfg.SetLoggerFactory(loggerFactory);

                        var config = context.GetService<IOptions<RabbitMQConfiguration>>().Value;

                        cfg.Host(new Uri($"rabbitmq://{config.Host}/"), host =>
                        {
                            host.Username(config.User);
                            host.Password(config.Password);
                        });

                        if (config.EnableConsuming)
                        {
                            cfg.ReceiveEndpoint("queue-name", ec =>
                            {
                                ec.Consumer(typeof(MyMessageConsumer), t => new MyMessageConsumer(context.GetRequiredService<ILogger<MyMessageConsumer>>()));
                            });
                        }
                    });
                });
            });

            serviceCollection.AddHostedService<BusService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMvc();
        }
    }
}
