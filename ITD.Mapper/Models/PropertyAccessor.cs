using System.Linq.Expressions;
using System.Reflection;

namespace ITD.Mapper.Models;
public class PropertyAccessor
{
    public string Name { get; }
    public Type Type { get; }
    public Func<object, object?>? Getter { get; }
    public Action<object, object?>? Setter { get; }

    public PropertyAccessor(PropertyInfo propertyInfo)
    {
        Name = propertyInfo.Name;
        Type = propertyInfo.PropertyType;

        ParameterExpression objParam = Expression.Parameter(typeof(object), "obj");
        ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");

        UnaryExpression instance = Expression.Convert(objParam, propertyInfo.DeclaringType!);

        Expression<Func<object, object?>> getterexpr = Expression.Lambda<Func<object, object?>>(
            Expression.Convert(Expression.Property(instance, propertyInfo), typeof(object)),
            objParam
        );

        Getter = getterexpr.Compile();

        Expression<Action<object, object?>> setterExpr = Expression.Lambda<Action<object, object?>>(
            Expression.Assign(
                Expression.Property(instance, propertyInfo),
                Expression.Convert(valueParam, propertyInfo.PropertyType)
            ),
            objParam,
            valueParam
        );

        Setter = setterExpr.Compile();
    }
}
