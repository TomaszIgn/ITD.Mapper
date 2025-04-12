using ITD.Mapper.Models;
using System.Reflection;

namespace ITD.Mapper.Cache;
public static class ReflectionCache
{
    public static readonly Dictionary<Type, Dictionary<string, PropertyAccessor>> _cache = new();

    public static Dictionary<string, PropertyAccessor> GetProperties(Type type)
    {
        if (!_cache.TryGetValue(type, out var accessors))
        {
            accessors = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToDictionary(
                    p => p.Name,
                    p => new PropertyAccessor(p)
                );

            _cache[type] = accessors;
        }

        return accessors;
    }
}
