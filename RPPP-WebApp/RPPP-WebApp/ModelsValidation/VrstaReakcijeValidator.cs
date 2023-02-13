using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation {
    public class VrstaReakcijeValidator : AbstractValidator<VrstaReakcije> {
        public VrstaReakcijeValidator() {
            RuleFor(np => np.Naziv)
                .NotEmpty().WithMessage("Potrebno je unijeti naziv naziv")
                .MaximumLength(50).WithMessage("Naziv mogi imati do 50 znakova");


            RuleFor(np => np.BrojTelefona)
              .NotEmpty().WithMessage("Potrebno je unijeti broj telefona")
              .MaximumLength(10).WithMessage("Broj telefona može imati do 10 znakova");
        }
    }
}
