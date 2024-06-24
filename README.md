## HangFireSample

This project demonstrates how to integrate Hangfire, a popular library for background job processing in .NET, into a C# application. The HangFireSample project provides examples of various types of background jobs, such as fire-and-forget, delayed, and recurring jobs, showcasing how to set them up, manage, and monitor them using Hangfire's dashboard.

### Features

- **Fire-and-Forget Jobs**: Execute a task immediately.
- **Delayed Jobs**: Schedule a task to run after a specified delay.
- **Recurring Jobs**: Set up tasks to run at specified intervals.
- **Continuations**: Execute a task after the completion of another job.
- **Dashboard Monitoring**: Real-time monitoring and management of background jobs.

### Getting Started

To run this project, clone the repository and follow the setup instructions provided in the `Setup.md` file.

### Requirements

- .NET 6.0 or later
- Hangfire 1.7 or later

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
    dotnet run
    ```

### Usage

Refer to the examples in the project to understand how to create and manage different types of background jobs. Use the Hangfire dashboard to monitor job execution and performance.

### Contributing

Feel free to fork this repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.

