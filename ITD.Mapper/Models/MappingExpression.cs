using ITD.Mapper.Models.Options;
using System.Linq.Expressions;

namespace ITD.Mapper.Models;
public class MappingExpression<TSrc, TDest>
{
    private readonly TypeMap _typeMap;

    public MappingExpression(TypeMap typeMap)
    {
        _typeMap = typeMap;
    }

    public MappingExpression<TSrc, TDest> ForMember<TMember>(
        Expression<Func<TDest, TMember>> destMember,
        Action<PropertyOptions<TSrc>> options)
    {
        if (destMember.Body is not MemberExpression memberExpr)
            throw new ArgumentException("Destination member must be a member expression", nameof(destMember));

        string propName = memberExpr.Member.Name;

        PropertyMap propertyMap = _typeMap.ForMember(propName);

        PropertyOptions<TSrc> opts = new PropertyOptions<TSrc>(propertyMap);

        options(opts);

        return this;
    }
}
