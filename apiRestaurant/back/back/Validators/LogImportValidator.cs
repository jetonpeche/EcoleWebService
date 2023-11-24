using back.ModelsImport.Logs;
using FluentValidation;

namespace back.Validators
{
    public sealed class LogImportValidator: AbstractValidator<LogImport>
    {
        public LogImportValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Mdp).NotEmpty();
        }
    }
}
