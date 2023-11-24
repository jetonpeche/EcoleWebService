
using back.Models;
using back.ModelsExport;
using back.ModelsImport.Personnes;
using back.Services.Mdps;
using Microsoft.EntityFrameworkCore;

namespace back.Services.Personnes;

public sealed class PersonneService : IPersonneService
{
    public BddContext Context { get; }
    private IMdpService MdpServ { get; }

    public PersonneService(BddContext _context, IMdpService _mdpServ)
    {
        Context = _context;
        MdpServ = _mdpServ;
    }
    
    public async Task<List<PersonneExport>> ListerAsync()
    {
        try
        {
            return await Context.Personnes.Select(x => new PersonneExport
            {
                Id = x.Id,
                Nom = x.Nom,
                EstAdmin = x.EstAmin
            }).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<PersonneExport?> InfoAsync(int _idPersonne)
    {
        try
        {
            return await Context.Personnes.Where(x => x.Id == _idPersonne).Select(x => new PersonneExport
            {
                Id = _idPersonne,
                Nom = x.Nom,
                EstAdmin = x.EstAmin
            }).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AjouterAsync(PersonneImport _personneImport)
    {
        try
        {
            Personne personne = new()
            {
                Login = _personneImport.Login,
                Mdp = MdpServ.Hasher(_personneImport.Mdp),
                Nom = _personneImport.Nom
            };

            var p = await Context.AddAsync(personne);
            int nb = await Context.SaveChangesAsync();

            return nb > 0 ? p.Entity.Id : 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ModifierAsync(PersonneModifierImport _personneImport, int _idPersonne)
    {
        try
        {
            int retour = await Context.Personnes
                    .Where(x => x.Id == _idPersonne)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.Nom, _personneImport.Nom));

            return retour is 1;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> SupprimerAsync(int _idPersonne)
    {
        try
        {
            int retour = await Context.Personnes.Where(x => x.Id == _idPersonne).ExecuteDeleteAsync();

            return retour is 1;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExisteAsync(string _login)
    {
        try
        {
            return await Context.Personnes.AnyAsync(x => x.Nom == _login);
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExisteAsync(int _idPersonne)
    {
        try
        {
            return await Context.Personnes.AnyAsync(x => x.Id == _idPersonne);
        }
        catch
        {
            throw;
        }
    }
}
