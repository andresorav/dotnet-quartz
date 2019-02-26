using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using Quartz;
using Quartz.Impl;

namespace Example1.Services
{
    public class Example1Service : IHostedService
    {
        private readonly ILogger<Example1Service> _logger;

        public Example1Service(ILogger<Example1Service> logger)
        {
            _logger = logger;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduler = await CreateScheduler();
            await scheduler.Start(cancellationToken);

            var job = CreateJob<BasicJob>("basic1", "group1");
            var trigger = CreateTrigger("trigger1", "group1");

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped");
            return Task.CompletedTask;
        }

        private static ITrigger CreateTrigger(string name, string group)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name, group)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(2)
                    .RepeatForever())
                .Build();
        }

        private static IJobDetail CreateJob<T>(string name, string group) where T : IJob
        {
            return JobBuilder.Create<T>()
                .WithIdentity(name, group)
                .Build();
        }
        
        private static async Task<IScheduler> CreateScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            
            var factory = new StdSchedulerFactory(props);
    
            return await factory.GetScheduler();
        }
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class BasicJob : IJob
    {
        private readonly Logger _logger = LogManager.GetLogger(typeof(BasicJob).Name);

        public Task Execute(IJobExecutionContext context)
        {
            _logger.Info($"Job executed");
            return Task.CompletedTask;
        }
    }
}