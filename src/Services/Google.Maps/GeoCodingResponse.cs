using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MagicMedia;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public record AddressComponent(
    [property: JsonPropertyName("long_name")] string LongName,
    [property: JsonPropertyName("short_name")] string ShortName,
    [property: JsonPropertyName("types")] IReadOnlyList<string> Types
);

public record Bounds(
    [property: JsonPropertyName("northeast")] Northeast Northeast,
    [property: JsonPropertyName("southwest")] Southwest Southwest
);

public record Geometry(
    [property: JsonPropertyName("location")] Location Location,
    [property: JsonPropertyName("location_type")] string LocationType,
    [property: JsonPropertyName("viewport")] Viewport Viewport,
    [property: JsonPropertyName("bounds")] Bounds Bounds
);

public record Location(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng
);

public record Northeast(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng
);

public record PlusCode(
    [property: JsonPropertyName("compound_code")] string CompoundCode,
    [property: JsonPropertyName("global_code")] string GlobalCode
);

public record Result(
    [property: JsonPropertyName("address_components")] IReadOnlyList<AddressComponent> AddressComponents,
    [property: JsonPropertyName("formatted_address")] string FormattedAddress,
    [property: JsonPropertyName("geometry")] Geometry Geometry,
    [property: JsonPropertyName("place_id")] string PlaceId,
    [property: JsonPropertyName("types")] IReadOnlyList<string> Types,
    [property: JsonPropertyName("plus_code")] PlusCode PlusCode
);

public record GeoEncodingResponse(
    [property: JsonPropertyName("plus_code")] PlusCode PlusCode,
    [property: JsonPropertyName("results")] IReadOnlyList<Result> Results,
    [property: JsonPropertyName("status")] string Status
);

public record Southwest(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng
);

public record Viewport(
    [property: JsonPropertyName("northeast")] Northeast Northeast,
    [property: JsonPropertyName("southwest")] Southwest Southwest
);

