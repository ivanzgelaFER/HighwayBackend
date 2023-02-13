using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class KoncesionarValidator : AbstractValidator<Koncesionar>
    {
        public KoncesionarValidator()
        {
            RuleFor(k => k.Naziv)
                .NotEmpty().WithMessage("Potrebno je unijeti naziv koncesionara");

            RuleFor(k => k.Adresa)
              .NotEmpty().WithMessage("Potrebno je unijeti adresu koncesionara");

            RuleFor(a => a.Email)
              .NotEmpty().WithMessage("Potrebno je unijeti email koncesionara");

            RuleFor(a => a.KoncesijaOd)
              .NotEmpty().WithMessage("Potrebno je unijeti datum početka koncesije");

            RuleFor(a => a.KoncesijaDo)
              .NotEmpty().WithMessage("Potrebno je unijeti datum kraja koncesije");

        }
    }
}
