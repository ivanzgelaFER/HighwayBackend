using RPPP_WebApp.Models;
using FluentValidation;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.ModelsValidation
{
    public class IvanMDValidator : AbstractValidator<IvanMasterDetailViewModel>
    {
        public IvanMDValidator()
        {
            RuleFor(md => md.NazivCestovnogObjekta)
                .NotEmpty().WithMessage("Naziv je obavezno polje");

            RuleFor(md => md.DionicaId)
              .NotEmpty().WithMessage("Dionica je obavezno polje");

            RuleFor(md => md.TipObjekta)
              .NotEmpty().WithMessage("Tip objekta je obavezno polje");

            RuleFor(md => md.OgranicenjeBrzine)
              .NotEmpty().WithMessage("Ogranicenje brzine obavezno polje")
              .LessThanOrEqualTo(130).WithMessage("Maksimalna brzina 130 km/h")
              .GreaterThanOrEqualTo(40).WithMessage("Minimalna brzina 40 km/h");

            RuleFor(md => md.BrojPrometnihTraka)
              .NotEmpty().WithMessage("Broj prometnih traka je obavezno polje");

            RuleFor(md => md.DuljinaObjekta)
              .NotEmpty().WithMessage("Duljina objekta je obavezno polje");

        }
    }
}
