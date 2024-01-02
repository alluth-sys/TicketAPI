using System.Threading;
using System.Threading.Tasks;

public class TrafficController
{
    private readonly SemaphoreSlim _semaphore;

    public TrafficController(int maxConcurrentRequests)
    {
        _semaphore = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);
    }

    public async Task<bool> EnterTrafficAsync()
    {
        return await _semaphore.WaitAsync(0);
    }

    public void ExitTraffic()
    {
        _semaphore.Release();
    }
}
