using back.Extensions;
using back.Models;
using back.ModelsExport;
using back.ModelsImport.Logs;
using back.Services.Jwts;
using back.Services.Logs;
using back.Services.Mdps;
using back.Services.Personnes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace back.Routes;

public static class LogRoute
{
    public static RouteGroupBuilder AjouterRouteLog(this RouteGroupBuilder builder)
    {
        builder.WithOpenApi();

        builder.MapPost("connexion", ConnexionAsync)
            .Produces<PersonneConnexionExport>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        return builder;
    }

    async static Task<IResult> ConnexionAsync([FromServices] ILogService _logServ, 
                                       [FromServices] IMdpService _mdpServ,
                                       [FromServices] IJwtService _jwtServ,
                                       [FromServices] IPersonneService _personneServ,
                                       [FromServices] IValidator<LogImport> _validator,
                                       [FromBody] LogImport _logImport)
    {
        try
        {
            var validate = _validator.Validate(_logImport);

            if(!validate.IsValid)
                return Results.Extensions.ErreurValidator(validate.Errors);

            if(!await _logServ.ExisteAsync(_logImport.Login))
                return Results.NotFound("Login ou mot de passe incorrect");

            string mdpHash = await _logServ.RecupererMdpHasherAsync(_logImport.Login);

            if(!_mdpServ.VerifierHash(_logImport.Mdp, mdpHash))
                return Results.NotFound("Login ou mot de passe incorrect");

            int idPersonne = await _logServ.RecupererIdPersonneAsync(_logImport.Login);
            var retour = await _personneServ.InfoAsync(idPersonne);

            PersonneConnexionExport export = new()
            {
                Id = idPersonne,
                Nom = retour!.Nom,
                Jwt = _jwtServ.Generer(
                [
                    new Claim(ClaimTypes.Role, retour.EstAdmin ? "admin" : "utilisateur"), 
                    new Claim(ClaimTypes.Role, "role1"),
                    new Claim(ClaimTypes.Role, "role2"),
                    new Claim("idPersonne", idPersonne.ToString())
                ])
            };

            return Results.Extensions.OK(export, PersonneConnexionExportContext.Default);
        }
        catch
        {
            return Results.Extensions.ErreurConnexionBdd();
        }
    }
}
