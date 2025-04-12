namespace ITD.Mapper.Models.Options;
public static class MapperConfig
{
    private static readonly Dictionary<(Type Source, Type Destination), TypeMap> _mappings = new();

    public static MappingExpression<TSrc, TDest> CreateMap<TSrc, TDest>()
    {
        TypeMap map = new TypeMap(typeof(TSrc), typeof(TDest));
        _mappings[(typeof(TSrc), typeof(TDest))] = map;

        return new MappingExpression<TSrc, TDest>(map);
    }

    internal static TypeMap GetMap(Type src, Type dest)
    {
        if (_mappings.TryGetValue((src, dest), out TypeMap? map))
        {
            return map;
        }
        throw new InvalidOperationException($"Mapping not found for {src} to {dest}");
    }
}
