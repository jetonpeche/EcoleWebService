using back.ModelsExport;
using back.ModelsImport.Restaurants;

namespace back.Services.Restaurants;

public interface IRestaurantService
{
    Task<List<RestaurantExport>> ListerAsync();
    Task<RestaurantExport> AleatoireAsync();
    Task<RestaurantExport?> InfoAsync(int _idRestaurant);
    Task<int> AjouterAsync(RestaurantImport _restaurantImport);
    Task<bool> ModifierAsync(RestaurantModifierImport _restaurantImport);
    Task<bool> SupprimerAsync(int _idRestaurant);
    Task<bool> ExisteAsync(int _idRestaurant);
}
