using back.Extensions;
using back.ModelsExport;
using back.ModelsImport.RestaurantsPersonnesAimer;
using back.Services.PersonnesRestaurantsAimer;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace back.Routes;

public static class PersonneRestaurantAimerRoute
{
    public static RouteGroupBuilder AjouterRoutePersonneRestoAimer(this RouteGroupBuilder builder)
    {
        builder.WithOpenApi().RequireAuthorization();

        builder.MapGet("lister/{idPersonne:int}", ListerAsync)
            .Produces<List<RestaurantExport>>()
            .Produces(StatusCodes.Status503ServiceUnavailable);

        builder.MapPost("ajouter", AjouterAsync)
            .Produces<ReponseExport>()
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        builder.MapPut("supprimer", SupprimerAsync)
            .Produces<ReponseExport>()
            .Produces<ErreurValidation[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        return builder;
    }

    static async Task<IResult> ListerAsync([FromServices] IPersonneRestaurantAimerService _personneRestoAimerServ,
                                           [FromRoute(Name = "idPersonne")] int _idPersonne)
    {
        try
        {
            var retour = await _personneRestoAimerServ.ListerAsync(_idPersonne);

            return Results.Extensions.OK(retour, RestaurantExportContext.Default);
        }
        catch
        {
            throw;
        }
    }

    static async Task<IResult> AjouterAsync(HttpContext _httpContext,
                                            [FromServices] IPersonneRestaurantAimerService _personneRestoAimerServ,
                                            [FromServices] IValidator<PersonneRestoAimerImport> _validator,
                                            [FromBody] PersonneRestoAimerImport _personneRestoAimerImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_personneRestoAimerImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            bool retour = await _personneRestoAimerServ.AjouterAsync(_personneRestoAimerImport.IdRestaurant, idPersonne);

            return Results.Ok(new ReponseExport { Response = retour });
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> SupprimerAsync(HttpContext _httpContext,
                                              [FromServices] IPersonneRestaurantAimerService _personneRestoAimerServ,
                                              [FromServices] IValidator<PersonneRestoAimerImport> _validator,
                                              [FromBody] PersonneRestoAimerImport _personneRestoAimerImport)
    {
        try
        {
            var validate = await _validator.ValidateAsync(_personneRestoAimerImport);

            if (!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            bool retour = await _personneRestoAimerServ.SupprimerAsync(_personneRestoAimerImport.IdRestaurant, idPersonne);

            return Results.Ok(new ReponseExport { Response = retour });
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }
}
