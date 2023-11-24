using back.Models;
using back.ModelsExport;
using back.ModelsImport.TypeRestraurant;
using Microsoft.EntityFrameworkCore;

namespace back.Services.TypesRestaurant;

public sealed class TypeRestaurantService : ITypeRestaurantService
{
    public BddContext Context { get; }

    public TypeRestaurantService(BddContext _context)
    {
        Context = _context;
    }

    public async Task<List<TypeRestaurantExport>> ListerAsync()
    {
        try
        {
            return await Context.Typerestaurants.Select(x => new TypeRestaurantExport
            {
                Id = x.Id,
                Nom = x.Nom
            }).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<TypeRestaurantExport?> InfoAsync(int _idTypeRestaurant)
    {
        try
        {
            return await Context.Typerestaurants
                .Where(x => x.Id == _idTypeRestaurant)
                .Select(x => new TypeRestaurantExport 
            { 
                Id = _idTypeRestaurant,
                Nom = x.Nom
            }).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AjouterAsync(TypeRestaurantImport _typeRestoImport)
    {
        try
        {
            var retour = await Context.Typerestaurants.AddAsync(new Typerestaurant { Nom = _typeRestoImport.Nom });
            int nb = await Context.SaveChangesAsync();
 
            return nb > 0 ? retour.Entity.Id : 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ModifierAsync(TypeRestaurantModifierImport _typeRestoImport)
    {
        try
        {
            int retour = await Context.Typerestaurants
                    .Where(x => x.Id == _typeRestoImport.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.Nom, _typeRestoImport.Nom));

            return retour is 1;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> SupprimerAsync(int _idTypeRestaurant)
    {
        try
        {
            int retour = await Context.Typerestaurants
                    .Where(x => x.Id == _idTypeRestaurant)
                    .ExecuteDeleteAsync();

            return retour is 1;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExisteAsync(int _idTypeRestaurant)
    {
        try
        {
            return await Context.Typerestaurants.AnyAsync(x => x.Id == _idTypeRestaurant);
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExisteAsync(string _nomTypeRestaurant)
    {
        try
        {
            return await Context.Typerestaurants.AnyAsync(x => x.Nom.Trim().ToLower() == _nomTypeRestaurant.Trim().ToLower());
        }
        catch
        {
            throw;
        }
    }
}
