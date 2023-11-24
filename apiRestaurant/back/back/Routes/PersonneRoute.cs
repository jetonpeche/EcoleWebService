using back.ModelsExport;
using back.ModelsImport.Personnes;
using back.Services.Personnes;
using back.Services.PersonnesRestaurantsAimer;
using Microsoft.AspNetCore.Mvc;
using back.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

namespace back.Routes;

public static class PersonneRoute
{
    public static RouteGroupBuilder AjouterRoutePersonne(this RouteGroupBuilder builder)
    {
        builder.WithOpenApi();

        builder.MapGet("lister", ListerAsync)
            .Produces<List<PersonneExport>>()
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .RequireAuthorization("admin")
            .CacheOutput(x => x.Tag("tagCacheListerPersonne"));

        builder.MapGet("info/{idPersonne:int}", InfoAsync)
            .WithSummary("Recupere les infos de l'utilisateur en temps que admin")
            .Produces<PersonneExport>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .RequireAuthorization("admin")
            .CacheOutput("parIdPersonne");

        builder.MapGet("info", Info2Async)
            .WithSummary("Recuperer les infos de son compte")
            .WithName("infoPersonne")
            .Produces<PersonneExport>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .RequireAuthorization();

        builder.MapPost("ajouter", AjouterAsync)
            .Produces<PersonneExport>(StatusCodes.Status201Created)
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        builder.MapPut("modifier", ModifierAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .RequireAuthorization();

        builder.MapDelete("supprimer/{idPersonne:int}", SupprimerAsync)
            .WithSummary("Supprimer l'utilisateur en temps que admin")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .RequireAuthorization("admin");

        builder.MapDelete("supprimer", Supprimer2Async)
            .WithSummary("Supprimer son compte")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .RequireAuthorization();

        return builder;
    }

    static async Task<IResult> ListerAsync([FromServices] IPersonneService _personneServ)
    {
        try
        {
            var retour = await _personneServ.ListerAsync();
            return Results.Extensions.OK(retour, PersonneExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> InfoAsync([FromServices] IPersonneService _personneServ,
                                         [FromRoute(Name = "idPersonne")] int _idPersonne)
    {
        try
        {
            if(_idPersonne <= 0)
                return Results.BadRequest($"L'id '{_idPersonne}' doit être plus grand que zéro");

            var retour = await _personneServ.InfoAsync(_idPersonne);

            if (retour is null)
                return Results.NotFound($"L'id '{_idPersonne}' n'existe pas");

            return Results.Extensions.OK(retour, PersonneExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        } 
    }

    static async Task<IResult> Info2Async(HttpContext _httpContext, [FromServices] IPersonneService _personneServ)
    {
        try
        {
            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            var retour = await _personneServ.InfoAsync(idPersonne);

            if (retour is null)
                return Results.NotFound($"L'id '{idPersonne}' n'existe pas");

            return Results.Extensions.OK(retour, PersonneExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> AjouterAsync(HttpContext _httpContext,
                                            [FromServices] IPersonneService _personneServ,
                                            [FromServices] IPersonneRestaurantAimerService _personneRestoAimerServ,
                                            [FromServices] LinkGenerator _linkGenerator,
                                            [FromServices] IValidator<PersonneImport> _validator,
                                            [FromServices] IOutputCacheStore _cache,
                                            [FromBody] PersonneImport _personneImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_personneImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            int idPersonne = await _personneServ.AjouterAsync(_personneImport);

            if(_personneImport.ListeIdRestaurant.Count > 0)
                await _personneRestoAimerServ.AjouterAsync(_personneImport.ListeIdRestaurant, idPersonne);

            await _cache.EvictByTagAsync("tagCacheListerPersonne", default);

            string url = _linkGenerator.GetUriByName(_httpContext, "infoPersonne")!;

            return Results.Created(url, new PersonneExport { Id = idPersonne, Nom = _personneImport.Nom, EstAdmin = false });
        }
        catch
        { 
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> ModifierAsync(HttpContext _httpContext,
                                             [FromServices] IPersonneService _personneServ,
                                             [FromServices] IValidator<PersonneModifierImport> _validator,
                                             [FromServices] IOutputCacheStore _cache,
                                             [FromBody] PersonneModifierImport _personneImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_personneImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            await _personneServ.ModifierAsync(_personneImport, idPersonne);

            await _cache.EvictByTagAsync($"personne_{idPersonne}", default);
            await _cache.EvictByTagAsync("tagCacheListerPersonne", default);

            return Results.NoContent();
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> SupprimerAsync([FromServices] IPersonneService _personneServ,
                                              [FromServices] IOutputCacheStore _cache,
                                              [FromRoute(Name = "idPersonne")] int _idPersonne)
    {
        try
        {
            if(_idPersonne <= 0)
                return Results.BadRequest($"L'id '{_idPersonne}' doit être plus grand que zéro");

            var retour = await _personneServ.SupprimerAsync(_idPersonne);

            if (!retour)
                return Results.NotFound($"L'id '{_idPersonne}' n'existe pas");

            await _cache.EvictByTagAsync("listerPersonne", default);
            await _cache.EvictByTagAsync($"personne_{_idPersonne}", default);

            return Results.NoContent();
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> Supprimer2Async(HttpContext _httpContext,
                                              [FromServices] IPersonneService _personneServ,
                                              [FromServices] IOutputCacheStore _cache)
    {
        try
        {
            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            var retour = await _personneServ.SupprimerAsync(idPersonne);

            if (!retour)
                return Results.NotFound($"L'id '{idPersonne}' n'existe pas");

            await _cache.EvictByTagAsync("listerPersonne", default);
            await _cache.EvictByTagAsync($"personne_{idPersonne}", default);

            return Results.NoContent();
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }
}
