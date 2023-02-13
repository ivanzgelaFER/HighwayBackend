using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation {
    public class ReakcijaValidator : AbstractValidator<Reakcija>{
            public ReakcijaValidator() {
                RuleFor(co => co.Datum)
                    .NotEmpty().WithMessage("Datum je obavezno polje");

                RuleFor(co => co.Opis)
                .MaximumLength(200).WithMessage("Opis može imati do 200 znakova");


                RuleFor(co => co.IncidentId)
                  .NotEmpty().WithMessage("Incident je obavezno polje");

                RuleFor(co => co.VrstaReakcijeId)
                    .NotEmpty().WithMessage("Vrsta reakcije je obavezno polje");

            }
    }
}
