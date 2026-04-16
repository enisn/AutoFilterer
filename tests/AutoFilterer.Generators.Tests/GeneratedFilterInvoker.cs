using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace AutoFilterer.Generators.Tests;

internal static class GeneratedFilterInvoker
{
    private static readonly ConcurrentDictionary<(Type EntityType, Type FilterType), MethodInfo> applyFilterCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> methodCache = new();

    public static IQueryable<TEntity> ApplyFilter<TEntity>(IQueryable<TEntity> source, object filter)
    {
        if (filter == null)
        {
            return source;
        }

        var method = applyFilterCache.GetOrAdd((typeof(TEntity), filter.GetType()), key => ResolveApplyFilterMethod(key.EntityType, key.FilterType));
        return (IQueryable<TEntity>)method.Invoke(null, new[] { source, filter });
    }

    public static MethodInfo GetGeneratedApplyFilterMethod(Type filterType)
    {
        return methodCache.GetOrAdd(filterType, ResolveApplyFilterMethod);
    }

    private static MethodInfo ResolveApplyFilterMethod(Type filterType)
    {
        var method = filterType.Assembly
            .GetTypes()
            .Where(t => t.IsSealed && t.IsAbstract)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .FirstOrDefault(m =>
            {
                if (m.Name != "ApplyFilter")
                {
                    return false;
                }

                var parameters = m.GetParameters();
                return parameters.Length == 2 && parameters[1].ParameterType == filterType;
            });

        if (method == null)
        {
            throw new InvalidOperationException($"Generated ApplyFilter overload not found for '{filterType.FullName}'.");
        }

        return method;
    }

    private static MethodInfo ResolveApplyFilterMethod(Type entityType, Type filterType)
    {
        var method = GetGeneratedApplyFilterMethod(filterType);
        var parameters = method.GetParameters();

        if (!parameters[0].ParameterType.IsGenericType || parameters[0].ParameterType.GetGenericTypeDefinition() != typeof(IQueryable<>) || parameters[0].ParameterType.GetGenericArguments()[0] != entityType)
        {
            throw new InvalidOperationException($"Generated ApplyFilter overload for '{filterType.FullName}' does not target '{entityType.FullName}'.");
        }

        return method;
    }
}
