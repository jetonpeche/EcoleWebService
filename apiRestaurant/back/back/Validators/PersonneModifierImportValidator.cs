using back.ModelsImport.Personnes;
using FluentValidation;

namespace back.Validators;

public class PersonneModifierImportValidator: AbstractValidator<PersonneModifierImport>
{
    public PersonneModifierImportValidator()
    {
        RuleFor(x => x.Nom).NotEmpty();
    }
}
