namespace MagicMedia.GoogleMaps;

public static class GoogleMapsExtensions
{
    public static string? GetAddressComponentName(this IEnumerable<AddressComponent> components, string type)
    {
        AddressComponent? component = components.FirstOrDefault(x => x.Types.Contains(type));

        if (component is { })
        {
            return component.LongName;
        }

        return null;
    }

    public static AddressComponent? GetAddressComponent(
        this IEnumerable<AddressComponent> components,
        string type)
    {
        AddressComponent? component = components.FirstOrDefault(x => x.Types.Contains(type));

        return component;
    }
}
