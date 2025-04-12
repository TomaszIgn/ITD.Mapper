namespace ITD.Mapper.Helpers;
public static class TypeExpressions
{
    public static bool IsSimpleType(this Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(Guid);
    }

    public static bool IsGenericList(this Type type)
    {
        return type.IsGenericType
            && typeof(IEnumerable<>).IsAssignableFrom(type)
            && type.GetGenericTypeDefinition() == typeof(List<>);
    }
}
