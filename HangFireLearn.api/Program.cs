using Hangfire;
using Hangfire.MemoryStorage;
using HangFireLearn.api;
using HangFireLearn.api.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SampleJobs>();

// Adicionando o Hangfire
builder.Services.AddHangfire(config => config.UseMemoryStorage());
// The default SchedulePollingInterval (15s) is longer than the Heartbeat
// job's 10s delay, so scheduled/recurring jobs would only fire every 15s.
// Poll more often so the "every 10 seconds" job actually runs close to that.
builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(2));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adicionando o Middleware (restrito ao ambiente de Development — veja HangfireDashboardAuthorizationFilter)
app.UseHangfireDashboard(options: new DashboardOptions
{
    Authorization = [new HangfireDashboardAuthorizationFilter(app.Environment)],
});

// Recurring jobs são idempotentes via AddOrUpdate, então podem ser registrados no startup.
RecurringJob.AddOrUpdate<SampleJobs>(
    "hello-recurring",
    job => job.HelloWorldRecurringJob(),
    Cron.Minutely());

// Hangfire's cron-based recurring jobs can't go below a 1-minute interval,
// so this "every 10 seconds" job reschedules itself (see SampleJobs.Heartbeat).
// MemoryStorage is wiped on every restart, so it's safe to kick off here.
app.Services.GetRequiredService<IBackgroundJobClient>().Enqueue<SampleJobs>(job => job.Heartbeat());

var jobs = app.MapGroup("/api/jobs").WithTags("Jobs");

jobs.MapPost("/fire-and-forget", (IBackgroundJobClient client) =>
{
    var jobId = client.Enqueue<SampleJobs>(job => job.HelloWorldFireAndForget());
    return Results.Ok(new { jobId });
});

jobs.MapPost("/fire-and-forget-with-error", (IBackgroundJobClient client) =>
{
    var jobId = client.Enqueue<SampleJobs>(job => job.HelloWorldFireAndForgetWithError());
    return Results.Ok(new { jobId });
});

jobs.MapPost("/delayed", (IBackgroundJobClient client, TimeSpan? delay) =>
{
    var jobId = client.Schedule<SampleJobs>(job => job.HelloWorldDelayedJob(), delay ?? TimeSpan.FromMinutes(1));
    return Results.Ok(new { jobId });
});

jobs.MapPost("/continuation", (IBackgroundJobClient client) =>
{
    var parentJobId = client.Enqueue<SampleJobs>(job => job.HelloWorldContinuationJobParent());
    var childJobId = client.ContinueJobWith<SampleJobs>(parentJobId, job => job.HelloWorldContinuationJobChild(parentJobId));
    return Results.Ok(new { parentJobId, childJobId });
});

jobs.MapPost("/notification", (IBackgroundJobClient client, string message) =>
{
    var jobId = client.Enqueue<SampleJobs>(job => job.SendNotification(message));
    return Results.Ok(new { jobId });
});

jobs.MapPost("/batch", (IBackgroundJobClient client, int itemCount = 5) =>
{
    var jobId = client.Enqueue<SampleJobs>(job => job.ProcessBatch(itemCount));
    return Results.Ok(new { jobId });
});

jobs.MapPost("/long-running", (IBackgroundJobClient client) =>
{
    var jobId = client.Enqueue<SampleJobs>(job => job.LongRunningJob());
    return Results.Ok(new { jobId });
});

// Controls the self-rescheduling "every 10 seconds" heartbeat job.
jobs.MapPost("/heartbeat/stop", () =>
{
    SampleJobs.DisableHeartbeat();
    return Results.Ok();
});

jobs.MapPost("/heartbeat/start", (IBackgroundJobClient client) =>
{
    if (!SampleJobs.TryEnableHeartbeat())
    {
        return Results.Ok(new { message = "Heartbeat is already running." });
    }

    var jobId = client.Enqueue<SampleJobs>(job => job.Heartbeat());
    return Results.Ok(new { jobId });
});

app.Run();
