
using back.Models;
using back.ModelsExport;
using back.ModelsImport.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace back.Services.Restaurants;

public class RestaurantService: IRestaurantService
{
    public BddContext Context { get; }

    public RestaurantService(BddContext _context)
    {
        Context = _context;
    }

    public async Task<List<RestaurantExport>> ListerAsync()
    {
        try
        {
            return await Context.Restaurants.Select(x => new RestaurantExport
            {
                Id = x.Id,
                Nom = x.Nom,
                Adresse = x.Adresse,
                Description = x.Description,
                Telephone = x.Telephone,
                Url = x.Url,

                TypeRestaurant = new TypeRestaurantExport 
                { 
                    Id = x.IdTypeRestaurant, 
                    Nom = x.IdTypeRestaurantNavigation.Nom
                }
            }).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<RestaurantExport> AleatoireAsync()
    {
        try
        {
            int nbRestaurant = await Context.Restaurants.CountAsync();

            int nbSkipeRestaurant = Random.Shared.Next(nbRestaurant);

            return await Context.Restaurants
                .Skip(nbSkipeRestaurant)
                .Select(x => new RestaurantExport
                {
                    Id = x.Id,
                    Nom = x.Nom,
                    Adresse = x.Adresse,
                    Description = x.Description,
                    Telephone = x.Telephone,
                    Url = x.Url,

                    TypeRestaurant = new TypeRestaurantExport 
                    { 
                        Id = x.Id, 
                        Nom = x.Nom 
                    }
            }).FirstAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<RestaurantExport?> InfoAsync(int _idRestaurant)
    {
        try
        {
            return await Context.Restaurants.Where(x => x.Id == _idRestaurant).Select(x => new RestaurantExport
            {
                Id = x.Id,
                Nom = x.Nom,
                Adresse = x.Adresse,
                Description = x.Description,
                Telephone = x.Telephone,   
                Url = x.Url,
                TypeRestaurant = new TypeRestaurantExport 
                { 
                    Id = x.Id, 
                    Nom = x.Nom 
                }
            }).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AjouterAsync(RestaurantImport _restaurantImport)
    {
        try
        {
            var retour = await Context.Restaurants.AddAsync(new Restaurant
            {
                Nom = _restaurantImport.Nom,
                Adresse = _restaurantImport.Adresse,
                Description = _restaurantImport.Description is "" ? null : _restaurantImport.Description,
                Telephone = _restaurantImport.Telephone,
                Url = _restaurantImport.Url,
                IdTypeRestaurant = _restaurantImport.IdTypeRestaurant
            });

            int nb = await Context.SaveChangesAsync();

            return nb > 0 ? retour.Entity.Id : 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ModifierAsync(RestaurantModifierImport _restaurantImport)
    {
        try
        {
            int retour = await Context.Restaurants
                    .Where(x => x.Id == _restaurantImport.Id)
                    .ExecuteUpdateAsync(x => 
                        x.SetProperty(y => y.Adresse, _restaurantImport.Adresse)
                        .SetProperty(y => y.Description, _restaurantImport.Description)
                        .SetProperty(y => y.Nom, _restaurantImport.Nom)
                        .SetProperty(y => y.Telephone, _restaurantImport.Telephone)
                        .SetProperty(y => y.Url, _restaurantImport.Url)
                        .SetProperty(y => y.IdTypeRestaurant, _restaurantImport.IdTypeRestaurant)
                    );

            return retour is 1;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> SupprimerAsync(int _idRestaurant)
    {
        try
        {
            int retour = await Context.Restaurants.Where(x => x.Id == _idRestaurant).ExecuteDeleteAsync();

            return retour is 1;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExisteAsync(int _idRestaurant)
    {
        try
        {
           return await Context.Restaurants.AnyAsync(x => x.Id == _idRestaurant);
        }
        catch
        {
            throw;
        }
    }
}
