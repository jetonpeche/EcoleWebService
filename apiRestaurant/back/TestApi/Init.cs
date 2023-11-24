using System.Text.Json;

namespace TestApi;
public sealed class Init
{
    private static readonly Init init = new();

    private static readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5289")
    };

    private static readonly JsonSerializerOptions jsonOption = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpClient HttpClient { get => httpClient; }
    public JsonSerializerOptions JsonOption { get => jsonOption; }
    public static Init Instance { get => init; }

    private Init()
    {
        
    }
}
