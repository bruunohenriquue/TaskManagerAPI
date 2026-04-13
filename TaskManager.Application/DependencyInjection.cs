using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Services;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }
}
