using back.ModelsExport;
using back.ModelsImport.Restaurants;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TestApi.Extensions;

namespace TestApi.RouteRestaurant;
public class TestRouteRestaurantConnecter
{
    private JsonSerializerOptions jsonOption = Init.Instance.JsonOption;

    [Fact]
    public async Task ListerRestaurantUtilisateurConnecterTest()
    {
        await Init.Instance.HttpClient.ConnexionAsync();
        var retour = await Init.Instance.HttpClient.GetAsync("/restaurant/lister");

        if (retour.IsSuccessStatusCode)
        {
            var info = await JsonSerializer.DeserializeAsync<List<RestaurantExport>>(await retour.Content.ReadAsStreamAsync(), jsonOption);

            Assert.NotNull(info);
        }
        else
            Assert.Fail("API pas allumé");
    }

    [Fact]
    public async Task InfoRestaurantUtilisateurConnecter_Idexiste_Test()
    {
        await Init.Instance.HttpClient.ConnexionAsync();
        var retour = await Init.Instance.HttpClient.GetAsync("/restaurant/info/1");

        Assert.True(retour.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task InfoRestaurantUtilisateurConnecter_IdexistePas_Test()
    {
        await Init.Instance.HttpClient.ConnexionAsync();
        var retour = await Init.Instance.HttpClient.GetAsync("/restaurant/info/0");

        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AjouterRestaurantUtilisateurConnecterTest()
    {
        RestaurantImport r = new()
        {
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = "0123456789",
            Url = "https://www.google.fr/"
        };

        await Init.Instance.HttpClient.ConnexionAsync();
        var retour = await Init.Instance.HttpClient.PostAsJsonAsync("/restaurant/ajouter", r);

        Assert.True(retour.StatusCode == HttpStatusCode.Created);
    }

    [Fact]
    public async Task AjouterRestaurantUtilisateurConnecter_infoPasBon_Test()
    {
        RestaurantImport r = new()
        {
            Adresse = "adresse",
            Description = null,
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = "aze",
            Url = "https://www.google.fr/"
        };

        await Init.Instance.HttpClient.ConnexionAsync();

        // telephone pas bon
        var retour = await Init.Instance.HttpClient.PostAsJsonAsync("/restaurant/ajouter", r);
        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest, "telephone pas bon");

        // url pas bon
        r.Telephone = "0612345678";
        r.Url = "aze";

        retour = await Init.Instance.HttpClient.PostAsJsonAsync("/restaurant/ajouter", r);
        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest, "Url pas bon");

        // id type restaurant pas bon
        r.Url = "https://www.google.fr/";
        r.IdTypeRestaurant = 0;

        retour = await Init.Instance.HttpClient.PostAsJsonAsync("/restaurant/ajouter", r);
        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest, "id type restaurant pas bon");
    }

    [Fact]
    public async Task ModifierRestaurantUtilisateurConnecterTest()
    {
        RestaurantModifierImport r = new()
        {
            Id = 1,
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = "0612671670",
            Url = "https://www.google.fr/"
        };

        await Init.Instance.HttpClient.ConnexionAsync();
        var retour = await Init.Instance.HttpClient.PutAsJsonAsync("/restaurant/modifier", r);

        Assert.True(retour.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ModifierRestaurantUtilisateurConnecter_infoPasBon_Test()
    {
        await Init.Instance.HttpClient.ConnexionAsync();

        // id pas bon
        var retour = await Init.Instance.HttpClient.PutAsJsonAsync("/restaurant/modifier", new RestaurantModifierImport
        {
            Id = 0,
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 0,
            Nom = "nom",
            Telephone = "0612671670",
            Url = "https://www.google.fr/"
        });

        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest, "id pas bon");

        // id type restaurant pas bon
        retour = await Init.Instance.HttpClient.PutAsJsonAsync("/restaurant/modifier", new RestaurantModifierImport
        {
            Id = 1,
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 0,
            Nom = "nom",
            Telephone = "0612671670",
            Url = "https://www.google.fr/"
        });

        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest, "id type restaurant pas bon");

        // telephone pas bon
        retour = await Init.Instance.HttpClient.PutAsJsonAsync("/restaurant/modifier", new RestaurantModifierImport
        {
            Id = 1,
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = "aze",
            Url = "https://www.google.fr/"
        });
        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest);

        // url pas bon
        retour = await Init.Instance.HttpClient.PutAsJsonAsync("/restaurant/modifier", new RestaurantModifierImport
        {
            Id = 1,
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = null,
            Url = "aze"
        });
        Assert.True(retour.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SupprimerRestaurantUtilisateurConnecterTest()
    {
        RestaurantImport r = new()
        {
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = "0123456789",
            Url = "https://www.google.fr/"
        };

        await Init.Instance.HttpClient.ConnexionAsync();
        var retour = await Init.Instance.HttpClient.PostAsJsonAsync("/restaurant/ajouter", r);

        var restaurant = await JsonSerializer.DeserializeAsync<RestaurantExport>(await retour.Content.ReadAsStreamAsync(), jsonOption);

        retour = await Init.Instance.HttpClient.DeleteAsync($"/restaurant/supprimer/{restaurant!.Id}");
        Assert.True(retour.StatusCode == HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 0)]
    [InlineData(HttpStatusCode.NotFound, 10_000)]
    public async Task SupprimerRestaurantUtilisateurConnecter_infoPasBon_Test(HttpStatusCode _statusCode, int _idRestaurant)
    {
        await Init.Instance.HttpClient.ConnexionAsync();

        var retour = await Init.Instance.HttpClient.DeleteAsync($"/restaurant/supprimer/{_idRestaurant}");
        Assert.True(retour.StatusCode == _statusCode);
    }
}
