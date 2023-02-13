using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class VrstaPopratnogValidator : AbstractValidator<VrstaPopratnog>
    {
        public VrstaPopratnogValidator()
        {
            RuleFor(vp => vp.Naziv)
                .NotEmpty().WithMessage("Potrebno je unijeti naziv vrste popratnog sadržaja");


        }
    }
}
