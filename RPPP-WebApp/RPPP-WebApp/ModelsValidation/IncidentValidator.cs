using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation {
    public class IncidentValidator : AbstractValidator<Incident>{
        public IncidentValidator() {
            RuleFor(el => el.Opis)
                .NotEmpty().WithMessage("Treba unijeti opis incidenta")
                .MaximumLength(200).WithMessage("Opis moze imati do 200 znakova");

            RuleFor(el => el.MeteoroloskiUvjeti)
              .MaximumLength(200).WithMessage("Meteorološki uvjeti mogi imati do 200 znakova");

            RuleFor(el => el.StanjeNaCesti)
              .NotEmpty().WithMessage("Treba unijeti stanje na cesti")
              .MaximumLength(200).WithMessage("Stanje može imati do 200 znakova");

            RuleFor(el => el.Prohodnost)
            .NotEmpty().WithMessage("Treba unijeti prohodnost")
            .MaximumLength(200).WithMessage("Prohodnost može imati do 200 znakova");
        }
    }
}
