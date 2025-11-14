using Application.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Application.BackgroundTask
{
    public class BackgroundTask : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public BackgroundTask(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run once immediately at app startup
            await RunLateReturnUpdate(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Hour < 1)
                {
                    await RunLateReturnUpdate(stoppingToken);
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Application stopping - running final late return update");
            await RunLateReturnUpdate(cancellationToken);
            await base.StopAsync(cancellationToken);
        }

        private async Task RunLateReturnUpdate(CancellationToken token)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var borrowRecordRepository = scope.ServiceProvider.GetRequiredService<IBorrowRecordRepository>();
            var memberRepository = scope.ServiceProvider.GetRequiredService<IMemberRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            Dictionary<string, int> lateCounts = [];
            var lateRecords = await borrowRecordRepository.GetLateBorrowRecordsAsync();

            foreach (var record in lateRecords)
            {
                if (!lateCounts.ContainsKey(record.MemberId))
                    lateCounts[record.MemberId] = 0;
                lateCounts[record.MemberId]++;
            }

            if (lateCounts.Count > 0)
            {
                await memberRepository.UpdateMemberLateReturns(lateCounts);
            }
            await unitOfWork.SaveChangesAsync();
            Log.Information(
                "Late return counters updated at {Time}. {Count} members affected",
                DateTime.Now,
                lateCounts.Count
            );
        }
    }
}