using back.Models;
using back.ModelsImport.Restaurants;
using back.Services.Restaurants;
using back.Services.TypesRestaurant;
using FluentValidation;
using System.Text.RegularExpressions;

namespace back.Validators;

public sealed class RestaurantModifierImportValidator: AbstractValidator<RestaurantModifierImport>
{
    public RestaurantModifierImportValidator(IRestaurantService _restaurantServ, ITypeRestaurantService _typeRestaurantServ)
    {
        // si une erreur dans une "RuleFor" d'un meme param passe au suivant "RuleFor" d'un autre param
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Nom).NotEmpty();
        RuleFor(x => x.Adresse).NotEmpty();

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .MustAsync(async (x, idRestaurant, token) => await _typeRestaurantServ.ExisteAsync(idRestaurant))
            .WithMessage((_, id) => $"'idRestaurant' {id} n'existe pas");

        RuleFor(x => x.IdTypeRestaurant)
            .GreaterThan(0)
            .MustAsync(async (x, idTypeRestaurant, token) => await _restaurantServ.ExisteAsync(idTypeRestaurant))
            .WithMessage((_, idTypeRestaurant) => $"'idTypeRestaurant' {idTypeRestaurant} n'existe pas");

        RuleFor(x => x.Url).Custom((url, context) =>
        {
            if (!string.IsNullOrWhiteSpace(url) && !Uri.TryCreate(url, UriKind.Absolute, out _))
                context.AddFailure($"'Url' {url} n'est pas une Url");
        });

        RuleFor(x => x.Telephone).Custom((tel, context) =>
        {
            if (!string.IsNullOrWhiteSpace(tel))
            {
                if (!Regex.IsMatch(tel, @"^0\d{9}$"))
                    context.AddFailure($"'Telephone' doit etre au format FR (0612345678)");
            }
        });
    }
}
