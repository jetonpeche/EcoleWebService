using back.ModelsImport.Restaurants;
using back.Services.Restaurants;
using FluentValidation;
using System.Text.RegularExpressions;

namespace back.Validators;

public class RestaurantImportValidator: AbstractValidator<RestaurantImport>
{
    public RestaurantImportValidator(IRestaurantService _restaurantServ)
    {
        RuleFor(x => x.Nom).NotEmpty();
        RuleFor(x => x.Adresse).NotEmpty();

        RuleFor(x => x.Url).Custom((url, context) =>
        {
            if(!string.IsNullOrWhiteSpace(url) && !Uri.TryCreate(url, UriKind.Absolute, out _))
                context.AddFailure($"'Url' {url} n'est pas une Url");
        });

        RuleFor(x => x.Telephone).Custom((tel, context) =>
        {
            if(!string.IsNullOrWhiteSpace(tel))
            {
                if (!Regex.IsMatch(tel, @"^0\d{9}$"))
                    context.AddFailure($"'Telephone' doit etre au format FR (0612345678)");
            }
        });

        RuleFor(x => x.IdTypeRestaurant)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .MustAsync(async (x, idRestaurant, token) => await _restaurantServ.ExisteAsync(idRestaurant))
            .WithMessage((_, idRestaurant) => $"'idRestaurant' {idRestaurant} n'existe pas");
    }
}
