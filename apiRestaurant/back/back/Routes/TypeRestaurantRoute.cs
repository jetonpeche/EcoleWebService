using back.Extensions;
using back.ModelsExport;
using back.ModelsImport.TypeRestraurant;
using back.Services.TypesRestaurant;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace back.Routes;

public static class TypeRestaurantRoute
{
    public static RouteGroupBuilder AjouterRouteTypeRestaurant(this RouteGroupBuilder builder)
    {
        builder.WithOpenApi().RequireAuthorization();

        builder.MapGet("lister", ListerAsync)
            .Produces<List<TypeRestaurantExport>>()
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .CacheOutput(x => x.Tag("tagCacheListerTypeRestaurant"));

        builder.MapGet("info/{idTypeRestaurant:int}", InfoAsync)
            .WithName("infoTypeRestaurant")
            .Produces<TypeRestaurantExport>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .CacheOutput("parIdTypeRestaurant");

        builder.MapPost("ajouter", AjouterAsync)
            .Produces<TypeRestaurantExport>(StatusCodes.Status201Created)
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        builder.MapDelete("supprimer/{idTypeRestaurant:int}", SupprimerAsync)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        return builder;
    }

    static async Task<IResult> ListerAsync([FromServices] ITypeRestaurantService _typeRestaurantServ)
    {
        try
        {
            var retour = await _typeRestaurantServ.ListerAsync();

            return Results.Extensions.OK(retour, TypeRestaurantExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }   
    }

    static async Task<IResult> InfoAsync([FromServices] ITypeRestaurantService _typeRestaurantServ, 
                                         [FromRoute(Name = "idTypeRestaurant")] int _idTypeRestaurant)
    {
        try
        {
            if (_idTypeRestaurant <= 0)
                return Results.BadRequest($"'idTypeRestaurant' {_idTypeRestaurant} doit être supérieur à zéro");

            var retour = await _typeRestaurantServ.InfoAsync(_idTypeRestaurant);

            if (retour is null)
                return Results.NotFound($"'idTypeRestaurant' {_idTypeRestaurant} n'existe pas");

            return Results.Extensions.OK(retour, TypeRestaurantExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> AjouterAsync(HttpContext _httpContext,
                                           [FromServices] ITypeRestaurantService _typeRestaurantServ,
                                           [FromServices] IValidator<TypeRestaurantImport> _validator,
                                           [FromServices] IOutputCacheStore _cache,
                                           [FromServices] LinkGenerator _linkGenerator,
                                           [FromBody] TypeRestaurantImport _typeRestoImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_typeRestoImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurConnexionBdd();

            var idTypeRestaurant = await _typeRestaurantServ.AjouterAsync(_typeRestoImport);

            await _cache.EvictByTagAsync("tagCacheListerTypeRestaurant", default);

            string url = _linkGenerator.GetUriByName(_httpContext, "infoTypeRestaurant", new { idTypeRestaurant })!;

            return Results.Created(url, new TypeRestaurantExport { Id = idTypeRestaurant, Nom = _typeRestoImport.Nom });
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> SupprimerAsync([FromServices] ITypeRestaurantService _typeRestaurantServ,
                                              [FromServices] IOutputCacheStore _cache,
                                              [FromRoute(Name = "idTypeRestaurant")] int _idTypeRestaurant)
    {
        try
        {
            if (_idTypeRestaurant <= 0)
                return Results.BadRequest($"'idTypeRestaurant' {_idTypeRestaurant} doit être supérieur à zéro");
                    
            bool retour = await _typeRestaurantServ.SupprimerAsync(_idTypeRestaurant);

            if (!retour)
                return Results.NotFound($"'idTypeRestaurant' {_idTypeRestaurant} n'existe pas");

            await _cache.EvictByTagAsync($"typeRestaurant_{_idTypeRestaurant}", default);
            await _cache.EvictByTagAsync("tagCacheListerTypeRestaurant", default);

            return Results.NoContent();
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }
}
