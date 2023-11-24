using back.Extensions;
using back.ModelsExport;
using back.ModelsImport.Restaurants;
using back.Services.PersonnesRestaurantsAimer;
using back.Services.Protections;
using back.Services.Restaurants;
using back.Services.TypesRestaurant;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace back.Routes;

public static class RestaurantRoute
{
    public static RouteGroupBuilder AjouterRouteRestaurant(this RouteGroupBuilder builder)
    {
        builder.WithOpenApi().RequireAuthorization();

        builder.MapGet("lister", ListerAsync)
            .Produces<List<RestaurantExport>>()
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
            .CacheOutput(x => x.Tag("tagCacheListerRestaurant"));

        builder.MapGet("aleatoire", AleatoireAsync)
            .Produces<List<RestaurantExport>>()
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        builder.MapGet("info/{idRestaurant:int}", InfoAsync)
            .WithName("infoRestaurant")
            .Produces<RestaurantExport>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
            .CacheOutput("parIdRestaurant");

        builder.MapPost("ajouter", AjouterAsync)
            .Produces<RestaurantExport>()
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        builder.MapPut("modifier", ModifierAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        builder.MapDelete("supprimer/{idRestaurant:int}", SupprimerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        return builder;
    }
    static async Task<IResult> ListerAsync([FromServices] IRestaurantService _restaurantServ)
    {
        try
        {
            var retour = await _restaurantServ.ListerAsync();

            return Results.Extensions.OK(retour, RestaurantExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> AleatoireAsync([FromServices] IRestaurantService _restaurantServ)
    {
        try
        {
            var retour = await _restaurantServ.AleatoireAsync();

            return Results.Extensions.OK(retour, RestaurantExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> InfoAsync([FromRoute(Name = "idRestaurant")] int _idRestaurant, 
                                         [FromServices] IRestaurantService _restaurantServ)
    {
        try
        {
            if (_idRestaurant <= 0)
                return Results.BadRequest($"'idRestaurant' {_idRestaurant} doit être plus grand que zéro");

            var retour = await _restaurantServ.InfoAsync(_idRestaurant);

            if (retour is null)
                return Results.NotFound($"'idRestaurant' {_idRestaurant} n'existe pas");

            return Results.Extensions.OK(retour, RestaurantExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> AjouterAsync(HttpContext _httpContext,
                                           [FromServices] LinkGenerator _linkGenerator,
                                           [FromServices] IRestaurantService _restaurantServ,
                                           [FromServices] ITypeRestaurantService _typeRestaurantServ,
                                           [FromServices] IValidator<RestaurantImport> _validator,
                                           [FromServices] IProtectionService _protectionServ,
                                           [FromServices] IOutputCacheStore _cache,
                                           [FromServices] IPersonneRestaurantAimerService _personneRestoAimerServ,
                                           [FromBody] RestaurantImport _restaurantImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_restaurantImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            // Protection XSS
            _restaurantImport.Nom = _protectionServ.XSS(_restaurantImport.Nom)!;
            _restaurantImport.Adresse = _protectionServ.XSS(_restaurantImport.Adresse)!;
            _restaurantImport.Description = _protectionServ.XSS(_restaurantImport.Description);

            int idRestaurant = await _restaurantServ.AjouterAsync(_restaurantImport);
            var retourTypeRestaurant = await _typeRestaurantServ.InfoAsync(_restaurantImport.IdTypeRestaurant);

            string url = _linkGenerator.GetUriByName(_httpContext, "infoRestaurant", new { idRestaurant })!;

            await _cache.EvictByTagAsync("tagCacheListerRestaurant", default);

            return Results.Created(url, new RestaurantExport
            {
                Id = idRestaurant,
                Nom = _restaurantImport.Nom,
                Adresse = _restaurantImport.Adresse,
                Telephone = _restaurantImport.Telephone,
                Url = _restaurantImport.Url,
                Description = _restaurantImport.Description,
                TypeRestaurant = new()
                {
                    Id = idRestaurant,
                    Nom = retourTypeRestaurant!.Nom
                }
            });
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> ModifierAsync([FromServices] IRestaurantService _restaurantServ,
                                             [FromServices] IValidator<RestaurantModifierImport> _validator,
                                             [FromServices] IOutputCacheStore _cache,
                                             [FromBody] RestaurantModifierImport _restaurantImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_restaurantImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            await _restaurantServ.ModifierAsync(_restaurantImport);

            await _cache.EvictByTagAsync("tagCacheListerRestaurant", default);
            await _cache.EvictByTagAsync($"restaurant_{_restaurantImport.Id}", default);

            return Results.NoContent();
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> SupprimerAsync([FromRoute(Name = "idRestaurant")] int _idRestaurant,
                                              [FromServices] IOutputCacheStore _cache,
                                              [FromServices] IRestaurantService _restaurantServ)
    {
        try
        {
            if(_idRestaurant <= 0)
                return Results.BadRequest($"L'id '{_idRestaurant}' doit être supérieur ou égal à zéro");

            bool retour = await _restaurantServ.SupprimerAsync(_idRestaurant);

            if (!retour)
                return Results.NotFound($"L'id '{_idRestaurant}' n'existe pas");

            await _cache.EvictByTagAsync("tagCacheListerRestaurant", default);
            await _cache.EvictByTagAsync($"restaurant_{_idRestaurant}", default);

            return Results.NoContent();
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }
}
