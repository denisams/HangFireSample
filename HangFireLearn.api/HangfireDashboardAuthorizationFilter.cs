using Hangfire.Dashboard;

namespace HangFireLearn.api;

/// <summary>
/// Restricts the Hangfire dashboard to the Development environment.
/// Hangfire no longer restricts dashboard access by default, so a filter
/// like this (or real authentication) is required before deploying anywhere
/// other than a developer machine.
/// </summary>
public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IHostEnvironment _environment;

    public HangfireDashboardAuthorizationFilter(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool Authorize(DashboardContext context) => _environment.IsDevelopment();
}
