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
builder.Services.AddHangfireServer();

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

app.Run();
