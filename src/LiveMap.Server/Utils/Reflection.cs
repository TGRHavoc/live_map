using System.Linq.Expressions;
using System.Reflection;

namespace LiveMap.Server.Utils;

public static class Reflection
{
    public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .SelectMany(p => p.GetMethods())
            .Where(m => m.GetCustomAttributes<TAttribute>().Any() && m.DeclaringType != typeof(LiveMap) && m.DeclaringType != null);
    }
    
    public static Delegate CreateDelegateFrom(MethodInfo method, object? instance)
    {
        var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var actionType = Expression.GetDelegateType(parameters.Concat(new[] { method.ReturnType }).ToArray());
        return method.IsStatic
            ? Delegate.CreateDelegate(actionType, method)
            : Delegate.CreateDelegate(actionType, instance, method);
    }
}