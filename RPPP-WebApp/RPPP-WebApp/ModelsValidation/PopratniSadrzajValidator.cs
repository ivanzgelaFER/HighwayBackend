using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class PopratniSadrzajValidator : AbstractValidator<PopratniSadrzaj>

    {
        public PopratniSadrzajValidator()
        {
            RuleFor(p => p.Naziv)
                .NotEmpty().WithMessage("Potrebno je unijeti naziv popratnog sadržaja");
            RuleFor(p => p.RadnimDanomOd)
                .NotEmpty().WithMessage("Potrebno je unijeti radno vrijeme");
            RuleFor(p => p.RadninDanomDo)
               .NotEmpty().WithMessage("Potrebno je unijeti radno vrijeme");
            RuleFor(p => p.VikendimaDo)
               .NotEmpty().WithMessage("Potrebno je unijeti radno vrijeme");
            RuleFor(p => p.VikendimaOd)
               .NotEmpty().WithMessage("Potrebno je unijeti radno vrijeme");



        }
    }
}
