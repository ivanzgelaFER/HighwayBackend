using RPPP_WebApp.Models;
using FluentValidation;

namespace RPPP_WebApp.ModelsValidation
{
    public class VrstaOdrzavanjaValidator : AbstractValidator<VrstaOdrzavanja>
    {
        public VrstaOdrzavanjaValidator()
        {
            RuleFor(vo => vo.Naziv)
                .NotEmpty().WithMessage("Naziv je obavezno polje");

            RuleFor(vo => vo.GodisnjeDoba)
              .NotEmpty().WithMessage("Godišnje doba je obavezno polje");
        }
    }
}
