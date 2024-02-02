using Hangfire;
using Hangfire.MemoryStorage;
using HangFireLearn.api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adicionando o Hangfire
builder.Services.AddHangfire(op =>
{
    op.UseMemoryStorage();
});
builder.Services.AddHangfireServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Adicionando o Middleware
app.UseHangfireDashboard();

//Testes do HangFire
BackgroundJob.Enqueue(() => HangfireMocs.HelloWorldFireAndForget());
//BackgroundJob.Enqueue(() => HangfireMocs.HelloWorldFireAndForgetWithError());
RecurringJob.AddOrUpdate("HelloRecurring", () => HangfireMocs.HelloWorldRecurringJob(), Cron.Minutely());
BackgroundJob.Schedule(() => HangfireMocs.HelloWorldDelayedJob(), TimeSpan.FromDays(1));

string jobId = BackgroundJob.Enqueue(() => HangfireMocs.HelloWorldContinuationJobPai());
BackgroundJob.ContinueJobWith(jobId, () => HangfireMocs.HelloWorldContinuationJobFilha(jobId));



app.Run();

