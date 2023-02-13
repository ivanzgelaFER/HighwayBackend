using RPPP_WebApp.Models;
using FluentValidation;

namespace RPPP_WebApp.ModelsValidation
{
    public class DionicaValidator : AbstractValidator<Dionica>
    {
        public DionicaValidator()
        {
            RuleFor(d => d.Naziv)
              .NotEmpty().WithMessage("Naziv je obavezno polje");
            
            RuleFor(d => d.UlaznaPostajaId)
                .NotEmpty().WithMessage("Ulazna postaja je obavezno polje");

            RuleFor(d => d.IzlaznaPostajaId)
              .NotEmpty().WithMessage("Izlazna postaja je obavezno polje");

            RuleFor(d => d.BrojTraka)
              .NotEmpty().WithMessage("Broj traka je obavezno polje");

            /*RuleFor(d => d.ZaustavnaTraka)
              .NotEmpty().WithMessage("Zaustavna traka je obavezno polje");

            RuleFor(d => d.DozvolaTeretnimVozilima)
                .NotEmpty().WithMessage("Dozvola teretnim vozilima je obavezno polje");

            RuleFor(d => d.OtvorenZaProlaz)
              .NotEmpty().WithMessage("Otvoren za prolaz je obavezno polje");*/

            RuleFor(d => d.GodinaOtvaranja)
              .NotEmpty().WithMessage("Godina otvaranja je obavezno polje");

            RuleFor(d => d.Duljina)
              .NotEmpty().WithMessage("Duljina je obavezno polje");

            RuleFor(d => d.OgranicenjeBrzine)
              .NotEmpty().WithMessage("Ogranicenje brzine je obavezno polje");

            RuleFor(d => d.AutocestaId)
            .NotEmpty().WithMessage("Autocesta je obavezno polje");
        }
    }
}
