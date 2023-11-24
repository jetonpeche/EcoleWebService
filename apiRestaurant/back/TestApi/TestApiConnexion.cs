using TestApi;
using TestApi.Extensions;

namespace TestApiConnexion;

public class TestApiConnexion
{

    [Fact]
    public async Task ConnexionUtilisateurTest()
    {
        bool retour = await Init.Instance.HttpClient.ConnexionAsync();

        Assert.True(retour);
    }

    [Fact]
    public async Task ConnexionAdminTest()
    {
        bool retour = await Init.Instance.HttpClient.ConnexionAsync(true);

        Assert.True(retour);
    }
}