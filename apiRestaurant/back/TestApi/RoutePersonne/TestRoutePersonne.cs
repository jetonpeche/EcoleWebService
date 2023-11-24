using back.ModelsImport.Personnes;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TestApi.Extensions;

namespace TestApi.RoutePersonne;

public class TestRoutePersonne
{
    private JsonSerializerOptions jsonOption = Init.Instance.JsonOption;

    [Theory]
    [InlineData(true, HttpStatusCode.OK)]
    [InlineData(false, HttpStatusCode.Forbidden)]
    public async Task ListerPersonneUtilisateurRoleTest(bool _estAdmin, HttpStatusCode _statusCode)
    {
        await Init.Instance.HttpClient.ConnexionAsync(_estAdmin);
        var retour = await Init.Instance.HttpClient.GetAsync("/personne/lister");

        Assert.True(retour.StatusCode == _statusCode);
    }

    [Theory]
    [InlineData(true, 1, HttpStatusCode.OK)]
    [InlineData(true, 10_000, HttpStatusCode.NotFound)]
    [InlineData(true, 0, HttpStatusCode.BadRequest)]
    [InlineData(false, 0, HttpStatusCode.Forbidden)]
    public async Task InfoPersonneUtilisateurTest(bool _estAdmin, int _idPersonne, HttpStatusCode _statusCode)
    {
        await Init.Instance.HttpClient.ConnexionAsync(_estAdmin);

        var retour = await Init.Instance.HttpClient.GetAsync($"/personne/info/{_idPersonne}");

        Assert.True(retour.StatusCode == _statusCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InfoDePersonneUtilisateurActuelleTest(bool _estAdmin)
    {
        await Init.Instance.HttpClient.ConnexionAsync(_estAdmin);

        var retour = await Init.Instance.HttpClient.GetAsync("/personne/info");

        Assert.True(retour.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task AjouterPersonneUtilisateurTest()
    {
        await Init.Instance.HttpClient.ConnexionAsync();

        var retour = await Init.Instance.HttpClient.PostAsJsonAsync("/personne/ajouter", new PersonneImport
        {
            Nom = "nom",
            Login = "login21",
            Mdp = "azertyuiop",
            ListeIdRestaurant = []
        });

        Assert.True(retour.StatusCode == HttpStatusCode.Created);

        // rien ne va
        retour = await Init.Instance.HttpClient.PostAsJsonAsync("/personne/ajouter", new PersonneImport
        {
            Nom = "",
            Login = "",
            Mdp = "123",
            ListeIdRestaurant = [0]
        });

        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest);
    }
}
