using back.ModelsImport.RestaurantsPersonnesAimer;
using back.Services.Personnes;
using FluentValidation;

namespace back.Validators
{
    public class PersonneRestoAimerImportValidator: AbstractValidator<PersonneRestoAimerImport>
    {
        public PersonneRestoAimerImportValidator(IPersonneService _personneServ)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.IdRestaurant)
                .GreaterThan(0)
                .MustAsync(async (x, token) => await _personneServ.ExisteAsync(x))
                .WithMessage((_, idRestaurant) => $"'idRestaurant' {idRestaurant} n'existe pas");
        }
    }
}
