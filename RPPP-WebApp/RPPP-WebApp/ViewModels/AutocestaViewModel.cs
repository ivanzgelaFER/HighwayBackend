using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class AutocestaViewModel
    {
        public int Id { get; set; }
        public string Oznaka { get; set; }
        public string Naziv { get; set; }
        public string Koncesionar { get; set; }

        public Koncesionar KoncesionarObj { get; set; }
        public int KoncesionarId { get; set; }

        public string NaziviDionica { get; set; }

       

        public IEnumerable<DionicaViewModel> Dionice { get; set; }

        public AutocestaViewModel()
        {
            this.Dionice = new List<DionicaViewModel>();
        }
    }
}
