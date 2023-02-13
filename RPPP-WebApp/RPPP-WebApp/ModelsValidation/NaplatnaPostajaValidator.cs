using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class NaplatnaPostajaValidator : AbstractValidator<NaplatnaPostaja>
    {
        public NaplatnaPostajaValidator()
        {
            RuleFor(np => np.Naziv)
                .NotEmpty().WithMessage("Potrebno je unijeti naziv naplatne postaje");

            RuleFor(np => np.AutocestaId)
              .NotEmpty().WithMessage("Potrebno je unijeti autocestu");

            RuleFor(np => np.KoordinataX)
              .NotEmpty().WithMessage("Potrebno je unijeti x koordinatu naplatne postaje");

            RuleFor(np => np.KoordinataY)
              .NotEmpty().WithMessage("Potrebno je unijeti y koordinatu naplatne postaje");

            RuleFor(np => np.GodinaOtvaranja)
              .NotEmpty().WithMessage("Potrebno je unijeti godinu otvaranja naplatne postaje");
        }
    }
}
