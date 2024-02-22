using LiveMap.Handlers;
using LiveMap.Tasks;
using LiveMap.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace LiveMap.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddSingletonClasses(this IServiceCollection services)
    {
        return services.AddSingleton<UpdateChecker>()
            .AddSingleton<SseService>()
            .AddSingleton<BlipHandler>()
            .AddSingleton<BasicHttpHandler>();
    }

    public static IServiceCollection AddTasks(this IServiceCollection services)
    {
        var taskTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ITask).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

        foreach (var taskType in taskTypes) services.AddSingleton(taskType);
        return services;
    }
}