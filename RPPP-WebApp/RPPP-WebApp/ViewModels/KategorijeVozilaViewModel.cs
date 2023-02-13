using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class KategorijeVozilaViewModel
    {
        public IEnumerable<KategorijaVozila> KategorijeVozila { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}