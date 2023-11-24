using System.Text.Json.Serialization;

namespace back.ModelsExport;

public sealed record RestaurantExport
{
    public required int Id { get; init; }
    public required string Nom { get; init; }
    public required string Adresse { get; init; }
    public required string? Description { get; init; }
    public required string? Url { get; init; }
    public required string? Telephone { get; init; }
    public required TypeRestaurantExport TypeRestaurant { get; init; }
}

[JsonSerializable(typeof(RestaurantExport))]
[JsonSerializable(typeof(List<RestaurantExport>))]
public partial class RestaurantExportContext : JsonSerializerContext { }

