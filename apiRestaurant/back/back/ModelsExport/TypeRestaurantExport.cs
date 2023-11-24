using System.Text.Json.Serialization;

namespace back.ModelsExport;

public sealed record TypeRestaurantExport
{
    public required int Id { get; init; }
    public required string Nom { get; init; }
}

[JsonSerializable(typeof(TypeRestaurantExport))]
[JsonSerializable(typeof(List<TypeRestaurantExport>))]
public partial class TypeRestaurantExportContext: JsonSerializerContext { }
