namespace ITD.Mapper.Models;
public class TypeMap
{
    public Type SourceType { get; }
    public Type DestinationType { get; }

    public IReadOnlyDictionary<string, PropertyMap> PropertyMaps => _propertyMaps.AsReadOnly();
    private readonly Dictionary<string, PropertyMap> _propertyMaps = new();

    public TypeMap(Type sourceType, Type destinationType)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }

    public PropertyMap ForMember(string destinationPropertyName)
    {
        if (!_propertyMaps.TryGetValue(destinationPropertyName, out PropertyMap? map))
        {
            map = new PropertyMap(destinationPropertyName);
            _propertyMaps[destinationPropertyName] = map;
        }

        return map;
    }
}
