namespace ITD.Mapper.Models.Options;
public class PropertyOptions<TSrc>
{
    private readonly PropertyMap _propertyMap;

    public PropertyOptions(PropertyMap propertyMap)
    {
        _propertyMap = propertyMap;
    }

    public void MapFrom<TValue>(Func<TSrc, TValue> resolver)
    {
        if (resolver == null)
            throw new ArgumentNullException(nameof(resolver));

        _propertyMap.SetResolver(src => resolver((TSrc)src));
    }

    public void Ignore()
    {
        _propertyMap.SetIgnore(true);
    }
}
