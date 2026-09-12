namespace HangFireLearn.api.Jobs;

public class SampleJobs
{
    private readonly ILogger<SampleJobs> _logger;

    public SampleJobs(ILogger<SampleJobs> logger)
    {
        _logger = logger;
    }

    public async Task HelloWorldFireAndForget()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        _logger.LogInformation("Fire-and-forget job executed.");
    }

    public Task HelloWorldFireAndForgetWithError()
    {
        throw new InvalidOperationException("Simulated failure to demonstrate Hangfire's automatic retries.");
    }

    public Task HelloWorldRecurringJob()
    {
        _logger.LogInformation("Recurring job executed at {Timestamp}.", DateTimeOffset.Now);
        return Task.CompletedTask;
    }

    public Task HelloWorldDelayedJob()
    {
        _logger.LogInformation("Delayed job executed.");
        return Task.CompletedTask;
    }

    public Task<int> HelloWorldContinuationJobParent()
    {
        var randomNumber = Random.Shared.Next(1, 1001);
        _logger.LogInformation("Parent job executed and generated value {Value}.", randomNumber);
        return Task.FromResult(randomNumber);
    }

    public Task HelloWorldContinuationJobChild(string parentJobId)
    {
        _logger.LogInformation("Child job executed after parent job {ParentJobId} completed.", parentJobId);
        return Task.CompletedTask;
    }
}
