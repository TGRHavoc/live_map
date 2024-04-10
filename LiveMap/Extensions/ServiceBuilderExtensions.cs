using System.Reflection;
using CitizenFX.Core;
using LiveMap.Handlers;
using LiveMap.Handlers.Events;
using LiveMap.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LiveMap.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddSingletonClasses(this IServiceCollection services)
    {
        return services
            .AddSingleton<UpdateChecker>()
            .AddSingleton<BlipEventHandler>()
            .AddSingleton<SseHandler>()
            .AddSingleton<BlipHandler>()
            .AddSingleton<PlayerHandler>()
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

    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        // Get types who have at least one method with the CommandAttribute
        var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.GetMethods().Any(m => m.GetCustomAttribute<CommandAttribute>() != null));
        
        foreach (var commandType in commandTypes) services.AddSingleton(commandType);
        
        return services;
    }
}