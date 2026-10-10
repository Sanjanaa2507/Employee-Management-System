using Microsoft.Extensions.Hosting;

namespace EmployeeManagementSystem.Service
{
    public class DailyAbsenceWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyAbsenceWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 🔁 Run forever
            while (!stoppingToken.IsCancellationRequested)
            {
                // ⏰ Calculate next run time: today at 12:05 AM
                var now = DateTime.Now;
                var nextRun = DateTime.Today.AddDays(1).AddMinutes(5);

                var delay = nextRun - now;
                if (delay.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                using var scope = _serviceProvider.CreateScope();
                var checker = scope.ServiceProvider.GetRequiredService<DailyAbsenceChecker>();

                await checker.RunAsync();

                // 💤 Safety delay (avoid double run in edge cases)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
