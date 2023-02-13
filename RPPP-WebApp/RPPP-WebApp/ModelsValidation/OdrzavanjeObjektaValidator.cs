using RPPP_WebApp.Models;
using FluentValidation;

namespace RPPP_WebApp.ModelsValidation
{
    public class OdrzavanjeObjektaValidator : AbstractValidator<OdrzavanjeObjekta>
    {
        public OdrzavanjeObjektaValidator()
        {
            RuleFor(oo => oo.VrstaId)
                .NotEmpty().WithMessage("Vrsta je obavezno polje");

            RuleFor(oo => oo.ImeFirme)
              .NotEmpty().WithMessage("Ime firme je obavezno polje");

            RuleFor(oo => oo.RadnimDanomOd)
              .NotEmpty().WithMessage("Radnim danom od je obavezno polje");

            RuleFor(oo => oo.RadnimDanomDo)
              .NotEmpty().WithMessage("Radnim danom do obavezno polje");

            RuleFor(oo => oo.Cijena)
              .NotEmpty().WithMessage("Cijena je obavezno polje");
            
            RuleFor(oo => oo.CestovniObjektId)
                .NotEmpty().WithMessage("Cestovni objekt je obavezno polje");
        }
    }
}
