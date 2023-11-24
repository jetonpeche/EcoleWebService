using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using back.ModelsExport;

namespace TestApi.Extensions;
public static class HttpClientExtension
{
    public static async Task<bool> ConnexionAsync(this HttpClient _httpClient, bool _estAdmin = false)
    {
        var retour = await _httpClient.PostAsJsonAsync("/log/connexion", _estAdmin ? new { Login = "Login2", Mdp = "azertyuiop" } : new { Login = "Login1", Mdp = "azertyuiop" });

        if(retour.IsSuccessStatusCode)
        {
            var info = await JsonSerializer.DeserializeAsync<PersonneConnexionExport>(await retour.Content.ReadAsStreamAsync(), Init.Instance.JsonOption);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", info!.Jwt);
        }

        return retour.IsSuccessStatusCode;
    }
}
