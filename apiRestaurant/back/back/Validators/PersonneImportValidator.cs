using back.ModelsImport.Personnes;
using back.Services.Personnes;
using back.Services.Restaurants;
using FluentValidation;

namespace back.Validators;

public sealed class PersonneImportValidator: AbstractValidator<PersonneImport>
{
    public PersonneImportValidator(IPersonneService _personneServ, IRestaurantService _restaurantServ)
    {
        RuleFor(x => x.Nom).NotEmpty();

        RuleFor(x => x.Mdp)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.Login)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (x, token) => !await _personneServ.ExisteAsync(x))
            .WithMessage((_, nom) => $"'Nom' {nom} existe déjà");

        RuleForEach(x => x.ListeIdRestaurant).ChildRules(x =>
        {
            x.RuleFor(y => y).CustomAsync(async (y, context, token) =>
            {
                if(y <= 0)
                    context.AddFailure($"'IdRestaurant' {y} n'existe pas");

                else if (!await _restaurantServ.ExisteAsync(y))
                    context.AddFailure($"'IdRestaurant' {y} n'existe pas");
            });
        });
    }
}
