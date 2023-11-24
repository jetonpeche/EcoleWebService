using back.Models;
using back.ModelsExport;
using back.ModelsImport.PersonnesRestaurantsHistorique;
using Microsoft.EntityFrameworkCore;

namespace back.Services.PersonnesRestaurantsHistorique;

public sealed class PersonneRestaurantHistoriqueService : IPersonneRestaurantHistoriqueService
{
    public BddContext Context { get; }

    public PersonneRestaurantHistoriqueService(BddContext _context)
    {
        Context = _context;
    }

    public async Task<List<PersonnesRestoHistoriqueExport>> ListerAsync(int _idPersonne)
    {
        try
        {
            return await Context.Personnerestauranthistoriques
                .Where(x => x.IdPersonne == _idPersonne)
                .Select(x => new PersonnesRestoHistoriqueExport
                {
                    Id = x.Id,
                    IdPersonne = x.IdPersonne,
                    IdRestaurant = x.IdRestaurant,
                    Prix = (double)x.Prix,
                    Date = x.Date
                }).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AjouterAsync(PersonnesRestoHistoriqueImport _personnesRestoImport, int _idPersonne)
    {
        try
        {
            var retour = await Context.Personnerestauranthistoriques.AddAsync(new Personnerestauranthistorique
            {
                IdPersonne = _idPersonne,
                IdRestaurant = _personnesRestoImport.IdRestaurant,
                Date = DateOnly.Parse(DateTime.Now.ToString())
            });

            await Context.SaveChangesAsync();
    
            return retour.Entity.Id;
        }
        catch
        {
            throw;
        }
    }
}
