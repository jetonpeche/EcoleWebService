using back.Models;
using back.ModelsExport;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text;
using System.Text.Json;

namespace back.Services.PersonnesRestaurantsAimer;

public sealed class PersonneRestaurantAimerService: IPersonneRestaurantAimerService
{
    private BddContext Context { get; }
    private string NomTable { get; } = "PersonneRestaurantAimer";

    public PersonneRestaurantAimerService(BddContext _context)
    {
        Context = _context;
    }

    public async Task<List<RestaurantExport>> ListerAsync(int _idPersonne)
    {
        using (MySqlConnection sqlCon = new(Context.Database.GetConnectionString()))
        {
            try
            {
                List<RestaurantExport> liste = new();

                await sqlCon.OpenAsync();

                MySqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = "SELECT JSON_OBJECT('Id', r.Id, 'Nom', r.Nom, 'Adresse', r.Adresse, 'Description', r.Description, 'Url', r.Url, 'Telephone', r.Telephone, 'TypeRestaurant', JSON_OBJECT('Id', tr.Id, 'Nom', tr.Nom)) " +
                                  "FROM PersonneRestaurantAimer pra " +
                                  "JOIN Restaurant r on r.Id = pra.IdRestaurant " +
                                  "JOIN TypeRestaurant tr ON tr.Id = r.IdTypeRestaurant " +
                                  $"WHERE idPersonne = {_idPersonne}";

                cmd.Prepare();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        liste.Add(JsonSerializer.Deserialize<RestaurantExport>(reader.GetString(0))!);

                    await reader.CloseAsync();
                    await sqlCon.CloseAsync();

                    return liste;
                }
            }
            catch
            {
                if (sqlCon.State is not System.Data.ConnectionState.Closed)
                    await sqlCon.CloseAsync();

                throw;
            }
        }
    }

    public async Task<bool> AjouterAsync(List<int> _listeIdRestaurantAimer, int _idPersonne)
    {
        try
        {
            StringBuilder sb = new();

            for (int i = 0; i < _listeIdRestaurantAimer.Count; i++)
            {
                int element = _listeIdRestaurantAimer[i];

                sb.Append($"({element}, {_idPersonne}) {(i == _listeIdRestaurantAimer.Count - 1 ? ';' : ',')}");
            }

            int retour = await Context.Database.ExecuteSqlRawAsync($"insert into {NomTable} (idRestaurant, idPersonne) VALUES {sb}");

            return retour > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> AjouterAsync(int _idRestaurant, int _idPersonne)
    {
        try
        {
            if (await ExisteAsync(_idPersonne, _idRestaurant))
                return false;

            int retour = await Context.Database.ExecuteSqlRawAsync($"insert into {NomTable} (idRestaurant, idPersonne) VALUES ({_idRestaurant}, {_idPersonne})");

            return retour > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> SupprimerAsync(int _idRestaurant, int _idPersonne)
    {
        try
        {
            int retour = await Context.Database.ExecuteSqlRawAsync($"DELETE FROM {NomTable} WHERE IdPersonne = {_idPersonne} AND IdRestaurant = {_idRestaurant}");

            return retour > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExisteAsync(int _idPersonne, int _idRestaurant)
    {

        using (MySqlConnection sqlCon = new(Context.Database.GetConnectionString()))
        {
            try
            {
                await sqlCon.OpenAsync();

                MySqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"SELECT count(*) FROM {NomTable} WHERE idPersonne = @idPersonne AND idRestaurant = @idRestaurant";

                cmd.Parameters.Add("@idPersonne", MySqlDbType.Int32).Value = _idPersonne;
                cmd.Parameters.Add("@idRestaurant", MySqlDbType.Int32).Value = _idRestaurant;

                cmd.Prepare();
                long? retour = (long?)await cmd.ExecuteScalarAsync();

                await sqlCon.CloseAsync();

                return retour is 1;
            }
            catch
            {
                if(sqlCon.State is not System.Data.ConnectionState.Closed)
                    await sqlCon.CloseAsync();

                throw;
            }
        }
    }
}
