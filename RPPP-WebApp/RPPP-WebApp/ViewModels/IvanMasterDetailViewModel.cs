using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class IvanMasterDetailViewModel
    {
        public int Id { get; set; }
        public string NazivCestovnogObjekta { get; set; }
        public string TipObjekta { get; set; }
        public int OgranicenjeBrzine { get; set; }
        public int BrojPrometnihTraka { get; set; } 
        public int DuljinaObjekta { get; set; }
        public bool ZaustavniTrak { get; set; }
        public bool? DozvolaTeretnimVozilima { get; set; }
        public string? Zanimljivost { get; set; }
        public int? GodinaIzgradnje { get; set; }
        public bool? PjesackaStaza { get; set; }
        public bool? NaplataPrijelaza { get; set; }
        public Dionica Dionica { get; set; }
        public int DionicaId { get; set; }
        public string NaziviOdrzavanjaObjekata { get; set; }
        public IEnumerable<OdrzavanjeObjekta> OdrzavanjeObjekata { get; set; }

        public IvanMasterDetailViewModel()
        {
            this.OdrzavanjeObjekata = new List<OdrzavanjeObjekta>();
        }
    }
}
