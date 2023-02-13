using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class OdmoristeViewModel
    {
        public int IdOdmorista { get; set; }
        public string NazivOdmorista { get; set; }
        
        public int? KoordinataX { get; set; }

        public int? KoordinataY { get; set; }

        public int? GodinaOtvaranja { get; set; }

        public int DionicaId { get; set; }

        public string Dionica { get; set; }

        public Dionica DionicaObj { get; set; }

        public string NaziviPopratnihSadrzaja { get; set; }

        public IEnumerable<PopratniSadrzajViewModel> PopratniSadrzaji { get; set; }

        public OdmoristeViewModel()
        {
            this.PopratniSadrzaji = new List<PopratniSadrzajViewModel>();
        }
    }
}
