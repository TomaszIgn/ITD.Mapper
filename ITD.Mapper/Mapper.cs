using ITD.Mapper.Cache;
using ITD.Mapper.Helpers;
using ITD.Mapper.Models;
using ITD.Mapper.Models.Options;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ITD.Mapper;
public static class Mapper
{
    public static TDestination Map<TDestination>(object source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Type srcType = source.GetType();
        Type destType = typeof(TDestination);

        TypeMap typeMap = MapperConfig.GetMap(srcType, destType);

        object destination = Activator.CreateInstance(destType)!;

        Dictionary<string, PropertyAccessor> srcProps = ReflectionCache.GetProperties(srcType);
        Dictionary<string, PropertyAccessor> destProps = ReflectionCache.GetProperties(destType);

        foreach (PropertyAccessor destProp in destProps.Values)
        {
            if (typeMap.PropertyMaps.TryGetValue(destProp.Name, out PropertyMap? propMap))
            {
                if (propMap.Ignroe)
                    continue;

                if (propMap.CustomResolver != null)
                {
                    object? value = propMap.CustomResolver(source);
                    destProp.Setter(destination, value);
                    continue;
                }
            }

            if (srcProps.TryGetValue(destProp.Name, out PropertyAccessor? srcProp))
            {
                object? srcValue = srcProp.Getter(source);

                if (srcValue != null)
                {
                    Type srcPropType = srcProp.Type;
                    Type destPropType = destProp.Type;

                    if (destPropType.IsSimpleType())
                    {
                        destProp.Setter(destination, srcValue);
                    }
                    else if (destPropType.IsGenericList())
                    {
                        Type srcItempType = srcPropType.GetGenericArguments()[0];
                        Type destItemType = destPropType.GetGenericArguments()[0];

                        if (MapperConfig.GetMap(srcItempType, destItemType) != null)
                        {
                            IEnumerable<object> srcList = (IEnumerable<object>)srcValue;
                            IList destList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(destItemType))!;

                            foreach (object item in srcList)
                            {
                                object? mappedItem = typeof(Mapper)
                                    .GetMethod(nameof(Map), BindingFlags.Static | BindingFlags.Public)!
                                    .MakeGenericMethod(destItemType)
                                    .Invoke(null, new object[] { item });

                                destList.Add(mappedItem);
                            }

                            destProp.Setter(destination, destList);
                        }

                    }
                    else if (MapperConfig.GetMap(srcPropType, destPropType) is not null)
                    {
                        object? nestedValue = typeof(Mapper)
                            .GetMethod(nameof(Map), BindingFlags.Static | BindingFlags.Public)!
                            .MakeGenericMethod(destPropType)
                            .Invoke(null, new object[] { srcValue });

                        destProp.Setter(destination, nestedValue);
                    }
                }
            }
        }

        return (TDestination)destination;
    }

    public static Expression<Func<TSource, TDestination>> GetProjectionExpression<TSource, TDestination>()
    {
        Type sourceType = typeof(TSource);
        Type destinationType = typeof(TDestination);

        ParameterExpression parameter = Expression.Parameter(sourceType, "src");
        List<MemberBinding> bindings = new List<MemberBinding>();

        PropertyInfo[] sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] destProps = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo destProp in destProps)
        {
            PropertyInfo? sourceProp = sourceProps.FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);
            if (sourceProp == null) continue;

            MemberExpression sourcePropertyExpr = Expression.Property(parameter, sourceProp);
            MemberAssignment binding = Expression.Bind(destProp, sourcePropertyExpr);
            bindings.Add(binding);
        }

        MemberInitExpression body = Expression.MemberInit(Expression.New(destinationType), bindings);
        return Expression.Lambda<Func<TSource, TDestination>>(body, parameter);
    }
}
