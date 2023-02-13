using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class OdmoristeValidation : AbstractValidator<Odmoriste>
    {
        public OdmoristeValidation()
        {
            RuleFor(o => o.Naziv)
                .NotEmpty().WithMessage("Potrebno je unijeti naziv odmorišta");

     
        }
    }
}
