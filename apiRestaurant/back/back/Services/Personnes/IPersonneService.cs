using back.ModelsExport;
using back.ModelsImport.Personnes;

namespace back.Services.Personnes;

public interface IPersonneService
{
    Task<List<PersonneExport>> ListerAsync();
    Task<PersonneExport?> InfoAsync(int _idPersonne);
    Task<int> AjouterAsync(PersonneImport _personneImport);
    Task<bool> ModifierAsync(PersonneModifierImport _personneImport, int _idPersonne);
    Task<bool> SupprimerAsync(int _idPersonne);
    Task<bool> ExisteAsync(string _login);
    Task<bool> ExisteAsync(int _idPersonne);
}
