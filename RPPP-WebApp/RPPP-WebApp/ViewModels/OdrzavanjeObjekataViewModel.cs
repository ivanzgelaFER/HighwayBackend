using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class OdrzavanjeObjekataViewModel
    {
        public IEnumerable<OdrzavanjeObjekta> OdrzavanjaObjekata { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
