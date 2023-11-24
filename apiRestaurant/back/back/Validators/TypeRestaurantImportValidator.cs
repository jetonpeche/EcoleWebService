using back.ModelsImport.TypeRestraurant;
using back.Services.TypesRestaurant;
using FluentValidation;

namespace back.Validators
{
    public class TypeRestaurantImportValidator: AbstractValidator<TypeRestaurantImport>
    {
        public TypeRestaurantImportValidator(ITypeRestaurantService _typeRestaurantServ)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Nom)
                .NotEmpty()
                .CustomAsync(async (x, context, token) =>
                {
                    if (await _typeRestaurantServ.ExisteAsync(x))
                        context.AddFailure($"'Nom' {x} existe déjà");
                });
        }
    }
}
