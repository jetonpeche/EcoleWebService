using back.ModelsExport;
using back.ModelsImport.TypeRestraurant;

namespace back.Services.TypesRestaurant
{
    public interface ITypeRestaurantService
    {
        Task<List<TypeRestaurantExport>> ListerAsync();
        Task<TypeRestaurantExport?> InfoAsync(int _idTypeRestaurant);
        Task<int> AjouterAsync(TypeRestaurantImport _typeRestoImport);
        Task<bool> ModifierAsync(TypeRestaurantModifierImport _typeRestoImport);
        Task<bool> SupprimerAsync(int _idTypeResto);
        Task<bool> ExisteAsync(int _idTypeRestaurant);
        Task<bool> ExisteAsync(string _nomTypeRestaurant);
    }
}
