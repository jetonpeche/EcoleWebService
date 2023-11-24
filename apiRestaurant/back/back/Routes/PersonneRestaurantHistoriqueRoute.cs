using back.Extensions;
using back.ModelsExport;
using back.ModelsImport.PersonnesRestaurantsHistorique;
using back.Services.Personnes;
using back.Services.PersonnesRestaurantsHistorique;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace back.Routes;

public static class PersonneRestaurantHistoriqueRoute
{
    public static RouteGroupBuilder AjouterRoutePersonneRestoHistorique(this RouteGroupBuilder builder)
    {
        builder.WithOpenApi().RequireAuthorization();

        builder.MapGet("info", InfoAsync)
            .WithName("infoPersonneRestoHistorique")
            .Produces<List<PersonnesRestoHistoriqueExport>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        builder.MapPost("ajouter", AjouterAsync)
            .Produces<PersonnesRestoHistoriqueExport>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        return builder;
    }

    static async Task<IResult> InfoAsync(HttpContext _httpContext,
                                         [FromServices] IPersonneRestaurantHistoriqueService _personneRestoHistoriqueServ,
                                         [FromServices] IPersonneService _personneServ)
    {
        try
        {
            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            if(!await _personneServ.ExisteAsync(idPersonne))
                return Results.BadRequest($"'idPersonne' {idPersonne} n'existe pas");

            var retour = await _personneRestoHistoriqueServ.ListerAsync(idPersonne);

            return Results.Ok(retour);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }

    static async Task<IResult> AjouterAsync(HttpContext _httpContext,
                                            [FromServices] LinkGenerator _linkGenerator,
                                            [FromServices] IPersonneRestaurantHistoriqueService _personneRestoHistoriqueServ,
                                            [FromBody] PersonnesRestoHistoriqueImport _personnesRestoImport)
    {
        try
        {
            int idPersonne = int.Parse(_httpContext.User.FindFirstValue("idPersonne")!);

            var idHistorique = await _personneRestoHistoriqueServ.AjouterAsync(_personnesRestoImport, idPersonne);

            string url = _linkGenerator.GetUriByName(_httpContext, "infoPersonneRestoHistorique", new { idPersonne })!;

            return Results.Created(url, idHistorique);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }
}
