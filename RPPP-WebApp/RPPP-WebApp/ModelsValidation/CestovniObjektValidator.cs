using RPPP_WebApp.Models;
using FluentValidation;

namespace RPPP_WebApp.ModelsValidation
{
    public class CestovniObjektValidator : AbstractValidator<CestovniObjekt>
    {
        public CestovniObjektValidator()
        {
            RuleFor(co => co.Naziv)
                .NotEmpty().WithMessage("Naziv je obavezno polje");

            RuleFor(co => co.DionicaId)
              .NotEmpty().WithMessage("Dionica je obavezno polje");

            RuleFor(co => co.TipObjekta)
              .NotEmpty().WithMessage("Tip objekta je obavezno polje");

            RuleFor(co => co.OgranicenjeBrzine)
              .NotEmpty().WithMessage("Ogranicenje brzine obavezno polje")
              .LessThanOrEqualTo(130).WithMessage("Maksimalna brzina 130 km/h")
              .GreaterThanOrEqualTo(40).WithMessage("Minimalna brzina 40 km/h");

            RuleFor(co => co.BrojPrometnihTraka)
              .NotEmpty().WithMessage("Broj prometnih traka je obavezno polje");

            RuleFor(co => co.DuljinaObjekta)
              .NotEmpty().WithMessage("Duljina objekta je obavezno polje");
        }
    }
}
