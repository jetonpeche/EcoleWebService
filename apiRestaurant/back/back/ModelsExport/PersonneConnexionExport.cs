using System.Text.Json.Serialization;

namespace back.ModelsExport;

public sealed record PersonneConnexionExport
{
    public required int Id { get; init; }
    public required string Nom { get; init; }
    public required string Jwt { get; init; }
}

[JsonSerializable(typeof(PersonneConnexionExport))]
public partial class PersonneConnexionExportContext: JsonSerializerContext { }
