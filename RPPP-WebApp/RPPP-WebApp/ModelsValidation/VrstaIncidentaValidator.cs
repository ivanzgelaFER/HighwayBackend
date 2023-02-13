using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation {
    public class VrstaIncidentaValidator : AbstractValidator<VrstaIncidenta> {
        public VrstaIncidentaValidator() {
            RuleFor(k => k.Naziv)
               .NotEmpty().WithMessage("Potrebno je unijeti naziv vrste incidenta")
                 .MaximumLength(50).WithMessage("Naziv vrste incidenta moze imati do 50 znakova");

            RuleFor(k => k.OpisPravilaPonasanja)
               .MaximumLength(200).WithMessage("Opis pravila ponasanja moze imati do 200 znakova");
        }
    }
}
