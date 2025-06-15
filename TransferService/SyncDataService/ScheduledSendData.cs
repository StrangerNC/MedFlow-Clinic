namespace TransferService.SyncDataService;

public class ScheduledSendData : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer? _timer;

    public ScheduledSendData(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Запускаем таймер каждые 5 минут
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var gather = scope.ServiceProvider.GetRequiredService<IGatherAndPutData>();
        var send = scope.ServiceProvider.GetRequiredService<ISendDataClient>();

        await gather.GetAndPutData();
        await send.SendData();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}