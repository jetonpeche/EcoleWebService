using back.ModelsImport.Restaurants;
using System.Net;
using System.Net.Http.Json;

namespace TestApi.RouteRestaurant;
public class TestRouteRestaurantNonConnecter
{
    [Fact]
    public async Task ListerRestaurantNonConnecterTest()
    {
        var retour = await Init.Instance.HttpClient.GetAsync("/restaurant/lister");

        Assert.True(retour.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InfoRestaurantNonConnecterTest()
    {
        var retour = await Init.Instance.HttpClient.GetAsync("/restaurant/info/1");

        Assert.True(retour.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AjouterRestaurantNonConnecterTest()
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

        var retour = await Init.Instance.HttpClient.PostAsJsonAsync("/restaurant/ajouter", r);

        Assert.True(retour.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ModifierRestaurantNonConnecterTest()
    {
        RestaurantModifierImport r = new()
        {
            Id = 1,
            Adresse = "adresse",
            Description = "description",
            IdTypeRestaurant = 1,
            Nom = "nom",
            Telephone = "0123456789",
            Url = "https://www.google.fr/"
        };

        var retour = await Init.Instance.HttpClient.PutAsJsonAsync("/restaurant/modifier", r);

        Assert.True(retour.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SupprimerRestaurantNonConnecterTest()
    {
        var retour = await Init.Instance.HttpClient.DeleteAsync("/restaurant/supprimer/1");

        Assert.True(retour.StatusCode == HttpStatusCode.Unauthorized);
    }
}
