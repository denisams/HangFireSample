## HangFireSample

This project demonstrates how to integrate Hangfire, a popular library for background job processing in .NET, into an ASP.NET Core minimal API. It shows how to set up, trigger, and monitor fire-and-forget, delayed, recurring, and continuation jobs using Hangfire's dashboard.

### Features

- **Fire-and-Forget Jobs**: Execute a task immediately, triggered via an API endpoint.
- **Delayed Jobs**: Schedule a task to run after a specified delay.
- **Recurring Jobs**: A job registered at startup that runs every minute.
- **Continuations**: Execute a task after the completion of another job.
- **Dashboard Monitoring**: Real-time monitoring and management of background jobs.
- Jobs are plain DI-resolved classes (`SampleJobs`) rather than static methods, so they can take dependencies like `ILogger<T>`.

### Requirements

- .NET 10.0 SDK or later
- Hangfire 1.8 or later

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/denisams/HangFireSample.git
    ```
2. Navigate to the project directory:
    ```bash
    cd HangFireSample
    ```
3. Restore the dependencies:
    ```bash
    dotnet restore
    ```
4. Run the project:
    ```bash
    dotnet run --project HangFireLearn.api
    ```

### Usage

With the project running, open:

- `/swagger` — Swagger UI, listing the available endpoints under `/api/jobs` to trigger jobs on demand.
- `/hangfire` — the Hangfire dashboard, to monitor and inspect job execution.

Available endpoints (all `POST`):

| Endpoint | Description |
| --- | --- |
| `/api/jobs/fire-and-forget` | Enqueues a job that runs immediately. |
| `/api/jobs/fire-and-forget-with-error` | Enqueues a job that always throws, useful for seeing Hangfire's retry behavior in the dashboard. |
| `/api/jobs/delayed?delay=00:01:00` | Schedules a job to run after the given `TimeSpan` (defaults to 1 minute). |
| `/api/jobs/continuation` | Enqueues a parent job and a child job that only runs after the parent completes. |

A recurring job (`hello-recurring`) is registered on startup and runs every minute — no need to trigger it manually.

### Dashboard security

Hangfire does not restrict dashboard access by default. This sample only allows access to `/hangfire` in the `Development` environment (see `HangfireDashboardAuthorizationFilter`). Replace that filter with real authentication/authorization before deploying anywhere else.

### Running with Docker

```bash
docker build -t hangfiresample .
docker run --rm -it -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development hangfiresample
```

### Contributing

Feel free to fork this repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.
