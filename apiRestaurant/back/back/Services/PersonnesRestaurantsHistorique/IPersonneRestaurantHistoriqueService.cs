using back.ModelsExport;
using back.ModelsImport.PersonnesRestaurantsHistorique;

namespace back.Services.PersonnesRestaurantsHistorique;

public interface IPersonneRestaurantHistoriqueService
{
    Task<List<PersonnesRestoHistoriqueExport>> ListerAsync(int _idPersonne);
    Task<int> AjouterAsync(PersonnesRestoHistoriqueImport _personnesRestoImport, int _idPersonne);
}
