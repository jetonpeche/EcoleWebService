using back.ModelsExport;
using back.ModelsImport.RestaurantsPersonnesAimer;

namespace back.Services.PersonnesRestaurantsAimer;

public interface IPersonneRestaurantAimerService
{
    Task<List<RestaurantExport>> ListerAsync(int _idPersonne);
    Task<bool> AjouterAsync(List<int> _listeIdRestaurantAimer, int _idPersonne);

    /// <summary>
    /// Ajoute un resto aimé par une personne
    /// </summary>
    /// <returns>true => OK / false => existe déjà</returns>
    Task<bool> AjouterAsync(int _idRestaurant, int idPersonne);
    Task<bool> SupprimerAsync(int _idRestaurant, int idPersonne);

    /// <summary>
    /// Verifie que la ligne existe
    /// </summary>
    /// <returns>true => existe / false => existe pas</returns>
    Task<bool> ExisteAsync(int _idPersonne, int _idRestaurant);
}
