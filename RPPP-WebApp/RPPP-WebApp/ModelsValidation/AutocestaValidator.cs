using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class AutocestaValidator : AbstractValidator<Autocesta>
    {
        public AutocestaValidator()
        {
            RuleFor(a=> a.Oznaka)
                .NotEmpty().WithMessage("Potrebno je unijeti oznaku autoceste")
                .MaximumLength(3).WithMessage("Oznaka autoceste može sadržavati maksimalno 3 znaka");

            RuleFor(a => a.Naziv)
              .NotEmpty().WithMessage("Potrebno je unijeti naziv autoceste");

            RuleFor(a => a.KoncesionarId)
              .NotEmpty().WithMessage("Potrebno je unijeti koncesionara");
        }
    }
}
