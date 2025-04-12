namespace ITD.Mapper.Models;
public class PropertyMap
{
    public string DestinationPropertyName { get; }
    public Func<object, object?>? CustomResolver { get; private set; }
    public bool Ignroe { get; private set; }

    public PropertyMap(string destinationPropertyName)
    {
        DestinationPropertyName = destinationPropertyName;
    }

    public void SetResolver(Func<object, object?> resolver)
    {
        CustomResolver = resolver;
    }

    public void SetIgnore(bool ignore)
    {
        Ignroe = ignore;
    }
}
