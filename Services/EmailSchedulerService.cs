using GlobalFlameMinistry.API.Interfaces.BulkEmail;

namespace GlobalFlameMinistry.API.Services
{
    public class EmailSchedulerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailSchedulerService> _logger;

        public EmailSchedulerService(IServiceScopeFactory scopeFactory, ILogger<EmailSchedulerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Scheduler Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider
                        .GetRequiredService<IBulkEmailService>();

                    await emailService.ProcessScheduledEmailsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in EmailSchedulerService");
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}