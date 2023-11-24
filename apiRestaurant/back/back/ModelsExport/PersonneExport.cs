using System.Text.Json.Serialization;

namespace back.ModelsExport;

public sealed record PersonneExport
{
    public required int Id { get; init; }
    public required string Nom { get; init; }
    public required bool EstAdmin { get; init; }
}

[JsonSerializable(typeof(PersonneExport))]
[JsonSerializable(typeof(List<PersonneExport>))]
public partial class PersonneExportContext: JsonSerializerContext { }
