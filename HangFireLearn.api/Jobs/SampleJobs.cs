using Hangfire;

namespace HangFireLearn.api.Jobs;

public class SampleJobs
{
    // Cron (used by RecurringJob.AddOrUpdate) has minute-level resolution, so
    // the "every 10 seconds" job below reschedules itself instead. This flag
    // lets /api/jobs/heartbeat/stop break that chain.
    private static volatile bool _heartbeatEnabled = true;

    private readonly ILogger<SampleJobs> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public SampleJobs(ILogger<SampleJobs> logger, IBackgroundJobClient backgroundJobClient)
    {
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    /// <summary>Starts the heartbeat chain if it isn't already running. Returns false if it was already enabled.</summary>
    public static bool TryEnableHeartbeat()
    {
        if (_heartbeatEnabled)
        {
            return false;
        }

        _heartbeatEnabled = true;
        return true;
    }

    public static void DisableHeartbeat() => _heartbeatEnabled = false;

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

    /// <summary>
    /// Logs a heartbeat and, while enabled, reschedules itself 10 seconds
    /// later. This is how sub-minute "recurring" jobs are done in Hangfire,
    /// since cron-based recurring jobs cannot go below one-minute intervals.
    /// </summary>
    public Task Heartbeat()
    {
        _logger.LogInformation("Heartbeat job executed at {Timestamp}.", DateTimeOffset.Now);

        if (_heartbeatEnabled)
        {
            _backgroundJobClient.Schedule<SampleJobs>(job => job.Heartbeat(), TimeSpan.FromSeconds(10));
        }

        return Task.CompletedTask;
    }

    public Task SendNotification(string message)
    {
        _logger.LogInformation("Sending notification: {Message}", message);
        return Task.CompletedTask;
    }

    public async Task ProcessBatch(int itemCount)
    {
        for (var item = 1; item <= itemCount; item++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            _logger.LogInformation("Processed item {Item} of {ItemCount}.", item, itemCount);
        }
    }

    public async Task LongRunningJob()
    {
        var steps = new[] { "Collecting data", "Transforming data", "Saving results" };

        for (var i = 0; i < steps.Length; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            _logger.LogInformation("Step {Step}/{TotalSteps}: {Description}.", i + 1, steps.Length, steps[i]);
        }
    }
}
