using System.Text.Json.Serialization;

namespace back.ModelsExport;

public sealed record PersonnesRestoHistoriqueExport
{
    public required int Id { get; init; }
    public required int IdPersonne { get; init; }
    public required int IdRestaurant { get; init; }
    public required double Prix { get; init; }
    public required DateOnly Date { get; init; }
}

[JsonSerializable(typeof(List<PersonnesRestoHistoriqueExport>))]
public partial class PersonnesRestoHistoriqueExportContext: JsonSerializerContext { }
